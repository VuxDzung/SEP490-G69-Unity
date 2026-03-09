namespace SEP490G69.Battle.Cards
{
    using System.Collections.Generic;
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
        private void Awake()
        {
            _cardsDAO = new GameCardsDAO();
            _deckDAO = new GameDeckDAO();
            _currentSessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);
        }

        public void SetManager(ContextManager manager)
        {
            _contextManager = manager;
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
            if (string.IsNullOrEmpty(rawCardId))
            {
                Debug.LogError("[GameDeckController] Lỗi: rawCardId không hợp lệ khi thêm bài mới!");
                return;
            }

            SessionCardData existingCard = _cardsDAO.GetById(_currentSessionId, rawCardId);

            if (existingCard != null)
            {
                existingCard.ObtainedAmount += amount;
                bool success = _cardsDAO.Update(existingCard);

                if (success)
                {
                    Debug.Log($"[GameDeckController] Đã tăng số lượng thẻ {rawCardId} thành {existingCard.ObtainedAmount}.");
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
                    Debug.Log($"[GameDeckController] Nhận thẻ bài mới thành công: {rawCardId}.");
                }
            }
        }
    }
}