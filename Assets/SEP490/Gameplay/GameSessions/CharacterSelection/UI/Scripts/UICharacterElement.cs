namespace SEP490G69.GameSessions
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class UICharacterElement : MonoBehaviour
    {
        [SerializeField] private Button m_BtnRef;
        [SerializeField] private Image m_CharImg;

        private string _characterId;
        private Action<string> _callback;

        public void Enable()
        {
            m_BtnRef.onClick.AddListener(Select);
        }
        public void Disable()
        {
            m_BtnRef.onClick.RemoveListener(Select);
        }

        public UICharacterElement SetSelectCallback(Action<string> callback)
        {
            _callback = callback;
            return this;
        }

        public void SetContent(string characterId, Sprite characterImg)
        {
            _characterId = characterId;
            m_CharImg.sprite = characterImg;
        }

        private void Select()
        {
            _callback?.Invoke(_characterId);
        }
    }
}