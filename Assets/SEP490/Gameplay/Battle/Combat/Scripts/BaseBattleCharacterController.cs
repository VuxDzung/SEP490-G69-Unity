namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public abstract class BaseBattleCharacterController : MonoBehaviour, ICardExecutor
    {
        #region Events

        public event Action<BaseBattleCharacterController> OnEnergyFull;
        public event Action OnDead;

        #endregion

        #region Configs (Lazy Loaded)

        private CharacterConfigSO _characterConfig;
        protected CharacterConfigSO CharacterConfig =>
            _characterConfig ??= ContextManager.Singleton.GetDataSO<CharacterConfigSO>();

        private CardConfigSO _cardConfig;
        protected CardConfigSO CardConfig =>
            _cardConfig ??= ContextManager.Singleton.GetDataSO<CardConfigSO>();

        #endregion

        #region Fields

        private bool _isBarPaused;
        private EnergyTurnBar _energyTurnBar;

        private CharacterDataHolder _readonlyDataHolder;
        private CharacterDataHolder _currentDataHolder;

        private StatusEffectManager _statEffectManager;

        private List<CardSO> _deckPool = new List<CardSO>();
        private List<CardSO> _discardPool = new List<CardSO>();
        private List<CardSO> _currentDrawPool = new List<CardSO>();
        private CardSO _selectedCard = null;


        private readonly List<CombatStatModifierSO> _pendingGaugeModifiers = new();
        #endregion

        #region Properties

        /// <summary>
        /// Get the data which is readonly in combat.
        /// </summary>
        public CharacterDataHolder ReadonlyDataHolder => _readonlyDataHolder;
        /// <summary>
        /// Get the data which is changed in combat.
        /// </summary>
        public CharacterDataHolder CurrentDataHolder => _currentDataHolder;

        public CardSO SelectedCard => _selectedCard;
        public StatusEffectManager StatEffectManager => _statEffectManager;

        public BaseBattleCharacterController LastAttacker { get; private set; }
        #endregion

        #region Initialization
        protected virtual void Awake()
        {
            _statEffectManager = new StatusEffectManager(this);
        }

        public abstract void Initialize(BaseCharacterSO characterSO);

        public void SetCharacterDataHolder(CharacterDataHolder holder)
        {
            _currentDataHolder = holder;
        }

        public void SetReadonlyDataHolder(CharacterDataHolder holder)
        {
            _readonlyDataHolder = holder;
        }

        #endregion

        // =========================================================
        // CARD PIPELINE
        // =========================================================
        #region Card Pipeline

        public virtual void ExecuteCard(
            BaseBattleCharacterController source,
            BaseBattleCharacterController target)
        {
            BaseCard runtimeCard = CardFactory.Create(_selectedCard);

            Debug.Log($"Before decrease.Current stamina: {CurrentDataHolder.GetStamina()}");
            DecreaseStamina();
            Debug.Log($"Before execute.Current stamina: {CurrentDataHolder.GetStamina()}");
            runtimeCard.Execute(this, target);
            Debug.Log($"After execute.Current stamina: {CurrentDataHolder.GetStamina()}");

            //ExecuteMainAction(SelectedCard, source, target);
            //AddStatusEffects(SelectedCard.StatusGains, source, target, ETargetType.Self);
            //AddStatusEffects(SelectedCard.StatusInflicts, source, target, ETargetType.Opponent);
        }

        public virtual void EndCurrentTurn()
        {
            StatEffectManager.OnAction();

            OnTurnEnd();

            foreach (CardSO card in _currentDrawPool)
            {
                if (card != _selectedCard)
                    _discardPool.Add(card);
            }
            _selectedCard = null;


            ResetCharge();

            ApplyQueuedGaugeModifiers();
        }
        #endregion

        // =========================================================
        // DAMAGE SYSTEM
        // =========================================================
        #region Damage System

        public void ReceiveDamage(float damage, BaseBattleCharacterController attacker)
        {
            LastAttacker = attacker;

            float finalDamage = Mathf.Max(
                1,
                damage - CurrentDataHolder.GetDef());

            CurrentDataHolder.ModifyStat(EStatusType.Vitality, -finalDamage);

            StatEffectManager.OnAfterReceiveDamage(finalDamage);

            CheckDeath();
        }

        private void CheckDeath()
        {
            if (CurrentDataHolder.GetVIT() > 0) return;

            Debug.Log($"{CurrentDataHolder.GetCharacterName()} has died.");
            PauseBar();

            OnDead?.Invoke();
        }

        public void DecreaseStamina()
        {
            if (_selectedCard == null)
            {
                return;
            }
            Debug.Log($"Card {_selectedCard.CardId} cost: {_selectedCard.Cost}");
            DecreaseStamina(_selectedCard.Cost);
        }

        public void DecreaseStamina(float stamina)
        {
            float cur = CurrentDataHolder.GetStamina();
            cur -= stamina;
            cur = Mathf.Clamp(cur, 0, ReadonlyDataHolder.GetStamina());
            CurrentDataHolder.SetStamina(cur);
        }
        #endregion

        // =========================================================
        // STATUS SYSTEM
        // =========================================================
        #region Status System

        public void OnTurnStart()
        {
            StatEffectManager.StartTurn();
        }

        public void OnTurnEnd()
        {
            StatEffectManager.EndTurn();
        }

        /// <summary>
        /// Apply status changes.
        /// NOTE: 
        /// + If the changes require/use max vit or any similar stats, get from ReadonlyDataHolder
        /// + If the changes use current stat like current vitality, current stamina, etc., get from CharacterDataHolder.
        /// Able to handle:
        /// - Tactical Rest
        /// 
        /// </summary>
        /// <param name="modifierSO"></param>
        public void ApplyStatusDelta(CombatStatModifierSO modifierSO, bool fromExternal)
        {
            if (modifierSO == null)
                return;

            if (modifierSO.StatType == EStatusType.ActionGauge)
            {
                QueueGaugeModifier(modifierSO);

                if (fromExternal) // If it from opponent, apply immediate.
                {
                    ApplyQueuedGaugeModifiers();
                }

                return;
            }

            if (modifierSO.IsInCombatStats())
            {
                Debug.Log("Is in combat stats");
                //CombatModifiers.Add(modifierSO);
                return;
            }

            EStatusType statType = modifierSO.StatType;

            float maxValue = ReadonlyDataHolder.GetStatus(statType);
            float currentValue = CurrentDataHolder.GetStatus(statType);

            float calculationValue = modifierSO.CalculateSource switch
            {
                EStatCalculationSource.Current => currentValue,
                EStatCalculationSource.Max => maxValue,
                EStatCalculationSource.Lost => maxValue - currentValue,
                EStatCalculationSource.FixedValue => modifierSO.Value,
                _ => currentValue
            };
            Debug.Log($"Calculation value: {calculationValue}, CurrentStamina: {CurrentDataHolder.GetStatus(statType)}");
            float delta = modifierSO.GetDelta(calculationValue);
            Debug.Log($"Modify stat: {statType.ToString()}\nPlusValue={delta}\nCurrentValue={CurrentDataHolder.GetStatus(statType)}");
            CurrentDataHolder.ModifyStat(statType, delta);

            // Clamp stat.
            float currentStat = CurrentDataHolder.GetStatus(statType);
            currentStat = Mathf.Clamp(currentStat, 0, ReadonlyDataHolder.GetStatus(statType));
            CurrentDataHolder.SetStatus(statType, currentStat);
        }

        private void QueueGaugeModifier(CombatStatModifierSO modifier)
        {
            _pendingGaugeModifiers.Add(modifier);
        }

        private void ApplyQueuedGaugeModifiers()
        {
            if (_pendingGaugeModifiers.Count == 0)
                return;

            float gauge = _energyTurnBar.CurrentValue;

            foreach (var modifier in _pendingGaugeModifiers)
            {
                gauge = modifier.GetModifiedStatus(_energyTurnBar.MaxValue);
                //gauge = value;
            }

            gauge = Mathf.Clamp(gauge, 0, _energyTurnBar.MaxValue);

            _energyTurnBar.SetValue(gauge);

            _pendingGaugeModifiers.Clear();
        }

        #endregion

        // =========================================================
        // ENERGY SYSTEM
        // =========================================================
        #region Energy System

        public void InitializeEnergySystem()
        {
            var strategy = new AgiBasedChargeStrategy(
                GameConstants.BASE_FILL_SPEED,
                CurrentDataHolder.GetAgi()
            );

            _energyTurnBar = new EnergyTurnBar(strategy);
        }

        public void UpdateCharge(float deltaTime)
        {
            if (_energyTurnBar == null || _isBarPaused || _energyTurnBar.IsFull)
                return;

            _energyTurnBar.Update(deltaTime);

            if (_energyTurnBar.IsFull)
                OnEnergyFull?.Invoke(this);
        }

        public void ResetCharge()
        {
            _energyTurnBar?.Reset();
        }

        public float GetCurrentEnergyValue() => _energyTurnBar.CurrentValue;
        public float GetMaxEnergyValue() => _energyTurnBar.MaxValue;
        public void PauseBar() => _isBarPaused = true;
        public void UnpauseBar() => _isBarPaused = false;

        #endregion

        #region Cards
        protected void InitializeDeck(string[] cardIdArray)
        {
            if (cardIdArray == null || cardIdArray.Length == 0)
            {
                Debug.LogError("Deck is empty");
                return;
            }

            _deckPool.Clear();
            _discardPool.Clear();
            _currentDrawPool.Clear();

            foreach (var id in cardIdArray)
            {
                CardSO card = CardConfig.GetCardById(id);
                if (card != null)
                    _deckPool.Add(card);
            }
        }

        private void Shuffle(List<CardSO> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rand = UnityEngine.Random.Range(i, list.Count);
                (list[i], list[rand]) = (list[rand], list[i]);
            }
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
                    cardSO = CardConfig.GetCardById(cardId);
                }
                else
                {
                    Debug.LogError("CardSO with id is not in the current draw");
                    return;
                }
            }
            SelectCard(cardSO);
        }

        public void SelectRest()
        {
            SelectCardById(CardConstants.CARD_ID_0000);
        }

        public void SelectCard(CardSO card)
        {
            _selectedCard = card;
        }
        public void DeselectCurrentCard()
        {
            _selectedCard = null;
        }

        public int CalculateCardCost(CardSO card)
        {
            float cost = card.Cost;

            cost = StatEffectManager.ModifyActionCost(cost);

            return Mathf.Max(0, Mathf.RoundToInt(cost));
        }
        #endregion
    }
}