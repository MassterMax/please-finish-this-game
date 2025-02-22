using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
public class DialogueController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogue;
    [SerializeField] Image emotionImage;
    [SerializeField] float typingSpeed = 0.2f;
    [SerializeField] float timeBetweenParagraphs = 2.5f;
    [SerializeField] AudioClip typingClip;

    [Range(1, 5)]
    [SerializeField] int audioSoundFreq;
    [Range(-3, 3)]
    [SerializeField] private float minPitch = 0.5f;
    [Range(-3, 3)]
    [SerializeField] private float maxPitch = 3f;
    AudioSource audioSource;

    // typing
    private Coroutine paragraphCoroutine;
    private bool isTyping;

    private const string HTML_ALPHA = "<color=#00000000>";
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

        audioSource = this.gameObject.AddComponent<AudioSource>();
        gameObject.SetActive(false);
    }

    public void EnqueueParagraph(GameDialogue gameDialogue)
    {
        Debug.Log("EnqueueParagraph inside DialogueController");
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
        Emotions[] emotionsToShow = gameDialogue.emotions;

        if (gameDialogue.emotions.Length != gameDialogue.paragraphs.Length)
        {
            Debug.LogError("gameDialogue.emotions.Length != gameDialogue.paragraphs.Length, " + gameDialogue.emotions.Length.ToString() + " != " + gameDialogue.paragraphs.Length.ToString());
        }

        if (gameDialogue.randomParagraph)
        {
            int index = UnityEngine.Random.Range(0, gameDialogue.paragraphs.Length);
            paragraphsToPrint = new string[] { gameDialogue.paragraphs[index] };
            emotionsToShow =  new Emotions[] { gameDialogue.emotions[index] };
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
            if (emotIdx < emotionsToShow.Length)
            {
                currentEmotion = emotionsToShow[emotIdx];
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
                // play audio
                PlayDiaologueSound(aplhaIndex);

                aplhaIndex++;
                dialogue.text = originalText;
                string displayedText = dialogue.text.Insert(aplhaIndex, HTML_ALPHA);
                dialogue.text = displayedText;
                yield return new WaitForSeconds(typingSpeed);
            }
            // sleep
            yield return new WaitForSeconds(timeBetweenParagraphs + additionalTime);
            emotIdx += 1;
        }

        gameObject.SetActive(false);
        isTyping = false;
    }

    private void PlayDiaologueSound(int index) {
        // return;
        if (index % audioSoundFreq == 0) {
            // audioSource.Stop();
            audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(typingClip, SoundFXManager.Instance.GetEffectsVolume() / 3);
        }
    }
}
