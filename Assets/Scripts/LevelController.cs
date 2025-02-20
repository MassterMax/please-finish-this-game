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

    // level restart options
    bool started = false;
    bool finished = false;
    float levelTimer = 0f;
    float bestTimer = float.MaxValue;
    private static string TIME_FORMAT = "mm':'ss':'ff";

    // code for controlling level phases
    private LevelState currentLevelState;
    private int currentDeathIndex = 0;
    public static LevelController Instance { get; private set; }

    // for the mext scene
    private string nextSceneName;
    private GameDialogue nextSceneDialogue;

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
        if (Input.GetKeyDown(KeyCode.K))
        {
            OnTrueDeath();
        }
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
        bestTimerText.text = "best time: " + bestTime.ToString(TIME_FORMAT);
    }

    public void OnFinishEnter()
    {
        Debug.Log("OnFinishEnter");
        SetBestTime();
        StartCoroutine(DelayAndResetPlayer(true));
    }

    public void OnTrueDeath()
    {
        Debug.Log("OnTrueDeath");
        // todo animation of death
        StartCoroutine(DelayAndResetPlayer(false));

        currentDeathIndex++;
        // if that was last state
        if (currentDeathIndex >= currentLevelState.levelPhases.Count)
        {
            objectRegistry.Clear();
            dialogueController.EnqueueParagraph(nextSceneDialogue);
            // TODO - stop player movement
            if (nextSceneName == "NONE")
            {
                // means current scene is final
                // in the end - show end screen

                return;
            }
            else
            {
                GoNextLevel();
            }
            return;
        }
        // objectRegistry.Clear
        ApplyCurrentPhase();
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
        yield return new WaitForSeconds(2f);
        player.ResetPlayer(levelStart.transform.position);
        finished = false;
    }

    private IEnumerator BlockPlayerMoveOnStart(float time)
    {
        Debug.Log("BlockPlayerMoveOnStart");
        finished = true;
        player.ResetPlayer(levelStart.transform.position);
        player.StopPlayer(true);
        yield return new WaitForSeconds(time);
        player.ResetPlayer(levelStart.transform.position);
        finished = false;
    }


    public void LoadLevel(LevelState levelState, GameDialogue gameDialogue, float blockMoveTime, string nextSceneName, GameDialogue nextSceneDialogue)
    {
        // init
        Debug.Log("LoadLevel");
        TimeSpan currentTime = System.TimeSpan.FromSeconds(0);
        levelTimerText.text = currentTime.ToString(TIME_FORMAT);
        bestTimerText.text = "best time: ?";

        StartCoroutine(BlockPlayerMoveOnStart(blockMoveTime));

        Debug.Log("Call - LoadLevel - EnqueueParagraph");
        dialogueController.EnqueueParagraph(gameDialogue);

        currentLevelState = levelState;
        currentDeathIndex = 0;  // first phase should always reset all active objects
        ApplyCurrentPhase();
        this.nextSceneName = nextSceneName;
        this.nextSceneDialogue = nextSceneDialogue;
    }


    private void ApplyCurrentPhase()
    {
        LevelPhase phase = currentLevelState.levelPhases[currentDeathIndex];
        Debug.Log("ApplyCurrentPhase: " + phase.ToString());

        foreach (string id in phase.objectsToActivate)
        {
            PhaseLevelObject obj = GetObjectByID(id);
            if (obj)
            {
                obj.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("Can't find object to activate: " + id);
            }
        }

        foreach (string id in phase.objectsToDeactivate)
        {
            PhaseLevelObject obj = GetObjectByID(id);
            if (obj)
            {
                obj.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Can't find object to deactivate: " + id);
            }
        }
    }

    private static Dictionary<string, PhaseLevelObject> objectRegistry = new Dictionary<string, PhaseLevelObject>();

    public static void RegisterObject(PhaseLevelObject obj)
    {
        if (!objectRegistry.ContainsKey(obj.objectID))
        {
            objectRegistry.Add(obj.objectID, obj);
        }
        else
        {
            Debug.LogError("met object inside RegisterObject with same id twice: " + obj.objectID);
        }
    }

    private static PhaseLevelObject GetObjectByID(string id)
    {
        return objectRegistry.TryGetValue(id, out PhaseLevelObject obj) ? obj : null;
    }
}
