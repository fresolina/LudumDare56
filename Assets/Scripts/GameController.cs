using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour {
    private TMP_Text coinCounterText;

    private int coinCounter = 0;

    void Awake() {
        // Survive across level loads
        DontDestroyOnLoad(this.gameObject);

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
    }

    public void PickUpCoin() {
        // TODO: kaching!

        coinCounter++;
        coinCounterText.text = "" + coinCounter;
        if (coinCounter >= 3) {
            // Autobuy a creature
        }
    }

    // TODO: restart. go to title scene, reset game counters, etc.

    // TODO: start game: load level 1

    // TODO: load level. find canvases and stuff...

    // TODO: game over



}
