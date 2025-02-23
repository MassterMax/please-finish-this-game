using System.Collections.Generic;
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

    // straw
    bool nearStraw = false;
    GameObject straw;
    bool holdStraw = false;
    [SerializeField] GameDialogue strawDialoguePickup;
    [SerializeField] GameDialogue strawDialogueUse;
    [SerializeField] GameObject lastfinish;
    bool startDrinking = false;
    [SerializeField] AudioClip strawSound;
    LavaController lavaController;
    DialogueController dialogueController;
    int drinkCounter = 10;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        dialogueController = FindObjectOfType<DialogueController>();
    }

    public float GetYVelocity()
    {
        return rb.velocity.y;
    }

    public void ResetPlayerPos(Vector2 position)
    {
        transform.position = position;
    }

    public void AllowPlayerMove()
    {
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

        if (Input.GetKeyDown(KeyCode.E)) {
            if (!holdStraw && nearStraw) {
                holdStraw = true;
                straw.transform.SetParent(transform);
                straw.transform.localPosition = Vector2.right * 0.3f;
            } else if (holdStraw) {
                // sound of drinking
                SoundFXManager.Instance.PlaySoundFXClip(strawSound, transform);
                if (lavaController.PlayerNearLava(transform.position)) {
                    if (!startDrinking) {
                        startDrinking = true;
                        lastfinish.SetActive(false);
                        dialogueController.EnqueueParagraph(strawDialogueUse);
                    }
                    transform.localScale *= 1.1f;
                    straw.transform.localScale /= 1.1f;
                    drinkCounter -= 1;

                    if (drinkCounter == 0) {
                        holdStraw = false;
                        Debug.Log("call OnTrueDeath");
                        FindObjectOfType<LevelController>().OnTrueDeath();
                        return;
                    }
                }
            }
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

    void OnTriggerEnter2D(Collider2D collider)
    {
        // Debug.Log(collider.gameObject.tag);
        if (!holdStraw && collider.gameObject.CompareTag("Straw"))
        {
            nearStraw = true;
            straw = collider.gameObject;
            // todo replic
            dialogueController.EnqueueParagraph(strawDialoguePickup);
            lavaController = FindObjectOfType<LavaController>();
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        // Debug.Log(collision.gameObject.tag);
        if (!holdStraw && collider.gameObject.CompareTag("Straw"))
        {
            nearStraw = false;
        }
    }
}
