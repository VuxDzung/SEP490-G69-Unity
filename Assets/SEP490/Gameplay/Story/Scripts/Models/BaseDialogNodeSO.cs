namespace SEP490G69
{
    using UnityEngine;

    public abstract class BaseDialogNodeSO : ScriptableObject
    {
        [SerializeField] private string m_NodeID;
        [SerializeField] private string m_SpeakerID;
        [SerializeField] private string m_DialogID;

        [SerializeField] private string m_BackgroundId;
        [SerializeField] private string m_BgmId;
        [SerializeField] private string m_SfxId;

        [Header("Deprecated fields")]
        [Tooltip("[Deprecated] This property is no longer support. Use BackgroundId property instead")]
        [SerializeField] private Sprite m_BgImage;

        public string NodeID => m_NodeID;
        public string SpeakerID => m_SpeakerID;
        public string DialogID => m_DialogID;

        public string BackgroundId => m_BackgroundId;

        /// <summary>
        /// [Deprecated] This property is no longer support. Use BackgroundId property instead.
        /// </summary>
        public Sprite BackgroundImage => m_BgImage;

        public string BgmId => m_BgmId;
        public string SfxId => m_SfxId;
    }
}