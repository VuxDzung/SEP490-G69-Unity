namespace SEP490G69.Exploration
{
    using SEP490G69.Addons.Localization;
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIExploreEventElement : MonoBehaviour, IPooledObject
    {
        private Action<EExploreEventType> _onClick;

        [SerializeField] private Sprite m_TreasureSprite;
        [SerializeField] private Sprite m_EncounterSprite;
        [SerializeField] private Sprite m_BossSprite;
        [SerializeField] private Sprite m_CombatSprite;

        [SerializeField] private Image m_Image;
        [SerializeField] private Button m_Button;
        [SerializeField] private TextMeshProUGUI m_EventTypeTmp;

        private EExploreEventType _eventType;

        public void Spawn()
        {
            if (m_Button != null) m_Button.onClick.AddListener(Click);
        }
        public void Despawn()
        {
            if (m_Button != null) m_Button.onClick.RemoveListener(Click);
            _eventType = EExploreEventType.None;
        }

        public UIExploreEventElement SetOnClickCallback(Action<EExploreEventType> onClick)
        {
            _onClick = onClick;
            return this;
        }

        public void SetContent(EExploreEventType eventType, LocalizationManager localizeManager)
        {
            _eventType = eventType;
            switch (_eventType)
            {
                case EExploreEventType.Boss:
                    m_Image.sprite = m_BossSprite;
                    break;
                case EExploreEventType.Encounter:
                    m_Image.sprite = m_EncounterSprite;
                    break;
                case EExploreEventType.Chest:
                    m_Image.sprite = m_TreasureSprite;
                    break;
                case EExploreEventType.Combat:
                    m_Image.sprite = m_CombatSprite;
                    break;
            }
            if (m_EventTypeTmp != null) m_EventTypeTmp.text = localizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_UI_MESSAGE, GameConstants.ConvertExploreEventType2LocalizeId(eventType));
        }

        private void Click()
        {
            _onClick?.Invoke(_eventType);
        }
    }
}