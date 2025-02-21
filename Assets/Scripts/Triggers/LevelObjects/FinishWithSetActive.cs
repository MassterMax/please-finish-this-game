using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishWithSetActive : BaseTriggerHandler
{
    [SerializeField] GameDialogue finishWithSetActiveDialogue;
    DialogueController dialogueController;
    LevelController levelController;
    [SerializeField] List<GameObject> objectsToActivate;
    [SerializeField] List<GameObject> objectsToDeactivate;
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
        levelController.OnFinishEnter();

        // start conversation
        dialogueController.EnqueueParagraph(finishWithSetActiveDialogue);

        StartCoroutine(DoAfterPlayerReset());
    }


    private IEnumerator DoAfterPlayerReset()
    {
        while (!levelController.IsPlayerReset()) {
            yield return new WaitForFixedUpdate();
        }

        foreach (GameObject gameObject in objectsToActivate)
        {
            gameObject.SetActive(true);
        }
        foreach (GameObject gameObject in objectsToDeactivate)
        {
            gameObject.SetActive(false);
        }
        
        // shutdown itself
        gameObject.SetActive(false);
    }
}
