namespace SEP490G69.GameSessions
{
    public interface IGameSessionCreator 
    {
        public bool TryCreateSession(string playerId, string characterId, out string errorMessage);
    }
}