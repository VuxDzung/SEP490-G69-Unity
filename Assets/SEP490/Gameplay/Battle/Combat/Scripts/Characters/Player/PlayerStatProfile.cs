namespace SEP490G69.Battle.Combat
{
    using System.Collections.Generic;

    public class PlayerStatProfile : IStatProfile
    {
        public IReadOnlyList<EStatusType> RequiredStats => new[]
        {
            EStatusType.Vitality,
            EStatusType.Power,
            EStatusType.Agi,
            EStatusType.Intelligence,
            EStatusType.Stamina,
            EStatusType.Defense,
            EStatusType.HP,
        };
    }
}