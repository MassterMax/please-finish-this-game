using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    [SerializeField] List<Coin> coins;
    [SerializeField] GameObject plateInactive;
    [SerializeField] GameObject plateActive;
    int activationCount = 0;
    DialogueController dialogueController;
    [SerializeField] GameObject finishUp;
    [SerializeField] GameObject straw;

    private bool activatedOnce = false;

    [SerializeField] GameDialogue firstActivationDialogue;
    void Start()
    {
        plateActive.SetActive(false);
        plateInactive.SetActive(true);
        dialogueController = FindObjectOfType<DialogueController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Activate()
    {
        foreach (Coin coin in coins)
        {
            coin.Activate();
        }
    }

    public void Unactivate()
    {
        foreach (Coin coin in coins)
        {
            coin.ResetPos();
            coin.DeactivateGravity();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        activationCount += 1;

        if (activationCount == 1)
        {
            if (!activatedOnce)
            {
                activatedOnce = true;
                dialogueController.EnqueueParagraph(firstActivationDialogue);
            }
            Activate();
            plateActive.SetActive(true);
            plateInactive.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        activationCount -= 1;

        if (activationCount == 0)
        {
            Unactivate();
            plateActive.SetActive(false);
            plateInactive.SetActive(true);
        }
    }

    public void TurnOffCoins() {
        foreach (Coin coin in coins)
        {
            coin.gameObject.SetActive(false);
        }

        // todo spawn straw near second coin (and destory finish)
        finishUp.SetActive(false);
        straw.SetActive(true);
    }
}
