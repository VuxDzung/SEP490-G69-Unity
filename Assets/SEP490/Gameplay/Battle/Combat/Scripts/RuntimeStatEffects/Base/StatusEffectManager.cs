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
        private readonly List<RuntimeStatusEffect> _statusEffects = new();

        private BaseBattleCharacterController owner;

        public IReadOnlyList<RuntimeStatusEffect> ActiveStatEffects => _statusEffects;

        public StatusEffectManager(BaseBattleCharacterController owner)
        {
            this.owner = owner;
        }

        public void AddStatusEffect(StatusEffectSO effect)
        {
            RuntimeStatusEffect exist =_statusEffects.FirstOrDefault(s => s.Data.EffectId == effect.EffectId);

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

        public void OnAfterReceiveDamage(float dmg)
        {
            foreach (var s in _statusEffects.ToList())
                s.OnAfterReceiveDamage(dmg);
        }

        public void Remove(RuntimeStatusEffect effect)
        {
            effect.onStackEmpty -= Remove;
            _statusEffects.Remove(effect);
        }

        public int Count(EEffectType effectType = EEffectType.Both)
        {
            return effectType switch 
            { 
                EEffectType.Buff => _statusEffects.Where(e => e.Data.EffectType == EEffectType.Buff).Count(),
                EEffectType.Debuff => _statusEffects.Where(e => e.Data.EffectType == EEffectType.Debuff).Count(),
                _ => _statusEffects.Count
            };
        }

        public RuntimeStatusEffect GetById(string effectId)
        {
            return _statusEffects.FirstOrDefault(e => e.Data.EffectId.Equals(effectId));
        }

        public RuntimeStatusEffect GetRandomEffect()
        {
            return _statusEffects[Random.Range(0, _statusEffects.Count - 1)];
        }

        public RuntimeStatusEffect[] GetEffectsByType(EEffectType type)
        {
            return _statusEffects.Where(e => e.Data.EffectType.Equals(type)).ToArray();
        }

        public float ModifyStatDelta(EStatusType statType, float delta)
        {
            foreach (RuntimeStatusEffect effect in _statusEffects)
            {
                if (effect.SpecialEffect != null)
                {
                    delta = effect.SpecialEffect.ModifyStatDelta(statType, delta, owner);
                }
            }

            return delta;
        }

        public void Trigger(ETurnFlowEvent flowEvent, BaseBattleCharacterController target)
        {
            foreach (var effect in _statusEffects)
            {
                switch (flowEvent)
                {
                    case ETurnFlowEvent.TurnStarted:
                        break;
                    case ETurnFlowEvent.BeforeCardAction:
                        OnBeforeAction(target);
                        break;
                    case ETurnFlowEvent.AfterCardAction:
                        OnAfterAction(target);
                        break;
                }
            }
        }

        public void OnBeforeAction(BaseBattleCharacterController target)
        {
            foreach (var s in _statusEffects)
            {
                s.SpecialEffect?.OnBeforeAction(owner, target);
            }
        }
        public void OnAfterAction(BaseBattleCharacterController target)
        {
            foreach (var s in _statusEffects)
            {
                s.SpecialEffect?.OnAfterAction(owner, target);
            }
        }
    }
}