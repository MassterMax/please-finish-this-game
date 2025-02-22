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

    float timeSleep = 0f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        player = FindObjectOfType<PlayerMovement>();
        dialogueController = FindObjectOfType<DialogueController>();
    }

    // Update is called once per frame
    void Update()
    {
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
        if (CheckUserInsideArea())
        {
            // set red eyes anim
            preparing = true;
            StartCoroutine(DefineDestinationPoint());
        }
    }

    private bool CheckUserInsideArea()
    {
        Vector2 playerPos = player.transform.position;
        return leftUpperBorder.position.x < playerPos.x && playerPos.x < rightDownBorder.position.x && leftUpperBorder.position.y > playerPos.y && playerPos.y > rightDownBorder.position.y;
    }

    private IEnumerator DefineDestinationPoint()
    {
        destination = new Vector2(player.transform.position.x, 0);
        animator.SetBool("isAngry", true);
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
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }
        float direction = Mathf.Sign(destination.x - transform.position.x);
        rb.velocity = new Vector2(speed * direction, rb.velocity.y);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))  // Check if it's a Player
        {
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
        }
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
