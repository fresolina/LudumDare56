using UnityEngine;
using UnityEngine.AI;

namespace character.states {
    [System.Serializable]
    public class HuntPlayerState : EnemyState {
        [SerializeField] NavMeshAgent _navMeshAgent;
        [SerializeField] Transform _target;

        public override void OnEnter() {
            base.OnEnter();
            // _animator?.SetBool("isHunting", true);
            _navMeshAgent.enabled = true;
        }

        public override void OnExit() {
            base.OnExit();
            // _animator?.SetBool("isHunting", false);
            _navMeshAgent.enabled = false;
        }

        public override void Init(Animator animator) {
            base.Init(animator);
            _navMeshAgent.enabled = false;
        }

        public override void Update() {
            base.Update();
            _navMeshAgent.SetDestination(_target.position);
        }

        public override void FixedUpdate() {
            base.FixedUpdate();
        }
    }
}
