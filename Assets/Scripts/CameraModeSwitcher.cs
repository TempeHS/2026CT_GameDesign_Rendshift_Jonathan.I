using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraModeSwitcher : MonoBehaviour
{
    public Camera playerCam;
    public Camera freeCam;
    public FreeCamController freeCamController;
    public PlayerMovement playerMovement;
    public Timer timer;
    public Canvas gameCanvas;

    [Header("Menu Blocking")]
    public UIManager uiManager;

    public bool freeMode = false;

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Start()
    {
        if (playerCam == null) playerCam = Camera.main;
        ResolveTimer();
        ResolveUIManager();
        ResolvePlayer();

        if (playerCam != null) playerCam.gameObject.SetActive(true);
        if (freeCam != null) freeCam.gameObject.SetActive(false);
        if (freeCamController != null) freeCamController.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!freeMode && IsMenuOpen())
                return; // block opening freecam while a popup menu is showing

            freeMode = !freeMode;
            if (freeMode) EnableFreeCamImmediate();
            else DisableFreeCam();
        }
    }

    private bool IsMenuOpen()
    {
        if (uiManager != null && uiManager.panel != null && uiManager.panel.activeSelf)
            return true;

        return false;
    }

    void EnableFreeCamImmediate()
    {
        Input.ResetInputAxes();

        ResolvePlayer();
        if (playerCam == null) playerCam = Camera.main;
        ResolveTimer();
        ResolveUIManager();

        if (freeCam == null || freeCamController == null || playerCam == null) return;

        // Determine spawn position: ALWAYS prefer the player's live position
        Vector3 targetPos = (playerMovement != null)
            ? playerMovement.transform.position
            : playerCam.transform.position;

        float z = freeCam.transform.position.z;
        if (Mathf.Approximately(z, 0f)) z = -10f;
        Vector3 finalPos = new Vector3(targetPos.x, targetPos.y, z);

        // Enable objects first
        freeCam.gameObject.SetActive(true);
        Camera fc = freeCam.GetComponent<Camera>();
        if (fc != null) fc.enabled = true;

        if (gameCanvas != null && gameCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            gameCanvas.worldCamera = fc;

        freeCamController.enabled = true;

        // Sync settings (fov/culling/etc.) then FORCE position to player last,
        // guaranteeing SyncWithMain cannot override it.
        freeCamController.SyncWithMain(playerCam, finalPos);
        freeCam.transform.position = finalPos;

        if (playerCam != null) playerCam.gameObject.SetActive(false);

        if (playerMovement != null) playerMovement.FreezePlayer();

        Time.timeScale = 0f;
        if (timer != null)
        {
            timer.blockStartInput = true;
            timer.PauseTimer();
        }
    }

    void DisableFreeCam()
    {
        Input.ResetInputAxes();
        ResolveTimer();

        if (playerCam != null) playerCam.gameObject.SetActive(true);

        if (gameCanvas != null && gameCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            gameCanvas.worldCamera = playerCam;

        if (freeCamController != null) freeCamController.enabled = false;
        if (freeCam != null)
        {
            Camera fc = freeCam.GetComponent<Camera>();
            if (fc != null) fc.enabled = false;
            freeCam.gameObject.SetActive(false);
        }

        if (playerMovement != null) playerMovement.UnfreezePlayer();

        Time.timeScale = 1f;
        if (timer != null)
        {
            timer.blockStartInput = false;
            if (timer.hasStarted) timer.ResumeTimer();
        }
    }

    void ResolvePlayer()
    {
#if UNITY_2023_2_OR_NEWER
        if (playerMovement == null) playerMovement = UnityEngine.Object.FindFirstObjectByType<PlayerMovement>();
#else
        if (playerMovement == null) playerMovement = FindObjectOfType<PlayerMovement>();
#endif
        if (playerMovement == null)
        {
            GameObject p = null;
            try { p = GameObject.FindWithTag("Player"); } catch { p = null; }
            if (p != null) playerMovement = p.GetComponent<PlayerMovement>();
        }
    }

    void ResolveTimer()
    {
        if (timer == null)
        {
#if UNITY_2023_2_OR_NEWER
            timer = UnityEngine.Object.FindFirstObjectByType<Timer>();
#else
            timer = FindObjectOfType<Timer>();
#endif
        }
    }

    void ResolveUIManager()
    {
        if (uiManager == null)
        {
#if UNITY_2023_2_OR_NEWER
            uiManager = UnityEngine.Object.FindFirstObjectByType<UIManager>();
#else
            uiManager = FindObjectOfType<UIManager>();
#endif
        }
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        Time.timeScale = 1f;
        ResolveTimer();
        ResolveUIManager();
        playerMovement = null; // force re-resolve for the new scene's player
        ResolvePlayer();
        freeMode = false;

        if (freeCam != null)
        {
            Camera fc = freeCam.GetComponent<Camera>();
            if (fc != null) fc.enabled = false;
            freeCam.gameObject.SetActive(false);
        }
        if (playerCam != null) playerCam.gameObject.SetActive(true);
    }
}