using UnityEngine;

public class Health : MonoBehaviour {
    [SerializeField] GameObject _deathEffect;
    [SerializeField] AudioClip[] _damageSounds;
    [SerializeField] AudioClip[] _deathSounds;
    [SerializeField] int _maxHitpoints = 10;
    [SerializeField] float _damageCooldown = 0.5f;

    public event System.Action Died;

    int _hitPoints;
    [SerializeField, HideInInspector]
    AudioSource _audioSource;
    float _tookDamageAt;

    void Awake() {
        _hitPoints = _maxHitpoints;
    }

    public bool IsAlive => _hitPoints > 0;

    public bool didDeathSound = false;

    public void TakeDamage(int damage) {
        if (Time.time - _tookDamageAt < _damageCooldown) {
            return;
        }

        _tookDamageAt = Time.time;
        _hitPoints -= damage;
        if (_hitPoints <= 0) {
            if (_deathSounds.Length > 0 && !didDeathSound) {
                _audioSource.PlayOneShot(_deathSounds[Random.Range(0, _deathSounds.Length)]);
                didDeathSound = true;
            }
            if (_deathEffect) {
                GameObject go = Instantiate(_deathEffect);
                go.transform.position = transform.position;
            }
            // gameObject.SetActive(false);
            Destroy(gameObject, 3);
            Died?.Invoke();
            // Debug.Log($"Died: {name}", gameObject);
        } else {
            if (_damageSounds.Length > 0) {
                _audioSource.PlayOneShot(_damageSounds[Random.Range(0, _damageSounds.Length)]);
            }
        }
    }

    void OnValidate() {
        if (!_audioSource) {
            _audioSource = GetComponent<AudioSource>();
            if (!_audioSource) {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }
}
