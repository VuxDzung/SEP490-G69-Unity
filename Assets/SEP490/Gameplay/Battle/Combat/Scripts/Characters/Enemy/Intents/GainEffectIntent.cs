namespace SEP490G69.Battle.Combat
{
    public class GainEffectIntent : BaseEnemyIntent
    {
        public override EIntentAction IntentType => EIntentAction.GainEffect;

        public override BaseCombatActor SelectTarget()
        {
            return _owner;
        }

        public override void Execute()
        {
            for (int i = 0; i < _data.GainAmount; i++)
            {
                //var effect = StatusEffectFactory.Create(_data.GainEffectId);
                //_owner.AddStatusEffect(effect);
            }
        }
    }
}