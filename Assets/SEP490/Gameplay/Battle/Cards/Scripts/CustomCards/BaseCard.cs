namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
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

        public virtual void Execute(PlayerActorController source, BaseCombatActor opponent)
        {
            if (!ExecuteCondition(source, opponent)) return;

            Debug.Log($"Execute card {Data.CardId}");

            ApplyStatModifiers(source, opponent, Data.PreStatModifiers);

            ExecuteAction(source, opponent);

            StackShield(source);
        }

        protected virtual void StackShield(PlayerActorController source)
        {
            if (Data.ActionType == EActionType.Shield)
            {
                source.StackShield(Data.BaseValue, Data.ModifierValue);
            }
        }

        protected virtual void ExecuteAction(PlayerActorController source, BaseCombatActor opponent)
        {
            Debug.Log("Do nothing by defaut.");

            // Trigger flow event now.
            OnAnimationCompleted(source, opponent);
        }

        protected void ApplyStatModifiers(PlayerActorController source, BaseCombatActor opponent, CombatStatModifierSO[] modifiers)
        {
            if (modifiers == null || modifiers.Length == 0)
            {
                Debug.Log("No modifier");
                return;
            }

            BaseCombatActor receiver = null;

            foreach (var mod in modifiers)
            {
                Debug.Log($"Start modifier: {mod.Id}");
                receiver = mod.ApplyTarget == ETargetType.Self ? source : opponent;
                receiver.ApplyStatusDelta(mod, mod.ApplyTarget == ETargetType.Opponent);
            }
        }

        protected void ApplyStatusEffects(PlayerActorController source, BaseCombatActor target)
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

        protected virtual bool ExecuteCondition(PlayerActorController source, BaseCombatActor opponent)
        {
            return true;
        }

        public virtual float CalculateExtraDmg(float curDmg, PlayerActorController source, BaseCombatActor opponent)
        {
            return 0;
        }

        protected virtual bool CheckGainCondition(PlayerActorController source, BaseCombatActor opponent)
        {
            return true;
        }
        protected virtual bool CheckInflictCondition(PlayerActorController source, BaseCombatActor opponent)
        {
            return true;
        }

        protected virtual void OnVfxCompleted()
        {
            _vfxBarrier.Signal();
        }

        //protected virtual void ExecuteVfxs(PlayerActorController source, BaseCombatActor target)
        //{
        //    if (Data.VfxList == null || Data.VfxList.Count == 0)
        //    {
        //        FinalizeCard(source, target);
        //        return;
        //    }

        //    List<SpawnVfxData> selfVfxList = Data.VfxList.Where(x => x.target == ETargetType.Self).ToList();
        //    List<SpawnVfxData> opponentVfxList = Data.VfxList.Where(x => x.target == ETargetType.Opponent).ToList();

        //    _vfxBarrier = new AnimationBarrier();
        //    _vfxBarrier.SetCount(Data.VfxList.Count);
        //    _vfxBarrier.SetOnCompletedCallback(() =>
        //    {
        //        FinalizeCard(source, target);
        //    });

        //    if (selfVfxList.Count > 0)
        //    {
        //        source.VFXController.PlayVfxList(selfVfxList.Select(vfx => new SpawnVfxSettings
        //        {
        //            data = vfx,
        //            onCompleted = OnVfxCompleted
        //        }).ToList());
        //    }

        //    if (opponentVfxList.Count > 0)
        //    {
        //        target.VFXController.PlayVfxList(opponentVfxList.Select(vfx => new SpawnVfxSettings
        //        {
        //            data = vfx,
        //            onCompleted = OnVfxCompleted
        //        }).ToList());
        //    }
        //}

        protected virtual void FinalizeCard(PlayerActorController source, BaseCombatActor opponent)
        {
            ApplyStatModifiers(source, opponent, Data.PostStatModifiers);
            ApplyStatusEffects(source, opponent);

            source.TriggerAfterCardResolved(opponent);
            opponent.CheckDeath();
        }

        protected virtual void OnAnimationCompleted(PlayerActorController source, BaseCombatActor opponent)
        {
            source.ExecuteVfxs(Data.VfxList, opponent, (opponent) =>
            {
                FinalizeCard(source, opponent);
            });
        }
    }
}