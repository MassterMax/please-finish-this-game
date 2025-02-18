using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] LayerMask groundLayer;

    private Rigidbody2D rb;

    private float coyoteTimer = 0f;
    BoxCollider2D boxCollider;
    [SerializeField] float groundedCastDistance = 0.05f;
    [SerializeField] BoxCollider2D tmpBoxCollider2D;  // todo - remove
    // public static float globalGravity = -9.81f;

    [SerializeField] float objectGravityScale = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        HandleGravityScale();
    }

    void Update()
    {
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
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // short jump for early space release
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);
        }

        HandleGravityScale();
    }


    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void HandleGravityScale()
    {
        // for fast falling
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = objectGravityScale * 1.5f;
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
