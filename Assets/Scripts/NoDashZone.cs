using UnityEngine;

public class NoDashZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement pm = other.GetComponentInParent<PlayerMovement>();
        if (pm == null)
            return;

        pm.canDash = false;
        pm.inNoJumpZone = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerMovement pm = other.GetComponentInParent<PlayerMovement>();
        if (pm == null)
            return;

        pm.canDash = true;
        pm.inNoJumpZone = false;
    }
}
