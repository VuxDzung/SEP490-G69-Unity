namespace SEP490G69.Battle.Cards
{
    using System.Collections.Generic;
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

        [Header("Dmg changer")]
        [SerializeField] private EActionType m_ActionType;
        [SerializeField] private float m_BaseDmg;
        [SerializeField] private EStatusType m_ModifyStatType;
        [SerializeField] private EOperator m_DmgModOp;
        [SerializeField] private float dmgModifierValue;
        [SerializeField] private int m_AtkCount;
        [SerializeField] private EDamageType m_DamageType;

        [Header("Recovery modifiers")]
        [SerializeField] private List<CombatStatModifierSO> m_RecoverModifiers;

        [Header("Status effects")]
        /// <summary>
        /// Status effects apply for owner
        /// </summary>
        [SerializeField] private List<StatusEffectSO> m_StatusGains;

        /// <summary>
        /// Status effects apply for enemy
        /// </summary>
        [SerializeField] private List<StatusEffectSO> m_StatusInflicts;

        [Header("Extra actions")]
        [SerializeField] private List<ExtraActionData> m_ExtraActions;

        public string CardId => cardId;
        public string CardName => cardName;
        public string CardDescription => cardDescription;
        public Sprite Icon => icon;
        public int Cost => cost;
        public EActionType ActionType => m_ActionType;
        public float BaseValue => m_BaseDmg;
        public EStatusType ModifyStatType => m_ModifyStatType;
        public EOperator ModifyOp => m_DmgModOp;
        public float ModifierValue => dmgModifierValue;
        public int AtkCount => m_AtkCount;  
        public EDamageType DamageType => m_DamageType;

        public CombatStatModifierSO[] RecoverModifiers => m_RecoverModifiers.ToArray();

        public StatusEffectSO[] StatusGains => m_StatusGains.ToArray();
        public StatusEffectSO[] StatusInflicts => m_StatusInflicts.ToArray();

        public ExtraActionData[] ExtraActions => m_ExtraActions.ToArray();
    }

    [System.Serializable]
    public class ExtraActionData
    {
        [Header("Action settings")]
        [SerializeField] private EExtraAction m_ExtraAction;
        [SerializeField] private ETargetType m_Target;
        [SerializeField] private EApplyDiscardType m_ApplyDiscardType;

        [Header("Value modify settings")]
        [SerializeField] private EStatusType m_BaseModStat;
        [SerializeField] private EOperator m_ModOp;
        [SerializeField] private float m_ModValue;

        public EExtraAction ExtraAction => m_ExtraAction;
        public ETargetType Target => m_Target;
        public EApplyDiscardType ApplyDiscardType => m_ApplyDiscardType;

        public EStatusType StatusType => m_BaseModStat;
        public EOperator ModOp => m_ModOp;
        public float ModValue => m_ModValue;
    }

    [System.Serializable]
    public class ActionCondition
    {
        [SerializeField] private ECompareOperator m_CompareOp;

        [Header("Opponent")]
        [SerializeField] private EStatusType m_EnemyStat;

        [Header("Seft")]
        [SerializeField] private EStatusType m_SelfStat;

        public ECompareOperator CompareOp => m_CompareOp;
        public EStatusType EnemyComparableStat => m_EnemyStat;
        public EStatusType SelftComparableStat => m_SelfStat;
    }

    public enum ECompareOperator
    {
        None = 0,
        Equal = 0,
        NotEqual = 0,
        GreaterThan = 0,
        GreaterThanOrEqual = 0,
        LessThan = 0,
        LessThanOrEqual = 0,
    }
}