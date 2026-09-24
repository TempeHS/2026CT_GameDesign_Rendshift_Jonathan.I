using UnityEngine;

public class DeathHint : MonoBehaviour
{
    public GameObject hintText;

    void Start()
    {
        if (hintText != null)
            hintText.SetActive(DeathHintState.ShouldShowHint());
    }

    public void ShowHint()
    {
        DeathHintState.ShowHint();

        if (hintText != null)
            hintText.SetActive(true);
    }
}