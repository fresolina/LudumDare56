using character.abilities;
using UnityEngine;

namespace character {
    public interface ICharacter {
        Animator Animator { get; }
        CharacterSensor Sensor { get; }
        CharacterInput Input { get; }
        CharacterMoveAbility MoveAbility { get; }
    }
}
