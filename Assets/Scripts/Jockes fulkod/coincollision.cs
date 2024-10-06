using UnityEngine;

public class coincollision : MonoBehaviour
{
    public CoinCounter coinCounter;
    public void OnCollisionEnter2D(Collision2D collision) {       

        if (collision.gameObject.tag == "Coin") {
            coinCounter.pickedupcoin();
            Destroy(collision.gameObject);
           
        }
    }
}
