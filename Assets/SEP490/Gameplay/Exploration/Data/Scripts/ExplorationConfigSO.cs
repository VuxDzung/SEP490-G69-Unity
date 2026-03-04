namespace SEP490G69.Exploration
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ExplorationConfig", menuName = OrganizationConstants.NAMESPACE + "/Exloration/Exploration config")]
    public class ExplorationConfigSO : ScriptableObject
    {
        [SerializeField] private List<ExplorationSO> m_ExploreLocations;

        public IReadOnlyList<ExplorationSO> ExplorationLocations => m_ExploreLocations;

        public ExplorationSO GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            if (m_ExploreLocations.Count == 0)
            {
                return null;
            }
            return m_ExploreLocations.FirstOrDefault(el => el.ExplorationId.Equals(id));
        }
    }
}