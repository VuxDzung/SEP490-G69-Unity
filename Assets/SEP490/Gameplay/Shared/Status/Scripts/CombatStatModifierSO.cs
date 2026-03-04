namespace SEP490G69
{
    using SEP490G69.Battle.Cards;
    using UnityEngine;

    [CreateAssetMenu(fileName = "CombatStatMod_", menuName = OrganizationConstants.NAMESPACE + "/Status/Combat status modifier data")]
    public class CombatStatModifierSO : StatusModifierSO
    {
        [SerializeField] private ETargetType m_ApplyTarget;

        public ETargetType ApplyTarget => m_ApplyTarget;
    }
}