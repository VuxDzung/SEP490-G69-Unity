#if UNITY_EDITOR
namespace SEP490G69.Addons.Editor.SceneChanger
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine.SceneManagement;
    using System.Collections.Generic;
    using System.IO;

    [InitializeOnLoad]
    public class SceneChangerToolbar
    {
        private static readonly List<string> sceneNames = new List<string>();
        private static readonly List<string> scenePaths = new List<string>();
        private static int selectedSceneIndex = 0;
        private static bool isInitialized = false;

        static SceneChangerToolbar()
        {
            // Use SceneView.duringSceneGui for a more reliable hook
            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.update += Initialize;
            RefreshSceneList();

            // Refresh scene list when build settings change
            EditorBuildSettings.sceneListChanged += RefreshSceneList;
        }

        private static void Initialize()
        {
            if (!isInitialized)
            {
                // Subscribe to toolbar GUI using a more stable approach
                System.Type toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
                if (toolbarType != null)
                {
                    EditorApplication.update -= Initialize;
                    SubscribeToToolbar();
                    isInitialized = true;
                }
                else
                {
                    Debug.LogError("No toolbar!");
                }
            }
            else
            {
                Debug.Log("Already initialized!");
            }
        }

        private static void SubscribeToToolbar()
        {
            // Use reflection to hook into toolbar, but with better error handling
            try
            {
                var toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
                var guiViewType = typeof(Editor).Assembly.GetType("UnityEditor.GUIView");

                if (toolbarType != null && guiViewType != null)
                {
                    // Create toolbar extension using a simpler approach
                    ToolbarExtender.Initialize();
                    ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
                }
                else
                {
                    Debug.Log("No toolbar and no GUI View type");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Scene Changer Toolbar: Could not hook into toolbar - {e.Message}");
                // Fallback: Create a separate window
                CreateSceneChangerWindow();
            }
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            // This ensures we update when scenes change through other means
            UpdateCurrentSceneIndex();
        }
        
        private static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            // Add some spacing
            GUILayout.Space(10);

            // Scene dropdown
            EditorGUI.BeginChangeCheck();

            GUIStyle dropdownStyle = new GUIStyle(EditorStyles.toolbarDropDown)
            {
                fixedWidth = 150
            };

            if (sceneNames.Count == 0)
            {
                GUILayout.Label("No Scenes", EditorStyles.toolbarButton, GUILayout.Width(150));
                return;
            }

            int newIndex = EditorGUILayout.Popup(selectedSceneIndex, sceneNames.ToArray(), dropdownStyle);

            if (EditorGUI.EndChangeCheck() && newIndex != selectedSceneIndex && newIndex < scenePaths.Count)
            {
                selectedSceneIndex = newIndex;
                ChangeScene(scenePaths[selectedSceneIndex]);
            }

            // Refresh button
            if (GUILayout.Button("↻", EditorStyles.toolbarButton, GUILayout.Width(20)))
            {
                RefreshSceneList();
            }
        }

        private static void RefreshSceneList()
        {
            sceneNames.Clear();
            scenePaths.Clear();

            // Get scenes from build settings first
            EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

            for (int i = 0; i < buildScenes.Length; i++)
            {
                if (buildScenes[i].enabled)
                {
                    string scenePath = buildScenes[i].path;
                    if (!string.IsNullOrEmpty(scenePath))
                    {
                        string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                        sceneNames.Add($"{i}: {sceneName}");
                        scenePaths.Add(scenePath);
                    }
                }
            }

            // If no scenes in build settings, get all scenes in project
            if (sceneNames.Count == 0)
            {
                string[] allScenes = AssetDatabase.FindAssets("t:Scene");

                foreach (string sceneGUID in allScenes)
                {
                    string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
                    if (!string.IsNullOrEmpty(scenePath))
                    {
                        string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                        sceneNames.Add(sceneName);
                        scenePaths.Add(scenePath);
                    }
                }
            }

            // Update selected index to current scene
            UpdateCurrentSceneIndex();
        }

        private static void UpdateCurrentSceneIndex()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            string currentScenePath = currentScene.path;

            for (int i = 0; i < scenePaths.Count; i++)
            {
                if (scenePaths[i] == currentScenePath)
                {
                    selectedSceneIndex = i;
                    return;
                }
            }

            selectedSceneIndex = 0;
        }

        private static void ChangeScene(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath))
                return;

            // Save current scene if it has changes
            if (EditorSceneManager.GetActiveScene().isDirty)
            {
                bool saveScene = EditorUtility.DisplayDialog(
                    "Scene has been modified",
                    "Do you want to save the changes to the current scene?",
                    "Save", "Don't Save");

                if (saveScene)
                {
                    EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                }
            }

            // Open the new scene
            EditorSceneManager.OpenScene(scenePath);
        }

        // Fallback: Create a simple window if toolbar integration fails
        private static void CreateSceneChangerWindow()
        {
            EditorApplication.delayCall += () =>
            {
                SceneChangerWindow.ShowWindow();
            };
        }
    }
}
#endif