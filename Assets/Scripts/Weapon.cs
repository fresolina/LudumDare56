using UnityEngine;

public class Weapon : MonoBehaviour {
    [SerializeField] int _damage = 1;
    [SerializeField] LayerMask _damageLayers;
    [SerializeField] float _attackRange = 0.5f;

    void FixedUpdate() {
        Collider2D collider2D = Physics2D.OverlapCircle(transform.position, _attackRange, _damageLayers);
        if (collider2D != null) {
            HandleCollision(collider2D);
        }
    }

    void HandleCollision(Collider2D other) {
        Health health = other.GetComponentInParent<Health>();
        if (health == null || ((1 << health.gameObject.layer) & _damageLayers) == 0)
            return;

        health.TakeDamage(_damage);
    }
}
