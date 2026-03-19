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
    /// - Add obtained card (rawCardId:string, amount:int)
    /// - Remove obtained card (rawCardId:string, amount:int)
    /// </summary>
    public class GameDeckController : MonoBehaviour, IGameContext
    {
        public const int MAX_DECK_COUNT = 9;

        private ContextManager _contextManager;

        private GameCardsDAO _cardsDAO;
        private GameDeckDAO _deckDAO;

        private string _currentSessionId;
        private CardConfigSO _cardConfig;

        private SessionPlayerDeck _deck;

        private void Awake()
        {
            _cardsDAO = new GameCardsDAO();
            _deckDAO = new GameDeckDAO();
            _currentSessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);

            SetSessionId(_currentSessionId);
        }

        public void SetManager(ContextManager manager)
        {
            _contextManager = manager;

            _cardConfig = _contextManager.GetDataSO<CardConfigSO>();
        }

        public void SetSessionId(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                Debug.LogError("Session id is null");
                return;
            }

            _currentSessionId = sessionId;

            _deck = _deckDAO.GetById(sessionId);

            if (_deck == null)
            {
                Debug.Log("[GameDeckController] No deck existed. Create new deck!");
                _deck = new SessionPlayerDeck();
                _deck.SessionId = sessionId;
                _deck.CardIds = new string[0];

                if (!_deckDAO.Upsert(_deck))
                {
                    Debug.LogError($"[GameDeckController] Failed to upsert deck for session {sessionId}");
                }
            }
        }

        #region Card Inventory

        /// <summary>
        /// Lấy toàn bộ thẻ bài người chơi đang sở hữu từ Database.
        /// </summary>
        public List<SessionCardData> GetAllObtainedCards()
        {
            return _cardsDAO.GetAllBySessionId(_currentSessionId);
        }

        /// <summary>
        /// Thêm thẻ bài mới vào kho (Inventory) khi người chơi nhận được (từ Shop, Exploration...).
        /// </summary>
        public void AddObtainedCard(string rawCardId, int amount = 1)
        {
            SessionCardData card = _cardsDAO.GetById(_currentSessionId, rawCardId);

            Debug.Log($"<color=green>[GameDeckController]</color> Add obtainedcard: {rawCardId}, amount: {amount}");

            if (card == null)
            {
                card = new SessionCardData
                {
                    SessionCardId = EntityIdConstructor.ConstructDBEntityId(_currentSessionId, rawCardId),
                    SessionId = _currentSessionId,
                    RawCardId = rawCardId,
                    ObtainedAmount = amount
                };

                _cardsDAO.Insert(card);
            }
            else
            {
                card.ObtainedAmount += amount;
                _cardsDAO.Update(card);
            }
        }

        /// <summary>
        /// Only use this at the beginning of the game when you initialize the starter data.
        /// </summary>
        /// <param name="cards"></param>
        public bool AddManyCards(Dictionary<string, int> cards)
        {
            List<SessionCardData> cardList = new List<SessionCardData>();
            foreach (var cardPair in cards)
            {
                string rawCardId = cardPair.Key;
                int amount = cardPair.Value;
                SessionCardData card = new SessionCardData
                {
                    SessionCardId = EntityIdConstructor.ConstructDBEntityId(_currentSessionId, rawCardId),
                    SessionId = _currentSessionId,
                    RawCardId = rawCardId,
                    ObtainedAmount = amount
                };
                cardList.Add(card);
            }
            return _cardsDAO.InsertMany(cardList);
        }

        public void AddManyCardsToDeck(Dictionary<string, int> cards)
        {

        }

        public bool RemoveObtainedCard(string rawCardId, int amount)
        {
            SessionCardData card = _cardsDAO.GetById(_currentSessionId, rawCardId);

            if (card == null || card.ObtainedAmount < amount)
                return false;

            card.ObtainedAmount -= amount;

            _cardsDAO.Update(card);

            return true;
        }
        public void AddObtainedCards(List<string> rawCardIds)
        {
            foreach (var id in rawCardIds)
                AddObtainedCard(id, 1);
        }
        #endregion


        public bool IsCardInDeck(string deckCardId)
        {
            if (string.IsNullOrEmpty(deckCardId))
            {
                return false;
            }
            return _deck.CardIds.Contains(deckCardId);
        }

        private int CountDeckCardsByRawId(List<string> deckCards, string rawCardId)
        {
            int count = 0;

            foreach (string id in deckCards)
            {
                string[] parts = id.Split(':');

                if (parts.Length >= 2 && parts[1].Equals(rawCardId))
                {
                    count++;
                }
            }

            return count;
        }

        public int GetDeckCardCount()
        {
            if (_deck == null)
            {
                Debug.LogError("Failed to get player deck");
                return -1;
            }
            return _deck.CardIds.Length;
        }

        /// <summary>
        /// When add card to deck, decrease the stack inventory
        /// </summary>
        /// <param name="rawCardId"></param>
        /// <param name="autoUpdateDB"></param>
        /// <returns></returns>
        public bool AddCardToDeck(string rawCardId, bool autoUpdateDB = true)
        {
            if (_deck == null)
            {
                Debug.LogError("Failed to get player deck");
                return false;
            }

            if (_deck.CardIds.Length >= MAX_DECK_COUNT)
            {
                Debug.LogError("Max cards amount in deck exceeded");
                return false;
            }

            CardSO cardSO = _cardConfig.GetCardById(rawCardId);

            foreach (var cardId in _deck.CardIds)
            {
                string inDeckRawId = CardUtils.ExtractRawCardId(cardId);

                if (cardSO.CardId.Equals(inDeckRawId) &&
                    !cardSO.Stackable)
                {
                    Debug.LogError("You're trying to insert an unstackable card to deck");
                    return false;
                }
            }

            SessionCardData ownedCard = _cardsDAO.GetById(_currentSessionId, rawCardId);

            if (ownedCard == null)
            {
                Debug.LogError($"Player does not own card {rawCardId}");
                return false;
            }

            List<string> deckCards = _deck.CardIds.ToList();

            //int deckCount = CountDeckCardsByRawId(deckCards, rawCardId);

            //if (deckCount >= ownedCard.ObtainedAmount)
            //{
            //    Debug.Log($"<color=yellow>Warning: </color>Cannot add {rawCardId} to deck. Not enough copies.");
            //    return false;
            //}

            //int variant = GenerateVariantIndex(deckCards, rawCardId);
            string variant = System.Guid.NewGuid().ToString();

            string deckCardId = string.Format(
                GameDeckDAO.FORMAT_IN_DECK_CARD_ID,
                _currentSessionId,
                rawCardId,
                variant
            );

            deckCards.Add(deckCardId);
            _deck.CardIds = deckCards.ToArray();

            if (autoUpdateDB)
            {
                RemoveObtainedCard(rawCardId, 1);
                SaveDeck();
            }

            return true;
        }

        /// <summary>
        /// When remove card from deck, increase the stack amount of the obtained card in inventory.
        /// </summary>
        /// <param name="deckCardId"></param>
        /// <param name="autoSaveDB"></param>
        /// <returns></returns>
        public bool RemoveCardFromDeck(string deckCardId, bool autoSaveDB = true)
        {
            List<string> deckCards = _deck.CardIds.ToList();

            if (!deckCards.Contains(deckCardId))
            {
                Debug.LogError($"Card {deckCardId} is not in deck");
                return false;
            }

            if (!deckCards.Remove(deckCardId))
            {
                return false;
            }

            _deck.CardIds = deckCards.ToArray();

            if (autoSaveDB)
            {
                string rawCardId = CardUtils.ExtractRawCardId(deckCardId);
                AddObtainedCard(rawCardId, 1);
                SaveDeck();
            }

            return true;
        }

        public void AddCardsToDeck(List<string> rawIds, bool autoUpdateDB = true)
        {
            foreach (var rawId in rawIds)
                AddCardToDeck(rawId, autoUpdateDB);
        }

        public void RemoveCardsFromDeck(List<string> deckIds)
        {
            foreach (var id in deckIds)
                RemoveCardFromDeck(id);
        }

        /// <summary>
        /// Lấy thông tin Deck hiện tại của người chơi.
        /// </summary>
        public SessionPlayerDeck GetCurrentDeck()
        {
            if (_deck == null)
            {
                Debug.LogError("Deck is null");
                _deck = new SessionPlayerDeck
                {
                    SessionId = _currentSessionId,
                    CardIds = new string[0]
                };
            }

            return _deck;
        }

        public void SaveDeck(List<string> cardIds)
        {
            if (_deck == null)
            {
                Debug.LogError("[GameDeckController] Deck data is null");
                return;
            }

            _deck.CardIds = cardIds.ToArray();

            SaveDeck();
        }

        /// <summary>
        /// Lưu cấu hình Deck mới vào Database (Ghi đè hoặc tạo mới).
        /// </summary>
        public void SaveDeck()
        {
            if (_deck == null)
            {
                Debug.LogError("[GameDeckController] Deck data is null");
                return;
            }

            bool success = _deckDAO.Update(_deck);

            if (success)
            {
                Debug.Log($"[GameDeckController] Đã lưu Deck thành công với {_deck.CardIds.Length} thẻ.");
            }
            else
            {
                Debug.LogError("[GameDeckController] Xảy ra lỗi khi lưu Deck xuống Database!");
            }
        }

        public void SaveInventory(List<SessionCardData> cards)
        {
            foreach (var card in cards)
            {
                bool success = _cardsDAO.Update(card);

                if (!success)
                {
                    Debug.LogError($"Failed to update card {card.RawCardId}");
                }
            }
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