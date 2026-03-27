namespace SEP490G69.Battle.Cards
{
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UICardDetailFrame : GameUIFrame
    {
        [Header("Overlay (Click outside to close)")]
        [SerializeField] private Button m_BackgroundOverlayBtn;

        [Header("Card Visual Information")]
        [SerializeField] private Image m_CardIcon;
        [SerializeField] private TextMeshProUGUI m_CardNameTmp;
        [SerializeField] private TextMeshProUGUI m_CardDescTmp;
        [SerializeField] private TextMeshProUGUI m_CardCostTmp;

        [Header("Card Type Config")]
        [SerializeField] private Image m_CardTypeImage;
        [SerializeField] private Sprite m_TypeSpriteAttack;
        [SerializeField] private Sprite m_TypeSpriteEffect;
        [SerializeField] private Sprite m_TypeSpriteRecovery;

        [Header("Tooltips")]
        [SerializeField] private Transform m_DescriptionContainer; // Tương ứng với StatusEffectContainer trên Scene
        [SerializeField] private GameObject m_StatusTooltipPrefab;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            if (m_BackgroundOverlayBtn != null)
            {
                m_BackgroundOverlayBtn.onClick.AddListener(HideThisView);
            }
        }

        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            if (m_BackgroundOverlayBtn != null)
            {
                m_BackgroundOverlayBtn.onClick.RemoveListener(HideThisView);
            }
        }

        public void LoadData(CardSO cardData)
        {
            if (cardData == null)
            {
                HideThisView();
                return;
            }

            if (m_CardIcon != null) m_CardIcon.sprite = cardData.Icon;
            if (m_CardNameTmp != null) m_CardNameTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_NAMES, cardData.CardName);
            if (m_CardCostTmp != null) m_CardCostTmp.text = cardData.Cost.ToString();

            if (m_CardDescTmp != null)
            {
                string rawDesc = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_DESCS, cardData.CardDescription);
                m_CardDescTmp.text = FormatCardDescription(rawDesc, cardData);
            }

            SetupCardTypeIcon(cardData);
            LoadStatusTooltips(cardData);
        }

        private void SetupCardTypeIcon(CardSO cardData)
        {
            if (m_CardTypeImage == null) return;

            switch (cardData.ActionType)
            {
                case EActionType.Attack:
                    m_CardTypeImage.sprite = m_TypeSpriteAttack;
                    m_CardTypeImage.gameObject.SetActive(true);
                    break;

                case EActionType.Effect:
                    m_CardTypeImage.sprite = m_TypeSpriteEffect;
                    m_CardTypeImage.gameObject.SetActive(true);
                    break;

                case EActionType.HPRecover:
                case EActionType.StatRecover:
                    m_CardTypeImage.sprite = m_TypeSpriteRecovery;
                    m_CardTypeImage.gameObject.SetActive(true);
                    break;

                default:
                    m_CardTypeImage.gameObject.SetActive(false);
                    break;
            }
        }

        private string FormatCardDescription(string rawDesc, CardSO cardData)
        {
            if (string.IsNullOrEmpty(rawDesc)) return rawDesc;

            if (cardData.ActionType == EActionType.Attack && rawDesc.Contains("{{DMG}}"))
            {
                string statColorHex = GetStatColorHex(cardData.ModifyStatType);
                string statName = GetStatShortName(cardData.ModifyStatType);

                float scalePercent = cardData.ModifierValue * 100f;

                string dynamicDmgString = $"{cardData.BaseValue} + <color={statColorHex}>({scalePercent}% {statName})</color>";

                return rawDesc.Replace("{{DMG}}", dynamicDmgString);
            }

            return rawDesc;
        }

        private string GetStatColorHex(EStatusType statType)
        {
            switch (statType)
            {
                case EStatusType.Power: return "#FF3B30";
                case EStatusType.Intelligence: return "#007AFF";
                case EStatusType.Vitality: return "#AF52DE";
                case EStatusType.Agi: return "#34C759";

                default: return "#FFFFFF";
            }
        }

        private string GetStatShortName(EStatusType statType)
        {
            switch (statType)
            {
                case EStatusType.Power: return "POW";
                case EStatusType.Intelligence: return "INT";
                case EStatusType.Vitality: return "VIT";
                case EStatusType.Agi: return "AGI";

                default: return statType.ToString();
            }
        }

        private void LoadStatusTooltips(CardSO cardData)
        {
            string poolKey = "UIStatusTooltip";
            if (PoolManager.Pools.ContainsKey(poolKey))
            {
                PoolManager.Pools[poolKey].DespawnAll();
            }
            else
            {
                for (int i = m_DescriptionContainer.childCount - 1; i >= 0; i--)
                {
                    Destroy(m_DescriptionContainer.GetChild(i).gameObject);
                }
            }

            if (m_StatusTooltipPrefab == null || m_DescriptionContainer == null) return;

            var allStatuses = new List<StatusEffectSO>();

            try
            {
                if (cardData.StatusGains != null) allStatuses.AddRange(cardData.StatusGains);
                if (cardData.StatusInflicts != null) allStatuses.AddRange(cardData.StatusInflicts);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"<color=red>[Tooltip Debug]</color> LỖI khi đọc mảng Status từ CardSO: {e.Message}");
            }

            var uniqueStatuses = allStatuses.Where(s => s != null).Distinct().ToList();

            if (uniqueStatuses.Count == 0)
            {
                // Nếu không có tooltip nào, TẮT HẲN container chứa tooltip.
                // Horizontal Layout Group sẽ tự động ném UIElement.CardDetail ra giữa màn hình!
                m_DescriptionContainer.gameObject.SetActive(false);
                return;
            }

            m_DescriptionContainer.gameObject.SetActive(true);

            foreach (var status in uniqueStatuses)
            {
                Transform tooltipTrans = null;

                if (PoolManager.Pools.ContainsKey(poolKey))
                {
                    tooltipTrans = PoolManager.Pools[poolKey].Spawn(m_StatusTooltipPrefab.transform, m_DescriptionContainer);
                }
                else
                {
                    tooltipTrans = Instantiate(m_StatusTooltipPrefab, m_DescriptionContainer).transform;
                }

                if (tooltipTrans == null) continue;

                UIStatusTooltipElement tooltipElement = tooltipTrans.GetComponent<UIStatusTooltipElement>();
                if (tooltipElement != null)
                {
                    string locName = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_STATUS_EFFECT_NAMES, status.EffectName);
                    string locDesc = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_STATUS_EFFECT_DESC, status.EffectDesc);
                    tooltipElement.Setup(status.Icon, locName, locDesc);
                }
            }
        }
    }
}