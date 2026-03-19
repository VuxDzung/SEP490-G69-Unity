namespace SEP490G69.GameSessions
{
    using SEP490G69.Battle.Cards;
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

        private GameSessionDAO _sessionDAO;
        private PlayerDataDAO _playerDAO;
        private PlayerCharacterDAO _characterDAO;
        private TournamentProgressDAO _tournamentDAO;
        private TrainingExerciseDAO _trainingDAO;
        private GameDeckDAO _deckDAO;
        private GameCardsDAO _cardsDAO;

        public SingleSessionCreator()
        {
            _sessionDAO = new GameSessionDAO();
            _playerDAO = new PlayerDataDAO();
            _characterDAO = new PlayerCharacterDAO();
            _tournamentDAO = new TournamentProgressDAO();
            _trainingDAO = new TrainingExerciseDAO();
            _deckDAO = new GameDeckDAO();
            _cardsDAO = new GameCardsDAO();
        }

        public List<PlayerTrainingSession> GetAllSessions(string playerId)
        {
            List<PlayerTrainingSession> sessionList = _sessionDAO.GetAllByPlayerId(playerId);
            return sessionList;
        }

        public bool TryCreateSession(string playerId, string rawCharacterId, out string sessionId, out string errorMessage)
        {
            errorMessage = "";
            sessionId = "";

            if (_sessionDAO.GetAllByPlayerId(playerId).Count > 0)
            {
                errorMessage = "error_session_02";
                return false;
            }

            PlayerData playerData = _playerDAO.GetById(playerId);

            int newRun = playerData.CurrentRun + 1;

            // Dung: Construct session id.
            string idPreFix = playerId;
            string idPostFix = newRun.ToString(); //GetSessionIdOrder(playerId).ToString(); // May change later
            sessionId = string.Format(SESSION_ID_FORMAT, idPreFix, idPostFix);

            PlayerTrainingSession newSession = new PlayerTrainingSession();
            newSession.SessionId = sessionId;
            newSession.PlayerId = playerId;
            newSession.RawCharacterId = rawCharacterId;
            newSession.CurrentGoldAmount = GameConstants.STARTER_MONEY_AMOUNT;

            if (_sessionDAO.Insert(newSession))
            {
                playerData.CurrentRun = newRun;
                _playerDAO.Update(playerData);
                return true;
            }
            
            errorMessage = "error_session_01";

            return false;
        }

        public bool TryDeleteAllSessions(string playerId)
        {
            List<PlayerTrainingSession> sessions = _sessionDAO.GetAllByPlayerId(playerId);

            var playerSessions = sessions.Where(s => s.PlayerId == playerId).ToList();

            if (playerSessions.Count == 0)
            {
                return false;
            }

            bool allDeleted = true;

            foreach (var session in playerSessions)
            {
                // Step 1: delete all characters.
                SessionCharacterData characterData = _characterDAO.GetById(session.SessionId, session.RawCharacterId);

                if (characterData != null)
                {
                    _characterDAO.Delete(characterData.Id);
                }

                // Step 2: Delete all tournament progress
                if (!_tournamentDAO.DeleteAllBySessionId(session.SessionId))
                {
                    Debug.LogError("[SingleSessionCreator] Failed to delete all progress by session.");
                    //continue;
                }

                // Step 3: Delete all training exercises.
                if (!_trainingDAO.DeleteAllBySessionId(session.SessionId))
                {
                    Debug.LogError("[SingleSessionCreator] Failed to clear all old training exercises.");
                }

                // Step 4: Delete deck.
                if (!_deckDAO.Delete(session.SessionId))
                {
                    Debug.LogError($"[SingleSessionCreator] Failed to delete the deck of session {session.SessionId}");
                }

                // Step 5: Delete all cards
                if (!_cardsDAO.DeleteAllBySessionId(session.SessionId))
                {
                    Debug.LogError($"[SingleSessionCreator] Failed to delete all cards of session {session.SessionId}");
                }

                // Final: delete the session.
                if (!_sessionDAO.DeleteById(session.SessionId))
                {
                    Debug.LogError($"[SingleSessionCreator] Failed to delete session {session.SessionId}");

                    allDeleted = false;
                }
            }

            return allDeleted;
        }

        public bool TryDeleteSession(string playerId, string sessionId)
        {
            List<PlayerTrainingSession> sessions = _sessionDAO.GetAllByPlayerId(playerId);

            var session = sessions
                .FirstOrDefault(s => s.SessionId == sessionId);

            if (session == null)
                return false;

            if (session.PlayerId != playerId)
                return false;

            return _sessionDAO.DeleteById(sessionId);
        }

        #region Archive (May delete later)
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

            while (orderIntList.Contains(newOrder))
            {
                newOrder++;
            }
            return newOrder;
        }
        #endregion
    }
}