using System.Collections.Generic;
using UnityEngine;

public class TakeDamageSensor : MonoBehaviour
{
    [field: SerializeField] public BoxCollider2D Collider { get; set; }

    Rigidbody2D _rigidbody;
    LayerMask _enemyLayer;

    void OnValidate() {
        if (Collider == null)
            Collider = GetComponentInChildren<BoxCollider2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Awake() {
        // _playerLayer = _collider.gameObject.layer;
        _enemyLayer = LayerMask.GetMask("Enemies");
        _rigidbody = Collider.attachedRigidbody;
    }

    void FixedUpdate() {
        UpdateSensors();
    }

    void UpdateSensors() {
        if (Collider.IsTouchingLayers()) {
            //Vector3 direction = (transform.position - other.transform.position).normalized;
            //Vector2 direction = Vector2.left;
            List<Collider2D> results = new List<Collider2D>();
            Collider.Overlap(results);
            if (results.Count > 0) {
                var player = GameObject.Find("Player").GetComponent<PlayerCharacter>();
                if (player.TakeDamage(1)) {
                    //results[0].transform.position
                    Vector2 direction = (transform.position - results[0].transform.position).normalized;
                    float pushingForce = 1000;
                    _rigidbody.AddForce(direction * pushingForce);
                }
            }
        }
        /*
        bool groundHit = Physics2D.CapsuleCast(Collider.bounds.center, Collider.size, Collider.direction, 0, Vector2.down, GrounderDistance, ~_enemyLayer);
        bool ceilingHit = Physics2D.CapsuleCast(Collider.bounds.center, Collider.size, Collider.direction, 0, Vector2.up, GrounderDistance, ~_enemyLayer);
        IsTouchingLeftWall = Physics2D.CapsuleCast(Collider.bounds.center, Collider.size, Collider.direction, 0, Vector2.left, GrounderDistance, ~_enemyLayer);
        IsTouchingRightWall = Physics2D.CapsuleCast(Collider.bounds.center, Collider.size, Collider.direction, 0, Vector2.right, GrounderDistance, ~_enemyLayer);

        IsTouchingGround = groundHit;
        IsTouchingCeiling = ceilingHit;*/
    }
}
