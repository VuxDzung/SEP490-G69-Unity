namespace SEP490G69
{
    using SEP490G69.Battle;
    using SEP490G69.Battle.Cards;
    using UnityEngine;

    [CreateAssetMenu(fileName = "CombatStatMod_", menuName = OrganizationConstants.NAMESPACE + "/Status/Combat status modifier data")]
    public class CombatStatModifierSO : StatusModifierSO
    {
        [SerializeField] private ETargetType m_StatProvider;
        [SerializeField] private ETargetType m_ApplyTarget;
        [SerializeField] private EStatCalculationSource m_CalculateSource;
        [SerializeField] private EApplyDiscardType m_ApplyDiscardType;

        public ETargetType StatProvider => m_StatProvider;
        public ETargetType ApplyTarget => m_ApplyTarget;
        public EStatCalculationSource CalculateSource => m_CalculateSource; 
        public EApplyDiscardType ApplyDiscardType => m_ApplyDiscardType;

        public bool IsInCombatStats()
        {
            return StatType == EStatusType.ActionGauge ||
                   StatType == EStatusType.ReceivedDmg ||
                   StatType == EStatusType.Damage ||
                   StatType == EStatusType.ActionCost;
        }
    }

    public enum EStatCalculationSource
    {
        None,
        Current,
        Max,
        Lost,
        FixedValue,
    }
}