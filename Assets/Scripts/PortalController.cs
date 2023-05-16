using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour {
    public float portalToX = 0.0f;
    public float portalToY = 0.0f;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerEnter2D( Collider2D collision ) {
        if (collision.CompareTag( "Player" )) {
            collision.transform.position = new Vector3( portalToX, portalToY, 0 );
            return;
        }
    }
}
