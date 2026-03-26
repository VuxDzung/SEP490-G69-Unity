namespace SEP490G69.Economy
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class UIInventoryItemSlot : MonoBehaviour, IPooledObject
    {
        private UnityAction<string, string> _action;

        [SerializeField] protected Image m_Icon;
        [SerializeField] protected TextMeshProUGUI m_AmountTmp;
        [SerializeField] private Button m_Button;

        private ItemDataHolder _item;
        protected string _rawItemId;
        protected string _sessionItemId;

        public string RawItemId => _rawItemId;
        public string SessionItemId => _sessionItemId;

        public virtual void Spawn()
        {
            if (m_Button != null) m_Button.onClick.AddListener(Click);
        }
        public virtual void Despawn()
        {
            if (m_Button != null) m_Button.onClick.RemoveListener(Click);
            _action = null;
        }

        public UIInventoryItemSlot BindInventoryItem(ItemDataHolder item)
        {
            _item = item;
            _rawItemId = _item.GetRawId();
            _sessionItemId = item.GetSessionItemId();

            if (m_Icon != null) m_Icon.sprite = item.GetIcon();
            if (m_AmountTmp != null) m_AmountTmp.text = $"x{item.GetRemainAmount().ToString()}";
            return this;
        }

        public UIInventoryItemSlot SetClickAction(UnityAction<string, string> action)
        {
            _action = action;
            return this;
        }

        private void Click()
        {
            _action?.Invoke(_rawItemId, _sessionItemId);
        }
    }
}