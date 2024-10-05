using character.abilities;
using Lotec.Utils.Attributes;
using UnityEngine;

namespace character {
    [CreateAssetMenu]
    public class CharacterConfig : ScriptableObject {
        [Expandable]
        public CharacterMovementConfig WalkMovementConfig;
    }
}
