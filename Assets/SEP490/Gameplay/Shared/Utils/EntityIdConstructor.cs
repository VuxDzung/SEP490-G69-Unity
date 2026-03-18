namespace SEP490G69
{
    using UnityEngine;

    /// <summary>
    /// Make by Vu Duc Dung.
    /// This is a helper class used to construct game/db entity id.
    /// </summary>
    public class EntityIdConstructor 
    {
        /// <summary>
        /// Format of the database's session id
        /// <PLAYER_ID>_<CURRENT_RUN>
        /// </summary>
        public const string FORMAT_SESSION_ID = "{0}_{1}";


        /// <summary>
        /// Format of the database's entity id
        /// <SESSION_ID>:{RAW_ENTITY_ID}
        /// </summary>
        public const string FORMAT_DB_ENTITY_ID = "{0}:{1}";

        public static string ConstructSessionId(string playerId, int currentRun)
        {
            return string.Format(FORMAT_SESSION_ID, playerId, currentRun);
        }

        /// <summary>
        /// Construct the database's entity id.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="rawEntityId">It can be the character raw id, item raw id, etc.</param>
        /// <returns></returns>
        public static string ConstructDBEntityId(string sessionId, string rawEntityId)
        {
            return string.Format(FORMAT_DB_ENTITY_ID, sessionId, rawEntityId);
        }
    }
}