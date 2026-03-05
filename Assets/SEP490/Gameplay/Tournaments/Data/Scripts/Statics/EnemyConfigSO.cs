namespace SEP490G69.Tournament
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "EnemyConfigSO", menuName = OrganizationConstants.NAMESPACE + "/Tournaments/Enemy config")]
    public class EnemyConfigSO : ScriptableObject
    {
        [SerializeField] private List<EnemySO> m_Enemies;

        public IReadOnlyList<EnemySO> Enemies => m_Enemies;

        public EnemySO GetEnemyById(string id)
        {
            if (string.IsNullOrEmpty(id) || m_Enemies == null || m_Enemies.Count == 0)
            {
                return null;
            }

            return m_Enemies.FirstOrDefault(e => e.CharacterId.Equals(id));
        }
    }
}