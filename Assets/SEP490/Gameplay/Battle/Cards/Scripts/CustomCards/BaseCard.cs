namespace SEP490G69.Battle
{
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    public class BaseCard
    {
        protected CardSO Data;

        public BaseCard(CardSO data)
        {
            Data = data;
        }

        public virtual void Execute(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            if (!ExecuteCondition(source, target)) return;

            Debug.Log($"Execute card {Data.CardId}");

            ApplyStatModifiers(source, target, Data.PreStatModifiers);

            ExecuteAction(source, target);

            ApplyStatModifiers(source, target, Data.PostStatModifiers);

            ApplyStatusEffects(source, target);
        }

        protected virtual void ExecuteAction(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            Debug.Log("Do nothing");

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

        protected virtual bool CheckGainCondition(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            return true;
        }
        protected virtual bool CheckInflictCondition(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            return true;
        }

        protected virtual void OnAnimationCompleted(BaseBattleCharacterController source, BaseBattleCharacterController target)
        {
            source.TriggerAfterCardResolved(target);
            target.CheckDeath();
        }
    }
}