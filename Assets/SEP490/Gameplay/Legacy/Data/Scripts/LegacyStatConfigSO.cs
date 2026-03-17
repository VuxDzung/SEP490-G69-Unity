namespace SEP490G69.Legacy
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "LegacyStatConfig", menuName = OrganizationConstants.NAMESPACE + "/Legacy/Legacy stat config")]
    public class LegacyStatConfigSO : ScriptableObject
    {
        [SerializeField] private List<LegacyStatSO> m_LegacyStats;
        public IReadOnlyList<LegacyStatSO> LegacyStats => m_LegacyStats;
    }
}