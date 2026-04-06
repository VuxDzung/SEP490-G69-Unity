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

        private BaseCombatActor _owner;
        private StatusEffectConfigSO _effectsConfig;

        public IReadOnlyList<RuntimeStatusEffect> ActiveStatEffects => _statusEffects;

        public StatusEffectManager(BaseCombatActor owner, StatusEffectConfigSO effectsConfig)
        {
            this._owner = owner;
            this._effectsConfig = effectsConfig;
        }

        public void AddStatusEffect(string effectId)
        {
            StatusEffectSO effectSO = _effectsConfig.GetById(effectId);
            if (effectSO != null)
            {
                AddStatusEffect(effectSO);
            }
            else
            {
                Debug.LogError($"[StatusEffectManager.AddStatusEffect(effectId:string) error] Effect SO with id {effectId} is not configured!");
            }
        }

        public void AddStatusEffect(StatusEffectSO effect)
        {
            RuntimeStatusEffect exist =_statusEffects.FirstOrDefault(s => s.Data.EffectId == effect.EffectId);

            if (exist != null)
            {
                if (exist.Data.DecayType == EDecayType.Fovever ||
                    exist.Data.DecayType == EDecayType.None)
                {
                    return;
                }
                exist.AddStack();
                return;
            }

            RuntimeStatusEffect runtime = new RuntimeStatusEffect(effect, _owner);

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

        public void OnAfterBeingAttacked(float dmg)
        {
            foreach (var s in _statusEffects.ToList())
                s.OnAfterBeingAttacked(dmg);
        }

        public void OnHitTarget(BaseCombatActor opponent)
        {
            foreach (var s in _statusEffects.ToList())
                s.OnHitTarget(opponent);
        }

        public void RemoveById(string effectId)
        {
            RuntimeStatusEffect effect = GetById(effectId);
            if (effect != null)
            {
                Remove(effect);
            }
        }

        public void Remove(RuntimeStatusEffect effect)
        {
            effect.onStackEmpty -= Remove;
            effect.OnDiscard();
            _statusEffects.Remove(effect);
        }

        public void ManualTriggerEffect(string effectId, BaseCombatActor attacker)
        {
            RuntimeStatusEffect effect =GetById(effectId);
            if (effect != null )
            {
                effect.TriggerManually(attacker);
            }
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

        public void Trigger(ETurnFlowEvent flowEvent, BaseCombatActor target)
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

        public void OnBeforeAction(BaseCombatActor target)
        {
            foreach (var s in _statusEffects)
            {
                s.SpecialEffect?.OnBeforeAction(_owner, target);
            }
        }
        public void OnAfterAction(BaseCombatActor target)
        {
            foreach (var s in _statusEffects)
            {
                s.SpecialEffect?.OnAfterAction(_owner, target);
            }
        }
    }
}