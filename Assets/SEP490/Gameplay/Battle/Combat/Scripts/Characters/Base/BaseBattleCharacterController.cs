namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(CharacterVFXController))]
    [RequireComponent(typeof(CharacterAnimationController))]
    public abstract class BaseBattleCharacterController : MonoBehaviour
    {
        #region Events
        /// <summary>
        /// Triggers when the energy bar is full.
        /// </summary>
        public event Action<BaseBattleCharacterController> onEnergyFull;

        /// <summary>
        /// Triggers when the character is dead.
        /// </summary>
        public event Action onDead;

        /// <summary>
        /// Triggers whenever the turn's flow event of a character changed.
        /// Sequence includes:
        /// - Turn started
        /// - Before card action.
        /// - After card action.
        /// - After reset action gauge.
        /// </summary>
        public event Action<ETurnFlowEvent, BaseBattleCharacterController> onFlowEventChanged;

        #endregion

        #region Configs (Lazy Loaded)

        private CharacterConfigSO _characterConfig;
        public CharacterConfigSO CharacterConfig => _characterConfig ??= ContextManager.Singleton.GetDataSO<CharacterConfigSO>();

        private CardConfigSO _cardConfig;
        public CardConfigSO CardConfig => _cardConfig ??= ContextManager.Singleton.GetDataSO<CardConfigSO>();

        #endregion

        #region Fields

        private CharacterDataHolder _readonlyDataHolder;
        private StatusEffectManager _statEffectManager;

        public InCombatStatus StatVit { get; private set; } = new InCombatStatus();
        public InCombatStatus StatPow { get; private set; } = new InCombatStatus();
        public InCombatStatus StatAgi { get; private set; } = new InCombatStatus();
        public InCombatStatus StatInt { get; private set; } = new InCombatStatus();
        public InCombatStatus StatStamina { get; private set; } = new InCombatStatus();
        public InCombatStatus StatDEF { get; private set; } = new InCombatStatus();

        public InCombatStatus StatOutputDmg { get; private set; } = new InCombatStatus();
        public InCombatStatus StatReceivedDmg { get; private set; } = new InCombatStatus();
        public InCombatStatus StatActionCost { get; private set; } = new InCombatStatus();
        public InCombatStatus StatHitRate { get; private set; } = new InCombatStatus();


        private readonly Dictionary<EStatusType, InCombatStatus> _statusContainer = new Dictionary<EStatusType, InCombatStatus>();

        private ICombatCardsProcessor _cardsProcessor;
        private IActionGaugeProcessor _gaugeProcessor;

        protected ICombatCardsProcessor CombatCardsProcessor => _cardsProcessor;
        #endregion

        #region Properties
        private CharacterAnimationController _animController;

        public CharacterVFXController VFXController { get; private set; }
        public CharacterAnimationController AnimationController
        {
            get
            {
                if (_animController == null)
                {
                    _animController = GetComponent<CharacterAnimationController>();
                }
                return _animController;
            }
        }

        /// <summary>
        /// Get the data which is readonly in combat.
        /// </summary>
        public CharacterDataHolder ReadonlyDataHolder => _readonlyDataHolder;
        /// <summary>
        /// Get the data which is changed in combat.
        /// </summary>
        //public CharacterDataHolder CurrentDataHolder => _currentDataHolder;

        public StatusEffectManager StatEffectManager => _statEffectManager;

        public BaseBattleCharacterController LastAttacker { get; private set; }
        #endregion

        #region Initialization
        protected virtual void Awake()
        {
            _statEffectManager = new StatusEffectManager(this);
            _animController = GetComponent<CharacterAnimationController>();
            VFXController = GetComponent<CharacterVFXController>();
            _cardsProcessor = GetComponent<ICombatCardsProcessor>();
            _gaugeProcessor = GetComponent<IActionGaugeProcessor>();

            if (_cardsProcessor == null)
            {
                _cardsProcessor = gameObject.AddComponent<CharacterCardsProcessor>();
                if (_cardsProcessor == null)
                {
                    Debug.LogError($"[BaseBattleCharacterController.Awake error] Failed to get the {nameof(ICombatCardsProcessor)} in {gameObject.name}");
                    return;
                }
            }

            if (_gaugeProcessor == null)
            {
                _gaugeProcessor = gameObject.AddComponent<CharacterActionGaugeProcessor>();

                if (_gaugeProcessor == null)
                {
                    Debug.LogError($"[BaseBattleCharacterController.Awake error] Failed to get the {nameof(ICombatCardsProcessor)} in {gameObject.name}");
                    return;
                }
            }

            _gaugeProcessor.onEnergyFull += HandleActionGaugeFullEvent;
        }

        private void OnDestroy()
        {
            if (_gaugeProcessor != null) 
                _gaugeProcessor.onEnergyFull -= HandleActionGaugeFullEvent;
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
            _statusContainer.Add(EStatusType.Defense, StatDEF);

            _statusContainer.Add(EStatusType.Damage, StatOutputDmg);
            _statusContainer.Add(EStatusType.ReceivedDmg, StatReceivedDmg);
            _statusContainer.Add(EStatusType.ActionCost, StatActionCost);
        }

        public void SetReadonlyDataHolder(CharacterDataHolder holder)
        {
            _readonlyDataHolder = holder;
        }

        #endregion

        #region Card APIs
        public void InitializeDeck(string[] deckCardIdArray)
        {
            _cardsProcessor.InitializeDeck(deckCardIdArray);
        }

        public float CalculateSelectedCardDmg()
        {
            return _cardsProcessor.CalculateSelectedCardDmg();
        }

        public void ExecuteCard(BaseBattleCharacterController target)
        {
            TriggerTurnFlowEvent(ETurnFlowEvent.BeforeCardAction);
            StatEffectManager.Trigger(ETurnFlowEvent.BeforeCardAction, target);
            
            if (!_cardsProcessor.ExecuteCard(target))
            {
                TriggerTurnFlowEvent(ETurnFlowEvent.AfterCardAction);
                StatEffectManager.Trigger(ETurnFlowEvent.AfterCardAction, target);
            }
        }

        public void DrawThreeCards(out IReadOnlyList<CardSO> cards)
        {
            _cardsProcessor.DrawThreeCards(out cards);
        }

        public void SelectRest()
        {
            _cardsProcessor.SelectRest();
        }

        public void SelectCardById(string cardId)
        {
            _cardsProcessor.SelectCardById(cardId);
        }

        public void SelectNoAction()
        {
            _cardsProcessor.SelectNoAction();
        }

        public virtual void EndCurrentTurn()
        {
            // Triggers the status effect's end-turn event.
            StatEffectManager.EndTurn();

            // Discard cards.
            _cardsProcessor.DiscardCurrentDraw();

            // Reset energy charged.
            _gaugeProcessor.ResetActionGauge();

            TriggerTurnFlowEvent(ETurnFlowEvent.AfterResetActionGaugue);
        }
        #endregion

        #region Damage Methods

        public void ReceiveDamage(float damage, BaseBattleCharacterController attacker)
        {
            LastAttacker = attacker;
            Debug.Log($"{gameObject.name} received pure dmg: {damage}");
            float finalDamage = Mathf.Max(0, damage - StatDEF.Value);

            StatReceivedDmg.SetCurrentValue(finalDamage);
            float finalVit = StatVit.Value - StatReceivedDmg.Value;
            StatVit.SetCurrentValue(finalVit);

            StatEffectManager.OnAfterReceiveDamage(damage);
        }

        public void CheckDeath()
        {
            if (StatVit.Value > 0) return;

            Debug.Log($"{ReadonlyDataHolder.GetCharacterName()} has died.");
            PauseBar();

            onDead?.Invoke();
        }

        #endregion

        #region Stats & Status Effects APIs

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
                _gaugeProcessor.QueueGaugeModifier(modifierSO, true);

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

        public void AddStatusEffect(StatusEffectSO effectSO)
        {
            StatEffectManager.AddStatusEffect(effectSO);
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
        #endregion

        #region Action Gauge APIs

        protected void InitializeEnergySystem()
        {
            _gaugeProcessor.InitializeActionGauge(this, StatAgi.Value);
        }

        public float GetCurrentEnergyValue() => _gaugeProcessor.GetCurrentGaugeValue();
        public float GetMaxEnergyValue() => _gaugeProcessor.GetMaxGaugeValue();
        public void PauseBar() => _gaugeProcessor.Pause();
        public void UnpauseBar() => _gaugeProcessor.Unpause();
        public void UpdateActionGauge(float dt)
        {
            _gaugeProcessor.UpdateActionGauge(dt);
        }
        private void HandleActionGaugeFullEvent()
        {
            onEnergyFull?.Invoke(this);
        }
        #endregion

        #region Turn flow methods
        public void StartTurn()
        {
            TriggerTurnFlowEvent(ETurnFlowEvent.TurnStarted);

            StatEffectManager.Trigger(ETurnFlowEvent.TurnStarted, LastAttacker);

            StatEffectManager.StartTurn();
        }
        public void TriggerAfterCardResolved(BaseBattleCharacterController target)
        {
            CheckDeath();

            TriggerTurnFlowEvent(ETurnFlowEvent.AfterCardAction);
            StatEffectManager.Trigger(ETurnFlowEvent.AfterCardAction, target);
        }

        private void TriggerTurnFlowEvent(ETurnFlowEvent flowEvent)
        {
            onFlowEventChanged?.Invoke(flowEvent, this);

            foreach (var status in _statusContainer.Values)
            {
                status.Trigger(flowEvent);
            }
        }

        #endregion

        #region Helpers
        public InCombatStatus GetCombatStatus(EStatusType statusType)
        {
            return _statusContainer[statusType];
        }

        #endregion
    }
}