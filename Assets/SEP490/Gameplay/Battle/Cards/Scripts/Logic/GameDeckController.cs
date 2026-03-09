namespace SEP490G69.Battle.Cards
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Handle cards and deck business logic.
    /// - Load obtained cards.
    /// - Load in-deck cards.
    /// - Add card to deck.
    /// - Remove card from deck.
    /// </summary>
    public class GameDeckController : MonoBehaviour, IGameContext
    {
        private ContextManager _contextManager;

        private GameCardsDAO _cardsDAO;
        private GameDeckDAO _deckDAO;

        private string _currentSessionId;
        private CardConfigSO _cardConfig;

        private void Awake()
        {
            _cardsDAO = new GameCardsDAO();
            _deckDAO = new GameDeckDAO();
            _currentSessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
        }

        public void SetManager(ContextManager manager)
        {
            _contextManager = manager;

            _cardConfig = _contextManager.GetDataSO<CardConfigSO>();
        }

        public void SetSessionId(string sessionId)
        {
            _currentSessionId = sessionId;
        }

        /// <summary>
        /// Lấy toàn bộ thẻ bài người chơi đang sở hữu từ Database.
        /// </summary>
        public List<SessionCardData> GetAllObtainedCards()
        {
            return _cardsDAO.GetAllBySessionId(_currentSessionId);
        }

        /// <summary>
        /// Lấy thông tin Deck hiện tại của người chơi.
        /// </summary>
        public SessionPlayerDeck GetCurrentDeck()
        {
            SessionPlayerDeck deck = _deckDAO.GetById(_currentSessionId);

            if (deck == null)
            {
                deck = new SessionPlayerDeck
                {
                    SessionId = _currentSessionId,
                    CardIds = new string[0]
                };
            }

            return deck;
        }

        /// <summary>
        /// Lưu cấu hình Deck mới vào Database (Ghi đè hoặc tạo mới).
        /// </summary>
        public void SaveDeck(List<string> cardIds)
        {
            SessionPlayerDeck deckToSave = new SessionPlayerDeck
            {
                SessionId = _currentSessionId,
                CardIds = cardIds.ToArray()
            };

            bool success = _deckDAO.Upsert(deckToSave);

            if (success)
            {
                Debug.Log($"[GameDeckController] Đã lưu Deck thành công với {cardIds.Count} thẻ.");
            }
            else
            {
                Debug.LogError("[GameDeckController] Xảy ra lỗi khi lưu Deck xuống Database!");
            }
        }

        /// <summary>
        /// Thêm thẻ bài mới vào kho (Inventory) khi người chơi nhận được (từ Shop, Exploration...).
        /// </summary>
        public void AddObtainedCard(string rawCardId, int amount = 1)
        {
            if (string.IsNullOrEmpty(rawCardId) || _cardConfig.GetCardById(rawCardId) == null)
            {
                Debug.LogError("[GameDeckController] Error: Invalid raw card id");
                return;
            }

            SessionCardData existingCard = _cardsDAO.GetById(_currentSessionId, rawCardId);

            if (existingCard != null)
            {
                existingCard.ObtainedAmount += amount;
                bool success = _cardsDAO.Update(existingCard);

                if (success)
                {
                    Debug.Log($"[GameDeckController] Increase card {rawCardId} stack amount to {existingCard.ObtainedAmount}.");
                }
                else
                {
                    Debug.LogError($"[GameDeckController] Failed to stack existed card {rawCardId}");
                }
            }
            else
            {
                SessionCardData newCard = new SessionCardData
                {
                    SessionCardId = $"{_currentSessionId}:{rawCardId}",
                    RawCardId = rawCardId,
                    SessionId = _currentSessionId,
                    ObtainedAmount = amount
                };

                bool success = _cardsDAO.Insert(newCard);

                if (success)
                {
                    Debug.Log($"[GameDeckController] Received new card success: {rawCardId}.");
                }
                else
                {
                    Debug.LogError($"[GameDeckController] Failed to receive new card: {rawCardId}");
                }
            }
        }

        private int CountDeckCardsByRawId(List<string> deckCards, string rawCardId)
        {
            int count = 0;

            foreach (string id in deckCards)
            {
                string[] parts = id.Split(':');

                if (parts.Length >= 2 && parts[1] == rawCardId)
                {
                    count++;
                }
            }

            return count;
        }

        public bool AddCardToDeck(string rawCardId)
        {
            SessionCardData ownedCard = _cardsDAO.GetById(_currentSessionId, rawCardId);

            if (ownedCard == null)
            {
                Debug.LogError($"Player does not own card {rawCardId}");
                return false;
            }

            SessionPlayerDeck deck = GetCurrentDeck();

            List<string> deckCards = deck.CardIds.ToList();

            int deckCount = CountDeckCardsByRawId(deckCards, rawCardId);

            if (deckCount >= ownedCard.ObtainedAmount)
            {
                Debug.LogWarning($"Cannot add {rawCardId} to deck. Not enough copies.");
                return false;
            }

            int variant = GenerateVariantIndex(deckCards, rawCardId);

            string deckCardId = string.Format(
                GameDeckDAO.FORMAT_IN_DECK_CARD_ID,
                _currentSessionId,
                rawCardId,
                variant
            );

            deckCards.Add(deckCardId);

            SaveDeck(deckCards);

            return true;
        }

        public bool RemoveCardFromDeck(string deckCardId)
        {
            SessionPlayerDeck deck = GetCurrentDeck();

            List<string> deckCards = deck.CardIds.ToList();

            bool removed = deckCards.Remove(deckCardId);

            if (!removed)
            {
                Debug.LogWarning($"Card {deckCardId} not found in deck.");
                return false;
            }

            SaveDeck(deckCards);

            return true;
        }

        private int GenerateVariantIndex(List<string> deckCards, string rawCardId)
        {
            int maxVariant = -1;

            foreach (string id in deckCards)
            {
                string[] parts = id.Split(':');

                if (parts.Length < 3) continue;

                if (parts[1] != rawCardId) continue;

                int variant = int.Parse(parts[2]);

                if (variant > maxVariant)
                    maxVariant = variant;
            }

            return maxVariant + 1;
        }
    }
}