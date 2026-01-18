namespace SEP490G69
{
    /// <summary>
    /// This is a interface for context which alive through the entire game loop.
    /// </summary>
    public interface IGameContext
    {
        public void SetManager(ContextManager manager);
    }
}