namespace SEP490G69.GameSessions
{
    public enum ESyncState
    {
        Idle,
        CheckingConnection,
        PullingFromCloud,
        PushingToCloud,
        Finished,
        Error
    }
}