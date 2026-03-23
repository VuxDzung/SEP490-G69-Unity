namespace SEP490G69.Battle.Cards
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "CardSO_", menuName = OrganizationConstants.NAMESPACE + "/Cards/Card data")]
    public class CardSO : ScriptableObject
    {
        [Header("Info")]
        [SerializeField] private string cardId;
        [SerializeField] private string cardName;
        [SerializeField] private string cardDescription;
        [SerializeField] private Sprite icon;
        [SerializeField] private int cost;
        [SerializeField] private bool m_IsReady;
        [SerializeField] private bool m_Stackable;

        [Header("Dmg changer")]
        [SerializeField] private EActionType m_ActionType;
        [SerializeField] private float m_BaseDmg;
        [SerializeField] private EStatusType m_ModifyStatType;
        [SerializeField] private EOperator m_DmgModOp;
        [SerializeField] private float dmgModifierValue;
        [SerializeField] private int m_AtkCount;
        [SerializeField] private EDamageType m_DamageType;

        [Header("Stat modifiers")]
        [SerializeField] private List<CombatStatModifierSO> m_PreStatModifiers;
        [SerializeField] private List<CombatStatModifierSO> m_PostStatModifiers;

        [Header("Status effects")]
        /// <summary>
        /// Status effects apply for owner
        /// </summary>
        [SerializeField] private List<StatusEffectSO> m_StatusGains;

        /// <summary>
        /// Status effects apply for enemy
        /// </summary>
        [SerializeField] private List<StatusEffectSO> m_StatusInflicts;

        [SerializeField] private string[] m_ExtraActions;
        [SerializeField] private List<CustomVariable> m_CustomVariables;

        [SerializeField] private string[] m_VfxIdArray;

        public string CardId => cardId;
        public string CardName => cardName;
        public string CardDescription => cardDescription;
        public Sprite Icon => icon;
        public int Cost => cost;
        public bool IsReady => m_IsReady;
        public bool Stackable => m_Stackable;

        public EActionType ActionType => m_ActionType;
        public float BaseValue => m_BaseDmg;
        public EStatusType ModifyStatType => m_ModifyStatType;
        public EOperator ModifyOp => m_DmgModOp;
        public float ModifierValue => dmgModifierValue;
        public int AtkCount => m_AtkCount;  
        public EDamageType DamageType => m_DamageType;
        public string[] ExtraActions => m_ExtraActions;

        /// <summary>
        /// Get modifier(s) before performing the attack action.
        /// </summary>
        public CombatStatModifierSO[] PreStatModifiers => m_PreStatModifiers.ToArray();

        /// <summary>
        /// Get modifier(s) after attack.
        /// </summary>
        public CombatStatModifierSO[] PostStatModifiers => m_PostStatModifiers.ToArray();

        /// <summary>
        /// Status effect(s) gain for self.
        /// </summary>
        public StatusEffectSO[] StatusGains => m_StatusGains.ToArray();

        /// <summary>
        /// Status effects assigned for the opponent.
        /// </summary>
        public StatusEffectSO[] StatusInflicts => m_StatusInflicts.ToArray();

        public IReadOnlyList<CustomVariable> CustomVariables => m_CustomVariables;

        public string[] VfxIdArray => m_VfxIdArray;

        /// <summary>
        /// Get custom/external variable.
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public CustomVariable GetVariableByName(string varName)
        {
            if (m_CustomVariables == null || m_CustomVariables.Count == 0)
            {
                return null;
            }
            return CustomVariables.FirstOrDefault(v => v.VariableName.Equals(varName));
        }

        public float GetDelta(float targetValue)
        {
            switch (m_DmgModOp)
            {
                case EOperator.PercentAdd:
                    return targetValue * m_BaseDmg;

                case EOperator.PercentSub:
                    return -targetValue * dmgModifierValue;

                case EOperator.FlatAdd:
                    return dmgModifierValue;

                case EOperator.FlatSub:
                    return -dmgModifierValue;
                case EOperator.Mul:
                    return targetValue * dmgModifierValue;
            }

            return 0f;
        }

        public List<StatusEffectSO> GetAllStatusEffects()
        {
            List<StatusEffectSO> effectList = new List<StatusEffectSO>();
            effectList.AddRange(StatusGains);
            effectList.AddRange(StatusInflicts);

            return effectList;
        }
    }

    public enum ECompareOperator
    {
        None = 0,
        Equal = 1,
        NotEqual = 2,
        GreaterThan = 3,
        GreaterThanOrEqual = 4,
        LessThan = 5,
        LessThanOrEqual = 6,
    }
}