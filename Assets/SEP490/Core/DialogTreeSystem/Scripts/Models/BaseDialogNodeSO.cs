namespace SEP490G69
{
    using UnityEngine;

    public abstract class BaseDialogNodeSO : ScriptableObject
    {
        [SerializeField] private string m_NodeID;
        [SerializeField] private string m_SpeakerID;
        [SerializeField] private string m_DialogID;

        public string NodeID => m_NodeID;
        public string SpeakerID => m_SpeakerID;
        public string DialogID => m_DialogID;
    }
}