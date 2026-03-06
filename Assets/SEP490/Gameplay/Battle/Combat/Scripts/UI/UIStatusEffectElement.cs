namespace SEP490G69.Battle.Combat
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIStatusEffectElement : MonoBehaviour, IPooledObject
    {
        private Action<string> _onClick;
        [SerializeField] private Image m_Image;
        [SerializeField] private TextMeshProUGUI m_StackAmountTmp;
        [SerializeField] private Button m_BtnRef;

        private string _id;

        public UIStatusEffectElement SetOnClickCallback(Action<string> onClick)
        {
            _onClick = onClick;
            return this;
        }

        public virtual void Spawn()
        {
            if (m_BtnRef) m_BtnRef.onClick.AddListener(Click);
        }
        public virtual void Despawn()
        {
            if (m_BtnRef) m_BtnRef.onClick.RemoveListener(Click);
            _onClick = null;
            m_StackAmountTmp.text = string.Empty;
            m_Image.sprite = null;
        }

        public UIStatusEffectElement SetId(string id)
        {
            _id = id;
            return this;
        }
        public UIStatusEffectElement SetImg(Sprite sprite)
        {
            m_Image.sprite = sprite;
            return this;
        }
        public UIStatusEffectElement SetRemainAmount(int amount)
        {
            m_StackAmountTmp.text = amount.ToString();
            return this;
        }

        private void Click()
        {
            _onClick?.Invoke(_id);
        }
    }
}