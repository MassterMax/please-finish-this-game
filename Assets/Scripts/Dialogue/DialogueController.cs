using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
public class DialogueController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogue;
    [SerializeField] Image emotionImage;
    [SerializeField] float typingSpeed = 5f;
    [SerializeField] float timeBetweenParagraphs = 1.5f;

    string p;

    // typing
    private Coroutine paragraphCoroutine;
    private bool isTyping;

    private const string HTML_ALPHA = "<color=#00000000>";
    private const float MAX_TYPE_TIME = 0.1f;
    private const float CHAR_FOR_MINUTE = 2500;  // 750

    // for emotions - saved resource
    [SerializeField] List<Sprite> emotionSprites;
    Dictionary<Emotions, Sprite> emotionToSprite = new Dictionary<Emotions, Sprite>();

    void Awake()
    {

        var myEnumMemberCount = Enum.GetNames(typeof(Emotions)).Length;
        if (myEnumMemberCount != emotionSprites.Count)
        {
            Debug.LogError("myEnumMemberCount != emotionSprites.Count, " + myEnumMemberCount.ToString() + " != " + emotionSprites.Count.ToString());
        }
        string[] emotionsArr = Enum.GetNames(typeof(Emotions));
        for (int i = 0; i < emotionSprites.Count; ++i)
        {
            Enum.TryParse(emotionsArr[i], out Emotions emot);
            emotionToSprite[emot] = emotionSprites[i];
        }

        gameObject.SetActive(false);
    }

    public void EnqueueParagraph(GameDialogue gameDialogue)
    {
        if (isTyping && !gameDialogue.force)
        {
            // gentle ignore
            Debug.Log("Gentle ignore gameDialogue: " + gameDialogue.ToString());
            return;
        }

        if (isTyping && gameDialogue.force)
        {
            dialogue.text = "";
            StopCoroutine(paragraphCoroutine);
        }

        // (re)start conversation
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        Debug.Log("will StartCoroutine - TypeParagraph");
        paragraphCoroutine = StartCoroutine(TypeParagraph(gameDialogue));
    }

    private IEnumerator TypeParagraph(GameDialogue gameDialogue)
    {
        Debug.Log("inside TypeParagraph");
        isTyping = true;
        string[] paragraphsToPrint = gameDialogue.paragraphs;
        if (gameDialogue.randomParagraph)
        {
            paragraphsToPrint = new string[] { gameDialogue.paragraphs[UnityEngine.Random.Range(0, gameDialogue.paragraphs.Length)] };
        }

        if (gameDialogue.emotions.Length != gameDialogue.paragraphs.Length)
        {
            Debug.LogError("gameDialogue.emotions.Length != gameDialogue.paragraphs.Length, " + gameDialogue.emotions.Length.ToString() + " != " + gameDialogue.paragraphs.Length.ToString());
        }

        int emotIdx = 0;
        foreach (string p in paragraphsToPrint)
        {
            Debug.Log("inside TypeParagraph, emotIdx=" + emotIdx.ToString());
            if (!isTyping)
            {
                break;
            }
            // image logic
            Emotions currentEmotion = Emotions.Neutral;
            if (emotIdx < gameDialogue.emotions.Length)
            {
                currentEmotion = gameDialogue.emotions[emotIdx];
            }
            emotionImage.sprite = emotionToSprite[currentEmotion];
            // print logic
            dialogue.text = "";
            string originalText = p;
            int aplhaIndex = 0;
            float additionalTime = 60.0f * p.Length / CHAR_FOR_MINUTE;
            Debug.Log("additionalTime: " + additionalTime.ToString());

            foreach (char c in p.ToCharArray())
            {
                aplhaIndex++;
                dialogue.text = originalText;
                string displayedText = dialogue.text.Insert(aplhaIndex, HTML_ALPHA);
                dialogue.text = displayedText;

                yield return new WaitForSeconds(MAX_TYPE_TIME / typingSpeed);
            }
            // sleep
            yield return new WaitForSeconds(timeBetweenParagraphs + additionalTime);
            emotIdx += 1;
        }

        gameObject.SetActive(false);
        isTyping = false;
    }
}
