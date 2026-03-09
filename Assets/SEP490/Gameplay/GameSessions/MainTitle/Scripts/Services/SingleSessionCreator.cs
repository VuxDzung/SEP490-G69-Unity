namespace SEP490G69.GameSessions
{
    using SEP490G69.Tournament;
    using SEP490G69.Training;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class SingleSessionCreator : IGameSessionCreator
    {
        /// <summary>
        /// Format of the session id includes:
        /// <PLAYER_ID>:<SESSION_ORDER/GUID>
        /// </summary>
        public const string SESSION_ID_FORMAT = "{0}_{1}";

        private GameSessionDAO _dao;
        private PlayerCharacterDAO _characterDAO;
        private TournamentProgressDAO _tournamentDAO;
        private TrainingExerciseDAO _trainingDAO;

        public SingleSessionCreator()
        {
            _dao = new GameSessionDAO(LocalDBInitiator.GetDatabase());
            _characterDAO = new PlayerCharacterDAO(LocalDBInitiator.GetDatabase());
            _tournamentDAO = new TournamentProgressDAO();
            _trainingDAO = new TrainingExerciseDAO(LocalDBInitiator.GetDatabase());
        }

        public List<PlayerTrainingSession> GetAllSessions(string playerId)
        {
            List<PlayerTrainingSession> sessionList = _dao.GetAllBydPlayerId(playerId);
            return sessionList;
        }

        public bool TryCreateSession(string playerId, string rawCharacterId, out string sessionId, out string errorMessage)
        {
            errorMessage = "";
            sessionId = "";

            if (_dao.GetAllBydPlayerId(playerId).Count > 0)
            {
                errorMessage = "error_session_02";
                return false;
            }

            // Dung: Construct session id.
            string idPreFix = playerId;
            string idPostFix = GetSessionIdOrder(playerId).ToString(); // May change later
            sessionId = string.Format(SESSION_ID_FORMAT, idPreFix, idPostFix);

            PlayerTrainingSession newSession = new PlayerTrainingSession();
            newSession.SessionId = sessionId;
            newSession.PlayerId = playerId;
            newSession.CharacterId = rawCharacterId;

            if (_dao.Insert(newSession))
            {
                return true;
            }
            
            errorMessage = "error_session_01";

            return false;
        }

        public bool TryDeleteAllSessions(string playerId)
        {
            List<PlayerTrainingSession> sessions = _dao.GetAllBydPlayerId(playerId);

            var playerSessions = sessions.Where(s => s.PlayerId == playerId).ToList();

            if (playerSessions.Count == 0)
                return false;

            bool allDeleted = true;

            foreach (var session in playerSessions)
            {
                // Step 1: delete all characters.
                SessionCharacterData characterData = _characterDAO.GetCharacterById(session.SessionId, session.CharacterId);

                if (characterData != null)
                {
                    _characterDAO.TryDeleteCharacter(characterData.Id);
                }

                // Step 2: Delete all tournament progress
                if (!_tournamentDAO.DeleteAllBySessionId(session.SessionId))
                {
                    Debug.LogError("Failed to delete all progress by session. Delete all by default (Testing only)");
                    _tournamentDAO.DeleteAll();
                    //continue;
                }

                // Step 3: Delete all training exercises.
                if (!_trainingDAO.DeleteAllBySessionId(session.SessionId))
                {
                    Debug.LogError("Failed to clear all old training exercises. Delete all by default.");
                    _trainingDAO.DeleteAll();
                }

                if (!_dao.DeleteById(session.SessionId)) // Error here.
                {
                    allDeleted = false;
                }
            }

            return allDeleted;
        }

        public bool TryDeleteSession(string playerId, string sessionId)
        {
            List<PlayerTrainingSession> sessions = _dao.GetAllBydPlayerId(playerId);

            var session = sessions
                .FirstOrDefault(s => s.SessionId == sessionId);

            if (session == null)
                return false;

            if (session.PlayerId != playerId)
                return false;

            return _dao.DeleteById(sessionId);
        }

        private int GetSessionIdOrder(string playerId)
        {
            List<PlayerTrainingSession> sessionList = GetAllSessions(playerId);
            List<int> orderIntList = new List<int>();
            foreach (var session in sessionList)
            {
                string[] idParts = session.SessionId.Split(':');
                if (idParts.Length == 2)
                {
                    string idPostfix = idParts[1];
                    if (int.TryParse(idPostfix, out int orderInt))
                    {
                        orderIntList.Add(orderInt);
                    }
                }
            }
            int newOrder = 0;
            if (orderIntList.Count == 0) return newOrder;

            while (!orderIntList.Contains(newOrder))
            {
                newOrder++;
            }
            return newOrder;
        }
    }
}