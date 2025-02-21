using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    LevelStart levelStart;
    PlayerMovement player;
    DialogueController dialogueController;
    TimerController timerController;

    // level restart options
    bool started = false;
    bool finished = false;
    float levelTimer = 0f;
    float bestTimer = float.MaxValue;

    // code for controlling level phases

    private const int MAX_HEALTH = 3;
    private int healthCounter = MAX_HEALTH;

    // Instance
    public static LevelController Instance { get; private set; }

    public const float FINISH_DELAY = 1f;
    private bool playerReset = false;

    // for the mext scene
    private string nextSceneName;
    private GameDialogue nextSceneDialogue;

    // audio clips
    [SerializeField] AudioClip deathClip;
    [SerializeField] AudioClip winClip;

    public bool IsPlayerReset()
    {
        return playerReset;
    }

    void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
        levelStart = FindObjectOfType<LevelStart>();
        dialogueController = FindObjectOfType<DialogueController>();
        if (dialogueController == null)
        {
            Debug.LogError("Can't find dialogueController!");
        }
        timerController = FindObjectOfType<TimerController>();
        if (timerController == null)
        {
            Debug.LogError("Can't find timerController!");
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (finished)
        {
            return;
        }
        if (!started && (Input.GetButtonDown("Jump") || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            started = true;
        }
        UpdateLevelTimer();

        // tmp killbind
        // if (Input.GetKeyDown(KeyCode.K))
        // {
        //     OnTrueDeath();
        // }
    }

    void UpdateLevelTimer()
    {
        if (!started)
        {
            return;
        }
        levelTimer += Time.deltaTime;
        timerController.SetCurrentTimeText(levelTimer);
    }

    void ResetLevelTimer()
    {
        levelTimer = 0f;
        timerController.SetCurrentTimeText(levelTimer);
    }

    void SetBestTime()
    {
        bestTimer = Mathf.Min(bestTimer, levelTimer);
        timerController.SetBestTimeText(bestTimer);
    }

    public void OnFinishEnter()
    {
        Debug.Log("OnFinishEnter");
        playerReset = false;
        SoundFXManager.Instance.PlaySoundFXClip(winClip, player.transform);
        SetBestTime();
        StartCoroutine(DelayAndResetPlayer(true));
    }

    public void OnTrueDeath()
    {
        Debug.Log("OnTrueDeath");
        playerReset = false;
        SoundFXManager.Instance.PlaySoundFXClip(deathClip, player.transform);
        DecreaseUserHealth();

        // if that was last state
        if (healthCounter == 0)
        {
            started = false;
            ResetLevelTimer();
            finished = true;
            player.StopPlayer(false);
            dialogueController.EnqueueParagraph(nextSceneDialogue);
            if (nextSceneName == "NONE")
            {
                // means current scene is final
                // in the end - show end screen

                return;
            }
            else
            {
                // make an effect that player is restored
                GoNextLevel();
            }
            return;
        }

        StartCoroutine(DelayAndResetPlayer(false));
    }

    private void GoNextLevel()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator DelayAndResetPlayer(bool alive)
    {
        started = false;
        ResetLevelTimer();
        finished = true;
        player.StopPlayer(alive);
        yield return new WaitForSeconds(FINISH_DELAY);
        player.ResetPlayerPos(levelStart.transform.position);
        playerReset = true;
        yield return new WaitForSeconds(FINISH_DELAY);
        player.AllowPlayerMove();
        finished = false;
    }

    private void DecreaseUserHealth()
    {
        healthCounter -= 1;
        timerController.SetHealth(healthCounter);
    }

    private void ResetUserHealth()
    {
        healthCounter = MAX_HEALTH;
        timerController.SetHealth(healthCounter);
    }

    private IEnumerator AllowPlayerMoveAfterStart(float time)
    {
        Debug.Log("AllowPlayerMoveAfterStart");
        yield return new WaitForSeconds(time);
        player.AllowPlayerMove();
        finished = false;
    }


    public void LoadLevel(GameDialogue gameDialogue, float blockMoveTime, string nextSceneName, GameDialogue nextSceneDialogue)
    {
        // init
        Debug.Log("LoadLevel");
        timerController.ResetTimeText();

        finished = true;
        player.ResetPlayerPos(levelStart.transform.position);
        player.StopPlayer(true);
        StartCoroutine(AllowPlayerMoveAfterStart(blockMoveTime));

        Debug.Log("Call - LoadLevel - EnqueueParagraph");
        dialogueController.EnqueueParagraph(gameDialogue);

        this.nextSceneName = nextSceneName;
        this.nextSceneDialogue = nextSceneDialogue;
        ResetUserHealth();
    }
}
