using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public Button[] levelButtons;

    void Start()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = (i + 1) <= unlockedLevel;
        }
    }

    public void LoadLevel(int level)
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (level > unlockedLevel)
            return;

        string sceneName = level == 10
            ? "Level 10"
            : "Level" + level;

        SceneManager.LoadScene(sceneName);
    }
}