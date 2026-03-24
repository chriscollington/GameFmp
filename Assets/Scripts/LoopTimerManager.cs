using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoopTimerManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public float loopDurationSeconds = 600f;   // 10 minutes default
    public bool startOnPlay = true;

    [Header("UI Reference")]
    public TMP_Text clockDisplay;        // Drag your TMP text here

    [Header("Player Reset")]
    public Transform player;                    // Drag your player here
    public Transform playerStartPosition;       // Empty GameObject at spawn point

    [Header("Visual Warning")]
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;      // Flashes red in last 30 seconds
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

        // Warning flash in last X seconds
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
        StartCoroutine(ResetSequence());
    }

    IEnumerator ResetSequence()
    {
        // Flash effect before reset
        if (clockDisplay != null)
            clockDisplay.color = warningColor;

        yield return new WaitForSeconds(0.5f);

        // Reset player position
        if (player != null && playerStartPosition != null)
        {
            // Disable controller briefly to prevent physics conflicts
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.position = playerStartPosition.position;
            player.rotation = playerStartPosition.rotation;

            if (cc != null) cc.enabled = true;
        }

        // Broadcast reset event to all other scripts
        // Any script can listen with: void OnLoopReset() { }
        BroadcastMessage("OnLoopReset", SendMessageOptions.DontRequireReceiver);

        // Reset timer
        timeRemaining = loopDurationSeconds;

        if (clockDisplay != null)
            clockDisplay.color = normalColor;

        isRunning = true;
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

    // Call these from other scripts or buttons if needed
    public void StartTimer() => isRunning = true;
    public void StopTimer() => isRunning = false;
    public void SetTime(float seconds) { timeRemaining = seconds; }
    public float GetTimeRemaining() => timeRemaining;
}