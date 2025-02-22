using UnityEngine;

public class Finish : BaseTriggerHandler
{
    [SerializeField] GameDialogue finishDialogue;
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
            levelController.OnFinishEnter();
        }

        // start conversation
        if (finishDialogue != null)
        {
            dialogueController.EnqueueParagraph(finishDialogue);
        }
        else
        {
            Debug.LogWarning("finishDialogue is NULL!");
        }
    }
}
