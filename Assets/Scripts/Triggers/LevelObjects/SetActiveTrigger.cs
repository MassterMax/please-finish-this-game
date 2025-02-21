using System.Collections.Generic;
using UnityEngine;

// Instantly changes things
public class SetActiveTrigger : BaseTriggerHandler
{
    [SerializeField] GameDialogue activationTriggerDialogue;
    DialogueController dialogueController;
    [SerializeField] List<GameObject> objectsToActivate;
    [SerializeField] List<GameObject> objectsToDeactivate;

    [SerializeField] bool onlyOne = true;
    AudioClip activationClip;
    private bool activated = false;
    void Awake()
    {
        activationClip = Resources.Load<AudioClip>("Sounds/activation");
        dialogueController = FindObjectOfType<DialogueController>();
        if (dialogueController == null)
        {
            Debug.LogError("Can't find dialogueController!");
        }
    }

    public override void OnTrigger()
    {
        if (onlyOne && activated)
        {
            return;
        }
        SoundFXManager.Instance.PlaySoundFXClip(activationClip, transform);
        activated = true;
        dialogueController.EnqueueParagraph(activationTriggerDialogue);
        foreach (GameObject gameObject in objectsToActivate)
        {
            gameObject.SetActive(true);
        }
        foreach (GameObject gameObject in objectsToDeactivate)
        {
            gameObject.SetActive(false);
        }
    }
}
