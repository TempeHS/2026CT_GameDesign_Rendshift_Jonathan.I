using UnityEngine;
using UnityEngine.SceneManagement;

public static class DeathHintState
{
    private static string hintScene = "";
    private static bool hintVisible = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Setup()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
        SceneManager.activeSceneChanged += OnSceneChanged;

        hintScene = "";
        hintVisible = false;
    }

    private static void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        // Reloading the SAME level keeps the hint.
        // Going anywhere else removes it.
        if (hintVisible && newScene.name != hintScene)
        {
            hintVisible = false;
            hintScene = "";
        }
    }

    public static void ShowHint()
    {
        hintScene = SceneManager.GetActiveScene().name;
        hintVisible = true;
    }

    public static bool ShouldShowHint()
    {
        return hintVisible &&
               hintScene == SceneManager.GetActiveScene().name;
    }
}