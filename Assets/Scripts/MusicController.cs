using UnityEngine;

public class MusicController : MonoBehaviour {
    [SerializeField] AudioClip mainTheme;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = mainTheme;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update() {

    }
}
