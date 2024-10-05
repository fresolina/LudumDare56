using Lotec.Utils.Attributes;
using UnityEngine;

namespace character.abilities {
    [System.Serializable]
    public class CharacterMoveAbility {
        [field: SerializeField, Expandable] public CharacterMovementConfig Config { get; set; }
        IDirection2Provider _input;
        IVelocity2 _speed;
        CharacterSensor _sensor;

        public void Init(CharacterSensor sensor, IDirection2Provider input, IVelocity2 speed) {
            _sensor = sensor;
            _input = input;
            _speed = speed;
        }

        // Physics update
        public void FixedUpdate() {
            float step = _input.Direction.x == 0 ? Config.Deceleration : Config.Acceleration;
            _speed.VelocityX = Mathf.MoveTowards(_speed.VelocityX, _input.Direction.x * Config.MaxSpeed, step * Time.fixedDeltaTime);
            step = _input.Direction.x == 0 ? Config.Deceleration : Config.Acceleration;
            _speed.VelocityY = Mathf.MoveTowards(_speed.VelocityY, _input.Direction.y * Config.MaxSpeed, step * Time.fixedDeltaTime);
        }
    }
}
