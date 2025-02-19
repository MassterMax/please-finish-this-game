using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogue;
    [SerializeField] float typingSpeed = 5f;
    [SerializeField] float timeBetweenParagraphs = 3f;

    string p;

    // typing
    private Coroutine paragraphCoroutine;
    private bool isTyping;

    private const string HTML_ALPHA = "<color=#00000000>";
    private const float MAX_TYPE_TIME = 0.1f;

    void Start() {
        gameObject.SetActive(false);
    }

    void Update()
    {
        
    }

    public void EnqueueParagraph(GameDialogue gameDialogue) {
        if (isTyping && !gameDialogue.force) {
            // gentle ignore
            Debug.Log("Gentle ignore gameDialogue: " + gameDialogue.ToString());
            return;
        }

        if (isTyping && gameDialogue.force) {
            dialogue.text = "";
            StopCoroutine(paragraphCoroutine);
        }

        // (re)start conversation
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        paragraphCoroutine = StartCoroutine(TypeParagraph(gameDialogue));
    }

    private IEnumerator TypeParagraph(GameDialogue gameDialogue) {
        isTyping = true;
        foreach (string p in gameDialogue.paragraphs) {
            if (!isTyping) {
                break;
            }
            // print logic
            dialogue.text = "";
            string originalText = p;
            int aplhaIndex = 0;
            foreach (char c in p.ToCharArray()) {
                aplhaIndex++;
                dialogue.text = originalText;
                string displayedText = dialogue.text.Insert(aplhaIndex, HTML_ALPHA);
                dialogue.text = displayedText;

                yield return new WaitForSeconds(MAX_TYPE_TIME / typingSpeed);
            }
            // sleep
            yield return new WaitForSeconds(timeBetweenParagraphs);
        }

        gameObject.SetActive(false);
        isTyping = false;
    }
}
