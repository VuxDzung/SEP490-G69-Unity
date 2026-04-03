namespace SEP490G69.Battle
{
    public enum EDecayType
    {
        None = 0,
        Immediate = 1,
        OnBeingAttacked = 3,
        TurnStart = 4,
        TurnEnd = 5,
        Fovever = 6,
    }

    public enum EEffectApplyType
    {
        None = 0,
        Immediate = 1,
        OnBeingAttacked = 3,
        TurnStart = 4,
        TurnEnd = 5,
        Fovever = 6,
        OnHitTarget = 7,
    }

    public enum EStatusEffectCategory
    {
        None = 0,
        Scaling = 1,
        Decay = 2,
        DOT = 3,
        Passive = 4,
        Special = 5,
    }
}