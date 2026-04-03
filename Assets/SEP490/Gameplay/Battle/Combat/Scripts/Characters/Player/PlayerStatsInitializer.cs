namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Economy;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerStatsInitializer : ICombatStatsInitializer
    {
        private IReadOnlyList<ItemDataHolder> _relics;
        private CharacterDataHolder holder;
        private PlayerCharacterDataSO _characterSO;

        public PlayerStatsInitializer(IReadOnlyList<ItemDataHolder> relics, CharacterDataHolder runtimeData)
        {
            _relics = relics;
            holder = runtimeData;
        }

        public void InitializeStats(BaseCombatActor actor)
        {
            float currentVit = holder.GetVIT();
            float currentPow = holder.GetPower();
            float currentInt = holder.GetINT();
            float currentAgi = holder.GetAgi();
            float currentSta = holder.GetStamina();
            float currentDef = holder.GetDef();

            float finalVit = currentVit;
            float finalPow = currentPow;
            float finalInt = currentInt;
            float finalAgi = currentAgi;
            float finalSta = currentSta;
            float finalDef = currentDef;

            float maxStaminaValue = actor.StaminaCalculator.CalculateMax(finalSta);
            float finalHp = actor.HPCalculator.Calculate(finalVit);

            if (_relics != null && _relics.Count > 0)
            {
                Debug.Log($"----------{holder.GetCharacterName()}----------");
                foreach (ItemDataHolder relic in _relics)
                {
                    Debug.Log($"Relic: {relic.GetRawId()}");
                    finalVit = relic.CalculateRelicModValue(EStatusType.Vitality, currentVit);

                    finalPow = relic.CalculateRelicModValue(EStatusType.Power, currentPow);

                    finalInt = relic.CalculateRelicModValue(EStatusType.Intelligence, currentInt);

                    finalAgi = relic.CalculateRelicModValue(EStatusType.Agi, currentAgi);

                    finalSta = relic.CalculateRelicModValue(EStatusType.Stamina, currentSta);

                    finalDef = relic.CalculateRelicModValue(EStatusType.Defense, currentDef);
                }

                finalHp = actor.HPCalculator.Calculate(finalVit);

                foreach (ItemDataHolder relic in _relics)
                {
                    finalHp = relic.CalculateRelicModValue(EStatusType.HP, finalHp);
                }
            }

            float hpValue = actor.HPCalculator.Calculate(finalVit);

            actor.StatsManager.SetMaxValue(EStatusType.Vitality, finalVit);
            actor.StatsManager.SetCurrentValue(EStatusType.Vitality, finalVit);

            actor.StatsManager.SetMaxValue(EStatusType.HP, finalHp);
            actor.StatsManager.SetCurrentValue(EStatusType.HP, finalHp);

            actor.StatsManager.SetMaxValue(EStatusType.Power, finalPow);
            actor.StatsManager.SetCurrentValue(EStatusType.Power, finalPow);

            actor.StatsManager.SetMaxValue(EStatusType.Agi, finalAgi);
            actor.StatsManager.SetCurrentValue(EStatusType.Agi, finalAgi);

            actor.StatsManager.SetMaxValue(EStatusType.Intelligence, finalInt);
            actor.StatsManager.SetCurrentValue(EStatusType.Intelligence, finalInt);

            actor.StatsManager.SetMaxValue(EStatusType.Stamina, maxStaminaValue);
            actor.StatsManager.SetCurrentValue(EStatusType.Stamina, maxStaminaValue);

            actor.StatsManager.SetMaxValue(EStatusType.Defense, finalDef);
            actor.StatsManager.SetCurrentValue(EStatusType.Defense, finalDef);
        }
    }
}