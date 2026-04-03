namespace SEP490G69.Battle.Combat
{
    public static class EnemyIntentFactory
    {
        public static IEnemyIntentStrategy CreateIntent(EnemyIntentSO data, EnemyActorController owner)
        {
            IEnemyIntentStrategy intent = data.Intent switch
            {
                EIntentAction.Attack => new AttackIntent(),
                EIntentAction.Shield => new ShieldIntent(),
                EIntentAction.InflictEffect => new InflictEffectIntent(),
                EIntentAction.GainEffect => new GainEffectIntent(),
                _ => null
            };

            intent?.Initialize(owner, data);
            return intent;
        }
    }
}