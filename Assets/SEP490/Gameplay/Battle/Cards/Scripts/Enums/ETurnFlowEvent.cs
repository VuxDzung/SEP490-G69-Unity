namespace SEP490G69.Battle
{
    public enum ETurnFlowEvent 
    {
        None = 0,

        TurnStarted = 1,

        BeforeCardAction = 2,

        /// <summary>
        /// After card action/turn end
        /// </summary>
        AfterCardAction = 3,

        AfterResetActionGaugue = 4,
    }
}