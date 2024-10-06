using UnityEngine;
using UnityEngine.AI;
using character;

public class CreatureStates : MonoBehaviour {

    public enum State {
        // Stay close to and around player
        FollowPlayer,
        // Idle around a fixed position on map
        AnchoredIdle,
        // Walk to a fixed position on map (and then idle)
        WalkToTarget,
    }

    // NavMeshAgent component for pathfinding and some movement magic rules
    NavMeshAgent agent;

    // Fixed position targetting tag for this creature to look for
    // (Checked against SetTarget() and ClearTarget() callbacks)
    private string targetTag = "Target1";
    private float idleSpeed = 1.0f;
    private float runSpeed = 5.0f;
    private float wanderRadius = 3.0f;

    private State state = State.FollowPlayer;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        idleAnchorPosition = transform.position;
        walkPosition = transform.position;

        agent = GetComponent<NavMeshAgent>();

        wallLayerMask = LayerMask.GetMask("Blocking Wall");

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
        if (!agent.pathPending) {
            if (agent.remainingDistance <= agent.stoppingDistance) {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
                    return true;
                }
            }
        }
        return false;
    }

    // Update is called once per frame
    void FixedUpdate() {
        bool stopped = isNavigationFinished();

        switch (state) {
            case State.FollowPlayer:
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
                if (stopped) {
                    updateWanderTarget(idleAnchorPosition);
                }
                break;

            case State.WalkToTarget:
                if (stopped) {
                    initAnchoredIdle(walkTargetPosition);
                } else if (Random.value < 0.01f) {
                    // Randomly retarget around target area (1% chance per frame)
                    updateWanderTarget(walkTargetPosition);
                }

                break;
        }
    }
}
