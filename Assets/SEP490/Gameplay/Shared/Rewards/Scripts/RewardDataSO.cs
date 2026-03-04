namespace SEP490G69
{
    using System.Collections.Generic;
    using UnityEngine;

    public enum ERewardType
    {
        None = 0,
        Gold = 1,
        ReputationPoint = 2,
        Stats = 3,
        Item = 4,
        Card = 5,
    }

    [CreateAssetMenu(fileName = "RewardSO_", menuName = OrganizationConstants.NAMESPACE + "/Shared/Reward data")]
    public class RewardDataSO : ScriptableObject
    {
        [SerializeField] private string m_RewardId;
        [SerializeField] private string m_RewardName;
        [SerializeField] private ERewardType m_RewardType;
        [Header("Non-Stats reward")]
        [SerializeField] private int m_RewardAmount;
        [Header("Stat rewards")]
        [SerializeField] private List<StatusModifierSO> m_StatModifiers;

        public string RewardId => m_RewardId;
        public string RewardName => m_RewardName;
        public ERewardType RewardType => m_RewardType;
        public int RewardAmount => m_RewardAmount;
        public IReadOnlyList<StatusModifierSO> StatModifiers => m_StatModifiers;
    }
}