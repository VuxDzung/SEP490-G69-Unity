namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class CharacterCardsService : MonoBehaviour, ICombatCardsService
    {
        private PlayerActorController _owner;

        private List<CardSO> _deckPool = new List<CardSO>();
        private List<CardSO> _discardPool = new List<CardSO>();
        private List<CardSO> _currentDrawPool = new List<CardSO>();
        private List<CardSO> _exhaustPool = new List<CardSO>();

        private int _playedCardCountInTurn = 0;

        private CardSO _selectedCard = null;

        private List<BaseCard> _runtimeCardPool = new List<BaseCard>();

        public void SetOwner(PlayerActorController owner)
        {
            _owner = owner;
        }

        public void InitializeDeck(string[] deckCardIdArray)
        {
            if (deckCardIdArray == null || deckCardIdArray.Length == 0)
            {
                Debug.LogError("Deck is empty");
                return;
            }

            if (_owner == null)
            {
                Debug.Log("[CharacterCardsProcessor.InitializeDeck fatal error] Owner instance is null");
            }

            _deckPool.Clear();
            _discardPool.Clear();
            _currentDrawPool.Clear();

            CardSO restCard = _owner.CardConfig.GetCardById(CardConstants.CARD_ID_0000);

            _runtimeCardPool.Add(CardFactory.Create(restCard));

            foreach (var deckCardId in deckCardIdArray)
            {
                Debug.Log($"Initialize deck card: {deckCardId}");

                string rawCardId = CardUtils.ExtractRawCardId(deckCardId);
                CardSO card = _owner.CardConfig.GetCardById(rawCardId);


                if (card != null)
                {
                    Debug.Log($"{gameObject.name} add card {rawCardId}");
                    _deckPool.Add(card);
                    BaseCard runtimeCard = CardFactory.Create(card);
                    _runtimeCardPool.Add(runtimeCard);
                }
            }

            Shuffle(_deckPool);
        }

        public bool ExecuteCard(BaseCombatActor opponent)
        {
            if (_selectedCard != null)
            {
                var runtimeCard = GetRuntimeInstanceById(_selectedCard.CardId);

                if (runtimeCard == null)
                {
                    Debug.LogError($"[CharacterCardsProcessor.ExecuteCard fatail error] Failed to find the runtime instance of card {_selectedCard.CardId} of {_owner.gameObject.name}");
                    return false;
                }

                DecreaseStamina();
                runtimeCard.Execute(_owner, opponent);

                PutCardToDiscardPool(_selectedCard);

                if (_selectedCard.DrawCardAmount > 0)
                {
                    // Draw extra n-th card(s) to hand.
                    AddRandomCardsToHand(_selectedCard.DrawCardAmount);
                }

                return true;
            }
            else
            {
                Debug.Log("No selected card. Skip");
                return false;
            }
        }

        /// <summary>
        /// Discard current drawed cards when the turn's ended.
        /// </summary>
        public void DiscardCurrentDraw()
        {
            foreach (CardSO card in _currentDrawPool)
            {
                if (card.IsExhaustCard)
                {
                    continue;
                }
                _discardPool.Add(card);
            }

            _selectedCard = null;
        }

        public string GetFinalCardDescription(CardSO cardSO, string localizedCardDesc)
        {
            float damage = CalculateBaseDmg(cardSO);
            localizedCardDesc = localizedCardDesc.Replace("{{DMG}}", damage.ToString());
            return localizedCardDesc;
        }

        private void Shuffle(List<CardSO> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rand = UnityEngine.Random.Range(i, list.Count);
                (list[i], list[rand]) = (list[rand], list[i]);
            }
        }

        public float CalculateSelectedCardDmg(bool writeToDmgOutput)
        {
            if (_selectedCard == null)
            {
                return 0;
            }
            float baseDmg = CalculateBaseDmg(_selectedCard);
            if (writeToDmgOutput) _owner.StatsManager.SetCurrentValue(EStatusType.Damage, baseDmg);
            return baseDmg;
        }

        public float CalculateBaseDmg(CardSO card)
        {
            if (card == null)
            {
                Debug.LogError("No selected card to calculate base damage. Return 0 by default.");
                return 0f;
            }

            InCombatStatus status = _owner.StatsManager.GetStatus(card.ModifyStatType);
            if (status == null)
            {
                Debug.Log($"<color=red>[BaseBattleCharacterController.CalculateBaseDmg] Scale stat {card.ModifyStatType} is not registered</color>");
                return 0f;
            }

            float damage = card.BaseValue + card.GetDelta(status.Value);
            damage = (float) System.Math.Round(damage, 0);

            return damage;
        }

        public void SelectRest()
        {
            SelectCardById(CardConstants.CARD_ID_0000);
        }

        public void SelectNoAction()
        {
            SelectCard(null);
        }

        public void SelectCard(CardSO card)
        {
            _selectedCard = card;
        }

        public void DrawCards(int amount, out IReadOnlyList<CardSO> currentCards)
        {
            _currentDrawPool.Clear();

            int remainingToDraw = amount;

            while (remainingToDraw > 0)
            {
                // Nếu deck hết -> reshuffle từ discard
                if (_deckPool.Count == 0)
                {
                    ReshuffleFromDiscard();

                    // Nếu vẫn không có bài -> break luôn
                    if (_deckPool.Count == 0)
                        break;
                }

                int drawThisRound = Mathf.Min(remainingToDraw, _deckPool.Count);

                for (int i = 0; i < drawThisRound; i++)
                {
                    CardSO card = _deckPool[0];
                    _deckPool.RemoveAt(0);
                    _currentDrawPool.Add(card);
                }

                remainingToDraw -= drawThisRound;
            }

            currentCards = _currentDrawPool;
        }

        private void PutCardToDiscardPool(CardSO cardSO)
        {
            if (!_currentDrawPool.Remove(cardSO))
            {
                Debug.LogWarning($"Card {cardSO.CardId} is not in current draw pool!");
                return;
            }

            if (cardSO.IsExhaustCard)
            {
                _exhaustPool.Add(cardSO);
                return;
            }

            _discardPool.Add(cardSO);
        }

        private void ReshuffleFromDiscard()
        {
            if (_discardPool.Count == 0)
                return;

            _deckPool.AddRange(_discardPool);
            _discardPool.Clear();
            Shuffle(_deckPool);
        }

        public void AddCardToHand(CardSO cardSO, bool removeFromPools = false)
        {
            if (cardSO == null)
            {
                Debug.LogError("AddCardToHand: card is null");
                return;
            }

            if (removeFromPools)
            {
                _deckPool.Remove(cardSO);
                _discardPool.Remove(cardSO);
            }

            _currentDrawPool.Add(cardSO);

            Debug.Log($"Add card to hand: {cardSO.CardId}");
        }

        public void AddRandomCardsToHand(int amount)
        {
            int remaining = amount;

            while (remaining > 0)
            {
                if (_deckPool.Count == 0)
                {
                    ReshuffleFromDiscard();

                    if (_deckPool.Count == 0)
                        break;
                }

                int take = Mathf.Min(remaining, _deckPool.Count);

                for (int i = 0; i < take; i++)
                {
                    int lastIndex = _deckPool.Count - 1;

                    CardSO card = _deckPool[lastIndex];
                    _deckPool.RemoveAt(lastIndex);

                    _currentDrawPool.Add(card);

                    Debug.Log($"Add random card to hand: {card.CardId}");
                }

                remaining -= take;
            }
        }

        public void SelectCardById(string cardId)
        {
            CardSO cardSO = _currentDrawPool.FirstOrDefault(c => c.CardId.Equals(cardId));
            if (cardSO == null)
            {
                if (cardId.Equals(CardConstants.CARD_ID_0000))
                {
                    cardSO = _owner.CardConfig.GetCardById(cardId);
                }
                else
                {
                    Debug.LogError("CardSO with id is not in the current draw");
                    return;
                }
            }
            SelectCard(cardSO);
        }

        public int CalculateCardCost(CardSO card)
        {
            _owner.StatsManager.SetCurrentValue(EStatusType.ActionCost, card.Cost);

            float cost = _owner.StatsManager.GetValue(EStatusType.ActionCost);
            Debug.Log($"{_owner.CharacterSO.CharacterName}, Card: {card.CardId}. Base cost: {card.Cost} - Final cost: {_owner.StatsManager.GetValue(EStatusType.ActionCost)}");
            return Mathf.Max(0, Mathf.RoundToInt(cost));
        }

        public void DecreaseStamina()
        {
            if (_selectedCard == null)
            {
                return;
            }
            Debug.Log($"Card {_selectedCard.CardId} cost: {_selectedCard.Cost}");
            float cost = CalculateCardCost(_selectedCard);
            DecreaseStamina(cost);
        }

        private void DecreaseStamina(float stamina)
        {
            _owner.StaminaManager.Spend(stamina);
        }

        private BaseCard GetRuntimeInstanceById(string rawCardId)
        {
            if (string.IsNullOrEmpty(rawCardId))
            {
                return null;
            }
            BaseCard card = _runtimeCardPool.FirstOrDefault(c => c.RawCardId ==  rawCardId);
            return card;
        }

        public int GetPlayedCardInTurn()
        {
            return _playedCardCountInTurn;
        }

        public bool IsCardUsable(CardSO card, SceneCombatController sceneController)
        {
            IUsableCondition usableCondition = UsableConditionFactory.GetById(card.UsableConditionId);

            if (usableCondition != null)
            {
                return usableCondition.IsCardUsable(card, sceneController);
            }

            return true;
        }

        public IReadOnlyList<CardSO> GetInHandCards()
        {
            return _currentDrawPool;
        }

        public IReadOnlyList<CardSO> GetInDeckCards()
        {
            return _deckPool;
        }

        public IReadOnlyList<CardSO> GetDiscardedCards()
        {
            return _discardPool;
        }
    }
}