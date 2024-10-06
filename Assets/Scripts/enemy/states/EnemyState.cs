using System;
using statemachine;
using UnityEngine;

namespace character.states {
    [Serializable]
    public class EnemyState : State {
        [SerializeField] string _animationName = string.Empty;
        protected int _animationHash = 0;
        protected Animator _animator;

        public virtual void Init(Animator animator) {
            Init();
            _animator = animator;
            if (!string.IsNullOrEmpty(_animationName))
                _animationHash = Animator.StringToHash(_animationName);
        }

        public override void OnEnter() {
            base.OnEnter();
            if (_animationHash != 0)
                _animator?.Play(_animationHash);
        }
    }
}
