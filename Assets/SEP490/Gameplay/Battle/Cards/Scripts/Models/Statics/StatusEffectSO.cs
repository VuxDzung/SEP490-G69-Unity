namespace SEP490G69.Battle.Cards
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "StatusEffect_", menuName = OrganizationConstants.NAMESPACE + "/Battle/Cards/Status effect data")]
    public class StatusEffectSO : ScriptableObject
    {
        [SerializeField] private string m_EffectId;
        [SerializeField] private string m_EffectName;
        [SerializeField] private string m_EffectDesc;
        [SerializeField] private string m_EffectType;
        [SerializeField] private EOperator m_Op;
        [SerializeField] private float m_EffectValue;

        public string EffectId => m_EffectId;
        public string EffectName => m_EffectName;
        public string EffectDesc => m_EffectDesc;
        public string EffectType => m_EffectType;
        public EOperator Op => m_Op;
        public float EffectValue => m_EffectValue;
    }
}