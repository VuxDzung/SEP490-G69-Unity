using System;

namespace SEP490G69.Battle.Combat
{
    public abstract class BaseEnemyIntent : IEnemyIntentStrategy
    {
        protected EnemyActorController _owner;
        protected EnemyIntentSO _data;
        protected SceneCombatController _battleManager;

        public abstract EIntentAction IntentType { get; }

        public virtual void Initialize(EnemyActorController owner, EnemyIntentSO data, SceneCombatController battleManager)
        {
            _owner = owner;
            _data = data;
            _battleManager = battleManager;
        }

        public abstract BaseCombatActor SelectTarget();

        public abstract void Preview();

        public abstract void Execute(Action onCompleted);
    }
}