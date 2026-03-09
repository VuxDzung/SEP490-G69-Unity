namespace SEP490G69.Battle.Cards
{
    using SEP490G69.Addons.LoadScreenSystem;
    using System.Collections.Generic;
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

        [Header("Data Config")]
        [SerializeField] private CardConfigSO m_CardConfig;

        private List<string> _currentDeckIds = new List<string>();
        private List<UICardElement> _spawnedCards = new List<UICardElement>();
        private int _maxDeckSize = 9;

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

        protected override void OnFrameShown()
        {
            base.OnFrameShown();
            m_BackBtn.onClick.AddListener(Back);
            m_SaveDeckBtn.onClick.AddListener(SaveDeck);

            LoadAllCards();
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
        private void LoadAllCards()
        {
            ClearSpawnedCards();

            SessionPlayerDeck playerDeck = DeckController.GetCurrentDeck();
            List<SessionCardData> obtainedCards = DeckController.GetAllObtainedCards();

            _currentDeckIds.Clear();

            if (playerDeck != null && playerDeck.CardIds != null)
            {
                _currentDeckIds.AddRange(playerDeck.CardIds);
            }

            Dictionary<string, int> deckUsage = new Dictionary<string, int>();

            // ---------- SPAWN DECK CARDS ----------
            foreach (string deckId in _currentDeckIds)
            {
                string rawId = CardUtils.ExtractRawCardId(deckId);

                CardSO staticCardData = m_CardConfig.GetCardById(rawId);
                if (staticCardData == null) continue;

                Transform cardUITransform =
                    PoolManager.Pools["UICard"].Spawn(m_CardUIPrefab, m_InDeckContainer);

                UICardElement cardElement = cardUITransform.GetComponent<UICardElement>();

                cardElement.Spawn();
                cardElement.SetContent(
                    deckId,
                    staticCardData.CardName,
                    staticCardData.CardDescription,
                    staticCardData.Icon);

                cardElement.SetOnSelectCallback(OnCardClicked);

                _spawnedCards.Add(cardElement);

                if (!deckUsage.ContainsKey(rawId))
                    deckUsage[rawId] = 0;

                deckUsage[rawId]++;
            }

            // ---------- SPAWN INVENTORY ----------
            foreach (var sessionCard in obtainedCards)
            {
                CardSO staticCardData = m_CardConfig.GetCardById(sessionCard.RawCardId);
                if (staticCardData == null) continue;

                int usedInDeck = deckUsage.ContainsKey(sessionCard.RawCardId)
                    ? deckUsage[sessionCard.RawCardId]
                    : 0;

                int remaining = sessionCard.ObtainedAmount - usedInDeck;

                for (int i = 0; i < remaining; i++)
                {
                    Transform cardUITransform =
                        PoolManager.Pools["UICard"].Spawn(m_CardUIPrefab, m_ObtainedContainer);

                    UICardElement cardElement = cardUITransform.GetComponent<UICardElement>();

                    cardElement.Spawn();
                    cardElement.SetContent(
                        sessionCard.RawCardId,
                        staticCardData.CardName,
                        staticCardData.CardDescription,
                        staticCardData.Icon);

                    cardElement.SetOnSelectCallback(OnCardClicked);

                    _spawnedCards.Add(cardElement);
                }
            }

            UpdateDeckCountText();
        }

        /// <summary>
        /// Xử lý logic di chuyển thẻ bài qua lại khi click.
        /// </summary>
        private void OnCardClicked(string cardId, bool isSelected, Transform cardTransform)
        {
            UICardElement cardUI = cardTransform.GetComponent<UICardElement>();
            bool isCurrentlyInDeck = cardTransform.parent == m_InDeckContainer;

            if (isCurrentlyInDeck)
            {
                _currentDeckIds.Remove(cardId);
                cardTransform.SetParent(m_ObtainedContainer, false);
            }
            else
            {
                if (_currentDeckIds.Count >= _maxDeckSize)
                {
                    Debug.LogWarning("Deck đã đầy!");
                    cardUI.Deselect();
                    return;
                }

                _currentDeckIds.Add(cardId);
                cardTransform.SetParent(m_InDeckContainer, false);
            }

            cardUI.Deselect();
            UpdateDeckCountText();
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
            // TODO: Bổ sung hiệu ứng hoặc Toast message báo "Lưu thành công"
        }

        private void ClearSpawnedCards()
        {
            foreach (var card in _spawnedCards)
            {
                if (card != null)
                {
                    PoolManager.Pools["UICard"].DespawnObject(card.transform);
                }
            }
            _spawnedCards.Clear();
        }

        private void Back()
        {
            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_MAIN_MENU);
        }
    }
}