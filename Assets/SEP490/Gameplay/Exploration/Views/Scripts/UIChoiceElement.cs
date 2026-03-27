namespace SEP490G69.Exploration
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIChoiceElement : MonoBehaviour, IPooledObject
    {
        private Action<string, int> _onClick;

        [SerializeField] private TextMeshProUGUI m_MessageTmp;
        [SerializeField] private Button m_Button;

        private int _choiceIndex = -1;
        private string _eventId;

        public void Spawn()
        {
            if (m_Button != null) m_Button.onClick.AddListener(Click);
        }
        public void Despawn()
        {
            if (m_Button != null) m_Button.onClick.RemoveListener(Click);
            _choiceIndex = -1;
            _eventId = string.Empty;
        }

        public void SetContent(Action<string, int> onClick, string eventId, int choiceIndex, string message)
        {
            _onClick = onClick;
            _eventId = eventId;
            _choiceIndex = choiceIndex;
            if (m_MessageTmp != null) m_MessageTmp.text = message;
        }

        private void Click()
        {
            _onClick?.Invoke(_eventId, _choiceIndex);
        }
    }
}