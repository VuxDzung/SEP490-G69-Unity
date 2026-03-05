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
        private List<RuntimeStatusEffect> statuses = new();

        private BaseBattleCharacterController owner;

        public IReadOnlyList<RuntimeStatusEffect> ActiveStatuses => statuses;

        public StatusEffectManager(BaseBattleCharacterController owner)
        {
            this.owner = owner;
        }

        public void AddStatusEffect(StatusEffectSO effect)
        {
            RuntimeStatusEffect exist =
                statuses.FirstOrDefault(s =>
                    s.Data.EffectId == effect.EffectId);

            if (exist != null)
            {
                exist.AddStack();
                return;
            }

            RuntimeStatusEffect runtime = new RuntimeStatusEffect(effect, owner);
            runtime.onStackEmpty += Remove;
            statuses.Add(runtime);

            runtime.OnApply();
        }

        public void StartTurn()
        {
            foreach (var s in statuses)
                s.OnTurnStart();
        }

        public void EndTurn()
        {
            foreach (var s in statuses)
                s.OnTurnEnd();
        }

        public float ModifyIncomingDamage(float dmg)
        {
            foreach (var s in statuses)
                dmg = s.ModifyIncomingDamage(dmg);

            return dmg;
        }

        public float ModifyDealableDamage(float dmg)
        {
            foreach (var s in statuses)
                dmg = s.ModifyDealDamage(dmg);

            return dmg;
        }

        public float ModifyActionCost(float cost)
        {
            foreach (var s in statuses)
                cost = s.ModifyActionCost(cost);
            return cost;
        }

        public void OnAfterReceiveDamage(float dmg)
        {
            foreach (var s in statuses)
                s.OnAfterReceiveDamage(dmg);
        }

        public void OnAction()
        {
            foreach (var s in statuses)
                s.OnAction();
        }

        public void Remove(RuntimeStatusEffect effect)
        {
            effect.onStackEmpty -= Remove;
            statuses.Remove(effect);
        }

        public RuntimeStatusEffect GetEffectById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            return statuses.FirstOrDefault(staEffect => staEffect.Data.EffectId.Equals(id));
        }

        public int Count()
        {
            return statuses.Count;
        }

        public RuntimeStatusEffect GetRandomStatusEffect()
        {
            if (statuses.Count == 0) return null;
            return statuses[Random.Range(0, statuses.Count - 1)];
        }

        public RuntimeStatusEffect[] GetEffectsByType(EEffectType type)
        {
            if (statuses.Count == 0) return null;
            return statuses.Where(effect => effect.Data.EffectType == type).ToArray();
        }
    }
}