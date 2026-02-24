using System.Collections.Generic;

namespace SEP490G69.GameSessions
{
    public interface IGameSessionCreator 
    {
        public bool TryCreateSession(string playerId, string characterId, out string sessionId, out string errorMessage);
        public List<PlayerTrainingSession> GetAllSessions();
        public bool TryDeleteSession(string playerId, string sessionId);
        public bool TryDeleteAllSessions(string playerId);
    }
}