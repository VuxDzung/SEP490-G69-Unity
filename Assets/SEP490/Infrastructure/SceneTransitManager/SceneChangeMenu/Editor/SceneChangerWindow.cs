#if UNITY_EDITOR 
namespace SEP490G69.Addons.Editor.SceneChanger
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine.SceneManagement;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class SceneChangerWindow : EditorWindow
    {
        private static readonly List<string> sceneNames = new List<string>();
        private static readonly List<string> scenePaths = new List<string>();
        private int selectedSceneIndex = 0;
        private Vector2 scrollPosition;

        [MenuItem(OrganizationConstants.NAMESPACE + "/Addons/Tools/Scene Management")]
        public static void ShowWindow()
        {
            var window = GetWindow<SceneChangerWindow>("Scene Changer");
            window.position = new Rect(100, 100, 300, 400);
            window.RefreshSceneList();
        }

        private void OnGUI()
        {
            GUILayout.Label("Quick Scene Changer", EditorStyles.boldLabel);

            // Current scene info
            Scene activeScene = SceneManager.GetActiveScene();
            if (!string.IsNullOrEmpty(activeScene.name))
            {
                EditorGUILayout.HelpBox($"Current Scene: {activeScene.name}", MessageType.Info);
            }

            GUILayout.Space(10);

            // Add current scene to build settings button
            if (GUILayout.Button("Add Current Scene to Build Settings"))
            {
                AddActiveSceneToBuildSettings();
            }

            GUILayout.Space(5);

            if (sceneNames.Count == 0)
            {
                GUILayout.Label("No scenes found in Build Settings");
                if (GUILayout.Button("Refresh"))
                {
                    RefreshSceneList();
                }
                return;
            }

            // Scene selection dropdown
            EditorGUI.BeginChangeCheck();
            selectedSceneIndex = EditorGUILayout.Popup("Switch to Scene:", selectedSceneIndex, sceneNames.ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                ChangeScene(scenePaths[selectedSceneIndex]);
            }

            GUILayout.Space(10);

            // Scrollable list of scenes with remove buttons
            GUILayout.Label("Scenes in Build Settings:", EditorStyles.boldLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

            for (int i = 0; i < sceneNames.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                // Scene name label
                GUILayout.Label(sceneNames[i], GUILayout.ExpandWidth(true));

                // Switch button
                if (GUILayout.Button("Switch", GUILayout.Width(60)))
                {
                    selectedSceneIndex = i;
                    ChangeScene(scenePaths[i]);
                }

                // Remove button
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    RemoveSceneFromBuildSettings(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);

            // Refresh button
            if (GUILayout.Button("Refresh Scene List"))
            {
                RefreshSceneList();
            }

            GUILayout.Space(5);

            // Additional utilities
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear All Scenes"))
            {
                ClearAllScenes();
            }
            if (GUILayout.Button("Add All Project Scenes"))
            {
                AddAllProjectScenes();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void AddActiveSceneToBuildSettings()
        {
            Scene activeScene = SceneManager.GetActiveScene();

            if (string.IsNullOrEmpty(activeScene.path))
            {
                EditorUtility.DisplayDialog("Error", "Current scene must be saved before adding to Build Settings.", "OK");
                return;
            }

            // Get current build settings scenes
            List<EditorBuildSettingsScene> buildScenes = EditorBuildSettings.scenes.ToList();

            // Check if scene is already in build settings
            bool sceneExists = buildScenes.Any(scene => scene.path == activeScene.path);

            if (sceneExists)
            {
                EditorUtility.DisplayDialog("Scene Already Added",
                    $"Scene '{activeScene.name}' is already in Build Settings.", "OK");
                return;
            }

            // Add the scene
            buildScenes.Add(new EditorBuildSettingsScene(activeScene.path, true));
            EditorBuildSettings.scenes = buildScenes.ToArray();

            Debug.Log($"Added scene '{activeScene.name}' to Build Settings at index {buildScenes.Count - 1}");

            // Refresh the list
            RefreshSceneList();

            // Update selection to the newly added scene
            for (int i = 0; i < scenePaths.Count; i++)
            {
                if (scenePaths[i] == activeScene.path)
                {
                    selectedSceneIndex = i;
                    break;
                }
            }
        }

        private void RemoveSceneFromBuildSettings(int index)
        {
            if (index < 0 || index >= scenePaths.Count)
                return;

            string sceneName = Path.GetFileNameWithoutExtension(scenePaths[index]);

            bool confirm = EditorUtility.DisplayDialog("Remove Scene",
                $"Are you sure you want to remove '{sceneName}' from Build Settings?",
                "Remove", "Cancel");

            if (!confirm)
                return;

            // Get current build settings scenes
            List<EditorBuildSettingsScene> buildScenes = EditorBuildSettings.scenes.ToList();

            // Find and remove the scene
            string scenePathToRemove = scenePaths[index];
            buildScenes.RemoveAll(scene => scene.path == scenePathToRemove);

            // Update build settings
            EditorBuildSettings.scenes = buildScenes.ToArray();

            Debug.Log($"Removed scene '{sceneName}' from Build Settings");

            // Refresh the list
            RefreshSceneList();

            // Adjust selected index if necessary
            if (selectedSceneIndex >= sceneNames.Count && sceneNames.Count > 0)
            {
                selectedSceneIndex = sceneNames.Count - 1;
            }
            else if (sceneNames.Count == 0)
            {
                selectedSceneIndex = 0;
            }
        }

        private void ClearAllScenes()
        {
            bool confirm = EditorUtility.DisplayDialog("Clear All Scenes",
                "Are you sure you want to remove ALL scenes from Build Settings?",
                "Clear All", "Cancel");

            if (!confirm)
                return;

            EditorBuildSettings.scenes = new EditorBuildSettingsScene[0];
            Debug.Log("Cleared all scenes from Build Settings");
            RefreshSceneList();
        }

        private void AddAllProjectScenes()
        {
            bool confirm = EditorUtility.DisplayDialog("Add All Project Scenes",
                "This will add ALL scene files found in the project to Build Settings. Continue?",
                "Add All", "Cancel");

            if (!confirm)
                return;

            // Find all scene files in the project
            string[] allSceneGUIDs = AssetDatabase.FindAssets("t:Scene");
            List<EditorBuildSettingsScene> buildScenes = EditorBuildSettings.scenes.ToList();

            int addedCount = 0;

            foreach (string sceneGUID in allSceneGUIDs)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);

                // Check if scene is already in build settings
                bool sceneExists = buildScenes.Any(scene => scene.path == scenePath);

                if (!sceneExists)
                {
                    buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                    addedCount++;
                }
            }

            if (addedCount > 0)
            {
                EditorBuildSettings.scenes = buildScenes.ToArray();
                Debug.Log($"Added {addedCount} scenes to Build Settings");
                RefreshSceneList();
            }
            else
            {
                EditorUtility.DisplayDialog("No New Scenes",
                    "All project scenes are already in Build Settings.", "OK");
            }
        }

        private void RefreshSceneList()
        {
            sceneNames.Clear();
            scenePaths.Clear();

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

            // Update selected index to current scene if possible
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

            // If current scene not found in build settings, reset to 0
            selectedSceneIndex = sceneNames.Count > 0 ? 0 : 0;
        }

        private void ChangeScene(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath))
                return;

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

            EditorSceneManager.OpenScene(scenePath);

            // Update the selection after scene change
            RefreshSceneList();
        }

        // Auto-refresh when the window gets focus
        private void OnFocus()
        {
            RefreshSceneList();
        }
    }
}
#endif