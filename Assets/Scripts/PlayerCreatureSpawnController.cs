using UnityEngine;

public class PlayerCreatureSpawnController : MonoBehaviour {
    [SerializeField] GameObject[] CreaturePrefabs;

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
    }
}
