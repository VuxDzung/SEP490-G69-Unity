namespace SEP490G69.Battle.Combat
{
    using SEP490G69.GameSessions;

    public interface IFinishCombatHandler 
    {
        public void HandleCombatResult(PlayerTrainingSession sessionData, bool isPlayerWon);
        public void NavigateToScene();
    }
}