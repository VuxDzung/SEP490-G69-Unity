namespace SEP490G69
{
    using UnityEngine;

    public enum EEventType
    {
        None = 0,
        ExploreEvent = 1,
    }

    [CreateAssetMenu(fileName = "gev_")]
    public class GameEventSO : ScriptableObject
    {
        [SerializeField] private string m_EventId;
        [SerializeField] private EEventType m_EventType;
        [TextArea]
        [SerializeField] private string m_Description;

        public string EventId => m_EventId;
        public EEventType EventType => m_EventType;
        public string Description => m_Description;
    }
}