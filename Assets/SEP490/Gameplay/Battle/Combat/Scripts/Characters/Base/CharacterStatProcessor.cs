namespace SEP490G69.Battle.Combat
{
    using System.Collections.Generic;
    using UnityEngine;

    public class CharacterStatProcessor : MonoBehaviour
    {
        private BaseBattleCharacterController _owner;
        private StatusEffectManager _effectManager;

        private readonly Dictionary<EStatusType, InCombatStatus> _stats = new();

        public CharacterStatProcessor SetOwner(BaseBattleCharacterController owner)
        {
            _owner = owner;
            return this;
        }
        public CharacterStatProcessor SetEffectManager(StatusEffectManager effectManager)
        {
            _effectManager = effectManager;
            return this;
        }

        public void Initialize(CharacterDataHolder holder)
        {
            CreateStat(EStatusType.Vitality, holder.GetVIT());
            CreateStat(EStatusType.Power, holder.GetPower());
            CreateStat(EStatusType.Agi, holder.GetAgi());
            CreateStat(EStatusType.Stamina, holder.GetStamina());
            CreateStat(EStatusType.Defense, holder.GetDef());
        }

        private void CreateStat(EStatusType type, float value)
        {
            var stat = new InCombatStatus();
            stat.SetCurrentValue(value);
            _stats[type] = stat;
        }

        public float GetValue(EStatusType type) => _stats[type].Value;

        public void ApplyDamage(float damage)
        {
            var vit = _stats[EStatusType.Vitality];
            vit.SetCurrentValue(vit.Value - damage);
        }

        public float CalculateFinalDamage(float damage)
        {
            float def = GetValue(EStatusType.Defense);
            return Mathf.Max(1, damage - def);
        }

        public bool IsDead()
        {
            return GetValue(EStatusType.Vitality) <= 0;
        }
    }
}