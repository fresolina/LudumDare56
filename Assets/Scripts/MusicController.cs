using UnityEngine;

public class MusicController : MonoBehaviour {
    [SerializeField] AudioClip mainTheme;
    [SerializeField] AudioClip secondTheme;

    [SerializeField] AudioSource source1;
    [SerializeField] AudioSource source2;

    private float fade = 0.0f; // 0 = source1, 1 = source2
    private float fadeDirection = 0.0f;
    private float fadeTime = 1.0f;
    private float maxVolume = 0.5f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        source1.clip = mainTheme;
        source2.clip = secondTheme;

        applyFade(0.0f);

        source1.Play();
        source2.Play();
    }

    void applyFade(float amount) {
        source1.volume = (1 - amount) * maxVolume;
        source2.volume = amount * maxVolume;
    }

    public void FadeToMain() {
        fadeDirection = -1.0f;
    }

    public void FadeToSecond() {
        fadeDirection = 1.0f;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            if (fade < 0.5f) {
                FadeToSecond();
            } else {
                FadeToMain();
            }
        }

        if (fadeDirection == 0.0f) {
            return;
        }

        fade += fadeDirection * Time.deltaTime / fadeTime;
        fade = Mathf.Clamp01(fade);
        applyFade(fade);

        if (fade == 0.0f || fade == 1.0f) {
            fadeDirection = 0.0f;
        }

    }

    void FixedUpdate() {
        int enemiesHunting = 0;
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
            EnemyCharacter enemyCharacter = enemy.GetComponent<EnemyCharacter>();
            if (enemyCharacter == null) {
                continue;
            }

            if (enemyCharacter.IsHuntingPlayer()) {
                enemiesHunting++;
            }
        }

        if (enemiesHunting > 0) {
            FadeToSecond();
        } else {
            FadeToMain();
        }
    }
}
