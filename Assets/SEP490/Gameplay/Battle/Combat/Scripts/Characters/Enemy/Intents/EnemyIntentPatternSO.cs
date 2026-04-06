namespace SEP490G69.Battle.Combat
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "intent_pattern_", menuName = OrganizationConstants.NAMESPACE + "/Battle/Enemy/Enemy Intent Pattern")]
    public class EnemyIntentPatternSO : ScriptableObject
    {
        [SerializeField] private List<EnemyIntentSO> m_Intents;

        public IReadOnlyList<EnemyIntentSO> Intents => m_Intents;
    }
}