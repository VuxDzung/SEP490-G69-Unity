namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class BaseCard
    {
        protected CardSO Data;

        protected readonly AnimationBarrier _barrier;

        public string RawCardId => Data.CardId;

        public BaseCard(CardSO data)
        {
            Data = data;
            _barrier = new AnimationBarrier();
        }

        public virtual void Execute(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            if (!ExecuteCondition(source, target)) return;

            Debug.Log($"Execute card {Data.CardId}");

            ApplyStatModifiers(source, target, Data.PreStatModifiers);

            ExecuteAction(source, target);

            ExecuteVfxs(source, target);

            ApplyStatModifiers(source, target, Data.PostStatModifiers);

            ApplyStatusEffects(source, target);
        }

        protected virtual void ExecuteAction(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            Debug.Log("Do nothing");

            // Trigger flow event now.
        }

        protected void ApplyStatModifiers(BaseBattleCharacterController source, BaseBattleCharacterController target, CombatStatModifierSO[] modifiers)
        {
            if (modifiers == null || modifiers.Length == 0)
            {
                Debug.Log("No modifier");
                return;
            }

            BaseBattleCharacterController receiver = null;

            foreach (var mod in modifiers)
            {
                Debug.Log($"Start modifier: {mod.Id}");
                receiver = mod.ApplyTarget == ETargetType.Self ? source : target;
                receiver.ApplyStatusDelta(mod, mod.ApplyTarget == ETargetType.Opponent);
            }
        }

        protected void ApplyStatusEffects(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            if (Data.StatusGains != null &&
                Data.StatusGains.Length > 0 &&
                CheckGainCondition(source, target))
            {
                foreach (var s in Data.StatusGains)
                {
                    //source.StatEffectManager.AddStatusEffect(s);
                    source.AddStatusEffect(s);
                }

                if (Data.StatusInflicts != null &&
                    Data.StatusInflicts.Length > 0 &&
                    CheckInflictCondition(source, target))
                {
                    foreach (var s in Data.StatusInflicts)
                    {
                        //target.StatEffectManager.AddStatusEffect(s);
                        target.AddStatusEffect(s);
                    }
                }
            }
        }

        protected virtual bool ExecuteCondition(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            return true;
        }

        public virtual float CalculateExtraDmg(float curDmg, BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            return 0;
        }

        protected virtual bool CheckGainCondition(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            return true;
        }
        protected virtual bool CheckInflictCondition(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            return true;
        }

        protected virtual void OnVfxCompleted()
        {
            _barrier.Signal();
        }

        protected virtual void ExecuteVfxs(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            if (Data.VfxList.Count > 0)
            {
                List<CardSpawnVfxData> selfVfxList = Data.VfxList.Where(x => x.target == ETargetType.Self).ToList();
                List<CardSpawnVfxData> opponentVfxList = Data.VfxList.Where(x => x.target == ETargetType.Opponent).ToList();

                _barrier.AddCount(Data.VfxList.Count);
                _barrier.SetOnCompletedCallback(() =>
                {
                    OnAnimationCompleted(source, target);
                });

                if (selfVfxList.Count > 0)
                {
                    List<SpawnVfxSettings> selfVfxSettings = selfVfxList.Select(vfx => new SpawnVfxSettings
                    {
                        data = vfx,
                        onCompleted = OnVfxCompleted
                    }).ToList();

                    source.VFXController.PlayVfxList(selfVfxSettings);
                }

                if (opponentVfxList.Count > 0)
                {
                    List<SpawnVfxSettings> opponentVfxSettings = opponentVfxList.Select(vfx => new SpawnVfxSettings
                    {
                        data = vfx,
                        onCompleted = OnVfxCompleted
                    }).ToList();
                    target.VFXController.PlayVfxList(opponentVfxSettings);
                }
            }
        }

        protected virtual void OnAnimationCompleted(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            source.TriggerAfterCardResolved(target);
            target.CheckDeath();
        }
    }
}