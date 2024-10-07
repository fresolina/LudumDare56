using UnityEngine;

public class OrderGiver : MonoBehaviour {
    private GameObject player;

    private AudioSource audioSource;
    [SerializeField] AudioClip spawnSound;
    [SerializeField] AudioClip orderSound;
    [SerializeField] AudioClip recallSound;


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

    private int numberOfCreatures() {
        return GameObject.FindGameObjectsWithTag("Creature").Length;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (numberOfCreatures() == 0) {
                return;
            }

            // left click
            replaceTarget("Target1");

            // TODO: less repetition. require some working takers that are in a perceptive state
            audioSource.Stop();
            audioSource.pitch = Random.Range(1.0f, 1.25f);
            audioSource.PlayOneShot(orderSound);
        } else if (Input.GetMouseButtonDown(1)) {
            if (numberOfCreatures() == 0) {
                return;
            }

            // right click
            clearTarget("Target1");

            audioSource.Stop();
            audioSource.pitch = Random.Range(1.0f, 1.5f);
            audioSource.PlayOneShot(recallSound);
        }

        /*
                if (Input.GetKeyDown(KeyCode.Alpha1)) {
                    if (updatePlayer() != null) {
                        player.BroadcastMessage("SpawnCreature", 0, SendMessageOptions.RequireReceiver);

                        // TODO: should maybe have a cooldown for less spammy audio
                        audioSource.Stop();
                        audioSource.pitch = Random.Range(1.0f, 2.0f);
                        audioSource.PlayOneShot(spawnSound);
                    }
                }
                if (Input.GetKeyDown(KeyCode.Alpha2)) {
                    if (updatePlayer() != null) {
                        player.BroadcastMessage("SpawnCreature", 1, SendMessageOptions.RequireReceiver);
                    }
                }
            */
    }

}
