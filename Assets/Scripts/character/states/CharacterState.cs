using System;
using statemachine;
using UnityEngine;

namespace character.states {
    [Serializable]
    public class CharacterState : State {
        [SerializeField] string _animationName = string.Empty;
        protected int _animationHash = 0;
        protected ICharacter _character;

        public CharacterState(ICharacter character) : base() {
            Init(character);
        }

        public void Init(ICharacter character) {
            Init();
            _character = character;
            if (!string.IsNullOrEmpty(_animationName))
                _animationHash = Animator.StringToHash(_animationName);
        }

        public override void OnEnter() {
            base.OnEnter();
            if (_animationHash != 0)
                _character.Animator?.Play(_animationHash);
        }
    }
}
