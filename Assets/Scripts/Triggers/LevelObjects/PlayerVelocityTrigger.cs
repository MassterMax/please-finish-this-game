using System.Collections.Generic;
using UnityEngine;

// Instantly changes things
public class PlayerVelocityTrigger : ConsequentTriggerHandler
{
    [SerializeField] float desiredVelocity;
    [SerializeField] GameObject objectToActivate;
    [SerializeField] GameDialogue activationTriggerDialogue;
    [SerializeField] GameDialogue alternativeDialogue;
    DialogueController dialogueController;

    private bool activated = false;
    private float playerSpeed;

    public void Awake()
    {
        dialogueController = FindObjectOfType<DialogueController>();
        if (dialogueController == null)
        {
            Debug.LogError("Can't find dialogueController!");
        }
    }
    public override void OnTrigger()
    {
        if (!activated) {return;}
        activated = false;
        if (Mathf.Abs(playerSpeed) < desiredVelocity)
        {
            dialogueController.EnqueueParagraph(alternativeDialogue);
            return;
        }
        dialogueController.EnqueueParagraph(activationTriggerDialogue);
        LevelController.Instance.OnTrueDeath();
        objectToActivate.SetActive(true);
        gameObject.SetActive(false);
    }

    public override void NextOnTrigger()
    {
        activated = true;
        playerSpeed = Mathf.Abs(LevelController.Instance.GetPlayerYVelocity());
        Debug.LogWarning("NextOnTrigger - PLAYER VELOCITY IS " + playerSpeed);
    }
}
