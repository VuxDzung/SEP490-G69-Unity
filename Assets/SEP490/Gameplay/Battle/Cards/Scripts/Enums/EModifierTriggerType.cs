namespace SEP490G69.Battle
{
    public enum EModifierTriggerType 
    {
        /// <summary>
        /// Default option
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Apply immediately.
        /// </summary>
        Immediate = 1,

        /// <summary>
        /// Apply when the specific turn flow event is triggered.
        /// </summary>
        ByTurnFlowEvent = 2,
    }
}