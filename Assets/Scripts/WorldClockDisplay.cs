using UnityEngine;
using TMPro;

public class WorldClockDisplay : MonoBehaviour
{
    [Header("References")]
    public LoopTimerManager loopManager;       // Drag LoopManager object here
    public TMP_Text worldClockText;         // TextMeshPro (3D), not UGUI

    [Header("Display Style")]
    public string prefix = "";                 // e.g. "RESET IN " or leave blank
    public bool showMilliseconds = false;

    void Update()
    {
        if (loopManager == null || worldClockText == null) return;

        float t = loopManager.GetTimeRemaining();

        int minutes = Mathf.FloorToInt(t / 60f);
        int seconds = Mathf.FloorToInt(t % 60f);

        if (showMilliseconds)
        {
            int ms = Mathf.FloorToInt((t % 1f) * 100f);
            worldClockText.text = prefix + string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, ms);
        }
        else
        {
            worldClockText.text = prefix + string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}