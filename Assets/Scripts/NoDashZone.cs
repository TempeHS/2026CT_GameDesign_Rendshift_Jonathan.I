using UnityEngine;

public class NoDashZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();

            if (pm != null)
            {
                // Stop dash instantly
                if (pm.isDashing)
                {
                    pm.StopAllCoroutines();
                    pm.isDashing = false;
                    pm.rb.gravityScale = 1f;
                    pm.tr.emitting = false;
                    pm.rb.linearVelocity = Vector2.zero;
                }

                // Disable dash + jump
                pm.canDash = false;
                pm.jumpCount = pm.maxJumps;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();

            if (pm != null)
            {
                pm.canDash = true;
                pm.jumpCount = 0;
            }
        }
    }
}
