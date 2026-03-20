namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class CharacterCardsProcessor : MonoBehaviour, ICombatCardsProcessor
    {
        private BaseBattleCharacterController _owner;

        private List<CardSO> _deckPool = new List<CardSO>();
        private List<CardSO> _discardPool = new List<CardSO>();
        private List<CardSO> _currentDrawPool = new List<CardSO>();
        private CardSO _selectedCard = null;

        private List<BaseCard> _runtimeCardPool = new List<BaseCard>();

        public void SetOwner(BaseBattleCharacterController owner)
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

            _deckPool.Clear();
            _discardPool.Clear();
            _currentDrawPool.Clear();

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
        }

        public bool ExecuteCard(BaseBattleCharacterController target)
        {
            if (_selectedCard != null)
            {
                BaseCard runtimeCard = _runtimeCardPool.FirstOrDefault(card => card.RawCardId == _selectedCard.CardId);
                if (runtimeCard == null)
                {
                    Debug.LogError($"[CharacterCardsProcessor.ExecuteCard fatail error] Failed to find the runtime instance of card {_selectedCard.CardId} of {_owner.gameObject.name}");
                    return false;
                }

                DecreaseStamina();
                runtimeCard.Execute(_owner, target);
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
                _discardPool.Add(card);
            }

            _selectedCard = null;
        }

        private void Shuffle(List<CardSO> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rand = UnityEngine.Random.Range(i, list.Count);
                (list[i], list[rand]) = (list[rand], list[i]);
            }
        }

        public float CalculateSelectedCardDmg()
        {
            if (_selectedCard == null)
            {
                return 0;
            }
            float baseDmg = CalculateBaseDmg(_selectedCard);
            _owner.StatOutputDmg.SetCurrentValue(baseDmg);
            return baseDmg;
        }

        public float CalculateBaseDmg(CardSO card)
        {
            if (card == null)
            {
                Debug.LogError("No selected card to calculate base damage. Return 0 by default.");
                return 0f;
            }

            InCombatStatus status = _owner.GetCombatStatus(card.ModifyStatType);
            if (status == null)
            {
                Debug.Log($"<color=red>[BaseBattleCharacterController.CalculateBaseDmg] Scale stat {card.ModifyStatType} is not registered</color>");
                return 0f;
            }

            float damage = card.BaseValue + card.GetDelta(status.Value);

            Debug.Log($"{gameObject.name} deal pure dmg: {damage}\nExtra final dmg: {_owner.StatOutputDmg.Value}");

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

        public void DrawThreeCards(out IReadOnlyList<CardSO> currentCards)
        {
            if (_deckPool.Count == 0)
            {
                ReshuffleFromDiscard();
            }

            _currentDrawPool.Clear();

            int drawCount = Mathf.Min(3, _deckPool.Count);

            for (int i = 0; i < drawCount; i++)
            {
                CardSO card = _deckPool[0];
                _deckPool.RemoveAt(0);
                _currentDrawPool.Add(card);
            }

            currentCards = _currentDrawPool;
        }

        private void ReshuffleFromDiscard()
        {
            if (_discardPool.Count == 0)
                return;

            _deckPool.AddRange(_discardPool);
            _discardPool.Clear();
            Shuffle(_deckPool);
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
            _owner.StatActionCost.SetCurrentValue(card.Cost);

            float cost = _owner.StatActionCost.Value;

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
            float cur = _owner.StatStamina.BaseValue;
            cur -= stamina;
            cur = Mathf.Clamp(cur, 0, _owner.ReadonlyDataHolder.GetStamina());

            _owner.StatStamina.SetCurrentValue((float)cur);
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
    }
}