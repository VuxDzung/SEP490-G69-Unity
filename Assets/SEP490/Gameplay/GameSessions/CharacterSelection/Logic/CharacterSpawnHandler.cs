namespace SEP490G69.GameSessions
{
    using UnityEngine;

    public class CharacterSpawnHandler : MonoBehaviour
    {
        private static CharacterSpawnHandler _instance;
        public static CharacterSpawnHandler Instance => _instance;

        [SerializeField] private string poolName;
        [SerializeField] private Transform m_CharacterHolder;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }

        public void SpawnCharacter(GameObject prefab)
        {
            if (PoolManager.Pools[poolName].Count > 0)
            {
                PoolManager.Pools[poolName].DespawnAll();
            }
            PoolManager.Pools[poolName].Spawn(prefab, m_CharacterHolder);
        }
    }
}