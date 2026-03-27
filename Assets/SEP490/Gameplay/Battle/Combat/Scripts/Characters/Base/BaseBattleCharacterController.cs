namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System;
    using System.Collections;
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
        public InCombatStatus StatHP { get; private set; } = new InCombatStatus();

        public InCombatStatus StatOutputDmg { get; private set; } = new InCombatStatus();
        public InCombatStatus StatReceivedDmg { get; private set; } = new InCombatStatus();
        public InCombatStatus StatActionCost { get; private set; } = new InCombatStatus();
        public InCombatStatus StatEvadeRate { get; private set; } = new InCombatStatus();
        public InCombatStatus StatCritChance { get; private set; } = new InCombatStatus();


        private readonly Dictionary<EStatusType, InCombatStatus> _statusContainer = new Dictionary<EStatusType, InCombatStatus>();

        private ICombatCardsProcessor _cardsProcessor;
        private IActionGaugeProcessor _gaugeProcessor;
        private ICritCalculator _critCalculator;
        private IEvasionCalculator _evasionCalculator;
        private IMaxHPCalculator _maxHPCalculator;
        private IMaxStaminaCalculator _maxStaminaCalculator;

        private IDmgReductionCalculator _dmgReduceCalculator = new IDmgReductionCalculator();

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
            _cardsProcessor.SetOwner(this);

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

            _maxHPCalculator = new MaxHPCalculator();
            _maxStaminaCalculator = new MaxStaminaCalculator();

            _critCalculator = new CombatCritCalculator();
            _evasionCalculator = new EvasionCalculator(this);
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

            float hpValue = _maxHPCalculator.Calculate(holder.GetVIT());
            float maxStaminaValue = _maxStaminaCalculator.CalculateMax(holder.GetStamina());

            StatVit.SetCurrentValue(holder.GetVIT());
            StatHP.SetCurrentValue(hpValue);
            StatPow.SetCurrentValue(holder.GetPower());
            StatAgi.SetCurrentValue(holder.GetAgi());
            StatInt.SetCurrentValue(holder.GetINT());
            StatStamina.SetCurrentValue(maxStaminaValue);
            StatDEF.SetCurrentValue(holder.GetDef());

            StatOutputDmg.SetCurrentValue(0f);
            StatReceivedDmg.SetCurrentValue(0f);
            StatActionCost.SetCurrentValue(0f);
            StatEvadeRate.SetCurrentValue(0f);
            StatCritChance.SetCurrentValue(0f);

            _statusContainer.Clear();

            _statusContainer.Add(EStatusType.Vitality, StatVit);
            _statusContainer.Add(EStatusType.Power, StatPow);
            _statusContainer.Add(EStatusType.Agi, StatAgi);
            _statusContainer.Add(EStatusType.Intelligence, StatInt);
            _statusContainer.Add(EStatusType.Stamina, StatStamina);
            _statusContainer.Add(EStatusType.Defense, StatDEF);
            _statusContainer.Add(EStatusType.HP, StatHP);

            _statusContainer.Add(EStatusType.Damage, StatOutputDmg);
            _statusContainer.Add(EStatusType.ReceivedDmg, StatReceivedDmg);
            _statusContainer.Add(EStatusType.ActionCost, StatActionCost);
            _statusContainer.Add(EStatusType.EvadeRate, StatEvadeRate);
            _statusContainer.Add(EStatusType.CriticalChance, StatCritChance);
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

        public float CalculateSelectedCardDmg(bool writeToOutputDmg)
        {
            return _cardsProcessor.CalculateSelectedCardDmg(writeToOutputDmg);
        }

        public float CalculateBaseCardDMG(CardSO card)
        {
            return _cardsProcessor.CalculateBaseDmg(card);
        }

        public void ExecuteCard(BaseBattleCharacterController target)
        {
            TriggerTurnFlowEvent(ETurnFlowEvent.BeforeCardAction);
            StatEffectManager.Trigger(ETurnFlowEvent.BeforeCardAction, target);
            
            if (!_cardsProcessor.ExecuteCard(target))
            {
                TriggerAfterCardResolved(target);
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
        #endregion

        #region Damage Methods

        public void ReceiveDamage(float damage, BaseBattleCharacterController attacker)
        {
            LastAttacker = attacker;

            Debug.Log($"{ReadonlyDataHolder.GetCharacterName()} received pure dmg: {damage}");
            float finalDamage = damage - damage * _dmgReduceCalculator.Calculate(StatDEF.Value);// Mathf.Max(0, damage - StatDEF.Value);

            StatReceivedDmg.SetCurrentValue(finalDamage);
            float finalVit = StatHP.Value - StatReceivedDmg.Value;

            Debug.Log($"{ReadonlyDataHolder.GetCharacterName()} receive pure dmg = {finalDamage} and final damage = {StatReceivedDmg.Value}");

            StatHP.SetCurrentValue(finalVit);
            Debug.Log($"{ReadonlyDataHolder.GetCharacterName()} remain health: {StatHP.Value}");
            StatEffectManager.OnAfterReceiveDamage(damage);
        }

        public void CheckDeath()
        {
            if (StatHP.Value > 0) return;

            Debug.Log($"{ReadonlyDataHolder.GetCharacterName()} has died.");
            PauseBar();

            onDead?.Invoke();
        }

        #endregion

        #region Crit APIs
        public float CalculateCritRate(bool writeToStat)
        {
            float baseValue = _critCalculator.CalculateCritChance(StatInt.Value);
            if (writeToStat) StatCritChance.SetCurrentValue(baseValue);
            return StatCritChance.Value;
        }
        public float CaculateCritMul()
        {
            return _critCalculator.CalculateCritMul(StatPow.Value);
        }

        public bool HasCrit(bool forceUseCrit)
        {
            if (forceUseCrit)
            {
                return true;
            }
            float critChance = CalculateCritRate(true);
            critChance = (float)Math.Round(critChance, 2);
            return UnityEngine.Random.Range(0, 1f) <= critChance;
        }

        #endregion

        #region Evasion APIs
        public float CalculateEvasionRate(BaseBattleCharacterController attacker, bool writeToStat)
        {
            float baseValue = _evasionCalculator.CalculateEvasionRate(attacker);
            if (writeToStat) StatEvadeRate.SetCurrentValue(baseValue);
            return StatEvadeRate.Value;
        }
        public bool CanEvade(BaseBattleCharacterController attacker, bool writeToStat = true)
        {
            float evadeChance = CalculateEvasionRate(attacker, writeToStat);

            evadeChance = (float)Math.Round(evadeChance, 2);

            float rollNum = UnityEngine.Random.Range(0, 1f);

            return rollNum <= evadeChance;
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

            float maxValue = GetMaxStatus(modifierSO.StatType);
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
            Debug.Log($"{ReadonlyDataHolder.GetCharacterName()} receives effect {effectSO.EffectId}");
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
            Debug.Log($"{ReadonlyDataHolder.GetCharacterName()} remove effect {statusEffectId}");

            foreach (var status in _statusContainer.Values)
            {
                status.RemoveModifiersByOwner(statusEffectId);
            }
        }

        public float GetMaxStatus(EStatusType statType)
        {
            switch(statType)
            {
                case EStatusType.Power:
                case EStatusType.Intelligence:
                case EStatusType.Defense:
                case EStatusType.Agi:
                case EStatusType.Vitality:
                    return ReadonlyDataHolder.GetStatus(statType);
                case EStatusType.HP:
                    return GetCombatStatus(EStatusType.HP).MaxValue;
                case EStatusType.Stamina:
                    return GetCombatStatus(EStatusType.Stamina).MaxValue;
                default:
                    Debug.Log($"Unsupported in-combat status {statType.ToString()}");
                    return -1f;
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

            EndCurrentTurn();
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

        public void SpawnDmgToast(float dmg)
        {
            StartCoroutine(DelaySpawnToast(dmg));
        }

        private IEnumerator DelaySpawnToast(float dmg)
        {
            yield return new WaitForSeconds(0.15f);

            string message = $"{dmg.ToString()}";

            Vector3 position = transform.position + new Vector3(0, 0.75f, 0f);

            GameToastManager.Singleton.SpawnToast(new SpawnToastSettingsData
            {
                Message = message,
                TextColor = Color.red,
                SpawnPosition = position,
                DelaySpawnTime = 0.01f,
                TextSize = 27f
            });
        }

        #endregion
    }
}