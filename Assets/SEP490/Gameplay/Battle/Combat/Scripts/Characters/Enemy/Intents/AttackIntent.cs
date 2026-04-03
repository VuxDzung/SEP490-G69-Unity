namespace SEP490G69.Battle.Combat
{
    public class AttackIntent : BaseEnemyIntent
    {
        public override EIntentAction IntentType => EIntentAction.Attack;

        public override BaseCombatActor SelectTarget()
        {
            //return BattleManager.Instance.Player;
            return null;
        }

        public override void Execute()
        {
            var target = SelectTarget();

            float damage = _data.BaseDamage * _data.AttackMultiplier;

            _owner.PlayAtkSfx();
            target.ReceiveAttack(damage, _owner);
        }

        public override void Preview()
        {
            float damage = _data.BaseDamage * _data.AttackMultiplier;

            //EnemyIntentUI.ShowAttack(damage);
        }
    }
}