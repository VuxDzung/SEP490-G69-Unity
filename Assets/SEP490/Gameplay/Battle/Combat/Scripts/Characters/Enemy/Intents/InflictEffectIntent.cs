namespace SEP490G69.Battle.Combat
{
    using UnityEngine;

    public class InflictEffectIntent : BaseEnemyIntent
    {
        public override EIntentAction IntentType => EIntentAction.InflictEffect;

        public override BaseCombatActor SelectTarget()
        {
            //return BattleManager.Instance.Player;
            return null;
        }

        public override void Execute()
        {
            var target = SelectTarget();

            for (int i = 0; i < _data.InflictAmount; i++)
            {
                //var effect = StatusEffectFactory.Create(_data.InflictEffectId);
                //target.AddStatusEffect(effect);
            }
        }
    }
}