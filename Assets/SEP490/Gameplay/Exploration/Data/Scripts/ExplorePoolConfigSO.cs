namespace SEP490G69.Exploration
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "RewardPoolConfig")]
    public class ExplorePoolConfigSO : ScriptableObject
    {
        [SerializeField] private List<ExplorePoolSO> m_PoolList;

        public IReadOnlyList<ExplorePoolSO> PoolList => m_PoolList;

        public ExplorePoolSO GetById(string poolId)
        {
            if (string.IsNullOrEmpty(poolId))
            {
                return null;
            }
            return m_PoolList.FirstOrDefault(p => p.PoolId == poolId);
        }
    }
}