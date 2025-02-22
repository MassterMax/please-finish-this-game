using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Rigidbody2D rb;
    Vector2 startPos;

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
    }

    public void Activate()
    {
        rb.gravityScale = 1f;
        activated = true;
        StartCoroutine(DeactivateAfterTime());
    }

    private IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(1.12f);
        if (activated) {
            DeactivateGravity();
        }

        // while (activated)
        // {
        //     yield return new WaitForSeconds(1.12f);
        //     if (activated)
        //     {
        //         SoundFXManager.Instance.PlaySoundFXClip(audioClip, transform);
        //         DeactivateGravity();
        //     } else {
        //         break;
        //     }
        //     yield return new WaitForSeconds(0.1f);
        //     if (activated)
        //     {
        //         transform.position = startPos;
        //         rb.gravityScale = 1f;
        //     } else {
        //         break;
        //     }
        // }
    }

    public void DeactivateGravity()
    {
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
    }

    public void ResetPos()
    {
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
