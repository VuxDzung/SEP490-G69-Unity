namespace SEP490G69.GameSessions
{
    using System.Collections.Generic;
    using System.Linq;

    public class SingleSessionCreator : IGameSessionCreator
    {
        /// <summary>
        /// Format of the session id includes:
        /// <PLAYER_ID>:<SESSION_ORDER/GUID>
        /// </summary>
        public const string SESSION_ID_FORMAT = "{0}_{1}";

        private GameSessionDAO _dao;

        public SingleSessionCreator()
        {
            _dao = new GameSessionDAO(LocalDBInitiator.GetDatabase());
        }

        public List<PlayerTrainingSession> GetAllSessions(string playerId)
        {
            List<PlayerTrainingSession> sessionList = _dao.GetAllSessions(playerId);
            return sessionList;
        }

        public bool TryCreateSession(string playerId, string characterId, out string sessionId, out string errorMessage)
        {
            errorMessage = "";
            sessionId = "";

            if (_dao.GetAllSessions(playerId).Count > 0)
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
            newSession.CharacterId = characterId;

            if (_dao.InsertSession(newSession))
            {
                return true;
            }
            
            errorMessage = "error_session_01";

            return false;
        }

        public bool TryDeleteAllSessions(string playerId)
        {
            List<PlayerTrainingSession> sessions = _dao.GetAllSessions(playerId);

            var playerSessions = sessions
                .Where(s => s.PlayerId == playerId)
                .ToList();

            if (playerSessions.Count == 0)
                return false;

            bool allDeleted = true;

            foreach (var session in playerSessions)
            {
                if (!_dao.DeleteSession(session.SessionId))
                {
                    allDeleted = false;
                }
            }

            return allDeleted;
        }

        public bool TryDeleteSession(string playerId, string sessionId)
        {
            List<PlayerTrainingSession> sessions = _dao.GetAllSessions(playerId);

            var session = sessions
                .FirstOrDefault(s => s.SessionId == sessionId);

            if (session == null)
                return false;

            if (session.PlayerId != playerId)
                return false;

            return _dao.DeleteSession(sessionId);
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