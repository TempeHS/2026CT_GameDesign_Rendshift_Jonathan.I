using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    public RectTransform arrowUI;   // UI arrow image
    public Transform finish;        // Finish flag

    private Transform player;       // Player instance

    void Update()
    {
        // If player doesn't exist yet, try to find it
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null)
                player = p.transform;

            return; // Wait until next frame
        }

        // Convert world positions to screen space
        Vector3 playerScreenPos3D = Camera.main.WorldToScreenPoint(player.position);
        Vector3 finishScreenPos3D = Camera.main.WorldToScreenPoint(finish.position);

        Vector2 playerScreenPos = new Vector2(playerScreenPos3D.x, playerScreenPos3D.y);
        Vector2 finishScreenPos = new Vector2(finishScreenPos3D.x, finishScreenPos3D.y);

        // Direction from player to finish
        Vector2 dir = finishScreenPos - playerScreenPos;

        // Angle in degrees
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Rotate arrow
        arrowUI.rotation = Quaternion.Euler(0, 0, angle);
    }
}
