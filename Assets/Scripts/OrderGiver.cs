using UnityEngine;

public class OrderGiver : MonoBehaviour {
    private GameObject player;

    private AudioSource audioSource;
    [SerializeField] AudioClip spawnSound;
    [SerializeField] AudioClip orderSound;


    public class Target {
        public string name;
        public Vector2 position;
    }

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    void replaceTarget(string targetName) {
        Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Target target = new Target();
        target.name = targetName;
        target.position = position;

        foreach (var creature in GameObject.FindGameObjectsWithTag("Creature")) {
            creature.BroadcastMessage("SetTarget", target);
        }
    }

    void clearTarget(string targetName) {
        foreach (var creature in GameObject.FindGameObjectsWithTag("Creature")) {
            creature.BroadcastMessage("ClearTarget", targetName);
        }
    }

    private GameObject updatePlayer() {
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        return player;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            // left click
            replaceTarget("Target1");
            audioSource.pitch = Random.Range(0.75f, 1.25f);
            audioSource.PlayOneShot(orderSound);
        } else if (Input.GetMouseButtonDown(1)) {
            // right click
            replaceTarget("Target2");
        } else if (Input.GetMouseButtonDown(2)) {
            // middle click
            replaceTarget("Target3");
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            clearTarget("Target1");
            clearTarget("Target2");
            clearTarget("Target3");
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            if (updatePlayer() != null) {
                player.BroadcastMessage("SpawnCreature", 0, SendMessageOptions.RequireReceiver);
                audioSource.pitch = Random.Range(1.0f, 2.0f);
                audioSource.PlayOneShot(spawnSound);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            if (updatePlayer() != null) {
                player.BroadcastMessage("SpawnCreature", 1, SendMessageOptions.RequireReceiver);
            }
        }
    }

}
