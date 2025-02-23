using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil : MonoBehaviour
{
    [SerializeField] Transform leftUpperBorder;
    [SerializeField] Transform rightDownBorder;
    [SerializeField] float speed;

    private Rigidbody2D rb;
    private PlayerMovement player;
    DialogueController dialogueController;
    bool preparing = false;
    bool running = false;
    Vector2 destination;
    Animator animator;
    SpriteRenderer spriteRenderer;
    public static bool meet;
    // for fun
    [SerializeField] AudioClip kissClip;
    [SerializeField] GameObject kissPrefab;
    [SerializeField] GameDialogue firstKissDialogue;

    // shit code
    [SerializeField] List<GameObject> gameObjectsToActivate;
    [SerializeField] List<GameObject> gameObjectsToDeactivate;
    [SerializeField] ParticleSystem damageParticles;
    LavaController lavaController;
    // end of shit code

    float timeSleep = 0f;
    float lastEntranceLevel = -29.5f;
    bool falling = false;
    bool onBottom = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        player = FindObjectOfType<PlayerMovement>();
        dialogueController = FindObjectOfType<DialogueController>();
        lavaController = FindObjectOfType<LavaController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (falling)
        {
            // Debug.Log("falling, transform.position.y=" + transform.position.y.ToString() + " lastEntranceLevel="+lastEntranceLevel.ToString());
            if (transform.position.y < lastEntranceLevel)
            {
                rb.gravityScale = 0f;
                rb.velocity = Vector2.zero;
                transform.position = new Vector3(21.5f, lastEntranceLevel, transform.position.z);
                falling = false;
                Debug.Log("fell, transform.position.y=" + transform.position.y.ToString() + " lastEntranceLevel=" + lastEntranceLevel.ToString());
                onBottom = true;
            }
            return;
        }

        if (onBottom)
        {
            if (player.transform.position.y < -29f)
            {
                // bump into wall
                // Debug.Log("will bump into wall");
                rb.velocity = Vector2.right * 5;
            }

            return;
        }

        if (timeSleep > 0)
        {
            timeSleep -= Time.deltaTime;
            return;
        }
        Rotate();
        if (running)
        {
            Run();
            return;
        }
        else if (preparing)
        {
            return;
        }
        else if (CheckUserInsideArea())
        {
            // set red eyes anim
            preparing = true;
            destination = new Vector2(player.transform.position.x, 0);
            animator.SetBool("isAngry", true);
            animator.SetBool("isMoving", false);
            StartCoroutine(DefineDestinationPoint());
        }
        else
        {
            // no user so we move to the center gently
            // MoveCenter();
        }
    }

    private bool CheckUserInsideArea()
    {
        Vector2 playerPos = player.transform.position;
        return leftUpperBorder.position.x < playerPos.x && playerPos.x < rightDownBorder.position.x && leftUpperBorder.position.y > playerPos.y && playerPos.y > rightDownBorder.position.y;
    }

    private IEnumerator DefineDestinationPoint()
    {
        yield return new WaitForSeconds(2f);
        if (CheckUserInsideArea())
        {
            // set running anim
            destination = new Vector2(player.transform.position.x, 0);
            running = true;
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isAngry", false);
        }
        preparing = false;
    }

    private void Run()
    {
        if (Mathf.Abs(destination.x - transform.position.x) < 0.01f)
        {
            running = false;
            animator.SetBool("isRunning", false);
            animator.SetBool("isMoving", false);
            animator.SetBool("isAngry", false);
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }
        float direction = Mathf.Sign(destination.x - transform.position.x);
        rb.velocity = new Vector2(speed * direction, rb.velocity.y);
    }

    // private void MoveCenter()
    // {
    //     if (Mathf.Abs(startPoint.x - transform.position.x) < 0.01f)
    //     {
    //         running = false;
    //         animator.SetBool("isMoving", false);
    //         rb.velocity = new Vector2(0, rb.velocity.y);
    //         return;
    //     }
    //     animator.SetBool("isMoving", true);
    //     float direction = Mathf.Sign(startPoint.x - transform.position.x);
    //     rb.velocity = new Vector2(speed * direction, rb.velocity.y);
    // }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))  // Check if it's a Player
        {
            if (onBottom) {
                return;
            }
            Debug.Log("warning met player");
            // todo kiss a player
            running = false;
            preparing = false;
            rb.velocity = new Vector2(0, rb.velocity.y);
            timeSleep = 1f;
            animator.SetBool("isRunning", false);
            animator.SetBool("isAngry", false);
            SoundFXManager.Instance.PlaySoundFXClip(kissClip, transform);
            GameObject kissObject = Instantiate(kissPrefab, (transform.position + collider.transform.position) / 2, Quaternion.identity);
            Destroy(kissObject, 1f);
            if (meet)
            {
                // skip
            }
            else
            {
                dialogueController.EnqueueParagraph(firstKissDialogue);
                meet = true;
            }
        }
        else if (collider.gameObject.CompareTag("BadWall"))
        {
            // todo - break it and die;(
            Debug.LogWarning("Break Lava");
            SoundFXManager.Instance.PlayDeathSound(transform);
            Instantiate(damageParticles, transform.position, Quaternion.identity);

            foreach (GameObject gameObject in gameObjectsToActivate) {
                gameObject.SetActive(true);
            }

            foreach (GameObject gameObject in gameObjectsToDeactivate) {
                gameObject.SetActive(false);
            }

            lavaController.StartFlood();

            gameObject.SetActive(false);
        }
        else if (collider.gameObject.CompareTag("DevilFall"))
        {
            Fall();
        }
    }
    void Fall()
    {
        rb.gravityScale = 1f;
        falling = true;
        rb.velocity = Vector2.left * 1.5f;
    }
    void Rotate()
    {
        float direction = Mathf.Sign(destination.x - transform.position.x);
        if (Time.timeScale == 0)
        {
            return;
        }
        if (spriteRenderer.flipX != Mathf.Sign(direction) < 0)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }

}
