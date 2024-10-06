using UnityEngine;
using UnityEngine.AI;

public class CreatureNavigation : MonoBehaviour {
    [SerializeField] Transform target;
    NavMeshAgent agent;

    CreatureStates states;

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        states = GetComponent<CreatureStates>();
    }

    private void Update() {
        // agent.SetDestination(target.position);
        if (agent.enabled) {
            agent.SetDestination(states.WalkPosition);
        }
    }
}
