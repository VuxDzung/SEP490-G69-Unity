namespace SEP490G69.Battle.Combat
{
    using System.Collections.Generic;

    public class EnemyStatProfile : IStatProfile
    {
        public IReadOnlyList<EStatusType> RequiredStats => new[]
        {
            EStatusType.HP,
            EStatusType.Attack,
            EStatusType.Defense,
            EStatusType.Intelligence,
        };
    }
}