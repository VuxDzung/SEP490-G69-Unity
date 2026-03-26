namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class CardContextWeightStrategy : ISelectCardStrategy
    {
        private readonly int BASE_CONTEXT_VALUE_MIN = 10;
        private readonly int BASE_CONTEXT_VALUE_MAX = 20;

        private readonly int MAX_STATUS_EFFECT_STACK = 10;

        private readonly BaseBattleCharacterController _owner;

        public CardContextWeightStrategy(BaseBattleCharacterController owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Select a card based on context point decision which align with each card.
        /// - If the AI's HP is <=30% HP, Recovery cards receive a massive priority boost (+50 points).
        /// - The AI scans active status effects. If a specific Buff or Debuff is not currently active on the field, corresponding Effect cards gain high priority (+30 points). If the status is already fully stacked, the priority drops to 0 to prevent redundant plays.
        /// - Normal Attack cards maintain a baseline priority (10-20 points), scaling slightly higher if the player's HP is low enough to secure a knockout.
        /// </summary>
        /// <param name="opponent">The player character</param>
        /// <param name="currentDrawPool">The current drawed cards at the moment</param>
        /// <param name="card">The selected card.</param>
        /// <returns></returns>
        public bool TrySelectCard(BaseBattleCharacterController opponent, IReadOnlyList<CardSO> currentDrawPool, out CardSO card)
        {
            card = null;

            if (currentDrawPool == null || currentDrawPool.Count == 0)
            {
                return false;
            }

            List<ContextualCardData> contextualCards = new List<ContextualCardData>();

            // 1. Base points
            foreach (var item in currentDrawPool)
            {
                contextualCards.Add(new ContextualCardData
                {
                    Card = item,
                    ContextPoints = Random.Range(BASE_CONTEXT_VALUE_MIN, BASE_CONTEXT_VALUE_MAX)
                });
            }

            float ownerCurrentHP = _owner.GetCombatStatus(EStatusType.Vitality).Value;
            float ownerMaxHP = _owner.ReadonlyDataHolder.GetVIT();
            float ownerHPPercent = ownerCurrentHP / ownerMaxHP;

            float opponentHP = opponent.GetCombatStatus(EStatusType.Vitality).Value;

            RuntimeStatusEffect[] ownerEffects = _owner.StatEffectManager.GetEffectsByType(EEffectType.Both);
            RuntimeStatusEffect[] opponentEffects = opponent.StatEffectManager.GetEffectsByType(EEffectType.Both);

            // 2. Apply context rules
            foreach (var data in contextualCards)
            {
                var cardSO = data.Card;

                // RULE 1: Low HP -> recover hp
                if (ownerHPPercent <= 0.3f && cardSO.ActionType == EActionType.StatRecover)
                {
                    data.ContextPoints += 50;
                }

                // RULE 2: Attack logic (kill potential)
                if (cardSO.ActionType == EActionType.Attack)
                {
                    float estimatedDamage = _owner.CalculateBaseCardDMG(cardSO);

                    if (estimatedDamage >= opponentHP)
                    {
                        // Killable
                        data.ContextPoints += 40;
                    }
                    else if (opponentHP <= estimatedDamage * 2)
                    {
                        // Almost dead -> increase slightly.
                        data.ContextPoints += 15;
                    }
                }

                // RULE 3: Buff / Debuff logic
                List<StatusEffectSO> effectList = cardSO.GetAllStatusEffects();
                if (effectList.Count > 0)
                {
                    foreach (var effect in effectList)
                    {
                        bool isBuff = effect.EffectType == EEffectType.Buff;
                        var targetEffects = isBuff ? ownerEffects : opponentEffects;

                        RuntimeStatusEffect existing = targetEffects.FirstOrDefault(e => e.Data.EffectId == effect.EffectId);

                        if (existing == null)
                        {
                            // Haven't used -> increase context point.
                            data.ContextPoints += 30;
                        }
                        else if (existing.Stack >= MAX_STATUS_EFFECT_STACK)
                        {
                            // Full stack -> useless
                            data.ContextPoints = 0;
                        }
                        else
                        {
                            // Not full stack yet -> increase context point.
                            data.ContextPoints += 10;
                        }
                    }
                }
            }

            // 3. Weighted random
            int totalWeight = contextualCards.Sum(x => Mathf.Max(0, x.ContextPoints));

            if (totalWeight <= 0)
            {
                // fallback random
                card = currentDrawPool[Random.Range(0, currentDrawPool.Count)];
                return true;
            }

            int roll = Random.Range(0, totalWeight);
            int cumulative = 0;

            foreach (var data in contextualCards)
            {
                cumulative += Mathf.Max(0, data.ContextPoints);

                if (roll < cumulative)
                {
                    card = data.Card;
                    return true;
                }
            }

            // fallback (should not happen)
            card = currentDrawPool[0];
            return true;
        }
    }

    public class ContextualCardData
    {
        public CardSO Card { get; set; }
        public int ContextPoints { get; set; }
    }
}