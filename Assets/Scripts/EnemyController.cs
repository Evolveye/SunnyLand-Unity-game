using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    private float startPositionX = 0.0f;
    public float moveRange = 1.0f;
    public float moveSpeed = 1.0f;
    private bool isFacingRight = false;
    private Animator animator;

    private void Awake() {
        startPositionX = transform.position.x;
        animator = GetComponent<Animator>();
    }

    void Start() { }
    void Update() {
        if (GameManager.CheckNotRunning()) return;
        if (moveRange == 0) return;

        if (isFacingRight) {
            MoveRight();
            if (transform.position.x - startPositionX >= moveRange) Flip();
        } else {
            MoveLeft();
            if (transform.position.x <= startPositionX) Flip();
        }
    }

    private void MoveRight() {
        if (!isFacingRight) Flip();
        transform.Translate( moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World );
    }

    private void MoveLeft() {
        if (isFacingRight) Flip();
        transform.Translate( -moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World );
    }

    private void Flip() {
        isFacingRight = !isFacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;

        transform.localScale = theScale;
    }
    public void Death() {
        animator.SetBool( "isDead", true );
        StartCoroutine( KillOnAnimationEnd() );
    }

    IEnumerator KillOnAnimationEnd() {
        if (animator.name.Contains( "Eagle" )) yield return new WaitForSeconds( 0.5f );

        gameObject.SetActive( false );
    }

    private void OnTriggerEnter2D( Collider2D collision ) {
    }
}
