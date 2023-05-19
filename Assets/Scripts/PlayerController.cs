using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header( "Movement parameters" )]
    [Range( 0.01f, 20.0f )][SerializeField] private float moveSpeed = 4.0f;
    [Space( 10 )]
    [Range( 0.01f, 20.0f )][SerializeField] private float jumpForce = 1.0f;

    private Rigidbody2D rigidBody;
    private Animator animator;

    public LayerMask groundLayer;

    const bool DEBUG = false;
    const float rayLength = 1.5f;
    const float groundedVectorOffset = 0.3f;

    private Vector2 startPosition = new();
    private bool isWalking = false;
    private bool isFacingRight = true;
    private int lives = 3;
    private int keysNumber = 3;

    // Start is called before the first frame update
    void Start() { }

    private void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;

        Debug.Log( "Game have been started" );
        //Debug.Log( $" - initial score: {GameManager.GetScore()}" );
        Debug.Log( $" - initial lives: {lives}" );
        //Debug.Log( $" - initial found keys: {keysFound}" );
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.CheckPause()) return;

        isWalking = false;
        float moveX = 0;

        if (Input.GetKey( KeyCode.RightArrow ) || Input.GetKey( KeyCode.D )) moveX += moveSpeed;
        if (Input.GetKey( KeyCode.LeftArrow ) || Input.GetKey( KeyCode.A )) moveX -= moveSpeed;
        if (Input.GetKeyDown( KeyCode.UpArrow ) || Input.GetKeyDown( KeyCode.W ) || Input.GetMouseButtonDown( 0 ) || Input.GetKeyDown( KeyCode.Space )) Jump();

        if (moveX != 0) {
            if ((moveX > 0 && !isFacingRight) || (moveX < 0 && isFacingRight)) Flip();

            isWalking = true;
            transform.Translate( moveX * Time.deltaTime, 0.0f, 0.0f, Space.World );
        }

        if (DEBUG) {
            var leftOffset = this.transform.position;
            leftOffset.x -= groundedVectorOffset;

            var rightOffset = this.transform.position;
            rightOffset.x += groundedVectorOffset;

            Debug.DrawRay( leftOffset, rayLength * Vector3.down, Color.white, 1, false );
            Debug.DrawRay( rightOffset, rayLength * Vector3.down, Color.white, 1, false );
        }

        animator.SetBool( "isGrounded", IsGrounded() );
        animator.SetBool( "isWalking", isWalking );
    }

    private bool IsGrounded() {
        var leftOffset = this.transform.position;
        leftOffset.x -= groundedVectorOffset;

        var rightOffset = this.transform.position;
        rightOffset.x += groundedVectorOffset;

        return Physics2D.Raycast( leftOffset, Vector2.down, rayLength, groundLayer.value )
            || Physics2D.Raycast( rightOffset, Vector2.down, rayLength, groundLayer.value );
    }

    private void Jump( bool force = false ) => Jump( 1, force );
    private void Jump( float multiplier, bool force = false ) {
        if (!IsGrounded() && !force) return;

        if (force) {
            Vector3 v = rigidBody.velocity;
            v.y = 0;
            rigidBody.velocity = v;
        }

        rigidBody.AddForce( Vector2.up * jumpForce * multiplier, ForceMode2D.Impulse );
    }

    private void Flip() {
        isFacingRight = !isFacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;

        transform.localScale = theScale;
    }

    private void Death() {
        lives -= 1;

        if (lives == 0) {
            Debug.Log( "Killed by enemy! Game over" );
        } else {
            Debug.Log( $"Killed by enemy! Current lives count is {lives}" );
            transform.position = startPosition;
        }
    }

    private void OnTriggerEnter2D( Collider2D collision ) {
        if (collision.CompareTag( "Bonus" )) {
            GameManager.AddPoints( 10 );

            collision.gameObject.SetActive( false );

            return;
        }

        if (collision.CompareTag( "Life" )) {
            lives += 1;
            Debug.Log( $"Life picked up. Actual lives count: {lives}" );

            collision.gameObject.SetActive( false );

            return;
        }

        if (collision.CompareTag( "Key" )) {
            collision.gameObject.SetActive( false );

            GameManager.AddKey();

            return;
        }

        if (collision.CompareTag( "MovingPlatform" )) {
            transform.SetParent( collision.transform );
            return;
        }

        if (collision.CompareTag( "Enemy" )) {
            if (collision.gameObject.transform.position.y > transform.position.y) {
                Death();
                return;
            }

            Jump( 1.25f, true );
            GameManager.AddPoints( 20 );
            Debug.Log( "Killed an enemy!" );

            return;
        }

        if (collision.CompareTag( "Finish" )) {
            if (GameManager.FoundAllKeys()) {
                Debug.Log( $"Final score: {GameManager.GetScore()}" );
                Debug.Log( "Level 1 finished!" );
            } else {
                Debug.Log( "You don't have enough keys to complete level" );
            }

            return;
        }

        if (collision.CompareTag( "FallLevel" )) {
            Death();
            return;
        }
    }

    private void OnTriggerExit2D( Collider2D collision ) {
        if (collision.CompareTag( "MovingPlatform" )) {
            transform.SetParent( null );
            return;
        }
    }
}
