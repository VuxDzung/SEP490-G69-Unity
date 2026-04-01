namespace SEP490G69
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITabButton : MonoBehaviour
    {
        private Action<string> _onClick;

        [SerializeField] private Button m_Button;
        [SerializeField] private Image m_SelectBorder;
        [SerializeField] private Color m_SelectedColor;
        [SerializeField] private Color m_NormalColor;

        private string _category;

        public void Select()
        {
            Debug.Log($"Select category: {_category}");
            m_SelectBorder.color = m_SelectedColor;
        }
        public void Deselect()
        {
            m_SelectBorder.color = m_NormalColor;
        }

        public void SetCategory(string category, Action<string> callback)
        {
            _category = category;
            _onClick = callback;
        }

        public void Enable()
        {
            m_Button.onClick.AddListener(Click);
        }
        public void Disable()
        {
            m_Button.onClick.RemoveListener(Click);
            _category = string.Empty;
            _onClick = null;
        }

        private void Click()
        {
            _onClick?.Invoke(_category);
        }
    }
}