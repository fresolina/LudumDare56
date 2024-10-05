using UnityEngine;
using UnityEngine.AI;

public class CreatureStates : MonoBehaviour {

    public enum State {
        Idle,
        WalkToPosition,
    }

    NavMeshAgent agent;

    private State state = State.Idle;

    private Vector3 idleAnchorPosition;
    private Vector3 walkPosition;

    int wallLayerMask = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        idleAnchorPosition = transform.position;
        walkPosition = transform.position;

        agent = GetComponent<NavMeshAgent>();

        wallLayerMask = LayerMask.GetMask("Blocking Wall");
        Debug.Log("Wall layer mask: " + wallLayerMask);

        initIdle();
    }

    public Vector3 WalkPosition {
        get {
            return walkPosition;
        }
    }

    // Start idling from the current position
    private void initIdle() {
        state = State.Idle;
        walkPosition = transform.position;
        idleAnchorPosition = transform.position;

        agent.speed = 1.0f;
        agent.stoppingDistance = 0.0f;
    }

    // Update is called once per frame
    void FixedUpdate() {
        switch (state) {
            case State.Idle:
                bool stopped = agent.velocity.magnitude < 0.01f;

                if (stopped) {
                    // Try a random position near the anchor
                    Vector3 randomDistance = Random.insideUnitCircle * 3.0f;

                    // raycast from anchor to random position and only go if we have clear line-of-sight
                    RaycastHit2D hit = Physics2D.Raycast(idleAnchorPosition, randomDistance, randomDistance.magnitude, wallLayerMask);
                    if (!hit) {
                        walkPosition = idleAnchorPosition + randomDistance;
                    }
                }
                break;
            case State.WalkToPosition:
                break;
        }
    }
}
