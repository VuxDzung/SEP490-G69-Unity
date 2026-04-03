namespace SEP490G69.Battle.Combat
{
    using System.Collections.Generic;

    public interface IStatProfile
    {
        IReadOnlyList<EStatusType> RequiredStats { get; }
    }
}