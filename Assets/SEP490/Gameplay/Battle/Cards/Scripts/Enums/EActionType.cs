namespace SEP490G69.Battle
{
    using UnityEngine;

    public enum EActionType 
    {
        Attack = 0,
        Effect = 1,
        StatRecover = 2,
        HPRecover = 3,
        Other = 4,
    }

    public enum EExtraAction
    {
        None = 0,
        RemoveRandomBuffOfEnemy = 1,
    }
}