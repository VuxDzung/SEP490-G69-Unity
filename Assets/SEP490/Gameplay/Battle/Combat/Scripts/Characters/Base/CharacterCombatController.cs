namespace SEP490G69.Battle.Combat
{
    using UnityEngine;

    public class CharacterCombatController 
    {
        private CharacterStatsManager _statsManager;
        private CharacterDamageHandler _damageHandler;
        private StatusEffectManager _effectManager;
        private CharacterShieldController _shieldController;

        private IEvasionCalculator _evasionCalculator;
        private ICritCalculator _critCalculator;

        public CharacterShieldController Shield => _shieldController;

        private CharacterCombatController() { }

        public void Initialize()
        {
            _shieldController.Initialize();
        }

        public void StackShield(EStatusType scalingStat, float baseValue, float modifierValue)
        {
            _shieldController.AddShield(scalingStat, baseValue, modifierValue);
        }

        public float CalculateReceivedShield(EStatusType scalingStat, float baseValue, float modifierValue)
        {
            return _shieldController.CalculateStackShieldValue(scalingStat, baseValue, modifierValue);
        }

        public void ResetShield()
        {
            _shieldController.ResetShield();
        }

        public float ReceiveDamage(float rawDamage, BaseCombatActor attacker, bool ignoreDef = false)
        {
            Debug.Log($"[CharacterCombatController.ReceiveDamage] Receives {rawDamage} raw damage");
            // Reduce damage by def value
            float reducedDmg = _damageHandler.CalculateFinalDamage(rawDamage, _statsManager.GetValue(EStatusType.Defense));

            // Reduce damage by shield
            _shieldController.AbsorbDamage(reducedDmg, out float finalDamage);

            Debug.Log($"[CharacterCombatController.ReceiveDamage] Receives {rawDamage} final damage");

            var statHP = _statsManager.Get(EStatusType.HP);

            // Decrease health.
            statHP.SetCurrentValue(statHP.Value - finalDamage, true);

            return finalDamage;
        }

        public float CalculateEvasionRate(BaseCombatActor attacker, bool writeToStat)
        {
            float baseValue = _evasionCalculator.CalculateEvasionRate(attacker);
            if (writeToStat) _statsManager.SetCurrentValue(EStatusType.EvadeRate, baseValue);
            return writeToStat ? _statsManager.GetValue(EStatusType.EvadeRate) : baseValue;
        }

        public bool CanEvade(BaseCombatActor attacker, bool writeToStat = true)
        {
            float evadeChance = CalculateEvasionRate(attacker, writeToStat);

            evadeChance = (float)System.Math.Round(evadeChance, 2);

            float rollNum = UnityEngine.Random.Range(0, 1f);

            return rollNum <= evadeChance;
        }

        public float CalculateCritRate(bool writeToStat)
        {
            float baseValue = _critCalculator.CalculateCritChance(_statsManager.GetValue(EStatusType.Agi));
            if (writeToStat) _statsManager.SetCurrentValue(EStatusType.CriticalChance, baseValue);
            return _statsManager.GetValue(EStatusType.CriticalChance);
        }

        public float CaculateCritMul()
        {
            return _critCalculator.CalculateCritMul(_statsManager.GetValue(EStatusType.Agi));
        }

        public bool HasCrit(bool forceUseCrit)
        {
            if (forceUseCrit)
            {
                return true;
            }
            float critChance = CalculateCritRate(true);
            critChance = (float)System.Math.Round(critChance, 2);
            return UnityEngine.Random.Range(0, 1f) <= critChance;
        }

        public class Builder
        {
            private CharacterStatsManager _statsManager;
            private IDmgReductionCalculator _dmgReductionHandler;
            private StatusEffectManager _effectManager;
            private IEvasionCalculator _evasionCalculator;
            private ICritCalculator _critHandler;
            private CharacterShieldController _shield;

            public Builder WithStatsManager(CharacterStatsManager statsManager)
            {
                _statsManager = statsManager;
                return this;
            }

            public Builder WithDamageHandler(IDmgReductionCalculator dmgReductor)
            {
                _dmgReductionHandler = dmgReductor;
                return this;
            }

            public Builder WithStatusEffectManager(StatusEffectManager effectManager)
            {
                _effectManager = effectManager;
                return this;
            }

            public Builder WithEvadeCalculator(IEvasionCalculator evadeCalculator)
            {
                _evasionCalculator = evadeCalculator;
                return this;
            }

            public Builder WithCritCalculator(ICritCalculator critCalculator)
            {
                _critHandler = critCalculator;
                return this;
            }

            public Builder WithShield(CharacterShieldController shield)
            {
                _shield = shield;
                return this;
            }

            public CharacterCombatController Build()
            {
                return new CharacterCombatController
                {
                    _statsManager = _statsManager,
                    _damageHandler = new CharacterDamageHandler(_dmgReductionHandler),
                    _effectManager = _effectManager,
                    _evasionCalculator = _evasionCalculator,
                    _critCalculator = _critHandler,
                    _shieldController = _shield,
                };
            }
        }
    }
}