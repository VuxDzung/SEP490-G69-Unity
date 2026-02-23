namespace SEP490G69.GameSessions
{
    using System.Collections.Generic;

    public class SingleSessionCreator : IGameSessionCreator
    {
        /// <summary>
        /// Format of the session id includes:
        /// <PLAYER_ID>:<SESSION_ORDER/GUID>
        /// </summary>
        public const string SESSION_ID_FORMAT = "{0}:{1}";

        private GameSessionDAO _dao;

        public SingleSessionCreator()
        {
            _dao = new GameSessionDAO(LocalDBInitiator.GetDatabase());
        }

        public bool TryCreateSession(string playerId, string characterId, out string errorMessage)
        {
            errorMessage = "";

            if (_dao.GetAllSessions().Count > 0)
            {
                errorMessage = "error_session_02";
                return false;
            }

            // Dung: Construct session id.
            string idPreFix = playerId;
            string idPostFix = GetSessionIdOrder().ToString(); // May change later
            string sessionId = string.Format(playerId, idPostFix);

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

        private int GetSessionIdOrder()
        {
            List<PlayerTrainingSession> sessionList = _dao.GetAllSessions();
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