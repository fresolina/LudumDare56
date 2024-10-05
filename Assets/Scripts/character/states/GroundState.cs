namespace character.states {
    [System.Serializable]
    public class GroundState : CharacterState {
        public GroundState(ICharacter character) : base(character) { }

        public override void FixedUpdate() {
            base.FixedUpdate();
            _character.MoveAbility.FixedUpdate();
        }
    }
}
