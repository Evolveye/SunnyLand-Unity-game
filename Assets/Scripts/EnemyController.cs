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
        startPositionX = this.transform.position.x;
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update() {}

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

    IEnumerator KillOnAnimationEnd() {
        yield return new WaitForSeconds( 0.5f );
        gameObject.SetActive( false );
    }

    private void OnTriggerEnter2D( Collider2D collision ) {
        if (collision.CompareTag( "Player" )) {
            if (collision.gameObject.transform.position.y > transform.position.y) {
                animator.SetBool( "isDead", true );
                StartCoroutine( KillOnAnimationEnd() );
            }

            return;
        }
    }
}
