namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Tournament;

    public class EnemyStatsInitializer : ICombatStatsInitializer
    {
        private InCombatStatus _statHP;
        private InCombatStatus _statAtk;
        private InCombatStatus _statINT;
        private InCombatStatus _statDEF;

        private EnemySO _enemySO;

        public EnemyStatsInitializer(EnemySO enemySO)
        {
            _enemySO = enemySO;
        }

        public void InitializeStats(BaseCombatActor actor)
        {
            float hp = _enemySO.EnemyMaxHP;
            float attack = _enemySO.EnemyAttack;
            float INT = _enemySO.EnemyINT;
            float def = _enemySO.EnemyDEF;

            actor.StatsManager.SetCurrentValue(EStatusType.HP, hp);
            actor.StatsManager.SetMaxValue(EStatusType.HP, hp);

            actor.StatsManager.SetCurrentValue(EStatusType.Attack, attack);
            actor.StatsManager.SetMaxValue(EStatusType.Attack, attack);

            actor.StatsManager.SetCurrentValue(EStatusType.Intelligence, INT);
            actor.StatsManager.SetMaxValue(EStatusType.Intelligence, INT);

            actor.StatsManager.SetCurrentValue(EStatusType.Defense, def);
            actor.StatsManager.SetMaxValue(EStatusType.Defense, def);
        }
    }
}