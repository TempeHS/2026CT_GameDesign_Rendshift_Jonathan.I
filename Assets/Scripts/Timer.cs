using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TMP_Text timerText;
    public CameraModeSwitcher camSwitcher;

    private float time;
    private bool timerRunning = false;
    public bool hasStarted = false;

    public bool blockStartInput = false;

    void Update()
    {
        if (camSwitcher.freeMode)
            return;

        bool keyInput =
            Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.D) ||
            Input.GetButtonDown("Jump") ||
            Input.GetKeyDown(KeyCode.LeftShift);

        bool axisInput =
            Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f ||
            Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f;

        if (blockStartInput)
        {
            if (keyInput || axisInput)
                blockStartInput = false;

            return;
        }

        if (!hasStarted && (keyInput || axisInput))
        {
            hasStarted = true;
            timerRunning = true;
        }

        if (timerRunning)
        {
            time += Time.deltaTime;
            timerText.text = "Time: " + FormatTime(time);
        }
    }

    public void ResumeTimer()
    {
        timerRunning = true;
    }

    public void PauseTimer()
    {
        timerRunning = false;
    }

    public float StopTimer()
    {
        timerRunning = false;
        return time;
    }

    string FormatTime(float t)
    {
        int seconds = (int)t;
        int milliseconds = (int)((t - seconds) * 1000);
        int microseconds = (int)(((t - seconds) * 1000000) % 1000);
        return string.Format("{0:00}:{1:000}:{2:000}", seconds, milliseconds, microseconds);
    }
}
