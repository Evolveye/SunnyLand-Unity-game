using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour {
    [SerializeField] private GameObject[] waypoints;
    [SerializeField] private bool dependsFromPlayerKeys;
    [SerializeField] private float speed = 1.0f;

    private int currentWaypoint = 0;
    private AudioSource soundSource;

    void Start() { }
    void Awake() {
        transform.position = waypoints[ 0 ].transform.position;
        soundSource = GetComponent<AudioSource>();
    }

    void Update() { 
        if (GameManager.CheckNotRunning()) return;
        float distance = Vector2.Distance( transform.position, waypoints[ currentWaypoint ].transform.position );

        if (distance < 0.1f) {
            int moduloVal = waypoints.Length;
            int foundKeys = GameManager.GetFoundKeysCount();

            if (dependsFromPlayerKeys && moduloVal > foundKeys) moduloVal = foundKeys + 1;

            currentWaypoint = (currentWaypoint + 1) % moduloVal;
        }

        Vector2 newPosition = Vector2.MoveTowards( transform.position, waypoints[ currentWaypoint ].transform.position, speed * Time.deltaTime );
        transform.position = newPosition;
    }

    private void OnTriggerEnter2D( Collider2D collision ) {
        if (collision.CompareTag( "Player" )) {
            if (collision.gameObject.transform.parent == this) soundSource.Play();
        }
    }

    private void OnTriggerExit2D( Collider2D collision ) {
        if (collision.CompareTag( "Player" )) {
            soundSource.Stop();
        }
    }
}
