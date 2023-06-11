using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header( "Movement parameters" )]
    [Range( 0.01f, 20.0f )][SerializeField] private float moveSpeed = 4.0f;
    [Range( 0.01f, 20.0f )][SerializeField] private float jumpForce = 1.0f;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip bonusSound;
    [SerializeField] private AudioClip keySound;
    [SerializeField] private AudioClip lifeSound;
    [SerializeField] private AudioClip movingOnPlatformSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip killSound;

    private Rigidbody2D rigidBody;
    private Animator animator;
    private AudioSource soundSource;

    public LayerMask groundLayer;

    const float rayLength = 1.0f;
    const float groundedVectorOffset = 0.3f;

    private Vector2 startPosition = new();
    private bool isWalking = false;
    private bool isFacingRight = true;
    private int lives = 3;
    private float jumpFactor = 1f;
    private bool hittable = true;

    void Start() { }
    private void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        soundSource = GetComponent<AudioSource>();

        startPosition = transform.position;

        GameManager.SetHelth( lives );
    }

    void Update() {
        if (GameManager.CheckNotRunning()) return;

        rigidBody.isKinematic = false;
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

        var leftOffset = this.transform.position;
        leftOffset.x -= groundedVectorOffset;

        var rightOffset = this.transform.position;
        rightOffset.x += groundedVectorOffset;

        Debug.DrawRay( leftOffset, rayLength * Vector3.down, Color.white, 0.2f, false );
        Debug.DrawRay( rightOffset, rayLength * Vector3.down, Color.white, 0.2f, false );

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

    public void Jump( bool force = false ) => Jump( 1, force );
    public void Jump( float multiplier, bool force = false ) {
        if (!IsGrounded() && !force && !GameManager.CheckCheatmode()) return;

        if (force) {
            Vector3 v = rigidBody.velocity;
            v.y = 0;
            rigidBody.velocity = v;
        } else {
            //Debug.Log( $"v.y={rigidBody.velocity.y}" );
            //if (rigidBody.velocity.y != 0) return;
            if (jumpSound != null) soundSource.PlayOneShot( jumpSound, AudioListener.volume );
        }

        rigidBody.AddForce( Vector2.up * jumpForce * multiplier * jumpFactor, ForceMode2D.Impulse );
    }

    private void Flip() {
        isFacingRight = !isFacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;

        transform.localScale = theScale;
    }

    private void Death() {
        if (!hittable) return;
        if (deathSound != null) soundSource.PlayOneShot( deathSound, AudioListener.volume );

        GameManager.AddHealth( -1 );
        lives += -1;

        hittable = false;

        Task.Run( async () => {
            await Task.Delay( 1000 );
            hittable = true;
        } );

        if (lives == 0) {
            Debug.Log( "Killed by enemy! Game over" );
            GameManager.FinishGame();
        } else {
            transform.position = startPosition;
        }
    }

    private void OnTriggerEnter2D( Collider2D collision ) {
        if (!collision.gameObject.activeInHierarchy) return;

        if (collision.CompareTag( "BigJump" )) {
            jumpFactor = 1.5f;
            return;
        }

        if (collision.CompareTag( "Bonus" )) {
            if (bonusSound != null) soundSource.PlayOneShot( bonusSound, AudioListener.volume );

            GameManager.AddPoints( 10 );

            collision.gameObject.SetActive( false );

            return;
        }

        if (collision.CompareTag( "Life" )) {
            if (lifeSound != null) soundSource.PlayOneShot( lifeSound, AudioListener.volume );
            GameManager.AddHealth( 1 );
            lives += 1;

            collision.gameObject.SetActive( false );

            return;
        }

        if (collision.CompareTag( "Key" )) {
            if (keySound!= null) soundSource.PlayOneShot( keySound, AudioListener.volume );
            collision.gameObject.SetActive( false );

            GameManager.AddKey();

            return;
        }

        if (collision.CompareTag( "MovingPlatform" )) {
            transform.SetParent( collision.transform );
            return;
        }

        if (collision.CompareTag( "Enemy" )) {
            var enemy = collision.GetComponent<EnemyController>();

            if (enemy == null) return;
            if (rigidBody.velocity.y >= 0 || collision.gameObject.transform.position.y > transform.position.y - 0.5) {
                Death();
                return;
            }

            if (killSound != null) soundSource.PlayOneShot( killSound, AudioListener.volume );
            enemy.Death();

            GameManager.AddPoints( 20 );
            GameManager.AddKilledEnemies( 1 );
            Jump( 1.25f, true );

            return;
        }

        if (collision.CompareTag( "Finish" )) {
            if (GameManager.FoundAllKeys()) {
                for (int i = 0; i < lives; i++) GameManager.AddPoints( 150 );

                Debug.Log( $"Final score: {GameManager.GetScore()}" );
                Debug.Log( "Level 1 finished!" );
                GameManager.CompleteLevel();
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

        if (collision.CompareTag( "BigJump" )) {
            jumpFactor = 1;
            return;
        }
    }
}
