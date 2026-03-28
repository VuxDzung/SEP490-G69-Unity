namespace SEP490G69
{
    using UnityEngine;

    public abstract class BaseDialogNodeSO : ScriptableObject
    {
        [SerializeField] private string m_NodeID;
        [SerializeField] private string m_SpeakerID;
        [SerializeField] private string m_DialogID;
        [SerializeField] private Sprite m_BgImage;
        [SerializeField] private string m_BgmId;
        [SerializeField] private string m_SfxId;

        public string NodeID => m_NodeID;
        public string SpeakerID => m_SpeakerID;
        public string DialogID => m_DialogID;
        public Sprite BackgroundImage => m_BgImage;

        public string BgmId => m_BgmId;
        public string SfxId => m_SfxId;
    }
}