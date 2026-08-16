using UnityEngine;
using System.Collections;

public class CameraModeSwitcher : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Camera playerCam;
    public Camera freeCam;
    public FreeCamController freeCamController;
    public PlayerMovement playerMovement;
    public Timer timer;

    public bool freeMode = false;

    void Start()
    {
        if (playerCam != null) playerCam.gameObject.SetActive(true);
        if (freeCam != null) freeCam.gameObject.SetActive(false);
        if (freeCamController != null) freeCamController.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            freeMode = !freeMode;

            if (freeMode)
                StartCoroutine(EnableFreeCam());
            else
                DisableFreeCam();
        }
    }

    private IEnumerator EnableFreeCam()
    {
        // Clear buffered input immediately so Player won't receive a queued jump
        Input.ResetInputAxes();

        // Wait one frame to ensure references are stable
        yield return null;

        if (playerCam == null)
        {
            Debug.LogError("CameraModeSwitcher.EnableFreeCam: playerCam not assigned.");
            yield break;
        }

        if (freeCam == null || freeCamController == null || playerMovement == null || timer == null)
        {
            Debug.LogError("CameraModeSwitcher.EnableFreeCam: Missing references (freeCam/freeCamController/playerMovement/timer).");
            yield break;
        }

        freeCamController.SyncWithMain(playerCam);

        playerCam.gameObject.SetActive(false);
        freeCam.gameObject.SetActive(true);

        playerMovement.FreezePlayer();
        freeCamController.enabled = true;

        Time.timeScale = 0f;
        timer.blockStartInput = true;
        timer.PauseTimer();
    }

    private void DisableFreeCam()
    {
        // Clear buffered input before unfreezing player
        Input.ResetInputAxes();

        if (freeCam == null || playerCam == null || freeCamController == null || playerMovement == null || timer == null)
        {
            Debug.LogError("CameraModeSwitcher.DisableFreeCam: Missing references.");
            return;
        }

        freeCam.gameObject.SetActive(false);
        playerCam.gameObject.SetActive(true);

        playerMovement.UnfreezePlayer();
        freeCamController.enabled = false;

        timer.blockStartInput = false;
        if (timer.hasStarted) timer.ResumeTimer();

        Time.timeScale = 1f;
    }
}
