namespace SolutionLabs.Addons.LoadScreenSystem.Utils
{
    using UnityEngine.SceneManagement;
    using System;

    public class SceneUtils
    {
        public static Action onSceneChanged;
        public static int GetCurrentSceneIndex()
        {
            return SceneManager.GetActiveScene().buildIndex;
        }

        public static string GetCurrentSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }
    }
}