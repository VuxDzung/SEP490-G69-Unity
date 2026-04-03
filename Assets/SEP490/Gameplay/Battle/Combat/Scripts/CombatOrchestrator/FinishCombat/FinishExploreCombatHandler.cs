namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.GameSessions;
    using UnityEngine;

    public class FinishExploreCombatHandler : IFinishCombatHandler
    {
        public void HandleCombatResult(PlayerTrainingSession sessionData, bool isPlayerWon)
        {
            PlayerPrefs.SetInt(GameConstants.PREF_KEY_IS_BATTLE_WON, isPlayerWon ? 1 : 0);
        }

        public void NavigateToScene()
        {
            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_EXPLORATION);
        }
    }
}