#if UNITY_EDITOR
namespace SEP490G69
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(GameUIButton))]
    public class GameUIButtonInspector : Editor
    {
        private GameUIButton _target;

        private void OnEnable()
        {
            _target = (GameUIButton)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
#endif