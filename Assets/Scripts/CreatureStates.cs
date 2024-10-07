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
    [SerializeField] public AudioClip chargeSound;

    private AudioSource audioSource;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    // Fixed position targetting tag for this creature to look for
    // (Checked against SetTarget() and ClearTarget() callbacks)
    private string targetTag = "Target1";
    private float idleSpeed = 1.0f;
    private float runSpeed = 5.0f;
    private float wanderRadius = 3.0f;
    private float detectionDistance = 3.0f;
    private float attackRange = 1.0f;
    private float attackCooldownTime = 1.0f;

    private bool ignoreEnemies = false;

    private State state = State.FollowPlayer;
    private State previousState = State.FollowPlayer;

    // Player reference for FollowPlayer state
    private Transform player;

    private GameObject huntedEnemy;

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
        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

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
        huntedEnemy = null;
        ignoreEnemies = false; // Patrol anchor for enemies
        idleAnchorPosition = anchorPosition;
        updateWanderTarget(idleAnchorPosition);

        agent.speed = idleSpeed;
        agent.stoppingDistance = 0.0f;
        // _animator.Play("idle");
    }

    private void initFollowPlayer() {
        state = State.FollowPlayer;
        huntedEnemy = null;
        agent.speed = idleSpeed;
        agent.stoppingDistance = 0.0f;
        ignoreEnemies = true; // First catch up with player. Then protect!
        _animator.Play("walking");
    }

    private void initWalkToTarget(Vector2 targetPosition) {
        state = State.WalkToTarget;
        agent.speed = runSpeed;
        agent.stoppingDistance = 0.0f;
        ignoreEnemies = true; // Be focused while walking

        walkTargetPosition = targetPosition;
        updateWanderTarget(walkTargetPosition);
    }

    private void initWalkToEnemy(GameObject enemy, bool stacks = true) {
        //agent.enabled = true;
        if (stacks) {
            previousState = state;

            audioSource.pitch = Random.Range(0.5f, 1.5f);
            audioSource.PlayOneShot(chargeSound);
        }
        state = State.WalkToEnemy;
        ignoreEnemies = false; // Can be distracted by nearer enemies
        walkPosition = enemy.transform.position;
        agent.speed = runSpeed;
        agent.stoppingDistance = attackRange;
    }

    private void initAttackEnemy(GameObject enemy) {
        state = State.AttackEnemy;
        ignoreEnemies = false; // Can be distracted by nearer enemies
        walkPosition = enemy.transform.position;
        //agent.enabled = false;
        agent.speed = runSpeed;
        agent.stoppingDistance = attackRange;
        _animator.Play("attacking");
    }

    private void restorePreviousState() {
        //agent.enabled = true;
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
            GameObject enemy = ClosestSensedEnemy(target.position);
            if (enemy != null && Vector2.Distance(target.position, enemy.transform.position) < 0.5f) {
                enemy = enemy.GetComponentInParent<Health>().gameObject; // Quick and dirty find parent
                initWalkToEnemy(enemy, false);
                huntedEnemy = enemy;
            } else {
                huntedEnemy = null;
                initWalkToTarget(target.position);
            }

        }
    }

    public void ClearTarget(string targetName) {
        if (targetName == targetTag) {
            huntedEnemy = null;
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

    private GameObject ClosestSensedEnemy(Vector2 point) {
        List<Collider2D> colliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayerMask);
        Physics2D.OverlapCircle(point, detectionDistance, filter, colliders);

        colliders.Sort((a, b) => {
            float distA = Vector2.Distance(a.transform.position, transform.position);
            float distB = Vector2.Distance(b.transform.position, transform.position);
            return distA.CompareTo(distB);
        });

        foreach (Collider2D enemy in colliders) {
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth == null || !enemyHealth.IsAlive)
                continue;

            if (!Physics2D.Linecast(point, enemy.transform.position, wallLayerMask))
                return enemy.gameObject;
        }

        return null;
    }

    void Update() {
        _animator.SetFloat("Speed", agent.speed);
        Vector2 direction = agent.destination - transform.position;
        _spriteRenderer.flipX = direction.x < 0;
    }

    // Update is called once per frame
    void FixedUpdate() {
        GameObject closestEnemy = null;
        if (!ignoreEnemies) {
            if (huntedEnemy) {
                closestEnemy = huntedEnemy;
            } else {
                closestEnemy = ClosestSensedEnemy(transform.position);
            }
        }
        bool stopped = isNavigationFinished();

        attackTimer += Time.deltaTime;

        switch (state) {
            case State.FollowPlayer:
                if (closestEnemy) {
                    initWalkToEnemy(closestEnemy);
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
                    ignoreEnemies = true; // Prioritize catching up
                }

                if (stopped) {
                    ignoreEnemies = false; // now protect player
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

                if (Vector2.Distance(transform.position, closestEnemy.transform.position) <= attackRange) {
                    initAttackEnemy(closestEnemy);
                }

                break;

            case State.AttackEnemy:
                if (!closestEnemy) {
                    restorePreviousState();
                    return;
                } else if (Vector2.Distance(transform.position, closestEnemy.transform.position) > attackRange) {
                    initWalkToEnemy(closestEnemy, false);
                    return;
                }

                if (attackTimer > attackCooldownTime) {
                    // Meele "animation"
                    attackTimer = 0.0f;

                    attackAnchor = transform.position;
                    attackTarget = (transform.position + closestEnemy.transform.position) / 2.0f;

                    audioSource.pitch = Random.Range(0.5f, 1.5f);
                    audioSource.PlayOneShot(attackSound);
                } else if (attackTimer < attackDuration / 4.0f) {
                    float subduration = attackDuration / 4.0f;
                    transform.position = Vector2.Lerp(attackAnchor, attackTarget, attackTimer / subduration);
                } /*

                TODO: didn't work as expected. No time to fix...
                else if (attackTimer < attackDuration) {
                    float subduration = 3.0f * attackDuration / 4.0f;
                    transform.position = Vector2.Lerp(attackTarget, attackAnchor, attackTimer / attackDuration);
                }*/
                // TODO: Graphical Attack animation
                // TODO: Damage enemy

                break;
        }
    }
}
