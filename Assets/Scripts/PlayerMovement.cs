using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] ParticleSystem damageParticles;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpForce = 20f;
    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] LayerMask groundLayer;

    private Rigidbody2D rb;

    private float coyoteTimer = 0f;
    BoxCollider2D boxCollider;
    [SerializeField] float groundedCastDistance = 0.05f;
    [SerializeField] BoxCollider2D tmpBoxCollider2D;  // todo - remove

    [SerializeField] float objectGravityScale = 8f;
    [SerializeField] float gravityScaleFactor = 1.5f;

    // Animation
    Animator animator;
    SpriteRenderer spriteRenderer;

    // sounds
    [SerializeField] AudioClip jumpSound;

    bool canMove;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public float GetYVelocity() {
        return rb.velocity.y;
    }

    public void ResetPlayerPos(Vector2 position)
    {
        transform.position = position;
    }

    public void AllowPlayerMove() {
        canMove = true;
        gameObject.SetActive(true);
    }

    public void StopPlayer(bool active)
    {
        if (!active)
        {
            // dead
            SpawnDamageParticles();
        }
        rb.velocity = new Vector2(0, 0);
        canMove = false;
        animator.SetBool("is_moving", false);
        gameObject.SetActive(active);
    }

    private void SpawnDamageParticles()
    {
        Instantiate(damageParticles, transform.position, Quaternion.identity);
    }

    void Update()
    {
        if (!canMove)
        {
            return;
        }

        Move();

        if (IsGrounded() && rb.velocity.y <= 0.01f)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        // jump logic
        if (Input.GetButtonDown("Jump") && coyoteTimer > 0)
        {
            SoundFXManager.Instance.PlaySoundFXClip(jumpSound, transform);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // short jump for early space release
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);
        }

        // make vertical velocity 0 for small values
        if (!Input.GetButtonDown("Jump") && Mathf.Abs(rb.velocity.y) < 0.001f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        HandleGravityScale();
        Rotate();
    }


    void Move()
    {
        Vector2 direction = Vector2.zero;

        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector2.left;
        }

        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector2.right;
        }

        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        animator.SetBool("is_moving", direction.x != 0);

        // if (Mathf.Abs(moveInput) > IDLE_DELTA)
        // {
        //     rb.velocity = new Vector2(Mathf.Sign(moveInput) * moveSpeed, rb.velocity.y);
        // }
        // else
        // {
        //     rb.velocity = new Vector2(0, rb.velocity.y);
        // }

        // // set anim
        // animator.SetBool("is_moving", Mathf.Abs(moveInput) > IDLE_DELTA);
    }

    void Rotate()
    {
        float moveInput = Input.GetAxis("Horizontal");
        if (moveInput == 0 || Time.timeScale == 0)
        {
            return;
        }
        if (spriteRenderer.flipX != Mathf.Sign(moveInput) < 0)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }

    void HandleGravityScale()
    {
        // for fast falling
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = objectGravityScale * gravityScaleFactor;
        }
        else
        {
            rb.gravityScale = objectGravityScale;
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0,
            Vector2.down,
            groundedCastDistance,
            groundLayer
        );
        return raycastHit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(tmpBoxCollider2D.bounds.center + Vector3.down * groundedCastDistance, tmpBoxCollider2D.bounds.size);
    }
}
