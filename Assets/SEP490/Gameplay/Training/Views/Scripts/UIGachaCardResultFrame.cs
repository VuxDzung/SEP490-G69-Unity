using DG.Tweening;
using SEP490G69.Battle;
using SEP490G69.Battle.Cards;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SEP490G69.Training
{
    public class UIGachaCardResultFrame : GameUIFrame
    {
        private CardConfigSO _cardConfig;
        private CardConfigSO CardConfig => _cardConfig ??= ContextManager.Singleton.GetDataSO<CardConfigSO>();

        [Header("Overlay & Background")]
        [SerializeField] private Image m_BackgroundOverlayImg;
        [SerializeField] private Button m_BackgroundOverlayBtn;

        [Header("Card Visual Information")]
        [SerializeField] private Transform m_CardContainer;
        [SerializeField] private GameObject m_CardCoverGO;

        [SerializeField] private Image m_CardIcon;
        [SerializeField] private TextMeshProUGUI m_CardNameTmp;
        [SerializeField] private TextMeshProUGUI m_CardDescTmp;
        [SerializeField] private TextMeshProUGUI m_CardCostTmp;

        [Header("Card Type Config")]
        [SerializeField] private Image m_CardTypeImage;
        [SerializeField] private Sprite m_TypeSpriteAttack;
        [SerializeField] private Sprite m_TypeSpriteEffect;
        [SerializeField] private Sprite m_TypeSpriteRecovery;

        [Header("Animation Elements (Sequence V2.1)")]
        [SerializeField] private TextMeshProUGUI m_RibbonText;

        [Header("--- TEST POOL (BY ID) ---")]
        [Tooltip("Nhập các ID thẻ (VD: card_0001, card_0016) để test random")]
        [SerializeField] private List<string> m_TestCardIds = new List<string> { "card_0001", "card_0016", "card_0036" };

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            Debug.Log("<color=green>[GachaUI] Bắt đầu mở Frame Gacha...</color>");

            if (m_BackgroundOverlayBtn != null)
            {
                m_BackgroundOverlayBtn.onClick.RemoveAllListeners();
                m_BackgroundOverlayBtn.onClick.AddListener(OnTapToContinue);
            }

            // 1. Lấy ngẫu nhiên 1 thẻ thông qua ID
            CardSO randomCard = GetRandomCardForTest();

            // 2. Load dữ liệu thẻ lên UI
            LoadCardData(randomCard);

            // 3. Chạy chuỗi Animation Sequence tạo Game Feel
            StartCoroutine(PlayGachaSequence());
        }

        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            Time.timeScale = 1f;
        }

        private CardSO GetRandomCardForTest()
        {
            if (CardConfig == null)
            {
                Debug.LogError("[GachaTest] LỖI: CardConfigSO chưa được load vào ContextManager!");
                return null;
            }

            if (m_TestCardIds != null && m_TestCardIds.Count > 0)
            {
                string randomId = m_TestCardIds[UnityEngine.Random.Range(0, m_TestCardIds.Count)];
                Debug.Log($"[GachaTest] ID random ra được là: '{randomId}'");

                CardSO card = CardConfig.GetCardById(randomId);

                if (card == null)
                {
                    Debug.LogError($"[GachaTest] LỖI CỰC MẠNH: Hàm GetCardById('{randomId}') trả về NULL. " +
                        $"Hãy kiểm tra lại CardConfigSO xem đã add thẻ '{randomId}' vào list chứa thẻ chưa!!!");
                }
                else
                {
                    Debug.Log($"[GachaTest] Lấy thành công dữ liệu thẻ: Tên SO là {card.name}");
                }

                return card;
            }

            Debug.LogWarning("[GachaTest] Chưa nhập ID nào vào m_TestCardIds để test!");
            return null;
        }

        private void LoadCardData(CardSO cardData)
        {
            if (cardData == null)
            {
                Debug.LogError("[GachaUI] LỖI: Hàm LoadCardData nhận vào một CardSO bị NULL -> Dừng load UI!");
                return;
            }

            Debug.Log($"[GachaUI] ---- BẮT ĐẦU LOAD THÔNG TIN LÊN UI CHO THẺ: {cardData.CardName} ----");

            try
            {
                // 1. Load Icon
                if (m_CardIcon != null)
                {
                    m_CardIcon.sprite = cardData.Icon;
                    if (cardData.Icon == null) Debug.LogWarning($"[GachaUI] Thẻ {cardData.CardName} không có Icon (ảnh bị null).");
                }

                // 2. Load Cost
                if (m_CardCostTmp != null) m_CardCostTmp.text = cardData.Cost.ToString();

                // 3. Load Tên
                if (m_CardNameTmp != null)
                {
                    string locName = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_NAMES, cardData.CardName);
                    m_CardNameTmp.text = locName;
                    Debug.Log($"[GachaUI] Dịch Tên thẻ thành: {locName}");
                }

                // 4. Load Description
                if (m_CardDescTmp != null)
                {
                    string rawDesc = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_DESCS, cardData.CardDescription);
                    Debug.Log($"[GachaUI] Raw Description (trước format): {rawDesc}");

                    string formattedDesc = FormatCardDescription(rawDesc, cardData);
                    m_CardDescTmp.text = formattedDesc;
                    Debug.Log($"[GachaUI] Formatted Description (sau format): {formattedDesc}");
                }

                // 5. Setup Type
                SetupCardTypeIcon(cardData);

                Debug.Log($"[GachaUI] ---- LOAD UI THÀNH CÔNG CHO THẺ: {cardData.CardName} ----");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GachaUI] LỖI EXCEPTION TRONG QUÁ TRÌNH LOAD UI: {ex.Message}\nStacktrace: {ex.StackTrace}");
            }
        }

        #region ANIMATION SEQUENCE V2.1
        private IEnumerator PlayGachaSequence()
        {
            Time.timeScale = 0f;

            DOTween.Kill(m_CardContainer);
            DOTween.Kill(m_BackgroundOverlayImg);

            // BƯỚC 0: SETUP
            if (m_BackgroundOverlayImg != null)
            {
                Color bgColor = m_BackgroundOverlayImg.color;
                bgColor.a = 0f;
                m_BackgroundOverlayImg.color = bgColor;
            }

            if (m_BackgroundOverlayBtn != null) m_BackgroundOverlayBtn.interactable = false;

            m_CardContainer.localScale = Vector3.zero;
            m_RibbonText.gameObject.SetActive(false);

            if (m_CardCoverGO != null) m_CardCoverGO.SetActive(true);

            // BƯỚC 1: DIM BACKGROUND 
            if (m_BackgroundOverlayImg != null)
            {
                m_BackgroundOverlayImg.DOFade(0.8f, 0.3f).SetUpdate(true);
            }
            yield return new WaitForSecondsRealtime(0.2f);

            // BƯỚC 2: SPAWN 
            m_CardContainer.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
            yield return new WaitForSecondsRealtime(0.6f);

            // BƯỚC 3: VIBRATION
            m_CardContainer.DOShakePosition(1.5f, strength: new Vector3(15f, 0, 0), vibrato: 30).SetUpdate(true);
            yield return new WaitForSecondsRealtime(1.5f);

            // BƯỚC 4: REVEAL
            if (m_CardCoverGO != null) m_CardCoverGO.SetActive(false);

            // BƯỚC 5: DISPLAY TEXT
            string charName = "Sakura";
            m_RibbonText.text = $"{charName} learned a new skill!";
            m_RibbonText.gameObject.SetActive(true);

            m_RibbonText.transform.localScale = new Vector3(1, 0, 1);
            m_RibbonText.transform.DOScaleY(1f, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);

            yield return new WaitForSecondsRealtime(0.5f);

            // BƯỚC 6: RESUME
            if (m_BackgroundOverlayBtn != null) m_BackgroundOverlayBtn.interactable = true;
        }

        private void OnTapToContinue()
        {
            Time.timeScale = 1f;
            HideThisView();
        }
        #endregion

        #region FORMATTING HELPERS
        private void SetupCardTypeIcon(CardSO cardData)
        {
            if (m_CardTypeImage == null) return;

            Debug.Log($"[GachaUI] Đang setup Icon Type cho ActionType: {cardData.ActionType}");

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
                    Debug.LogWarning($"[GachaUI] CHÚ Ý: Chưa có logic đổi hình Type cho ActionType [{cardData.ActionType}]. Đang tắt icon này đi.");
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
                string dynamicDmgString = $"<color={statColorHex}>{cardData.BaseValue} + ({scalePercent}% {statName})</color>";
                return rawDesc.Replace("{{DMG}}", dynamicDmgString);
            }

            if (rawDesc.Contains("{{SHIELD}}"))
            {
                string statColorHex = GetStatColorHex(cardData.ModifyStatType);
                string statName = GetStatShortName(cardData.ModifyStatType);
                float scalePercent = cardData.ModifierValue * 100f;
                string dynamicShieldString = $"<color={statColorHex}>{cardData.BaseValue} + ({scalePercent}% {statName})</color>";
                return rawDesc.Replace("{{SHIELD}}", dynamicShieldString);
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
        #endregion
    }
}