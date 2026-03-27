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

        // ================= THÊM MỚI: PHẦN CARD TYPE =================
        [Header("Card Type Config")]
        [SerializeField] private Image m_CardTypeImage; // Kéo object Field.CardType vào đây
        [SerializeField] private Sprite m_TypeSpriteAttack;
        [SerializeField] private Sprite m_TypeSpriteEffect;
        [SerializeField] private Sprite m_TypeSpriteRecovery;
        // ==========================================================

        [Header("Tooltips")]
        [SerializeField] private Transform m_DescriptionContainer;
        [SerializeField] private GameObject m_StatusTooltipPrefab;

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            // Gắn sự kiện đóng Popup khi click vào vùng mờ
            if (m_BackgroundOverlayBtn != null)
            {
                m_BackgroundOverlayBtn.onClick.AddListener(HideThisView);
            }
        }

        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            // Gỡ sự kiện khi đóng để tránh tràn bộ nhớ
            if (m_BackgroundOverlayBtn != null)
            {
                m_BackgroundOverlayBtn.onClick.RemoveListener(HideThisView);
            }
        }

        /// <summary>
        /// Hàm này được gọi từ các màn hình khác (như màn Chọn nhân vật) để truyền Data vào
        /// </summary>
        public void LoadData(CardSO cardData)
        {
            if (cardData == null)
            {
                HideThisView();
                return;
            }

            // 1. Gán thông tin trực quan của thẻ bài
            if (m_CardIcon != null) m_CardIcon.sprite = cardData.Icon;
            if (m_CardNameTmp != null) m_CardNameTmp.text = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_NAMES, cardData.CardName);
            if (m_CardCostTmp != null) m_CardCostTmp.text = cardData.Cost.ToString();

            // Xử lý Description động (Hiển thị công thức kèm màu sắc)
            if (m_CardDescTmp != null)
            {
                string rawDesc = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_DESCS, cardData.CardDescription);
                m_CardDescTmp.text = FormatCardDescription(rawDesc, cardData);
            }

            // 2. THÊM MỚI: Xử lý đổi icon Card Type
            SetupCardTypeIcon(cardData);

            // 3. Tự động quét và Load các Tooltip giải thích trạng thái bằng PoolManager
            LoadStatusTooltips(cardData);
        }

        /// <summary>
        /// Logic thay đổi hình ảnh Card Type dựa trên ActionType của thẻ
        /// </summary>
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
                    // Cả hồi máu và hồi stat đều dùng chung icon Recovery
                    m_CardTypeImage.sprite = m_TypeSpriteRecovery;
                    m_CardTypeImage.gameObject.SetActive(true);
                    break;

                default:
                    // Các type khác (ví dụ Other) có thể ẩn đi hoặc bạn có thể gán 1 icon mặc định
                    m_CardTypeImage.gameObject.SetActive(false);
                    break;
            }
        }

        /// <summary>
        /// Hàm format hiển thị công thức Base Damage + Scale Stats kèm màu sắc
        /// </summary>
        private string FormatCardDescription(string rawDesc, CardSO cardData)
        {
            if (string.IsNullOrEmpty(rawDesc)) return rawDesc;

            // Nếu là thẻ Attack và mô tả có chứa thẻ {{DMG}}
            if (cardData.ActionType == EActionType.Attack && rawDesc.Contains("{{DMG}}"))
            {
                string statColorHex = GetStatColorHex(cardData.ModifyStatType);
                string statName = GetStatShortName(cardData.ModifyStatType);

                // Giả sử giá trị modifier đang là số thập phân (0.5), nhân 100 để ra %. 
                float scalePercent = cardData.ModifierValue * 100f;

                string dynamicDmgString = $"{cardData.BaseValue} + <color={statColorHex}>({scalePercent}% {statName})</color>";

                return rawDesc.Replace("{{DMG}}", dynamicDmgString);
            }

            return rawDesc;
        }

        // ================= CÁC HÀM BỔ TRỢ MÀU SẮC & TEXT =================

        private string GetStatColorHex(EStatusType statType)
        {
            switch (statType)
            {
                case EStatusType.Power: return "#FF3B30"; // Đỏ
                case EStatusType.Intelligence: return "#007AFF"; // Xanh dương
                case EStatusType.Vitality: return "#AF52DE"; // Tím
                case EStatusType.Agi: return "#34C759"; // Xanh lá

                default: return "#FFFFFF"; // Mặc định trắng
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

                default: return statType.ToString(); // Trả về tên gốc nếu không map được
            }
        }

        // =================================================================

        private void LoadStatusTooltips(CardSO cardData)
        {
            // [GIỮ NGUYÊN ĐOẠN CODE LOAD TOOLTIPS CỦA BẠN]
            Debug.Log($"<color=yellow>[Tooltip Debug]</color> 1. Bắt đầu load Tooltip cho thẻ: {cardData.CardName}");

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

            if (uniqueStatuses.Count == 0) return;

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