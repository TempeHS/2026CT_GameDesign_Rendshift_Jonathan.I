using UnityEngine;

public class CameraModeSwitcher : MonoBehaviour
{
    public Camera playerCam;
    public Camera freeCam;

    public FreeCamController freeCamController;
    public PlayerMovement playerMovement;

    bool freeMode = false;

    void Start()
    {
        playerCam.enabled = true;
        freeCam.enabled = false;
        freeCamController.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            freeMode = !freeMode;

            playerCam.enabled = !freeMode;
            freeCam.enabled = freeMode;

            if (freeMode)
            {
                playerMovement.FreezePlayer();
                freeCamController.enabled = true;
                Time.timeScale = 0f;
            }
            else
            {
                playerMovement.UnfreezePlayer();
                freeCamController.enabled = false;
                Time.timeScale = 1f;
            }
        }
    }
}
