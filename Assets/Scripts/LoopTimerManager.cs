using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoopTimerManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public float loopDurationSeconds = 600f;
    public bool startOnPlay = true;

    [Header("UI Reference")]
    public TMP_Text clockDisplay;

    [Header("Visual Warning")]
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;
    public float warningThreshold = 30f;

    private float timeRemaining;
    private bool isRunning = false;
    private bool isFlashing = false;

    void Start()
    {
        timeRemaining = loopDurationSeconds;
        if (startOnPlay) StartTimer();
    }

    void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;

        UpdateClockDisplay();

        if (timeRemaining <= warningThreshold && !isFlashing)
        {
            isFlashing = true;
            StartCoroutine(FlashWarning());
        }

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            TriggerReset();
        }
    }

    void UpdateClockDisplay()
    {
        if (clockDisplay == null) return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        clockDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void TriggerReset()
    {
        isRunning = false;
        isFlashing = false;
        StartCoroutine(ResetScene());
    }

    IEnumerator ResetScene()
    {
        if (clockDisplay != null)
            clockDisplay.color = warningColor;

        yield return new WaitForSeconds(0.5f);

        // 🔁 RELOAD ENTIRE SCENE (full reset)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator FlashWarning()
    {
        while (isFlashing && timeRemaining > 0)
        {
            if (clockDisplay != null)
                clockDisplay.color = warningColor;

            yield return new WaitForSeconds(0.4f);

            if (clockDisplay != null)
                clockDisplay.color = normalColor;

            yield return new WaitForSeconds(0.4f);
        }
    }

    public void StartTimer() => isRunning = true;
    public void StopTimer() => isRunning = false;
    public void SetTime(float seconds) { timeRemaining = seconds; }
    public float GetTimeRemaining() => timeRemaining;
}