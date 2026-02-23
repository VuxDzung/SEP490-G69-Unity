namespace SEP490G69.GameSessions
{
    public interface IGameSessionContinuation 
    {
        public bool TryContinueGame(out string messager);
    }
}