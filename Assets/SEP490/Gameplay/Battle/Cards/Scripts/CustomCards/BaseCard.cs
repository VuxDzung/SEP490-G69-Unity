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

        protected AnimationBarrier _vfxBarrier;

        public string RawCardId => Data.CardId;

        public BaseCard(CardSO data)
        {
            Data = data;
            _vfxBarrier = new AnimationBarrier();
        }

        public virtual void Execute(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            if (!ExecuteCondition(source, target)) return;

            Debug.Log($"Execute card {Data.CardId}");

            ApplyStatModifiers(source, target, Data.PreStatModifiers);

            ExecuteAction(source, target);
        }

        protected virtual void ExecuteAction(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            Debug.Log("Do nothing by defaut.");

            // Trigger flow event now.
            OnAnimationCompleted(source, target);
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
            _vfxBarrier.Signal();
        }

        protected virtual void ExecuteVfxs(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            if (Data.VfxList == null || Data.VfxList.Count == 0)
            {
                FinalizeCard(source, target);
                return;
            }

            List<CardSpawnVfxData> selfVfxList = Data.VfxList.Where(x => x.target == ETargetType.Self).ToList();
            List<CardSpawnVfxData> opponentVfxList = Data.VfxList.Where(x => x.target == ETargetType.Opponent).ToList();

            _vfxBarrier = new AnimationBarrier();
            _vfxBarrier.SetCount(Data.VfxList.Count);
            _vfxBarrier.SetOnCompletedCallback(() =>
            {
                FinalizeCard(source, target);
            });

            if (selfVfxList.Count > 0)
            {
                source.VFXController.PlayVfxList(selfVfxList.Select(vfx => new SpawnVfxSettings
                {
                    data = vfx,
                    onCompleted = OnVfxCompleted
                }).ToList());
            }

            if (opponentVfxList.Count > 0)
            {
                target.VFXController.PlayVfxList(opponentVfxList.Select(vfx => new SpawnVfxSettings
                {
                    data = vfx,
                    onCompleted = OnVfxCompleted
                }).ToList());
            }
        }

        protected virtual void FinalizeCard(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            ApplyStatModifiers(source, target, Data.PostStatModifiers);
            ApplyStatusEffects(source, target);

            source.TriggerAfterCardResolved(target);
            target.CheckDeath();
        }

        protected virtual void OnAnimationCompleted(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            ExecuteVfxs(source, target);
        }
    }
}