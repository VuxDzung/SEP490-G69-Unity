namespace SEP490G69.Battle.Cards
{
    using SEP490G69.Addons.LoadScreenSystem;
    using System.Collections.Generic;
    using System.Collections;
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
        private int _maxDeckSize = 9;

        private ImageMasterConfigSO _imgMasterConfig;
        private ImageMasterConfigSO ImgMasterConfig
        {
            get
            {
                if (_imgMasterConfig == null)
                {
                    _imgMasterConfig = Resources.Load<ImageMasterConfigSO>("Images/ImageMasterConfig");
                }
                return _imgMasterConfig;
            }
        }

        private GameDeckController _deckController;
        protected GameDeckController DeckController
        {
            get
            {
                if (_deckController == null)
                {
                    _deckController = ContextManager.Singleton.ResolveGameContext<GameDeckController>();
                }
                return _deckController;
            }
        }

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

            SessionPlayerDeck playerDeck = DeckController.GetCurrentDeck();

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
                            return;
                        }

                        SessionCardData cardData = _obtainedCards.FirstOrDefault(c => c.RawCardId.Equals(cardUI.RawCardId));
                        cardData.ObtainedAmount--;
                    }
                }
                else
                {
                    if (isInDeck)
                    {
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
                m_InDeckCountTmp.text = $"MY DECK: {_currentDeckIds.Count}/{_maxDeckSize} CARDS";
            }
        }

        private void SaveDeck()
        {
            DeckController.SaveDeck(_currentDeckIds);
            DeckController.SaveInventory(_obtainedCards);
            LocalDBOrchestrator.UpdateDBChangeTime();
            // TODO: Bổ sung hiệu ứng hoặc Toast message báo "Lưu thành công"
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

        public Sprite GetCardTypeImg(EActionType type)
        {
            string id = string.Empty;
            switch (type)
            {
                case EActionType.Attack:
                    id = "ic_atk";
                    break;
                case EActionType.Effect:
                    id = "ic_effect";
                    break;
                case EActionType.StatRecover:
                case EActionType.HPRecover:
                    id = "ic_recover";
                    break;
                default:
                    break;
            }
            
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            return ImgMasterConfig.GetImage("card_types", id)?.image;
        }

        private void Back()
        {
            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_MAIN_MENU);
        }
    }
}