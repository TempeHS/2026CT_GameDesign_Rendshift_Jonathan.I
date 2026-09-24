using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishBlock : MonoBehaviour
{
    public UIManager uiManager;
    public Timer timer;
    public BestTime bestTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            float finalTime = timer.StopTimer();
            bestTime.TrySetBest(finalTime);

            UnlockNextLevel();

            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            pm.FreezePlayer();

            string sceneName = SceneManager.GetActiveScene().name;
            bool allowNext = sceneName != "TestLevel";

            uiManager.ShowFinishOptions(finalTime, allowNext);
        }
    }

    void UnlockNextLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (!sceneName.StartsWith("Level"))
            return;

        string numberText = sceneName.Replace("Level", "").Trim();

        if (!int.TryParse(numberText, out int currentLevel))
            return;

        if (currentLevel >= 10)
            return;

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        int nextLevel = currentLevel + 1;

        if (nextLevel > unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextLevel);
            PlayerPrefs.Save();
        }
    }
}