namespace SEP490G69.Battle.Cards
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "StatusEffect_", menuName = OrganizationConstants.NAMESPACE + "/Battle/Cards/Status effect data")]
    public class StatusEffectSO : ScriptableObject
    {
        [SerializeField] private string m_EffectId;
        [SerializeField] private string m_EffectName;
        [SerializeField] private string m_EffectDesc;
        [SerializeField] private Sprite m_Icon;
        [SerializeField] private EApplyDiscardType m_ApplyType;
        [SerializeField] private EEffectType m_EffectType;
        [Header("Discard by turn count")]
        [SerializeField] private int m_AliveTurnCount;
        

        [Header("Modifier value settings")]
        [SerializeField] private List<CombatStatModifierSO> m_Modifiers;

        [Header("Dev notation")]
        [SerializeField, TextArea] private string m_DevNote;

        public string EffectId => m_EffectId;
        public string EffectName => m_EffectName;
        public string EffectDesc => m_EffectDesc;
        public Sprite Icon => m_Icon;
        public EApplyDiscardType ApplyType => m_ApplyType;
        public int AliveTurnCount => m_AliveTurnCount;

        public CombatStatModifierSO[] Modifiers => m_Modifiers.ToArray();
        public EEffectType EffectType => m_EffectType;
    }
}