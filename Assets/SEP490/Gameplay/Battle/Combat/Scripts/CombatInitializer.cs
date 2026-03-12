namespace SEP490G69.Battle.Combat
{
    using SEP490G69.GameSessions;
    using UnityEngine;

    public class CombatInitializer
    {
        public (PlayerBattleCharaterController, EnemyCombatController)
            Initialize(Transform playerParent, Transform enemyParent, string poolName)
        {
            PlayerBattleCharaterController player = SpawnPlayer(playerParent, poolName);
            EnemyCombatController enemy = SpawnEnemy(enemyParent, poolName);

            return (player, enemy);
        }

        private PlayerBattleCharaterController SpawnPlayer(Transform parent, string pool)
        {
            string sessionId = PlayerPrefs.GetString(GameConstants.PREF_KEY_CURRENT_SESSION_ID);

            GameSessionDAO dao = new GameSessionDAO(LocalDBInitiator.GetDatabase());
            PlayerTrainingSession session = dao.GetById(sessionId);

            CharacterConfigSO config = ContextManager.Singleton.GetDataSO<CharacterConfigSO>();

            BaseCharacterSO charSO = config.GetCharacterById(session.CharacterId);

            Transform trans = PoolManager.Pools[pool].Spawn(charSO.CombatPrefab, parent);

            var controller = trans.GetComponent<PlayerBattleCharaterController>();
            controller.Initialize(charSO);

            return controller;
        }

        private EnemyCombatController SpawnEnemy(Transform parent, string pool)
        {
            string enemyId = PlayerPrefs.GetString(GameConstants.PREF_KEY_TOURNAMENT_ENEMY_ID);

            CharacterConfigSO config = ContextManager.Singleton.GetDataSO<CharacterConfigSO>();
            BaseCharacterSO enemySO = config.GetCharacterById(enemyId);

            Transform trans = PoolManager.Pools[pool].Spawn(enemySO.CombatPrefab, parent);

            var controller = trans.GetComponent<EnemyCombatController>();
            controller.Initialize(enemySO);

            return controller;
        }
    }
}