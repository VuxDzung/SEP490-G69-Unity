namespace SEP490G69.Battle.Combat
{
    public class ShieldIntent : BaseEnemyIntent
    {
        public override EIntentAction IntentType => EIntentAction.Shield;

        public override BaseCombatActor SelectTarget()
        {
            return _owner;
        }

        public override void Execute()
        {
            _owner.StackShield(_data.BaseDefend, _data.DefendMultiplier);
        }

        public override void Preview()
        {
            //float shield = _data.BaseDefend * _data.DefendMultiplier;

            //EnemyIntentUI.ShowShield(shield);
        }
    }
}