using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelController : MonoBehaviour
{
    LevelStart levelStart;
    PlayerMovement player;
    DialogueController dialogueController;
    // UI, TODO - move to other place or make singleton
    [SerializeField] TextMeshProUGUI levelTimerText;
    [SerializeField] TextMeshProUGUI bestTimerText;
    [SerializeField] List<GameObject> healths;  // 3

    // level restart options
    bool started = false;
    bool finished = false;
    float levelTimer = 0f;
    float bestTimer = float.MaxValue;
    private static string TIME_FORMAT = "mm':'ss':'ff";

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

    public bool IsPlayerReset() {
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
        TimeSpan currentTime = System.TimeSpan.FromSeconds(levelTimer);
        levelTimerText.text = currentTime.ToString(TIME_FORMAT);
    }

    void ResetLevelTimer()
    {
        levelTimer = 0f;
        TimeSpan currentTime = System.TimeSpan.FromSeconds(levelTimer);
        levelTimerText.text = currentTime.ToString(TIME_FORMAT);
    }

    void SetBestTime()
    {
        bestTimer = Mathf.Min(bestTimer, levelTimer);
        TimeSpan bestTime = System.TimeSpan.FromSeconds(bestTimer);
        bestTimerText.text = "best " + bestTime.ToString(TIME_FORMAT);
    }

    public void OnFinishEnter()
    {
        Debug.Log("OnFinishEnter");
        playerReset = false;
        SetBestTime();
        StartCoroutine(DelayAndResetPlayer(true));
    }

    public void OnTrueDeath()
    {
        Debug.Log("OnTrueDeath");
        playerReset = false;
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

    private void DecreaseUserHealth() {
        healthCounter -= 1;
        foreach (GameObject health in healths) {
            health.SetActive(false);
        }
        healths[healthCounter].SetActive(true);
    }

    private void ResetUserHealth() {
        healthCounter = MAX_HEALTH;
        foreach (GameObject health in healths) {
            health.SetActive(false);
        }
        healths[healthCounter].SetActive(true);
    }

    private IEnumerator AllowPlayerMoveAfterStart(float time)
    {
        Debug.Log("AllowPlayerMoveAfterStart");
        yield return new WaitForSeconds(time);
        player.AllowPlayerMove();
        finished = false;
    }


    public void LoadLevel( GameDialogue gameDialogue, float blockMoveTime, string nextSceneName, GameDialogue nextSceneDialogue)
    {
        // init
        Debug.Log("LoadLevel");
        TimeSpan currentTime = System.TimeSpan.FromSeconds(0);
        levelTimerText.text = currentTime.ToString(TIME_FORMAT);
        bestTimerText.text = "best ?";

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
