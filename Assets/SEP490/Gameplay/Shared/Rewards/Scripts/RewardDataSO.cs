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
        [SerializeField] private string m_Id;
        [SerializeField] private string m_RewardName;
        [SerializeField] private ERewardType m_RewardType;
        [Header("Non-Stats reward")]
        [Tooltip("Can be the item id or card id or relic id.")]
        [SerializeField] private string m_RewardTargetId;
        [SerializeField] private int m_RewardAmount;
        [Header("Stat rewards")]
        [SerializeField] private List<StatusModifierSO> m_StatModifiers;

        public string Id => m_Id;
        public string RewardName => m_RewardName;
        public ERewardType RewardType => m_RewardType;
        /// <summary>
        /// Can be the item id or card id or relic id.
        /// </summary>
        public string RewardTargetId => m_RewardTargetId;
        public int RewardAmount => m_RewardAmount;
        public IReadOnlyList<StatusModifierSO> StatModifiers => m_StatModifiers;
    }
}