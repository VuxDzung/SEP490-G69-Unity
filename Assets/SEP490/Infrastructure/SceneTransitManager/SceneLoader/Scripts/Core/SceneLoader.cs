namespace SEP490G69.Addons.LoadScreenSystem
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using UnityEngine.SceneManagement;

    public class SceneLoader : GlobalSingleton<SceneLoader>
    {
        [SerializeField] private TextMeshProUGUI m_LoadTaskTmp;
        [SerializeField] GameObject loadingPanel;
        [SerializeField] private Image image;
        [SerializeField] Slider loadingBar;
        [SerializeField] TextMeshProUGUI loadingText;
        [SerializeField] private Sprite[] backgroundList;

        /// <summary>
        /// Load tasks only.
        /// </summary>
        /// <param name="preLoadTask"></param>
        /// <param name="postLoadTasks"></param>
        public void StartLoadTasks(List<LoadTask> preLoadTask = null, List<LoadTask> postLoadTasks = null, bool showBG = true)
        {
            StartLoad("", preLoadTask, postLoadTasks, false, showBG);
        }

        /// <summary>
        /// Load to target scene.
        /// </summary>
        /// <param name="sceneName">Target scene name</param>
        /// <param name="showBG">Whether you want to display loading background or not.</param>
        public void StartLoadScene(string sceneName, bool showBG = true)
        {
            StartLoad(sceneName, null, null, false, showBG);
        }

        /// <summary>
        /// Load scene along with pre/post load task(s).
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="preLoadTasks"></param>
        /// <param name="postLoadTasks"></param>
        /// <param name="loadScene"></param>
        /// <param name="showBG"></param>
        public void StartLoad(string sceneName,
                          List<LoadTask> preLoadTasks = null,
                          List<LoadTask> postLoadTasks = null, 
                          bool loadScene = true, 
                          bool showBG = true)
        {
            StartCoroutine(LoadSceneRoutine(sceneName, preLoadTasks, postLoadTasks, loadScene, showBG));
        }

        /// <summary>
        /// Before load to target scene, the system loads preLoadTasks
        /// After load to target scene, the system loads postLoadTasks
        /// </summary>
        /// <param name="sceneName">Target scene</param>
        /// <param name="preLoadTasks">Task(s) to load before exist the current scene</param>
        /// <param name="postLoadTasks">Task(s) to load after load to the target scene</param>
        /// <param name="showBG">Whether you want to show the background</param>
        /// <returns></returns>
        private IEnumerator LoadSceneRoutine(string sceneName,
                                             List<LoadTask> preLoadTasks,
                                             List<LoadTask> postLoadTasks, bool loadScene = true, bool showBG = true)
        {
            if (backgroundList.Length > 0)
            {
                image.sprite = backgroundList[Random.Range(0, backgroundList.Length - 1)];
            }
            loadingPanel.SetActive(showBG);

            int totalTasks = (preLoadTasks?.Count ?? 0) + 
                             (loadScene == true ? 1 : 0) + // +1 for scene load
                             (postLoadTasks?.Count ?? 0); 

            int completedTasks = 0;

            // Pre-loading tasks
            if (preLoadTasks != null)
            {
                if (preLoadTasks.Count > 0)
                {
                    foreach (var task in preLoadTasks)
                    {
                        yield return StartCoroutine(task.task());
                        completedTasks++;
                        UpdateProgress(completedTasks, task.name, totalTasks);
                    }
                }
            }

            // Scene loading
            if (loadScene)
            {
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
                while (!asyncLoad.isDone)
                {
                    float sceneProgress = Mathf.Clamp01(asyncLoad.progress / 1f);
                    UpdateProgress(completedTasks + sceneProgress, "Load scene", totalTasks);
                    yield return null;
                }
                completedTasks++;
                UpdateProgress(completedTasks, "Load scene completed", totalTasks);
            }

            // Post-loading tasks
            if (postLoadTasks != null)
            {
                foreach (var task in postLoadTasks)
                {
                    UpdateProgress(completedTasks, task.name, totalTasks);
                    yield return StartCoroutine(task.task());
                    completedTasks++;
                    UpdateProgress(completedTasks, task.name, totalTasks);
                }
            }

            loadingPanel.SetActive(false);
        }

        private void UpdateProgress(float current, string taskName, float total)
        {
            if (m_LoadTaskTmp != null) m_LoadTaskTmp.text = taskName;
            float progress = Mathf.Clamp01(current / total);
            if (loadingText != null)
                loadingText.text = $"{progress * 100f:0.0}%";
            if (loadingBar != null)
                loadingBar.value = progress;
        }
    }
}