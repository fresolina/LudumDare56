using UnityEngine;

public class PlayerCreatureSpawnController : MonoBehaviour {
    [SerializeField] GameObject[] CreaturePrefabs;

    [SerializeField] AudioClip spawnSound;
    private AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    // TODO:
    // -  spawn limits
    // - spawn costs
    // - spawn cooldowns
    // - spawn animations
    // - spawn sounds
    // - spawn offset

    public void SpawnCreature(int index) {
        if (index < 0 || index >= CreaturePrefabs.Length) {
            Debug.LogWarning("Invalid creature index: " + index);
            return;
        }

        GameObject creaturePrefab = CreaturePrefabs[index];
        GameObject creature = Instantiate(creaturePrefab, transform.position, Quaternion.identity);

        // TODO: should maybe have a cooldown for less spammy audio
        audioSource.Stop();
        audioSource.pitch = Random.Range(1.0f, 2.0f);
        audioSource.PlayOneShot(spawnSound);
    }
}
