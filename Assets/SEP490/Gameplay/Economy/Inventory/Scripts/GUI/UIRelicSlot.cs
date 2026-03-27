namespace SEP490G69.Economy
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIRelicSlot : MonoBehaviour
    {
        private Action<string, int> _onClick;

        [SerializeField] private Image m_ItemImg;
        [SerializeField] private Button m_Button;
        [SerializeField] private int m_Slot;
        [SerializeField] private TextMeshProUGUI m_ItemNameTmp;

        private string _relicId;

        public int Slot => m_Slot;

        public UIRelicSlot SetRelicInfo(string relicId, Sprite itemImg, string relicName = "")
        {
            _relicId = relicId;
            if (itemImg != null)
            {
                m_ItemImg.sprite = itemImg;
                m_ItemImg.enabled = true;
            }
            if (m_ItemNameTmp != null && !string.IsNullOrEmpty(relicName))
            {
                m_ItemNameTmp.text = relicName;
                m_ItemNameTmp.enabled = !string.IsNullOrEmpty(relicName);
            }
            return this;
        }

        public void SetEmpty()
        {
            if (m_ItemImg != null)
            {
                m_ItemImg.sprite = null;
                m_ItemImg.enabled = false;
            }
            _relicId = string.Empty;
        }

        public UIRelicSlot SetOnClickCallback(Action<string, int> onClick)
        {
            _onClick = onClick;
            if (m_Button) m_Button.onClick.RemoveListener(Click);
            if (m_Button) m_Button.onClick.AddListener(Click);
            return this;
        }

        private void Click()
        {
            _onClick?.Invoke(_relicId, m_Slot);
        }
    }
}