using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Rigidbody2D rb;
    Vector2 startPos;
    Vector2 finalPos;

    bool activated = false;

    DialogueController dialogueController;

    [SerializeField] GameDialogue activationDialogue;

    CoinController coinController;
    AudioClip audioClip;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        dialogueController = FindObjectOfType<DialogueController>();
        coinController = FindObjectOfType<CoinController>();
        audioClip = Resources.Load<AudioClip>("Sounds/pickupCoin");

        finalPos = startPos + Vector2.down * 6;
    }

    void Update()
    {
        if (activated) {
            if (Mathf.Abs(finalPos.y - transform.position.y) < 0.01f) {
                SoundFXManager.Instance.PlaySoundFXClip(audioClip, transform);
                rb.velocity = Vector2.zero;
                transform.position = startPos;
            }
        } else {

        }
    }

    public void Activate()
    {
        rb.gravityScale = 1f;
        activated = true;
    }

    // private IEnumerator DeactivateAfterTime()
    // {
    //     // yield return new WaitForSeconds(1.12f);
    //     // if (activated) {
    //     //     SoundFXManager.Instance.PlaySoundFXClip(audioClip, transform);
    //     //     DeactivateGravity();
    //     // }

    //     // while (activated)
    //     // {
    //     //     yield return new WaitForSeconds(1.12f);
    //     //     if (activated)
    //     //     {
    //     //         SoundFXManager.Instance.PlaySoundFXClip(audioClip, transform);
    //     //         DeactivateGravity();
    //     //     } else {
    //     //         break;
    //     //     }
    //     //     yield return new WaitForSeconds(0.1f);
    //     //     if (activated)
    //     //     {
    //     //         transform.position = startPos;
    //     //         rb.gravityScale = 1f;
    //     //     } else {
    //     //         break;
    //     //     }
    //     // }
    // }

    public void Deactivate()
    {
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
        transform.position = startPos;
        activated = false;
    }


    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))  // Check if it's a Player
        {
            Debug.Log("Coin met player");
            LevelController.Instance.OnTrueDeath();
            dialogueController.EnqueueParagraph(activationDialogue);
            coinController.TurnOffCoins();
        }
    }
}
