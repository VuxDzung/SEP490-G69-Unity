namespace SEP490G69.GameSessions
{
    using UnityEngine;
    using UnityEngine.TextCore.Text;

    public class CharacterSpawnHandler : MonoBehaviour
    {
        private static CharacterSpawnHandler _instance;
        public static CharacterSpawnHandler Instance => _instance;

        [SerializeField] private string poolName;
        [SerializeField] private Transform m_CharacterHolder;

        private Transform _character;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }

        public void SpawnCharacter(GameObject prefab)
        {
            DespawnCharacter();
            _character = PoolManager.Pools[poolName].Spawn(prefab, m_CharacterHolder);
        }

        public void DespawnCharacter()
        {
            if (_character != null)
            {
                PoolManager.Pools[poolName].DespawnObject(_character);
                _character = null;
            }
        }
    }
}