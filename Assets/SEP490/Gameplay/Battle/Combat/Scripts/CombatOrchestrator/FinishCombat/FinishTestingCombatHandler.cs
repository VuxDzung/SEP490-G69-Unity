namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.GameSessions;

    public class FinishTestingCombatHandler : IFinishCombatHandler
    {
        public void HandleCombatResult(PlayerTrainingSession sessionData, bool isPlayerWon)
        {
            
        }

        public void NavigateToScene()
        {
            SceneLoader.Singleton.StartLoad(GameConstants.SCENE_MAIN_MENU);
        }
    }
}