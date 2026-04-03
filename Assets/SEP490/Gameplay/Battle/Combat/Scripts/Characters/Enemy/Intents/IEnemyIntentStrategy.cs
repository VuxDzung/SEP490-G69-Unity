namespace SEP490G69.Battle.Combat
{
    public interface IEnemyIntentStrategy
    {
        EIntentAction IntentType { get; }

        void Initialize(EnemyActorController owner, EnemyIntentSO data);

        /// Decide target (player, self...)
        BaseCombatActor SelectTarget();

        /// Show intent (UI, preview damage, icon...)
        void Preview();

        /// Execute logic
        void Execute();
    }
}