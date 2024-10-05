using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.AI;
using character;

public class CreatureStates : MonoBehaviour {

    public enum State {
        // Stay close and around player
        FollowPlayer,

        // Idle around a fixed position on map
        AnchoredIdle,
        // Walk to a fixed position on map (and then idle)
        WalkToPosition,
    }

    NavMeshAgent agent;

    private State state = State.FollowPlayer;

    private Transform player;

    // Fixed position to idle around
    private Vector2 idleAnchorPosition;
    private Vector2 wanderOffset;

    // Target position for most navigation states
    private Vector2 walkPosition;

    // Raycast collider mask for walls and other fixed obstacles
    private int wallLayerMask = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        idleAnchorPosition = transform.position;
        walkPosition = transform.position;

        agent = GetComponent<NavMeshAgent>();

        wallLayerMask = LayerMask.GetMask("Blocking Wall");

        // initAnchoredIdle();
        initFollowPlayer();
    }

    public Vector3 WalkPosition {
        get {
            return walkPosition;
        }
    }

    // Start idling from the current position
    private void initAnchoredIdle() {
        state = State.AnchoredIdle;
        walkPosition = transform.position;
        idleAnchorPosition = transform.position;

        agent.speed = 1.0f;
        agent.stoppingDistance = 0.0f;
    }

    private void initFollowPlayer() {
        state = State.FollowPlayer;
        agent.speed = 1.0f;
        agent.stoppingDistance = 0.0f;
    }

    private Vector2 randomOffset() {
        return Random.insideUnitCircle * 3.0f;
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

    // Update is called once per frame
    void FixedUpdate() {
        bool stopped = agent.velocity.magnitude == 0.0f;

        switch (state) {
            case State.FollowPlayer:
                if (player == null) {
                    player = GameObject.FindWithTag("Player")?.transform;
                    if (player == null) {
                        initAnchoredIdle();
                        return;
                    }
                }

                IVelocity2 velocity = player.GetComponent<IVelocity2>();
                if (Vector3.Distance(transform.position, player.position) > 3.0f) {
                    // Falling behind player. Catch up faster.
                    agent.speed = 5.0f;
                    if (velocity != null) {
                        float playerSpeed = new Vector2(velocity.VelocityX, velocity.VelocityY).magnitude;
                        agent.speed = Mathf.Max(agent.speed, playerSpeed * 1.25f);
                    }
                }

                if (stopped) {
                    agent.speed = 1.0f; // idle speed
                    wanderOffset = randomOffset();
                } else if (Random.value < 0.01f) {
                    // Randomly wander around player (1% chance per frame)
                    wanderOffset = randomOffset();
                }

                Vector2 offset = wanderOffset;
                if (velocity != null) {
                    offset += new Vector2(velocity.VelocityX, velocity.VelocityY);
                }

                walkPosition = findNearbyPosition(player.position, offset);

                break;

            case State.AnchoredIdle:

                if (stopped) {
                    wanderOffset = randomOffset();
                    walkPosition = findNearbyPosition(idleAnchorPosition, wanderOffset);
                }
                break;
            case State.WalkToPosition:
                break;
        }
    }
}
