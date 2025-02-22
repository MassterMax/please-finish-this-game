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
    [SerializeField] bool shouldPlaySound = true;
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
        if (shouldPlaySound)
        {
            SoundFXManager.Instance.PlaySoundFXClip(activationClip, transform);
        }
        activated = true;
        if (activationTriggerDialogue != null)
        {
            dialogueController.EnqueueParagraph(activationTriggerDialogue);
        }
        else
        {
            Debug.LogWarning("activationTriggerDialogue is NULL!");
        }
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
