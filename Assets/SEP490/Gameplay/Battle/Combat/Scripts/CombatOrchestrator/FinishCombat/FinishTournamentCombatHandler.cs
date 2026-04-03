namespace SEP490G69.Battle.Combat
{
    using SEP490G69.Addons.LoadScreenSystem;
    using SEP490G69.GameSessions;
    using SEP490G69.Tournament;
    using UnityEngine;

    public class FinishTournamentCombatHandler : IFinishCombatHandler
    {
        private readonly TournamentProgressDAO _tournamentsDAO;

        public FinishTournamentCombatHandler(TournamentProgressDAO tournamentsDAO)
        {
            _tournamentsDAO = tournamentsDAO;
        }

        public void HandleCombatResult(PlayerTrainingSession sessionData, bool isPlayerWon)
        {
            TournamentProgressData tournamentData = _tournamentsDAO.GetById(sessionData.ActiveTournamentId);

            if (string.IsNullOrEmpty(sessionData.ActiveTournamentId))
            {
                Debug.LogError($"[SceneCombatController.OnVictorious] Active tournament id in session {sessionData.SessionId} is null/empty");
                return;
            }

            if (tournamentData == null)
            {
                Debug.LogError($"[SceneCombatController.OnVictorious] Tournament data with id {sessionData.ActiveTournamentId} does not exist in the database");
                return;
            }

            tournamentData.IsBattleFinished = true;
            tournamentData.IsPlayerWon = isPlayerWon;

            _tournamentsDAO.Update(tournamentData);
        }

        public void NavigateToScene()
        {
            SceneLoader.Singleton.StartLoadScene(GameConstants.SCENE_TOURNAMENT);
        }
    }
}