namespace SEP490G69.Battle.Cards
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "StatusEffect_", menuName = OrganizationConstants.NAMESPACE + "/Battle/Status Effects/Status effect data")]
    public class StatusEffectSO : ScriptableObject
    {
        [SerializeField] private string m_EffectId;
        [SerializeField] private string m_EffectName;
        [SerializeField] private string m_EffectDesc;
        [SerializeField] private Sprite m_Icon;
        [SerializeField] private EEffectType m_EffectType;

        [Tooltip("Decay/Discard type")]
        [SerializeField] private EDecayType m_DiscardType;
        [SerializeField] private EEffectApplyType m_ApplyType;
        [SerializeField] private EStatusEffectCategory m_Category;
        [SerializeField] private bool m_StackableValue;
        [Header("Discard by turn count (DOT)")]
        [SerializeField] private int m_AliveTurnCount;

        [SerializeField] private List<StatusEffectSO> m_EffectList;
        

        [Header("Modifier value settings")]
        [SerializeField] private List<CombatStatModifierSO> m_Modifiers;

        [SerializeField] private CustomVariable[] m_CustomVariables;

        [Header("Dev notation")]
        [SerializeField, TextArea] private string m_DevNote;

        public string EffectId => m_EffectId;
        public string EffectName => m_EffectName;
        public string EffectDesc => m_EffectDesc;
        public Sprite Icon => m_Icon;
        public EDecayType DecayType => m_DiscardType;
        public EEffectApplyType ApplyType => m_ApplyType;
        public EStatusEffectCategory Category => m_Category;
        public int AliveTurnCount => m_AliveTurnCount;
        public bool StackableValue => m_StackableValue;

        public CombatStatModifierSO[] Modifiers => m_Modifiers.ToArray();
        public EEffectType EffectType => m_EffectType;
        public CustomVariable[] CustomVariables => m_CustomVariables;

        public IReadOnlyList<StatusEffectSO> EffectList => m_EffectList;

        /// <summary>
        /// Get custom/external variable.
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public CustomVariable GetVariableByName(string varName)
        {
            if (m_CustomVariables == null || m_CustomVariables.Length == 0)
            {
                return null;
            }
            return CustomVariables.FirstOrDefault(v => v.VariableName.Equals(varName));
        }
    }
}