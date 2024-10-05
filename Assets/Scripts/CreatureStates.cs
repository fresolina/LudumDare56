using UnityEngine;
using UnityEngine.AI;

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

    // Fixed position to idle around
    private Vector3 idleAnchorPosition;

    // Target position for most navigation states
    private Vector3 walkPosition;

    // Raycast collider mask for walls and other fixed obstacles
    int wallLayerMask = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        idleAnchorPosition = transform.position;
        walkPosition = transform.position;

        agent = GetComponent<NavMeshAgent>();

        wallLayerMask = LayerMask.GetMask("Blocking Wall");

        initAnchoredIdle();
        // initFollowPlayer();
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

    private bool tryFindNearbyPosition(Vector3 anchor, ref Vector3 outPosition) {
        // Try a random position near the anchor
        Vector3 randomDistance = Random.insideUnitCircle * 3.0f;

        // raycast from anchor to random position and only go if we have clear line-of-sight
        RaycastHit2D hit = Physics2D.Raycast(anchor, randomDistance, randomDistance.magnitude, wallLayerMask);
        if (!hit) {
            outPosition = idleAnchorPosition + randomDistance;
            return true;
        }

        return false;
    }

    // Update is called once per frame
    void FixedUpdate() {
        switch (state) {
            case State.AnchoredIdle:
                bool stopped = agent.velocity.magnitude < 0.01f;

                if (stopped) {
                    tryFindNearbyPosition(idleAnchorPosition, ref walkPosition);
                }
                break;
            case State.WalkToPosition:
                break;
        }
    }
}
