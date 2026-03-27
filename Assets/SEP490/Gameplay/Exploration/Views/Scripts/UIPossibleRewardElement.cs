namespace SEP490G69.Exploration
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIPossibleRewardElement : MonoBehaviour, IPooledObject
    {
        private Action<ERewardType, string> _onClick;

        [SerializeField] private Image m_Image;
        [SerializeField] private Button m_Button;

        private string _entityId;

        private ERewardType _rewardType;

        public void Despawn()
        {
            m_Button.onClick.RemoveListener(Click);
        }

        public void Spawn()
        {
            m_Button.onClick.AddListener(Click);
        }

        public UIPossibleRewardElement SetContent(string entityId, ERewardType rewardType, Sprite icon)
        {
            _entityId = entityId;
            _rewardType = rewardType;
            m_Image.sprite = icon;
            return this;
        }

        public UIPossibleRewardElement SetOnClickCallback(Action<ERewardType, string> onClick)
        {
            _onClick = onClick; 
            return this;
        }

        private void Click()
        {
            _onClick?.Invoke(_rewardType, _entityId);
        }
    }
}