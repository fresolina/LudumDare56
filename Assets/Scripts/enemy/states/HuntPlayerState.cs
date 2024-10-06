using UnityEngine;

namespace character.states {
    [System.Serializable]
    public class HuntPlayerState : EnemyState {
        [SerializeField] CreatureNavigation _creatureNavigation;

        public override void OnEnter() {
            base.OnEnter();
            // _animator?.SetBool("isHunting", true);
            _creatureNavigation.enabled = true;
        }

        public override void OnExit() {
            base.OnExit();
            // _animator?.SetBool("isHunting", false);
            _creatureNavigation.enabled = false;
        }

        public override void Init(Animator animator) {
            base.Init(animator);
            _creatureNavigation.enabled = false;
        }

        public override void Update() {
            base.Update();
        }

        public override void FixedUpdate() {
            base.FixedUpdate();
        }
    }
}
