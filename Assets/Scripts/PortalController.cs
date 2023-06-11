using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour {
    [SerializeField] private AudioClip portalSound;
    public float portalToX = 0.0f;
    public float portalToY = 0.0f;

    private AudioSource soundSource;

    void Awake() {
        soundSource = GetComponent<AudioSource>();
    }
    void Start() { }
    void Update() {
    
    }

    private void OnTriggerEnter2D( Collider2D collision ) {
        if (collision.CompareTag( "Player" )) {
            if (portalSound != null) soundSource.PlayOneShot( portalSound, AudioListener.volume );
            collision.transform.position = new Vector3( portalToX, portalToY, 0 );
            return;
        }
    }
}
