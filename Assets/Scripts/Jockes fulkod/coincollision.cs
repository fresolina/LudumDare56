using UnityEngine;

public class coincollision : MonoBehaviour
{
    void OnCollisionEnter(Collision collision) {
        print("Collision Detected");
        if (collision.gameObject.name == "Sprite") { 
            Destroy(collision.gameObject);
        }
    }
}
