using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FreeCamController : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float zoomSpeed = 5f;
    public float minZoom = 1f;
    public float maxZoom = 40f;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null) enabled = false;
    }

    public void SyncWithMain(Camera mainCam, Vector3 worldPosition)
    {
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return;

        float z = transform.position.z;
        if (Mathf.Approximately(z, 0f)) z = -10f;
        transform.position = new Vector3(worldPosition.x, worldPosition.y, z);

        cam.orthographic = mainCam.orthographic;
        cam.orthographicSize = mainCam.orthographicSize;
        cam.fieldOfView = mainCam.fieldOfView;
        cam.cullingMask = mainCam.cullingMask;
        cam.clearFlags = mainCam.clearFlags;
        cam.backgroundColor = mainCam.backgroundColor;
        cam.rect = mainCam.rect;
        cam.depth = mainCam.depth + 1f;
        cam.targetTexture = null;
        cam.useOcclusionCulling = mainCam.useOcclusionCulling;

        if (cam.rect.width <= 0f || cam.rect.height <= 0f) cam.rect = new Rect(0f, 0f, 1f, 1f);

        cam.enabled = true;
    }

    void Update()
    {
        float mx = 0f;
        float my = 0f;
        if (Input.GetKey(KeyCode.W)) my += 1f;
        if (Input.GetKey(KeyCode.S)) my -= 1f;
        if (Input.GetKey(KeyCode.A)) mx -= 1f;
        if (Input.GetKey(KeyCode.D)) mx += 1f;

        Vector3 move = new Vector3(mx, my, 0f);
        if (move.sqrMagnitude > 0f) move = move.normalized;

        transform.position += move * moveSpeed * Time.unscaledDeltaTime;

        if (Input.GetKey(KeyCode.Q))
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - zoomSpeed * Time.unscaledDeltaTime, minZoom, maxZoom);

        if (Input.GetKey(KeyCode.E))
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + zoomSpeed * Time.unscaledDeltaTime, minZoom, maxZoom);
    }
}
