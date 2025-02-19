using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    LevelStart levelStart;
    PlayerMovement player;

    // UI
    [SerializeField] TextMeshProUGUI levelTimerText;
    [SerializeField] TextMeshProUGUI bestTimerText;

    // level restart options
    bool started = false;
    float levelTimer = 0f;
    float bestTimer = float.MaxValue;
    private static string TIME_FORMAT = "mm'`'ss'``'ff";
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        levelStart = FindObjectOfType<LevelStart>();

        // init
        TimeSpan currentTime = System.TimeSpan.FromSeconds(0);
        levelTimerText.text = currentTime.ToString(TIME_FORMAT);
        bestTimerText.text = "best time: ???";
    }

    // Update is called once per frame
    void Update()
    {
        if (!started && (Input.GetButtonDown("Jump") || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            started = true;
        }
        UpdateLevelTimer();
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

        started = false;
        SetBestTime();
        ResetLevelTimer();

        if (levelStart == null)
        {
            Debug.LogError("Can't find levelStart!");
            return;
        }
        player.transform.position = levelStart.transform.position;

        // TODO:
        // pause the game
        // dialog with level restart
        // after delay on any input - restart level
    }
}
