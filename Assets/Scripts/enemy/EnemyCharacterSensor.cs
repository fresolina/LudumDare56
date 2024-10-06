using UnityEngine;

namespace character {
    public class EnemyCharacterSensor : MonoBehaviour {
        [field: SerializeField] public CircleCollider2D PlayerInRangeCollider { get; set; }
        LayerMask _playerLayer;

        public bool IsPlayerInRange { get; private set; }

        void OnValidate() {
            if (PlayerInRangeCollider == null)
                PlayerInRangeCollider = GetComponentInChildren<CircleCollider2D>();
            PlayerInRangeCollider.isTrigger = true;
        }

        void Awake() {
            _playerLayer = LayerMask.GetMask("Player");
        }

        void FixedUpdate() {
            UpdateSensors();
        }

        void UpdateSensors() {
            IsPlayerInRange = Physics2D.OverlapCircle(PlayerInRangeCollider.bounds.center, PlayerInRangeCollider.radius, _playerLayer);
        }
    }
}
