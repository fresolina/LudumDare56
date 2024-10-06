using UnityEngine;
using UnityEngine.AI;
using character;
using System.Collections.Generic;

public class CreatureStates : MonoBehaviour {

    public enum State {
        // Stay close to and around player
        FollowPlayer,
        // Idle around a fixed position on map
        AnchoredIdle,
        // Walk to a fixed position on map (and then idle)
        WalkToTarget,
        // Close in to attack an enemy
        WalkToEnemy,
        // Attack an enemy in range
        AttackEnemy,
    }

    // NavMeshAgent component for pathfinding and some movement magic rules
    NavMeshAgent agent;

    [SerializeField] public AudioClip attackSound;

    private AudioSource audioSource;

    // Fixed position targetting tag for this creature to look for
    // (Checked against SetTarget() and ClearTarget() callbacks)
    private string targetTag = "Target1";
    private float idleSpeed = 1.0f;
    private float runSpeed = 5.0f;
    private float wanderRadius = 3.0f;
    private float attackDistance = 1.0f;
    private float attackCooldownTime = 1.0f;

    private State state = State.FollowPlayer;
    private State previousState = State.FollowPlayer;

    // Player reference for FollowPlayer state
    private Transform player;

    // Fixed position to walk towards (WalkToTarget)
    private Vector2 walkTargetPosition;

    // Fixed position to idle around (AnchoredIdle)
    private Vector2 idleAnchorPosition;

    // Random offset from precise target position
    private Vector2 wanderOffset;
    // Final target position for most navigation states
    private Vector2 walkPosition;

    // Raycast collider mask for walls and other fixed obstacles
    private int wallLayerMask = 0;

    // Raycast collider mask for enemies
    private int enemyLayerMask = 0;

    // Used for meelee attack animation
    private float attackDuration = 0.5f;
    private float attackTimer = 0.0f;
    private Vector2 attackAnchor, attackTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        audioSource = GetComponent<AudioSource>();

        idleAnchorPosition = transform.position;
        walkPosition = transform.position;

        agent = GetComponent<NavMeshAgent>();

        wallLayerMask = LayerMask.GetMask("Blocking Wall");
        enemyLayerMask = LayerMask.GetMask("Enemies");

