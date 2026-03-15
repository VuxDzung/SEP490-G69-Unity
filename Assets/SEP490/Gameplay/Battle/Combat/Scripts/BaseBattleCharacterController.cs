namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Training;
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
        //private CharacterDataHolder _currentDataHolder;

        private StatusEffectManager _statEffectManager;

        private List<CardSO> _deckPool = new List<CardSO>();
        private List<CardSO> _discardPool = new List<CardSO>();
        private List<CardSO> _currentDrawPool = new List<CardSO>();
        private CardSO _selectedCard = null;

        public InCombatStatus StatVit {  get; private set; } = new InCombatStatus();
        public InCombatStatus StatPow { get; private set; } = new InCombatStatus();
        public InCombatStatus StatAgi { get; private set; } = new InCombatStatus();
        public InCombatStatus StatInt { get; private set; } = new InCombatStatus();
        public InCombatStatus StatStamina { get; private set; } = new InCombatStatus();
        public InCombatStatus StatDEF { get; private set; } = new InCombatStatus();


        public InCombatStatus StatOutputDmg { get; private set; } = new InCombatStatus();
        public InCombatStatus StatReceivedDmg { get; private set; } = new InCombatStatus();
        public InCombatStatus StatActionCost { get; private set; } = new InCombatStatus();
        public InCombatStatus StatHitRate { get; private set; } = new InCombatStatus();


        private readonly List<CombatStatModifierSO> _pendingGaugeModifiers = new List<CombatStatModifierSO>();
        private readonly Dictionary<EStatusType, InCombatStatus> _statusContainer = new Dictionary<EStatusType, InCombatStatus>();
        #endregion

        #region Properties
        public CharacterVFXController VFXController { get; private set; }
        public CharacterAnimationController AnimationController { get; private set; }

        /// <summary>
        /// Get the data which is readonly in combat.
        /// </summary>
        public CharacterDataHolder ReadonlyDataHolder => _readonlyDataHolder;
        /// <summary>
        /// Get the data which is changed in combat.
        /// </summary>
        //public CharacterDataHolder CurrentDataHolder => _currentDataHolder;

        public CardSO SelectedCard => _selectedCard;
        public StatusEffectManager StatEffectManager => _statEffectManager;

        public BaseBattleCharacterController LastAttacker { get; private set; }
        #endregion

        #region Initialization
        protected virtual void Awake()
        {
            _statEffectManager = new StatusEffectManager(this);
            AnimationController = GetComponent<CharacterAnimationController>();
            VFXController = GetComponent<CharacterVFXController>();
        }

        public abstract void Initialize(BaseCharacterSO characterSO);

        public void SetCharacterDataHolder(CharacterDataHolder holder)
        {
            //_currentDataHolder = holder;

            StatVit.SetCurrentValue(holder.GetVIT());
            StatPow.SetCurrentValue(holder.GetPower());
            StatAgi.SetCurrentValue(holder.GetAgi());
            StatInt.SetCurrentValue(holder.GetINT());
            StatStamina.SetCurrentValue(holder.GetStamina());
            StatDEF.SetCurrentValue(holder.GetDef());

            StatOutputDmg.SetCurrentValue(0f);
            StatReceivedDmg.SetCurrentValue(0f);
            StatActionCost.SetCurrentValue(0f);

            _statusContainer.Clear();

            _statusContainer.Add(EStatusType.Vitality, StatVit);
            _statusContainer.Add(EStatusType.Power, StatPow);
            _statusContainer.Add(EStatusType.Agi, StatAgi);
            _statusContainer.Add(EStatusType.Intelligence, StatInt);
            _statusContainer.Add(EStatusType.Stamina, StatStamina);

            _statusContainer.Add(EStatusType.Damage, StatOutputDmg);
            _statusContainer.Add(EStatusType.ReceivedDmg, StatReceivedDmg);
            _statusContainer.Add(EStatusType.ActionCost, StatActionCost);
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

        public virtual void ExecuteCard(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            TriggerTurnFlowEvent(ETurnFlowEvent.BeforeCardAction);
            StatEffectManager.Trigger(ETurnFlowEvent.BeforeCardAction, target);

            if (_selectedCard != null)
            {
                BaseCard runtimeCard = CardFactory.Create(_selectedCard);

                DecreaseStamina();
                runtimeCard.Execute(this, target);
            }
            else
            {
                Debug.Log("No selected card. Skip");
            }

            TriggerTurnFlowEvent(ETurnFlowEvent.AfterCardAction);
            StatEffectManager.Trigger(ETurnFlowEvent.AfterCardAction, target);
        }

        public virtual void EndCurrentTurn()
        {
            OnTurnEnd();

            foreach (CardSO card in _currentDrawPool)
            {
                _discardPool.Add(card); // Put all 3 cards to trash pool.
            }

            _selectedCard = null;

            ResetCharge();

            TriggerTurnFlowEvent(ETurnFlowEvent.AfterResetActionGaugue);

            OnAfterResetActionGauge();
        }
        #endregion

        // =========================================================
        // DAMAGE SYSTEM
        // =========================================================
        #region Damage System

        public void ReceiveDamage(float damage, BaseBattleCharacterController attacker)
        {
            LastAttacker = attacker;

            float finalDamage = Mathf.Max(1, damage - StatDEF.Value);

            StatReceivedDmg.SetCurrentValue(finalDamage);
            float finalVit = StatVit.Value - StatReceivedDmg.Value;
            StatVit.SetCurrentValue(finalVit);

            OnAfterReceiveDamage(finalDamage, attacker);

            CheckDeath();
        }

        private void CheckDeath()
        {
            if (StatVit.Value > 0) return;

            Debug.Log($"{ReadonlyDataHolder.GetCharacterName()} has died.");
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
            float cost = CalculateCardCost(_selectedCard);
            DecreaseStamina(cost);
        }

        private void DecreaseStamina(float stamina)
        {
            float cur = StatStamina.BaseValue;
            cur -= stamina;
            cur = Mathf.Clamp(cur, 0, ReadonlyDataHolder.GetStamina());
            //CurrentDataHolder.SetStamina(cur);
            StatStamina.SetCurrentValue((float)cur);
        }
        #endregion

        // =========================================================
        // STATUS SYSTEM
        // =========================================================
        #region Status System

        public void OnTurnStart()
        {
            TriggerTurnFlowEvent(ETurnFlowEvent.TurnStarted);
            StatEffectManager.Trigger(ETurnFlowEvent.TurnStarted, LastAttacker);

            StatEffectManager.StartTurn();
        }

        public void OnTurnEnd()
        {
            StatEffectManager.EndTurn();
        }

        /// <summary>
        /// Apply status changes immediately (FIX: Handle all the immediate status modify action at the InCombatStatus.
        /// </summary>
        /// <param name="modifierSO"></param>
        public void ApplyStatusDelta(CombatStatModifierSO modifierSO, bool fromExternal)
        {
            if (modifierSO == null)
                return;

            if (modifierSO.StatType == EStatusType.ActionGauge)
            {
                QueueGaugeModifier(modifierSO);

                if (fromExternal)
                    ApplyQueuedGaugeModifiers();

                return;
            }

            if (_statusContainer.TryGetValue(modifierSO.StatType, out var status) == false)
                return;

            float maxValue = ReadonlyDataHolder.GetStatus(modifierSO.StatType);
            float currentValue = status.Value;

            float calculationValue = modifierSO.CalculateSource switch
            {
                EStatCalculationSource.Current => currentValue,
                EStatCalculationSource.Max => maxValue,
                EStatCalculationSource.Lost => maxValue - currentValue,
                EStatCalculationSource.FixedValue => modifierSO.Value,
                _ => currentValue
            };

            float delta = modifierSO.GetDelta(calculationValue);

            // Let status effects modify the delta
            delta = _statEffectManager.ModifyStatDelta(modifierSO.StatType, delta);

            float newValue = currentValue + delta;

            newValue = Mathf.Clamp(newValue, 0, maxValue);

            status.SetCurrentValue(newValue);
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

        public void AddStatusEffect(StatusEffectSO effectSO)
        {
            StatEffectManager.AddStatusEffect(effectSO);
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
                StatAgi.Value
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
        protected void InitializeDeck(string[] deckCardIdArray)
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
                string rawCardId = CardUtils.ExtractRawCardId(deckCardId);
                CardSO card = CardConfig.GetCardById(rawCardId);
                if (card != null)
                {
                    _deckPool.Add(card);
                }
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
        public void SelectNoAction()
        {
            SelectCard(null);
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
            StatActionCost.SetCurrentValue(card.Cost);

            float cost = StatActionCost.Value;

            return Mathf.Max(0, Mathf.RoundToInt(cost));
        }
        #endregion

        private void OnAfterReceiveDamage(float damage, BaseBattleCharacterController attacker)
        {
            StatEffectManager.OnAfterReceiveDamage(damage);
        }

        public void AddEffectModifier(CombatStatModifierSO modifierSO, string statusEffectId)
        {
            if (_statusContainer.TryGetValue(modifierSO.StatType, out InCombatStatus status))
            {
                status.AddModifier(modifierSO, statusEffectId);
            }
        }

        /// <summary>
        /// Remove all effect's modifiers in each status.
        /// </summary>
        /// <param name="statusEffectId"></param>
        public void RemoveStatusEffect(string statusEffectId)
        {
            foreach (var status in _statusContainer.Values)
            {
                status.RemoveModifiersByOwner(statusEffectId);
            }
        }

        private void OnAfterResetActionGauge()
        {
            ApplyQueuedGaugeModifiers();
        }

        public InCombatStatus GetCombatStatus(EStatusType statusType)
        {
            return _statusContainer[statusType];
        }

        private void TriggerTurnFlowEvent(ETurnFlowEvent flowEvent)
        {
            foreach (var status in _statusContainer.Values)
            {
                status.Trigger(flowEvent);
            }
        }

        public float CalculateBaseDmg()
        {
            if (_selectedCard == null)
            {
                Debug.LogError("No selected card to calculate base damage. Return 0 by default.");
                return 0f;
            }
            InCombatStatus status = GetCombatStatus(_selectedCard.ModifyStatType);
            if (status == null)
            {
                return 0f;
            }
            float damage = _selectedCard.BaseValue + _selectedCard.GetDelta(status.Value);
            StatOutputDmg.SetCurrentValue(damage);
            return damage;
        }
    }
}