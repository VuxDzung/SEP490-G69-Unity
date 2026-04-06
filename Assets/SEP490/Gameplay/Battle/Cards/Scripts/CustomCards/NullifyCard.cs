using SEP490G69.Battle.Cards;
using UnityEngine;

namespace SEP490G69.Battle.Combat
{
    public class NullifyCard : BaseAttackCard
    {
        public NullifyCard(CardSO cardSO) : base(cardSO) { }

        protected override void OnAfterAttack(float curDmg, BaseCombatActor source, BaseCombatActor opponent)
        {
            base.OnAfterAttack(curDmg, source, opponent);
            RuntimeStatusEffect[] buffs = opponent.EffectsManager.GetEffectsByType(EEffectType.Buff);

            if (buffs != null && buffs.Length > 0)
            {
                RuntimeStatusEffect buff = buffs[Random.Range(0, buffs.Length)];
                opponent.EffectsManager.Remove(buff);
            }
        }
    }
}