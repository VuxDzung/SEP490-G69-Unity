namespace SEP490G69
{
    using SEP490G69.Battle;
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class StatusEffectManager 
    {
        private readonly List<RuntimeStatusEffect> _statusEffects = new List<RuntimeStatusEffect>();

        private BaseBattleCharacterController owner;

        public IReadOnlyList<RuntimeStatusEffect> ActiveStatEffects => _statusEffects;

        public StatusEffectManager(BaseBattleCharacterController owner)
        {
            this.owner = owner;
        }

        public void AddStatusEffect(StatusEffectSO effect)
        {
            RuntimeStatusEffect exist =
                _statusEffects.FirstOrDefault(s =>
                    s.Data.EffectId == effect.EffectId);

            if (exist != null)
            {
                exist.AddStack();
                return;
            }

            RuntimeStatusEffect runtime = new RuntimeStatusEffect(effect, owner);
            runtime.onStackEmpty += Remove;
            _statusEffects.Add(runtime);

            runtime.OnApply();
        }

        public void StartTurn()
        {
            foreach (var s in _statusEffects.ToList())
                s.OnTurnStart();
        }

        public void EndTurn()
        {
            foreach (var s in _statusEffects.ToList())
                s.OnTurnEnd();
        }

        public float ModifyIncomingDamage(float dmg)
        {
            foreach (var s in _statusEffects)
                dmg = s.ModifyIncomingDamage(dmg);

            return dmg;
        }

        public float ModifyDealableDamage(float dmg)
        {
            foreach (var s in _statusEffects)
                dmg = s.ModifyDealDamage(dmg);

            return dmg;
        }

        public float ModifyActionCost(float cost)
        {
            foreach (var s in _statusEffects)
                cost = s.ModifyActionCost(cost);
            return cost;
        }

        public void OnAfterReceiveDamage(float dmg)
        {
            foreach (var s in _statusEffects)
                s.OnAfterReceiveDamage(dmg);
        }

        public void OnAction()
        {
            foreach (var s in _statusEffects)
                s.OnAction();
        }

        public void Remove(RuntimeStatusEffect effect)
        {
            effect.onStackEmpty -= Remove;
            _statusEffects.Remove(effect);
        }

        public RuntimeStatusEffect GetEffectById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            return _statusEffects.FirstOrDefault(staEffect => staEffect.Data.EffectId.Equals(id));
        }

        public int Count()
        {
            return _statusEffects.Count;
        }

        public RuntimeStatusEffect GetRandomStatusEffect()
        {
            if (_statusEffects.Count == 0) return null;
            return _statusEffects[Random.Range(0, _statusEffects.Count - 1)];
        }

        public RuntimeStatusEffect[] GetEffectsByType(EEffectType type)
        {
            if (_statusEffects.Count == 0) return null;
            return _statusEffects.Where(effect => effect.Data.EffectType == type).ToArray();
        }
    }
}