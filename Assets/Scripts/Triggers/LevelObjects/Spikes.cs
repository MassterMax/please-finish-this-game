using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : BaseTriggerHandler
{
    [SerializeField] GameDialogue spikesDialogue;
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
        if (levelController == null)
        {
            Debug.LogError("Can't find levelController!");
        }
        else
        {
            levelController.OnTrueDeath();
        }

        // start conversation
        dialogueController.EnqueueParagraph(spikesDialogue);
    }
}
