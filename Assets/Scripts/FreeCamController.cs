using UnityEngine;

public class FreeCamController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 20f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        transform.rotation = Quaternion.identity;
    }

    public void SyncWithMain(Camera mainCam)
    {
        if (mainCam == null)
        {
            Debug.LogError("❌ FreeCamController.SyncWithMain: mainCam is null!");
            return;
        }

        if (cam == null)
        {
            cam = GetComponent<Camera>();
            if (cam == null)
            {
                Debug.LogError("❌ FreeCamController.SyncWithMain: FreeCam has no Camera component!");
                return;
            }
        }

        cam.orthographicSize = mainCam.orthographicSize;
        transform.position = mainCam.transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            return;

        float mx = 0f;
        float my = 0f;

        if (Input.GetKey(KeyCode.W)) my += 1f;
        if (Input.GetKey(KeyCode.S)) my -= 1f;
        if (Input.GetKey(KeyCode.A)) mx -= 1f;
        if (Input.GetKey(KeyCode.D)) mx += 1f;

        Vector3 move = new Vector3(mx, my, 0f).normalized;
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
