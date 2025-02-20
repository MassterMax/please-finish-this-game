using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : BaseTriggerHandler
{
    [SerializeField] GameDialogue deathDialogue;
    DialogueController dialogueController;
    LevelController levelController;

    void Awake()
    {
        levelController = FindObjectOfType<LevelController>();
        if (levelController == null)
        {
            Debug.LogError("Can't find levelController!");
        }
        dialogueController = FindObjectOfType<DialogueController>();
        if (dialogueController == null)
        {
            Debug.LogError("Can't find dialogueController!");
        }
    }

    public override void OnTrigger()
    {
        // start conversation
        dialogueController.EnqueueParagraph(deathDialogue);
        if (levelController == null)
        {
            Debug.LogError("Can't find levelController!");
        }
        else
        {
            levelController.OnTrueDeath();
        }
    }
}
