namespace SEP490G69.GameSessions 
{
    public enum EMetadataResult
    {
        /// <summary>
        /// The player has the profile and the session
        /// </summary>
        HasProfileHasSession = 0,

        /// <summary>
        /// The player has no profile -> not register yet. Represent an error.
        /// </summary>
        NoProfile = 1,

        /// <summary>
        /// The player has player profile but no session available.
        /// </summary>
        HasProfileNoSession = 2,
    }
}