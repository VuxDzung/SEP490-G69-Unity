#if UNITY_EDITOR
namespace SEP490G69.Battle.Combat
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(SceneAnimationTester))]
    public class CharacterAnimControllerInspector : Editor
    {
        private SceneAnimationTester _controller;

        private void OnEnable()
        {
            _controller = (SceneAnimationTester)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Play Animation"))
            {
                _controller.PlayAnimations();
            }
        }
    }
}
#endif