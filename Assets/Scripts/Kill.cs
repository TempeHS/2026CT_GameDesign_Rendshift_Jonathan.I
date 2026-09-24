using UnityEngine;

public class KillBlock : MonoBehaviour
{
    public UIManager uiManager;
    public DeathExplosion deathExplosion;
    public DeathHint deathHint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();

            if (pm != null)
                pm.FreezePlayer();

            // Death animation
            if (deathExplosion != null)
                deathExplosion.PlayExplosion(other.transform.position);

            // Show this level's world hint
            if (deathHint != null)
                deathHint.ShowHint();

            // Existing death popup
            if (uiManager != null)
                uiManager.ShowDeathPopup();
        }
    }
}