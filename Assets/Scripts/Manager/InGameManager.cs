using System;
using UnityEngine;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public float GameTime;
    [SerializeField] Text timer;

    void Update()
    {
        GameTime += Time.deltaTime;
        timer.text = FormatTime(GameTime);
    }

    public string FormatTime(float time)
    {
        int hours = Mathf.FloorToInt(time / 3600f);
        int minutes = Mathf.FloorToInt((time % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int fraction = Mathf.FloorToInt((time - Mathf.Floor(time)) * 100f); // 소수점 두 자리

        if (hours > 0)
            return string.Format("{0}:{1:00}:{2:00}.{3:00}", hours, minutes, seconds, fraction);
        else
            return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, fraction);
    }
}
