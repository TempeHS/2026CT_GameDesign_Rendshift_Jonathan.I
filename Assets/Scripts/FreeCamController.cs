using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FreeCamController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 20f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("FreeCamController: Camera component missing.");
            enabled = false;
            return;
        }

        cam.orthographic = true;
        transform.rotation = Quaternion.identity;
    }

    public void SyncWithMain(Camera mainCam)
    {
        if (mainCam == null)
        {
            Debug.LogError("FreeCamController.SyncWithMain: mainCam is NULL. Assign Player Cam in CameraModeSwitcher.");
            return;
        }

        if (cam == null) cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("FreeCamController.SyncWithMain: FreeCam has no Camera component.");
            return;
        }

        cam.orthographicSize = mainCam.orthographicSize;
        Vector3 p = mainCam.transform.position;
        p.z = transform.position.z;
        transform.position = p;
    }

    void Update()
    {
        // Prevent jump buffering originating from FreeCam input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Input.ResetInputAxes();
            return;
        }

        float mx = 0f;
        float my = 0f;

        // Correct mapping: W/S -> Y axis, A/D -> X axis
        if (Input.GetKey(KeyCode.W)) my += 1f;
        if (Input.GetKey(KeyCode.S)) my -= 1f;
        if (Input.GetKey(KeyCode.A)) mx -= 1f;
        if (Input.GetKey(KeyCode.D)) mx += 1f;

        Vector3 move = new Vector3(mx, my, 0f);
        if (move.sqrMagnitude > 0f) move = move.normalized;

        transform.position += move * moveSpeed * Time.unscaledDeltaTime;

        if (Input.GetKey(KeyCode.Q))
            cam.orthographicSize = Mathf.Max(minZoom, cam.orthographicSize - zoomSpeed * Time.unscaledDeltaTime);

        if (Input.GetKey(KeyCode.E))
            cam.orthographicSize = Mathf.Min(maxZoom, cam.orthographicSize + zoomSpeed * Time.unscaledDeltaTime);

        if (Input.GetKeyDown(KeyCode.R))
            moveSpeed += 2f;

        if (Input.GetKeyDown(KeyCode.T))
            moveSpeed = Mathf.Max(2f, moveSpeed - 2f);
    }
}
