using UnityEngine;

public class CoinTrigger : MonoBehaviour {
    private GameController controller;

    void Awake() {
        controller = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public void OnTriggerEnter2D(Collider2D collider) {
        controller.PickUpCoin();
        Destroy(gameObject);
    }
}
