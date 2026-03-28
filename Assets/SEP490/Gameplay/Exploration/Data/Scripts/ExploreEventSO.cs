namespace SEP490G69.Exploration
{
    using System.Collections.Generic;
    using UnityEngine;

    public enum EExploreEventType
    {
        None = 0,
        Boss = 1,
        Encounter = 2,
        Chest = 3,
        Combat = 4,
    }

    public enum EExploreEventOutcome
    {
        None = 0,
        Success = 1,
        Failed = 2,
        Combat = 3,
    }

    [CreateAssetMenu(fileName = "WW_")]
    public class ExploreEventSO : GameEventSO
    {
        [SerializeField] private EExploreEventType m_ExploreEventType;
        [SerializeField] private List<ExploreEventChoiceData> m_Choices;

        public EExploreEventType ExploreEventType => m_ExploreEventType;    
        public IReadOnlyList<ExploreEventChoiceData> Choices => m_Choices;
    }

    [System.Serializable]
    public class ExploreEventChoiceData
    {
        [TextArea]
        [SerializeField] private string choiceName;
        [SerializeField] private ChoiceSuccessCondition m_Condition;
        [SerializeField] private List<ExploreEventOutcomeData> m_Outcomes;

        public string ChoicesName => choiceName;
        public ChoiceSuccessCondition Condition => m_Condition;
        public IReadOnlyList<ExploreEventOutcomeData> Outcomes => m_Outcomes;
    }

    [System.Serializable]
    public class ExploreEventOutcomeData
    {
        [SerializeField] private EExploreEventOutcome outcomeType;
        [TextArea]
        [SerializeField] private string outcomeMsg;

        [Header("Combat")]
        [SerializeField] private bool m_RandomEnemyFromPool;
        [SerializeField] private string m_EnemyId;

        [Header("Rewards")]
        [SerializeField] private ERewardPenaltyOrder m_DisplayRewardOrder;
        [SerializeField] private List<RewardDataSO> m_RewardList;
        [SerializeField] private bool m_GetMoreFromPool;

        [SerializeField] private List<PoolRewardData> m_RewardPoolList;

        [Header("Penaty modifiers")]
        [SerializeField] private ERewardPenaltyOrder m_DisplayPenaltyOrder;
        [SerializeField] private List<StatusModifierSO> m_PenatyModifiers;

        [Header("[Deprecated]")]
        [Tooltip("[Deprectated] Configuring in RewardPoolList instead.")]
        [SerializeField] private string m_PoolId;
        [Tooltip("[Deprectated] Configuring in RewardPoolList instead.")]
        [SerializeField] private int m_PoolRewardAmount;

        public EExploreEventOutcome OutcomeType => outcomeType;
        public string OutcomeMsg => outcomeMsg;
        public bool RandomEnemyFromPool => m_RandomEnemyFromPool; 
        public string EnemyId => m_EnemyId;
        public bool GetMoreFromPool => m_GetMoreFromPool;
        //public string PoolId => m_PoolId;
        //public int PoolRewardAmount => m_PoolRewardAmount;  
        public IReadOnlyList<PoolRewardData> RewardPoolList => m_RewardPoolList;
        public ERewardPenaltyOrder DisplayRewardOrder => m_DisplayRewardOrder;
        public IReadOnlyList<RewardDataSO> Rewards => m_RewardList;
        public ERewardPenaltyOrder DisplayPenaltyOrder => m_DisplayPenaltyOrder;
        public IReadOnlyList<StatusModifierSO> PenatyModifiers => m_PenatyModifiers;
    }

    [System.Serializable]
    public class ChoiceSuccessCondition
    {
        [SerializeField] private EChoiceSuccessType m_SuccessCondition;

        [Header("Stat condition")]
        [SerializeField] private EStatusType m_ConditionStat;
        [SerializeField] private float m_RequiredValue;

        [Header("Random percent")]
        [SerializeField] private float m_RandomValue;

        public EChoiceSuccessType SuccessCondition => m_SuccessCondition;
        public float RequiredValue => m_RequiredValue;
        public EStatusType ConditionStat => m_ConditionStat;
        public float RandomValue => m_RandomValue;
    }

    public enum EChoiceSuccessType
    {
        None = 0,
        ByStatus = 1,
        Random = 2,
    }

    public enum ERewardPenaltyOrder
    {
        Immedite = 0,
        DisplayBeforeBattle = 1,
        DisplayAfterBattle = 2,
    }

    [System.Serializable]
    public class PoolRewardData
    {
        [SerializeField] private string m_PoolId;
        [SerializeField] private int m_AmountFromPool;

        public string PoolId => m_PoolId;
        public int AmountFromPool => m_AmountFromPool;
    }
}