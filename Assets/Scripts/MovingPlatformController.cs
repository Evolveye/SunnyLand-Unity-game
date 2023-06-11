using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour {
    public int requiredKeys = 0;
    public float moveRangeX = 0f;
    public float moveRangeY = 0f;
    public float moveSpeed = 1.5f;
    private Vector3[] waypoints = new Vector3[ 2 ];
    private int currentWaypoint = 0;

    private AudioSource soundSource;

    private void Awake() {
        soundSource = GetComponent<AudioSource>();

        waypoints[ 0 ] = transform.position;
        waypoints[ 1 ] = new Vector3( transform.position.x + moveRangeX, transform.position.y + moveRangeY, 0 );
    }

    void Start() { }

    void Update() {
        if (GameManager.CheckNotRunning()) return;

        float distance = Vector2.Distance( transform.position, waypoints[ currentWaypoint ] );

        if (distance < 0.1f) {
            int moduloVal = waypoints.Length;
            int foundKeys = GameManager.GetFoundKeysCount();

            if (requiredKeys > foundKeys) return;

            currentWaypoint = (currentWaypoint + 1) % moduloVal;
        }

        Vector2 newPosition = Vector2.MoveTowards( transform.position, waypoints[ currentWaypoint ], moveSpeed * Time.deltaTime );
        transform.position = newPosition;
    }

    private void OnTriggerEnter2D( Collider2D collision ) {
        if (collision.CompareTag( "Player" )) {
            soundSource.Play();
        }
    }

    private void OnTriggerExit2D( Collider2D collision ) {
        if (collision.CompareTag( "Player" )) {
            soundSource.Stop();
        }
    }
}
