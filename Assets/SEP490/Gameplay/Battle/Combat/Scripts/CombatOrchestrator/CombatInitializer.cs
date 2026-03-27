namespace SEP490G69.Battle.Combat
{
    using SEP490G69.GameSessions;
    using SEP490G69.Tournament;
    using UnityEngine;

    public class CombatInitializer
    {
        private TournamentProgressDAO _tournamentDAO;
        private GameSessionDAO _sessionDAO;

        public void Initialize(Transform playerParent, Transform enemyParent, string poolName, out PlayerBattleCharaterController player, out EnemyCombatController enemy)
        {
            player = null;
            enemy = null;

            _sessionDAO = new GameSessionDAO();
            _tournamentDAO = new TournamentProgressDAO();

            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);

            PlayerTrainingSession sessionData = _sessionDAO.GetById(sessionId);
            if (sessionData == null )
            {
                Debug.LogError($"[CombatInitializer.Initialize] Session data with id {sessionId} does not exist in the database");
                return;
            }

            string combatType = PlayerPrefs.GetString(GameConstants.PREF_KEY_COMBAT_TYPE);

            if (string.IsNullOrEmpty(combatType))
            {
                Debug.LogError("[CombatInitializer.Initialize fatal error] No combat type is assigned.");
                return;
            }

            string enemyId = GetEnemyId(combatType, sessionData);

            if (string.IsNullOrEmpty(enemyId))
            {
                Debug.LogError("[CombatInitializer.Initialize fatal error] EnemyId is empty -> No enemy is assigned.");
                return;
            }

            player = SpawnPlayer(playerParent, poolName);
            enemy = SpawnEnemy(enemyId, enemyParent, poolName);
        }

        private PlayerBattleCharaterController SpawnPlayer(Transform parent, string pool)
        {
            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);

            GameSessionDAO dao = new GameSessionDAO();
            PlayerTrainingSession session = dao.GetById(sessionId);

            CharacterConfigSO config = ContextManager.Singleton.GetDataSO<CharacterConfigSO>();

            BaseCharacterSO charSO = config.GetCharacterById(session.RawCharacterId);

            Transform trans = PoolManager.Pools[pool].Spawn(charSO.CombatPrefab, parent);

            var controller = trans.GetComponent<PlayerBattleCharaterController>();
            controller.Initialize(charSO);

            return controller;
        }

        private EnemyCombatController SpawnEnemy(string enemyId, Transform parent, string pool)
        {
            enemyId = enemyId.Trim();
            CharacterConfigSO config = ContextManager.Singleton.GetDataSO<CharacterConfigSO>();
            if (string.IsNullOrEmpty(enemyId))
            {
                Debug.LogError("Enemy id is empty");
                return null;
            }
            BaseCharacterSO enemySO = config.GetCharacterById(enemyId);

            if (enemySO == null)
            {
                Debug.LogError($"Enemy SO with id {enemyId} does not exist");
                return null;
            }

            Transform trans = PoolManager.Pools[pool].Spawn(enemySO.CombatPrefab, parent);

            var controller = trans.GetComponent<EnemyCombatController>();
            controller.Initialize(enemySO);

            return controller;
        }

        private string GetEnemyId(string combatType, PlayerTrainingSession sessionData)
        {
            return combatType switch
            {
                GameConstants.COMBAT_TYPE_TOURNAMENT => GetTournamentEnemyId(sessionData),
                GameConstants.COMBAT_TYPE_EXPLORATION => GetExploreEnemyId(),
                _ => string.Empty
            };
        }

        private string GetTournamentEnemyId(PlayerTrainingSession sessionData)
        {
            TournamentProgressData tournamentData = _tournamentDAO.GetById(sessionData.ActiveTournamentId);
            if (tournamentData == null)
            {
                Debug.LogError($"[CombatInitializer.Initialize] Tournament data with id {sessionData.ActiveTournamentId} does not exist in the database");
                return string.Empty;
            }
            return tournamentData.PendingEnemyId;
        }

        private string GetExploreEnemyId()
        {
            string enemyId = PlayerPrefs.GetString(GameConstants.PREF_KEY_EXPLORE_ENEMY_ID, string.Empty);
            return enemyId;
        }
    }
}