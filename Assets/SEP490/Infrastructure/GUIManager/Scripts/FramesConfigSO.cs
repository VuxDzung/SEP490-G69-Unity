namespace SEP490G69
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "FramesConfig_", menuName = OrganizationConstants.NAMESPACE + "/GUI Framework/Frames configuration")]
    public class FramesConfigSO : ScriptableObject
    {
        [SerializeField] private string m_StarterFrameId;
        [SerializeField] private GameUIFrame[] m_Frames;

        public string StarterFrameId => m_StarterFrameId;
        public GameUIFrame[] Frames => m_Frames;
    }
}