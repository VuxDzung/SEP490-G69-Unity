#if UNITY_EDITOR
namespace SEP490G69.Addons.Editor.SceneChanger
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;

    // Simplified toolbar extender
    public static class ToolbarExtender
    {
        public static readonly List<System.Action> RightToolbarGUI = new List<System.Action>();
        private static bool initialized = false;

        public static void Initialize()
        {
            if (initialized) return;

            try
            {
                // Use a safer approach for toolbar extension
                var toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
                if (toolbarType != null)
                {
                    EditorApplication.update += OnUpdate;
                    initialized = true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"ToolbarExtender initialization failed: {e.Message}");
            }
        }

        private static void OnUpdate()
        {
            // Simple approach: draw GUI in a consistent location
            if (Event.current?.type == EventType.Repaint)
            {
                // This is a simplified approach that works across Unity versions
                SceneView.RepaintAll();
                RightToolbarGUI.ForEach(t => t?.Invoke());
            }
        }
    }
}
#endif