using System.Collections.Generic;
using UnityEngine;

// Instantly changes things
public class PlayerVelocityTrigger : BaseTriggerHandler
{
    [SerializeField] float desiredVelocity;
    [SerializeField] GameObject objectToActivate;
    [SerializeField] GameDialogue activationTriggerDialogue;
    [SerializeField] GameDialogue alternativeDialogue;
    DialogueController dialogueController;

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
        if (LevelController.Instance.GetPlayerYVelocity() < desiredVelocity)
        {
            dialogueController.EnqueueParagraph(alternativeDialogue);
            return;
        }
        dialogueController.EnqueueParagraph(activationTriggerDialogue);
        LevelController.Instance.OnTrueDeath();
        gameObject.SetActive(false);
    }
}
