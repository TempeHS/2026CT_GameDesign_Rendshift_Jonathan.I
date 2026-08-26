using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    public RectTransform arrowUI;   // UI arrow image
    public Transform finish;        // Finish flag

    void Update()
    {
        if (finish == null || arrowUI == null)
            return;

        Camera activeCam = GetActiveCamera();
        if (activeCam == null)
            return;

        // Convert finish world position to screen space
        Vector3 finishScreenPos3D = activeCam.WorldToScreenPoint(finish.position);
        Vector2 finishScreenPos = new Vector2(finishScreenPos3D.x, finishScreenPos3D.y);

        // Use the arrow's OWN screen position instead of the player's
        Vector2 arrowScreenPos = RectTransformUtility.WorldToScreenPoint(
            arrowUI.GetComponentInParent<Canvas>().worldCamera,
            arrowUI.position
        );

        // Direction from arrow's position to finish
        Vector2 dir = finishScreenPos - arrowScreenPos;

        // Angle in degrees
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Rotate arrow
        arrowUI.rotation = Quaternion.Euler(0, 0, angle);
    }

    private Camera GetActiveCamera()
    {
        Camera[] cams = FindObjectsByType<Camera>(FindObjectsSortMode.None);

        foreach (Camera c in cams)
        {
            if (c != null && c.isActiveAndEnabled && c.gameObject.activeInHierarchy)
                return c;
        }

        return null;
    }
}