        // initAnchoredIdle(transform.position);
        initFollowPlayer();
    }

    public Vector3 WalkPosition {
        get {
            return walkPosition;
        }
    }

    // Start idling from the current position
    private void initAnchoredIdle(Vector2 anchorPosition) {
        state = State.AnchoredIdle;
        idleAnchorPosition = anchorPosition;
        updateWanderTarget(idleAnchorPosition);

        agent.speed = idleSpeed;
        agent.stoppingDistance = 0.0f;
    }

    private void initFollowPlayer() {
        state = State.FollowPlayer;
        agent.speed = idleSpeed;
        agent.stoppingDistance = 0.0f;
    }

    private void initWalkToTarget(Vector2 targetPosition) {
        state = State.WalkToTarget;
        agent.speed = runSpeed;
        agent.stoppingDistance = 0.0f;

        walkTargetPosition = targetPosition;
        updateWanderTarget(walkTargetPosition);
    }

    private void initWalkToEnemy(GameObject enemy, bool stacks = true) {
        agent.enabled = true;
        if (stacks)
            previousState = state;
        state = State.WalkToEnemy;
        walkPosition = enemy.transform.position;
        agent.speed = runSpeed;
        agent.stoppingDistance = attackDistance;
    }

    private void initAttackEnemy(GameObject enemy) {
        state = State.AttackEnemy;
        walkPosition = enemy.transform.position;
        agent.enabled = false;
        agent.speed = runSpeed;
        agent.stoppingDistance = attackDistance;
    }

    private void restorePreviousState() {
        agent.enabled = true;
        state = previousState;
    }

    private Vector2 randomOffset() {
        return Random.insideUnitCircle * wanderRadius;
    }

    private void updateWanderTarget(Vector2 anchor) {
        wanderOffset = randomOffset();
        walkPosition = findNearbyPosition(anchor, wanderOffset);
    }

    private Vector2 findNearbyPosition(Vector2 anchor, Vector2 offset) {
        // raycast from anchor to random position and only go if we have clear line-of-sight
        RaycastHit2D hit = Physics2D.Raycast(anchor, offset, offset.magnitude, wallLayerMask);
        if (!hit) {
            return anchor + offset;
        } else {
            return hit.point - offset.normalized * 0.1f; // move slightly away from the wall
        }
    }

    // Called from OrderGiver.cs
    public void SetTarget(OrderGiver.Target target) {
        if (target.name == targetTag) {
            initWalkToTarget(target.position);
        }
    }

    public void ClearTarget(string targetName) {
        if (targetName == targetTag) {
            initFollowPlayer(); // Recalls all creatures marked for this target
        }
    }

    // This is much more annoying than it shoudl be...
    // https://discussions.unity.com/t/how-can-i-tell-when-a-navmeshagent-has-reached-its-destination/52403/4
    private bool isNavigationFinished() {
        if (agent.enabled && !agent.pathPending) {
            if (agent.remainingDistance <= agent.stoppingDistance) {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
                    return true;
                }
            }
        }
        return false;
    }

    private GameObject ClosestSensedEnemy() {
        List<Collider2D> colliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayerMask);
        Physics2D.OverlapCircle(transform.position, 3.0f, filter, colliders);

        colliders.Sort((a, b) => {
            float distA = Vector2.Distance(a.transform.position, transform.position);
            float distB = Vector2.Distance(b.transform.position, transform.position);
            return distA.CompareTo(distB);
        });

        foreach (Collider2D enemy in colliders) {
            if (!Physics2D.Linecast(transform.position, enemy.transform.position, wallLayerMask))
                return enemy.gameObject;
        }

        return null;
    }

    // Update is called once per frame
    void FixedUpdate() {
        GameObject closestEnemy = ClosestSensedEnemy();
        bool stopped = isNavigationFinished();

        if (attackTimer > 0.0f) {
            attackTimer -= Time.deltaTime;
        }

        switch (state) {
            case State.FollowPlayer:
                if (closestEnemy) {
                    initAttackEnemy(closestEnemy);
                    return;
                }

                // Lost player reference? Start idling where they stood?
                if (player == null) {
                    player = GameObject.FindWithTag("Player")?.transform;
                    if (player == null) {
                        initWalkToTarget(walkPosition);
                        return;
                    }
                }

                IVelocity2 velocity = player.GetComponent<IVelocity2>();
                if (Vector3.Distance(transform.position, player.position) > wanderRadius) {
                    // Falling behind player. Catch up, and try to be slightly faster than player.
                    agent.speed = runSpeed;
                    if (velocity != null) {
                        float playerSpeed = new Vector2(velocity.VelocityX, velocity.VelocityY).magnitude;
                        agent.speed = Mathf.Max(agent.speed, playerSpeed * 1.25f);
                    }
                }

                if (stopped) {
                    agent.speed = idleSpeed; // idle speed
                    wanderOffset = randomOffset();
                } else if (Random.value < 0.01f) {
                    // Randomly wander around target area (1% chance per frame)
                    wanderOffset = randomOffset();
                }

                // Try to aim ahead of a moving player
                Vector2 offset = wanderOffset;
                if (velocity != null) {
                    offset += new Vector2(velocity.VelocityX, velocity.VelocityY);
                }
                walkPosition = findNearbyPosition(player.position, offset);

                break;

            case State.AnchoredIdle:
                if (closestEnemy) {
                    initWalkToEnemy(closestEnemy);
                    return;
                }

                if (stopped) {
                    updateWanderTarget(idleAnchorPosition);
                }
                break;

            case State.WalkToTarget:
                if (closestEnemy) {
                    initWalkToEnemy(closestEnemy);
                    return;
                }

                if (stopped) {
                    initAnchoredIdle(walkTargetPosition);
                } else if (Random.value < 0.01f) {
                    // Randomly retarget around target area (1% chance per frame)
                    updateWanderTarget(walkTargetPosition);
                }

                break;

            case State.WalkToEnemy:
                if (!closestEnemy) {
                    restorePreviousState();
                    return;
                }
                walkPosition = closestEnemy.transform.position;

                if (Vector2.Distance(transform.position, closestEnemy.transform.position) < attackDistance) {
                    initAttackEnemy(closestEnemy);
                }

                break;

            case State.AttackEnemy:
                if (!closestEnemy) {
                    restorePreviousState();
                    return;
                } else if (Vector2.Distance(transform.position, closestEnemy.transform.position) >= attackDistance) {
                    initWalkToEnemy(closestEnemy, false);
                }

                if (attackTimer <= 0.0f) {
                    // Meele "animation"
                    Debug.Log("Attack!");
                    attackTimer = attackCooldownTime;

                    attackAnchor = transform.position;
                    attackTarget = transform.position + (closestEnemy.transform.position - transform.position).normalized * attackDistance / 2.0f;

                    transform.position = attackTarget;

                    audioSource.pitch = Random.Range(0.5f, 1.5f);
                    audioSource.PlayOneShot(attackSound);
                } else if (attackTimer > attackDuration) {
                    transform.position = Vector2.Lerp(attackAnchor, attackTarget, (attackTimer - attackDuration) / attackCooldownTime);
                }
                // TODO: Graphical Attack animation
                // TODO: Damage enemy

                break;
        }
    }
}
