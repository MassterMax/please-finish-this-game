using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelTimerText;
    [SerializeField] TextMeshProUGUI bestTimerText;
    [SerializeField] List<GameObject> healths;  // 3

    private static string TIME_FORMAT = "mm':'ss':'ff";

    public void ResetTimeText() {
        TimeSpan currentTime = System.TimeSpan.FromSeconds(0);
        levelTimerText.text = currentTime.ToString(TIME_FORMAT);
        bestTimerText.text = "best ?";
    }

    public void SetCurrentTimeText(float time)
    {
        TimeSpan currentTime = System.TimeSpan.FromSeconds(time);
        levelTimerText.text = currentTime.ToString(TIME_FORMAT);
    }

    public void SetBestTimeText(float time)
    {
        TimeSpan bestTime = System.TimeSpan.FromSeconds(time);
        bestTimerText.text = "best " + bestTime.ToString(TIME_FORMAT);
    }

    public void SetHealth(int count) {
        foreach (GameObject health in healths)
        {
            health.SetActive(false);
        }
        healths[count].SetActive(true);
    }
}
