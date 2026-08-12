using UnityEngine;
using System.Collections;

public class CameraModeSwitcher : MonoBehaviour
{
    public Camera playerCam;
    public Camera freeCam;
    public FreeCamController freeCamController;
    public Canvas gameCanvas;
    public Timer timer;
    public PlayerMovement playerMovement;

    public bool freeMode = false;

    void Start()
    {
        playerCam.gameObject.SetActive(true);
        freeCam.gameObject.SetActive(false);
        freeCamController.enabled = false;
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
        yield return null; // Wait one frame for PlayerCam to initialize

        if (playerCam == null)
        {
            Debug.LogError("❌ CameraModeSwitcher: PlayerCam is not assigned!");
            yield break;
        }

        if (freeCamController == null)
        {
            Debug.LogError("❌ CameraModeSwitcher: FreeCamController is not assigned!");
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
        freeCam.gameObject.SetActive(false);
        playerCam.gameObject.SetActive(true);

        playerMovement.UnfreezePlayer();
        freeCamController.enabled = false;

        Input.ResetInputAxes();
        timer.blockStartInput = false;

        if (timer.hasStarted)
            timer.ResumeTimer();

        Time.timeScale = 1f;
    }
}
