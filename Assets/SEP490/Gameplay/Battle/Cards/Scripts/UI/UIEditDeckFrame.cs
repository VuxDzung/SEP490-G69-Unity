namespace SEP490G69.Battle.Cards
{
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.Shared;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIEditDeckFrame : GameUIFrame
    {
        [SerializeField] private Button m_BackBtn;
        [SerializeField] private TextMeshProUGUI m_InDeckCountTmp;
        [SerializeField] private Button m_SaveDeckBtn;
        [SerializeField] private Transform m_InDeckContainer;

        [SerializeField] private Transform m_ObtainedContainer;
        [SerializeField] private TMP_InputField m_SearchInputTmp;
        [SerializeField] private Transform m_CardUIPrefab;

        [SerializeField] private TMP_Dropdown m_FilterDropdown;

        [Header("Data Config")]
        [SerializeField] private CardConfigSO m_CardConfig;

        [SerializeField] private RectTransform m_OnDragParent;

        private List<string> _currentDeckIds = new List<string>();

        private ImageMasterConfigSO _imgMasterConfig;
        private ImageMasterConfigSO ImgMasterConfig => _imgMasterConfig ??= Resources.Load<ImageMasterConfigSO>("Images/ImageMasterConfig");

        private GameDeckController _deckController;
        protected GameDeckController DeckController => _deckController ??= ContextManager.Singleton.ResolveGameContext<GameDeckController>();

        private List<SessionCardData> _obtainedCards = new List<SessionCardData>();

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_BackBtn.onClick.AddListener(Back);
            m_SaveDeckBtn.onClick.AddListener(SaveDeck);

            LoadAllCards(true);
        }
        protected override void OnFrameHidden()
        {
            base.OnFrameHidden();
            m_BackBtn.onClick.RemoveListener(Back);
            m_SaveDeckBtn.onClick.RemoveListener(SaveDeck);

            ClearSpawnedCards();
        }

        /// <summary>
        /// Tải toàn bộ thẻ bài (cả trong Deck và Inventory) và sắp xếp vào đúng Container.
        /// </summary>
        private void LoadAllCards(bool refreshFromDB)
        {
            ClearSpawnedCards();

            SessionPlayerDeck playerDeck = DeckController.GetCurrentDeck(refreshFromDB);

            if (refreshFromDB)
            {
                _obtainedCards = DeckController.GetAllObtainedCards();
            }

            _currentDeckIds.Clear();

            if (playerDeck != null && playerDeck.CardIds != null)
            {
                _currentDeckIds.AddRange(playerDeck.CardIds);
            }

            Debug.Log($"Player deck count: {_currentDeckIds.Count}");

            // ---------- SPAWN DECK CARDS ----------
            foreach (string deckCardId in _currentDeckIds)
            {
                string rawId = CardUtils.ExtractRawCardId(deckCardId);

                CardSO staticCardData = m_CardConfig.GetCardById(rawId);

                if (staticCardData == null) continue;

                Transform deckCardUITrans = PoolManager.Pools["UIDeckCard"].Spawn(m_CardUIPrefab, m_InDeckContainer);
                deckCardUITrans.gameObject.name = $"InDeckCard_{deckCardId}:Parent_{deckCardUITrans.parent.gameObject.name}";

                UIEditableCardElement deckCardElement = deckCardUITrans.GetComponent<UIEditableCardElement>();
                Sprite cardTypeSprite = GetCardTypeImg(staticCardData.ActionType);
                string rawDesc = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_DESCS, staticCardData.CardDescription);
                string finalDes = FormatCardDescription(rawDesc, staticCardData);

                deckCardElement.SetContent(
                    rawId,
                    deckCardId,
                    LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_NAMES, staticCardData.CardName),
                    finalDes,
                    staticCardData.Icon,
                    1);

                deckCardElement.SetCost(staticCardData.Cost);
                deckCardElement.SetCardTypeSprite(cardTypeSprite);

                UIDragableElement dragableCardUI = deckCardUITrans.GetComponent<UIDragableElement>();

                if (dragableCardUI != null)
                {
                    dragableCardUI._onDragParent = m_OnDragParent;
                    dragableCardUI.onDropped = OnDropCard;
                }
            }

            // ---------- SPAWN INVENTORY ----------
            foreach (SessionCardData card in _obtainedCards)
            {
                if (card.ObtainedAmount <= 0)
                {
                    Debug.Log($"<color=yellow>Warning: </color> {card.SessionCardId} stack amount is zero.");
                    continue;
                }

                CardSO staticCardData = m_CardConfig.GetCardById(card.RawCardId);
                if (staticCardData == null)
                {
                    Debug.LogError($"CardSO of {card.RawCardId} is not registered.");
                    continue;
                }

                Transform cardUITransform = PoolManager.Pools["UICard"].Spawn(m_CardUIPrefab, m_ObtainedContainer);
                cardUITransform.gameObject.name = $"InventoryCard_{card.RawCardId}";

                UIEditableCardElement cardElement = cardUITransform.GetComponent<UIEditableCardElement>();
                Sprite cardTypeSprite = GetCardTypeImg(staticCardData.ActionType);

                string rawDesc = LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_DESCS, staticCardData.CardDescription);
                string finalDes = FormatCardDescription(rawDesc, staticCardData);

                cardElement.SetContent(
                    card.RawCardId,
                    "",
                    LocalizeManager.GetText(GameConstants.LOCALIZE_CATEGORY_CARD_NAMES, staticCardData.CardName),
                    finalDes,
                    staticCardData.Icon,
                    card.ObtainedAmount);

                cardElement.SetCost(staticCardData.Cost);
                cardElement.SetCardTypeSprite(cardTypeSprite);

                UIDragableElement dragableCardUI = cardUITransform.GetComponent<UIDragableElement>();
                if (dragableCardUI != null)
                {
                    dragableCardUI._onDragParent = m_OnDragParent;
                    dragableCardUI.onDropped = OnDropCard;
                }
            }

            UpdateDeckCountText();
        }

        private void OnDropCard(Transform cardUITrans, Transform parent)
        {
            UIDropHandler dropHandler = parent.GetComponentInParent<UIDropHandler>();
            if (dropHandler != null)
            {
                UIEditableCardElement cardUI = cardUITrans.GetComponent<UIEditableCardElement>();

                bool isInDeck = DeckController.IsCardInDeck(cardUI.DeckCardId);

                if (dropHandler.HandlerName.Equals("deck"))
                {
                    if (!isInDeck)
                    {
                        Debug.Log("<color=green>[UIEditDeckFrame]</color> Move to deck");
                        bool added = DeckController.AddCardToDeck(cardUI.RawCardId);

                        if (!added)
                        {
                            Debug.LogError("<color=red>[UIEditDeckFrame]</color> Failed to add to deck");
                        }
                        else
                        {
                            SessionCardData cardData = _obtainedCards.FirstOrDefault(c => c.RawCardId.Equals(cardUI.RawCardId));
                            cardData.ObtainedAmount--;
                        }
                    }
                }
                else
                {
                    if (isInDeck)
                    {
                        // =========================================================
                        // THÊM CHẶN Ở ĐÂY: KHÔNG CHO RÚT LÁ BÀI CUỐI CÙNG RA KHỎI DECK
                        // =========================================================
                        if (_currentDeckIds.Count <= 1)
                        {
                            UIMessagePopup warningPopup = UIManager.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP).AsFrame<UIMessagePopup>();
                            warningPopup.SetContent(
                                "title_warning",            // LƯU Ý: Cấu hình key này trong Localization
                                "msg_empty_deck_warning",   // (VD: "Không thể tháo lá bài cuối cùng!")
                                true, false, null
                            );

                            cardUI.Deselect();
                            LoadAllCards(false); // Reset lại hình ảnh thẻ bài về chỗ cũ
                            return; // Ngắt luồng, từ chối Remove
                        }
                        // =========================================================

                        Debug.Log("<color=green>[UIEditDeckFrame]</color> Move to inventory");
                        bool removed = DeckController.RemoveCardFromDeck(cardUI.DeckCardId, false);

                        if (!removed)
                        {
                            Debug.LogError("Failed to remove from deck");
                            LoadAllCards(false);
                            return;
                        }

                        string rawId = cardUI.RawCardId;

                        SessionCardData cardData = _obtainedCards.FirstOrDefault(c => c.RawCardId.Equals(rawId));
                        cardData.ObtainedAmount++;
                    }
                }
                cardUI.Deselect();
                LoadAllCards(false);
            }
            else
            {
                Debug.Log("No drop parent");
            }
        }

        private void UpdateDeckCountText()
        {
            if (m_InDeckCountTmp != null)
            {
                m_InDeckCountTmp.text = $"MY DECK: {_currentDeckIds.Count}/{GameDeckController.MAX_DECK_COUNT} CARDS";
            }
        }

        private void SaveDeck()
        {
            // Kiểm tra nếu deck không có lá bài nào
            if (_currentDeckIds == null || _currentDeckIds.Count == 0)
            {
                // Hiện cảnh báo
                UIMessagePopup warningPopup = UIManager.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP).AsFrame<UIMessagePopup>();
                warningPopup.SetContent(
                    "title_warning",            // LƯU Ý: Khai báo key này trong file Localize
                    "msg_empty_deck_warning",   // LƯU Ý: Khai báo key này trong file Localize (VD: "Bộ bài không được để trống!")
                    true,                       // Hiện nút Confirm
                    false,                      // Ẩn nút Cancel
                    null                        // Không làm gì thêm khi đóng
                );
                return; // Ngắt luồng, không cho lưu
            }

            // Nếu qua được bước kiểm tra thì lưu bình thường
            DeckController.SaveDeck(_currentDeckIds);
            DeckController.SaveInventory(_obtainedCards);
            LocalDBOrchestrator.UpdateDBChangeTime();

            // Hiện thông báo lưu thành công (Tuỳ chọn, nếu bạn muốn UI phản hồi tốt hơn)
            UIMessagePopup successPopup = UIManager.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP).AsFrame<UIMessagePopup>();
            successPopup.SetContent("title_success", "msg_save_deck_success", true, false, null);
        }

        private void ClearSpawnedCards()
        {
            if (PoolManager.Pools["UICard"].Count > 0)
            {
                Debug.Log("<color=green>[UIEditDeckFrame.ClearSpawnedCards]</color> Clear all card inventory");
                PoolManager.Pools["UICard"].DespawnAll();
            }
            if (PoolManager.Pools["UIDeckCard"].Count > 0)
            {
                Debug.Log("<color=green>[UIEditDeckFrame.ClearSpawnedCards]</color> Clear all card deck");
                PoolManager.Pools["UIDeckCard"].DespawnAll();
            }
        }

        private string FormatCardDescription(string rawDesc, CardSO cardData)
        {
            if (string.IsNullOrEmpty(rawDesc)) return rawDesc;

            // Nếu là thẻ Attack và mô tả có chứa thẻ {{DMG}}
            if (cardData.ActionType == EActionType.Attack && (rawDesc.Contains("{{DMG}}") || rawDesc.Contains("{{SHIELD}}")))
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

        private string GetStatColorHex(EStatusType statType)
        {
            switch (statType)
            {
                case EStatusType.Power: return "#FF3B30"; // Đỏ
                case EStatusType.Intelligence: return "#007AFF"; // Xanh dương
                case EStatusType.Vitality: return "#AF52DE"; // Tím
                case EStatusType.Agi: return "#34C759"; // Xanh lá
                case EStatusType.Shield: return "#0074FF";

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

        public Sprite GetCardTypeImg(EActionType type)
        {
            string id = CardConstants.GetCardTypeIconId(type);

            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            return ImgMasterConfig.GetImage("card_types", id)?.image;
        }

        private void Back()
        {
            // =========================================================
            // THÊM CHẶN Ở ĐÂY: NẾU DECK TRỐNG THÌ KHÔNG CHO BACK
            // =========================================================
            if (_currentDeckIds == null || _currentDeckIds.Count == 0)
            {
                UIMessagePopup warningPopup = UIManager.ShowFrame(GameConstants.FRAME_ID_MESSAGE_POPUP).AsFrame<UIMessagePopup>();
                warningPopup.SetContent(
                    "title_warning",
                    "msg_empty_deck_warning", // Bắt buộc người chơi phải có ít nhất 1 lá
                    true, false, null
                );
                return; // Ngắt luồng, không cho load Scene Main Menu
            }
            // =========================================================

            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_MAIN_MENU);
        }
    }
}