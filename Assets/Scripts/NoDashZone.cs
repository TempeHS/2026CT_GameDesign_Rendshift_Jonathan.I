using UnityEngine;

public class NoDashZone : MonoBehaviour
{
    private Collider2D zone;

    private void Awake()
    {
        zone = GetComponent<Collider2D>();
        zone.isTrigger = true;
    }

    private bool PlayerFullyInside(Collider2D player)
    {
        Bounds pb = player.bounds;
        Bounds zb = zone.bounds;

        Vector3 c1 = new Vector3(pb.min.x, pb.min.y);
        Vector3 c2 = new Vector3(pb.min.x, pb.max.y);
        Vector3 c3 = new Vector3(pb.max.x, pb.min.y);
        Vector3 c4 = new Vector3(pb.max.x, pb.max.y);

        return zb.Contains(c1) && zb.Contains(c2) &&
               zb.Contains(c3) && zb.Contains(c4);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm == null)
            return;

        pm.inNoJumpZone = PlayerFullyInside(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm == null)
            return;

        pm.inNoJumpZone = false;
    }
}
