using UnityEngine;

namespace character {
    public class CharacterSensor : MonoBehaviour, IVelocity2 {
        public float GrounderDistance = 0.05f;
        [field: SerializeField] public CapsuleCollider2D Collider { get; set; }
        LayerMask _playerLayer;

        // IVelocity2
        public float VelocityX { get => _rigidbody.linearVelocityX; set => _rigidbody.linearVelocityX = value; }
        public float VelocityY { get => _rigidbody.linearVelocityY; set => _rigidbody.linearVelocityY = value; }

        public bool IsTouchingGround { get; private set; }
        public bool IsTouchingCeiling { get; private set; }
        public bool IsTouchingWall => IsTouchingLeftWall || IsTouchingRightWall;
        public bool IsTouchingLeftWall { get; private set; }
        public bool IsTouchingRightWall { get; private set; }

        Rigidbody2D _rigidbody;

        void OnValidate() {
            if (Collider == null)
                Collider = GetComponentInChildren<CapsuleCollider2D>();
        }

        void Awake() {
            // _playerLayer = _collider.gameObject.layer;
            _playerLayer = LayerMask.GetMask("Player");
            _rigidbody = Collider.attachedRigidbody;
        }

        void FixedUpdate() {
            UpdateSensors();
        }

        void UpdateSensors() {
            bool groundHit = Physics2D.CapsuleCast(Collider.bounds.center, Collider.size, Collider.direction, 0, Vector2.down, GrounderDistance, ~_playerLayer);
            bool ceilingHit = Physics2D.CapsuleCast(Collider.bounds.center, Collider.size, Collider.direction, 0, Vector2.up, GrounderDistance, ~_playerLayer);
            IsTouchingLeftWall = Physics2D.CapsuleCast(Collider.bounds.center, Collider.size, Collider.direction, 0, Vector2.left, GrounderDistance, ~_playerLayer);
            IsTouchingRightWall = Physics2D.CapsuleCast(Collider.bounds.center, Collider.size, Collider.direction, 0, Vector2.right, GrounderDistance, ~_playerLayer);

            IsTouchingGround = groundHit;
            IsTouchingCeiling = ceilingHit;
        }
    }
}
