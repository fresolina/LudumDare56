using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour {
    private TMP_Text coinCounterText;
    private TMP_Text enemyCounterText;
    private TMP_Text winText;

    private int coinCounter = 0;

    private AudioSource audioSource;
    [SerializeField] AudioClip coinSound;

    void Awake() {
        // Survive across level loads
        DontDestroyOnLoad(this.gameObject);
        audioSource = GetComponent<AudioSource>();

        // For now, reset game on start
        resetGame();
        initScene();
    }

    private void resetGame() {
        coinCounter = 0;
    }

    private void initScene() {
        coinCounterText = GameObject.Find("CoinCounterText").GetComponent<TMP_Text>();
        coinCounterText.text = "" + coinCounter;
        enemyCounterText = GameObject.Find("EnemyCounterText").GetComponent<TMP_Text>();
        winText = GameObject.Find("WinText").GetComponent<TMP_Text>();
    }

    public void PickUpCoin() {
        audioSource.Stop();
        audioSource.pitch = Random.Range(1.0f, 1.50f);
        audioSource.PlayOneShot(coinSound);

        coinCounter++;
        if (coinCounter >= 3) {
            // Autobuy a creature
            coinCounter -= 3;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                player.BroadcastMessage("SpawnCreature", 1, SendMessageOptions.RequireReceiver);
        }
        coinCounterText.text = "" + coinCounter;
    }

    void FixedUpdate() {
        GameObject[] enemiesLeft = GameObject.FindGameObjectsWithTag("Enemy");
        enemyCounterText.text = "" + enemiesLeft.Length;

        if (enemiesLeft.Length == 0) {
            winText.enabled = true;
        }
    }

    // TODO: restart. go to title scene, reset game counters, etc.

    // TODO: start game: load level 1

    // TODO: load level. find canvases and stuff...

    // TODO: game over



}
