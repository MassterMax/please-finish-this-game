using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 10f;
    // [SerializeField] float coyoteTime = 0.2f;
    [SerializeField] LayerMask groundLayer;

    private Rigidbody2D rb;

    // private float coyoteTimeTimer = 0f;
    BoxCollider2D boxCollider;
    [SerializeField] float groundedCastDistance = 0.05f;
    [SerializeField] BoxCollider2D tmpBoxCollider2D;  // todo - remove
    // public static float globalGravity = -9.81f;

    [SerializeField] float objectGravityScale = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        Move();

        bool isGrounded = IsGrounded();
        // if (isGrounded())
        // {
        //     // reset
        //     coyoteTimeTimer = coyoteTime;
        //     jumpUsed = false;
        // }
        // else
        // {
        //     coyoteTimeTimer -= Time.deltaTime;
        // }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        HandleGravityScale();
    }


    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        // coyoteTimeTimer = 0f;
    }

    void HandleGravityScale() {
        // for fast falling
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = objectGravityScale * 2f;
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
