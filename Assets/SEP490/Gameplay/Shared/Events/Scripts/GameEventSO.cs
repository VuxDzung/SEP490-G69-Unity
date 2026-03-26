namespace SEP490G69
{
    using UnityEngine;

    public enum EEventType
    {
        None = 0,

    }

    [CreateAssetMenu(fileName = "gev_")]
    public class GameEventSO : ScriptableObject
    {
        [SerializeField] private string m_EventId;
        [SerializeField] private EEventType m_EventType;

        public string EventId => m_EventId;
        public EEventType EventType => m_EventType;
    }
}