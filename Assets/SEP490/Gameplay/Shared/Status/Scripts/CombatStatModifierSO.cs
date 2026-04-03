namespace SEP490G69
{
    using SEP490G69.Battle;
    using SEP490G69.Battle.Cards;
    using SEP490G69.Battle.Combat;
    using UnityEngine;

    [CreateAssetMenu(fileName = "CombatStatMod_", menuName = OrganizationConstants.NAMESPACE + "/Status/Combat status modifier data")]
    public class CombatStatModifierSO : StatusModifierSO
    {
        [SerializeField] private ETargetType m_ApplyTarget;
        [SerializeField] private EStatCalculationSource m_CalculateSource;
        [Tooltip("Consists 2 main option: base/current value and final getter value")]
        [SerializeField] private EApplyValueType m_ApplyValueType;
        [SerializeField] private EModifierTriggerType m_TriggerType;
        [SerializeField] private ETurnFlowEvent m_TurnFlowEvent;
        [Tooltip("The minimum value which the modifier cannot decrease lower than it")]
        [SerializeField] private float m_FloorValue;

        public ETargetType ApplyTarget => m_ApplyTarget;
        public EStatCalculationSource CalculateSource => m_CalculateSource; 
        public EApplyValueType ApplyValueType => m_ApplyValueType;
        public EModifierTriggerType TriggerType => m_TriggerType;
        public ETurnFlowEvent TurnFlowEvent => m_TurnFlowEvent;
        public float FloorValue => m_FloorValue;
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