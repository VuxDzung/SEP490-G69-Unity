namespace SEP490G69.Battle.Combat
{
    public static class EnemyIntentFactory
    {
        public static IEnemyIntentStrategy CreateIntent(EnemyIntentSO data, EnemyActorController owner, SceneCombatController battleManager)
        {
            var composite = new CompositeIntent();

            if ((data.Intent & EIntentAction.Attack) != 0)
                composite.Add(new AttackIntent());

            if ((data.Intent & EIntentAction.Shield) != 0)
                composite.Add(new ShieldIntent());

            if ((data.Intent & EIntentAction.InflictEffect) != 0)
                composite.Add(new InflictEffectIntent());

            if ((data.Intent & EIntentAction.GainEffect) != 0)
                composite.Add(new GainEffectIntent());

            //if ((data.Intent & EIntentAction.AddCardToPlayer) != 0)
            //    composite.Add(new AddCardIntent());

            //if ((data.Intent & EIntentAction.Pierce) != 0)
            //    composite.Add(new PierceIntent());

            composite.Initialize(owner, data, battleManager);
            return composite;
        }
    }
}