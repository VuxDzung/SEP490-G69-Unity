namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Battle.Cards;
    using System;
    using UnityEngine;

    [RequireComponent(typeof(CharacterVFXController))]
    [RequireComponent(typeof(CharacterAnimationController))]
    [RequireComponent(typeof(CharacterToastSpawner))]
    public abstract class BaseCombatActor : MonoBehaviour
    {
        #region Events
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
        public event Action<ETurnFlowEvent, BaseCombatActor> onFlowEventChanged;

        #endregion

        #region Configs (Lazy Loaded)
        private AudioManager _audioManager;
        private AudioManager AudioManager => _audioManager ??= ContextManager.Singleton.ResolveGameContext<AudioManager>();

        private CharacterConfigSO _characterConfig;
        public CharacterConfigSO CharacterConfig => _characterConfig ??= ContextManager.Singleton.GetDataSO<CharacterConfigSO>();

        private CardConfigSO _cardConfig;
        public CardConfigSO CardConfig => _cardConfig ??= ContextManager.Singleton.GetDataSO<CardConfigSO>();

        private StatusEffectConfigSO _effectsConfig;
        public StatusEffectConfigSO EffectsConfig => _effectsConfig ??= ContextManager.Singleton.GetDataSO<StatusEffectConfigSO>();

        #endregion

        #region Fields
        private bool _isTurnSkipped;

        private CharacterStaminaManager _staminaManager;
        private CharacterCombatController _combatController;
        private CharacterStatsManager _statsManager;

        private CharacterToastSpawner _toastSpawner;

        private CharacterDataHolder _readonlyDataHolder;
        private StatusEffectManager _effectsManager;

        public CharacterStaminaManager StaminaManager => _staminaManager;
        public CharacterCombatController CombatController => _combatController;
        public CharacterStatsManager StatsManager => _statsManager;

        // Combat calculators
        private ICritCalculator _critCalculator;
        private IEvasionCalculator _evasionCalculator;
        private IDmgReductionCalculator _dmgReduceCalculator;

        // HP & Energy Calculators
        private ICombatHPCalculator _maxHPCalculator;
        private ICombatStaminaCalculator _maxStaminaCalculator;

        public ICritCalculator CritCalculator => _critCalculator;
        public IEvasionCalculator EvasionCalculator => _evasionCalculator;
        public IDmgReductionCalculator DmgReductionCalculator => _dmgReduceCalculator;
        public ICombatHPCalculator HPCalculator => _maxHPCalculator;
        public ICombatStaminaCalculator StaminaCalculator => _maxStaminaCalculator;

        public bool IsSkippingTurn => _isTurnSkipped;
        #endregion

        #region Properties
        protected BaseCharacterSO _baseDataSO;

        private ECharacterType _characterType;

        private ICombatStatsInitializer _statsInitializer;

        private CharacterAnimationController _animController;

        public CharacterVFXController VFXController { get; private set; }
        public CharacterAnimationController AnimationController => _animController;
        public StatusEffectManager EffectsManager => _effectsManager;
        public BaseCombatActor LastAttacker { get; private set; }

        public bool IsPlayerOwnership => _characterType == ECharacterType.Playable;
        public bool IsMachineOwnership => _characterType == ECharacterType.NPC;
        #endregion

        #region Initialization
        protected virtual void Awake()
        {
            _animController = GetComponent<CharacterAnimationController>();
            VFXController = GetComponent<CharacterVFXController>();
            _toastSpawner = GetComponent<CharacterToastSpawner>();

            if (_toastSpawner == null)
            {
                _toastSpawner = gameObject.AddComponent<CharacterToastSpawner>();
            }

            // Calculator.
            _maxHPCalculator = new CombatHPCalculator();
            _maxStaminaCalculator = new CombatStaminaCalculator();
            _dmgReduceCalculator = new DmgReductionCalculator();
            _critCalculator = new CombatCritCalculator();
            _evasionCalculator = new EvasionCalculator(this);

            _statsManager = new CharacterStatsManager(this);
            _staminaManager = new CharacterStaminaManager(this);
            _effectsManager = new StatusEffectManager(this, EffectsConfig);

            CharacterShieldController shield = new CharacterShieldController(this);

            _combatController = new CharacterCombatController.Builder().WithStatsManager(StatsManager)
                                                                       .WithDamageHandler(DmgReductionCalculator)
                                                                       .WithStatusEffectManager(EffectsManager)
                                                                       .WithEvadeCalculator(EvasionCalculator)
                                                                       .WithCritCalculator(CritCalculator)
                                                                       .WithShield(shield)
                                                                       .Build();

            CreateStat(EStatusType.Damage);
            CreateStat(EStatusType.ReceivedDmg);
            CreateStat(EStatusType.ActionCost);
            CreateStat(EStatusType.EvadeRate);
            CreateStat(EStatusType.CriticalChance);
            CreateStat(EStatusType.Shield);
            CreateStat(EStatusType.ReceivedShield);
        }

        public abstract void Initialize(BaseCharacterSO characterSO);

        protected void InitializeStats()
        {
            if (_statsInitializer != null)
            {
                _statsInitializer.InitializeStats(this);
            }
            else
            {
                Debug.LogError("[BaseCombatActor.InitializeStats error] Stat initializer instance is null!");
            }
            InitGeneralInCombatStats();
        }
        
        protected void UseByPlayer()
        {
            _characterType = ECharacterType.Playable;
        }

        protected void ActAsNPC()
        {
            _characterType = ECharacterType.NPC;
        }

        public void SetInitializer(ICombatStatsInitializer initializer)
        {
            _statsInitializer = initializer;
        }

        private void InitGeneralInCombatStats()
        {
            StatsManager.SetCurrentValue(EStatusType.Damage, 0f);
            StatsManager.SetCurrentValue(EStatusType.ReceivedDmg, 0f);
            StatsManager.SetCurrentValue(EStatusType.ActionCost, 0f);
            StatsManager.SetCurrentValue(EStatusType.EvadeRate, 0f);
            StatsManager.SetCurrentValue(EStatusType.CriticalChance, 0f);
            StatsManager.SetCurrentValue(EStatusType.Shield, 0f);
            StatsManager.SetCurrentValue(EStatusType.ReceivedShield, 0f);
        }

        private void CreateStat(EStatusType type)
        {
            var stat = new InCombatStatus(_effectsManager);
            _statsManager.AddStatus(type, stat);
        }

        protected void CreateStatsByProfile(IStatProfile profile)
        {
            foreach (var type in profile.RequiredStats)
            {
                CreateStat(type);
            }
        }

        public void SetReadonlyDataHolder(CharacterDataHolder holder)
        {
            _readonlyDataHolder = holder;
        }

        #endregion

        #region Damage Methods

        public void ReceiveAttack(float damage, BaseCombatActor attacker, bool ignoreDef = false)
        {
            ReceiveDamage(damage, attacker, ignoreDef);

            attacker.EffectsManager.OnHitTarget(this);

            EffectsManager.OnAfterBeingAttacked(damage);
        }

        public void ReceiveDamage(float damage, BaseCombatActor attacker, bool ignoreDef = false)
        {
            LastAttacker = attacker;

            float finalDamage = _combatController.ReceiveDamage(damage, attacker, ignoreDef);

            SpawnReceivedDamageToast(finalDamage);
        }

        public void CheckDeath()
        {
            if (StatsManager.GetValue(EStatusType.HP) > 0) return;

            Debug.Log($"{_baseDataSO.CharacterName} has died.");

            onDead?.Invoke();
        }

        #endregion

        #region Shield
        protected void StackShield(EStatusType scalingStat, float baseShield, float modifierValue)
        {
            _combatController.StackShield(scalingStat,baseShield, modifierValue);
        }
        public void ResetShield()
        {
            _combatController.ResetShield();
        }
        #endregion

        #region Crit APIs
        public float CalculateCritRate(bool writeToStat)
        {
            return _combatController.CalculateCritRate(writeToStat);
        }

        public float CaculateCritMul()
        {
            return _combatController.CaculateCritMul();
        }

        public bool HasCrit(bool forceUseCrit)
        {
            return _combatController.HasCrit(forceUseCrit);
        }

        #endregion

        #region Evasion APIs
        public bool CanEvade(BaseCombatActor attacker, bool writeToStat = true)
        {
            return _combatController.CanEvade(attacker, writeToStat);
        }

        #endregion

        #region Stats & Status Effects APIs

        /// <summary>
        /// Apply status changes immediately (FIX: Handle all the immediate status modify action at the InCombatStatus.
        /// </summary>
        /// <param name="modifierSO"></param>
        public void ApplyStatusDelta(CombatStatModifierSO modifierSO, bool fromExternal)
        {
            _statsManager.ApplyStatsDelta(modifierSO);
        }

        public void AddStatusEffectById(string effectId)
        {
            EffectsManager.AddStatusEffect(effectId);

            Debug.Log($"{_baseDataSO.CharacterName} receives effect {effectId}");
        }

        public void AddStatusEffect(StatusEffectSO effectSO)
        {
            Debug.Log($"{_baseDataSO.CharacterName} receives effect {effectSO.EffectId}");
            EffectsManager.AddStatusEffect(effectSO);
        }

        public void AddEffectModifier(CombatStatModifierSO modifierSO, string statusEffectId)
        {
            _statsManager.AddEffectModifier(modifierSO, statusEffectId);
        }

        /// <summary>
        /// Remove all effect's modifiers in each status.
        /// </summary>
        /// <param name="statusEffectId"></param>
        public void RemoveEffectModifiers(string effectId)
        {
            Debug.Log($"{_baseDataSO.CharacterName} remove effect {effectId}");

            _statsManager.RemoveEffectModifiers(effectId);
        }
        #endregion

        #region Turn flow methods
        public void StartTurn()
        {
            TriggerTurnFlowEvent(ETurnFlowEvent.TurnStarted);

            EffectsManager.Trigger(ETurnFlowEvent.TurnStarted, LastAttacker);

            EffectsManager.StartTurn();

            StaminaManager.RefillStatmina();
        }

        public virtual void EndCurrentTurn()
        {
            ResetShield();
            TriggerTurnFlowEvent(ETurnFlowEvent.TurnFinished);
            EffectsManager.EndTurn();
        }

        public void TriggerAfterCardResolved(BaseCombatActor target)
        {
            TriggerTurnFlowEvent(ETurnFlowEvent.AfterCardAction);
            EffectsManager.Trigger(ETurnFlowEvent.AfterCardAction, target);

            CheckDeath();
        }

        protected void TriggerTurnFlowEvent(ETurnFlowEvent flowEvent)
        {
            onFlowEventChanged?.Invoke(flowEvent, this);
            _statsManager.TriggerFlowEvent(flowEvent);
        }

        #endregion

        #region Helpers
        public void SpawnReceivedDamageToast(float damage)
        {
            _toastSpawner.SpawnReceivedDamageToast(damage);
        }

        public void SpawnDodgeToast()
        {
            _toastSpawner.SpawnDodgeToast();
        }

        public void SpawnCritToast(float critMul)
        {
            _toastSpawner.SpawnCritToats(critMul);
        }
        #endregion

        #region Sfx
        public void PlayAtkSfx()
        {
            switch (_readonlyDataHolder.GetAtkType())
            {
                case EAttackType.Melee:
                    PlayMeleeSfx();
                    break;
                case EAttackType.Ranged:
                    PlayRangedSfx();
                    break;
                default:
                    break;
            }
        }

        public void PlayMeleeSfx()
        {
            if (!string.IsNullOrEmpty(_readonlyDataHolder.GetMeleeSfxId()))
            {
                AudioManager.PlaySFX(_readonlyDataHolder.GetMeleeSfxId());
            }
        }
        public void PlayRangedSfx()
        {
            if (!string.IsNullOrEmpty(_readonlyDataHolder.GetMeleeSfxId()))
            {
                AudioManager.PlaySFX(_readonlyDataHolder.GetRangedSfxId());
            }
        }
        #endregion

        #region Lock Turn
        public void EnableSkipTurn()
        {
            _isTurnSkipped = true;
        }
        public void DisableSkipTurn()
        {
            _isTurnSkipped = false;
        }
        #endregion
    }
}