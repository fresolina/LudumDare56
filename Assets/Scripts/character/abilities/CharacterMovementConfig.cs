using UnityEngine;

namespace character.abilities {
    [CreateAssetMenu]
    public class CharacterMovementConfig : ScriptableObject {
        [Tooltip("Do not accelerate beyond this speed")]
        public float MaxSpeed = 14;

        [Tooltip("Units per second?")]
        public float Acceleration = 120;

        [Tooltip("Units per second? (friction)")]
        public float Deceleration = 60;
    }
}
