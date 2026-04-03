namespace SEP490G69.Battle.Combat
{
    public abstract class BaseEnemyIntent : IEnemyIntentStrategy
    {
        protected EnemyActorController _owner;
        protected EnemyIntentSO _data;

        public abstract EIntentAction IntentType { get; }

        public virtual void Initialize(EnemyActorController owner, EnemyIntentSO data)
        {
            _owner = owner;
            _data = data;
        }

        public abstract BaseCombatActor SelectTarget();

        public virtual void Preview()
        {
            // Default: show icon / value
        }

        public abstract void Execute();
    }
}