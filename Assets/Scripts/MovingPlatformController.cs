using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour {
    public float moveRange = 10.0f;
    public float moveSpeed = 1.5f;
    private float startPositionX = 0.0f;
    private bool isFacingRight = false;

    private void Awake() {
        startPositionX = transform.position.x;
    }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() {
        if (isFacingRight) {
            MoveRight();
            if (transform.position.x >= moveRange) isFacingRight = !isFacingRight;
        } else {
            MoveLeft();
            if (transform.position.x <= startPositionX) isFacingRight = !isFacingRight;
        }
    }

    private void MoveRight() {
        transform.Translate( moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World );
    }

    private void MoveLeft() {
        transform.Translate( -moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World );
    }
}
