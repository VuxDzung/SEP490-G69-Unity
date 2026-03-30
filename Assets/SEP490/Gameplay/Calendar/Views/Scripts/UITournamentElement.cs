namespace SEP490G69.Calendar
{
    using SEP490G69.Tournament;
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITournamentElement : MonoBehaviour, IPooledObject
    {
        private Action<string> _onClick;
        [SerializeField] private Button m_DetailsBtn;
        [SerializeField] private TextMeshProUGUI m_TournamentNameTmp;
        [SerializeField] private TextMeshProUGUI m_RequiredRankTmp;

        [Header("Objective Settings")]
        [SerializeField] private GameObject m_ObjectiveGO; // <-- KÉO CỤC GOAL / OBJECTIVE VÀO ĐÂY

        [Header("Reward Settings")]
        [SerializeField] private Transform m_RewardContainer;
        [SerializeField] private Transform m_NumericRewardPrefab;
        [SerializeField] private Transform m_ItemRewardPrefab;
        [SerializeField] private Transform m_CardRewadPrefab;

        private string _id;

        public void Spawn()
        {
            m_DetailsBtn.onClick.AddListener(Click);
        }
        public void Despawn()
        {
            m_DetailsBtn.onClick.RemoveListener(Click);
        }

        public UITournamentElement SetOnClickDetails(Action<string> onClick)
        {
            _onClick = onClick;
            return this;
        }
        public UITournamentElement SetId(string id)
        {
            _id = id;
            return this;
        }

        // ĐÃ THÊM THAM SỐ bool isObjective VÀO HÀM NÀY
        public void SetContent(string tournamentName, string requiredRank, IReadOnlyList<RewardDataSO> rewards, bool isObjective)
        {
            m_TournamentNameTmp.text = tournamentName;
            m_RequiredRankTmp.text = requiredRank;

            // Bật / tắt cục Goal tùy thuộc vào việc giải này có phải Objective không
            if (m_ObjectiveGO != null)
            {
                m_ObjectiveGO.SetActive(isObjective);
            }

            if (rewards == null || rewards.Count == 0) return;

            LoadTop1Rewards(rewards);
        }

        private void LoadTop1Rewards(IReadOnlyList<RewardDataSO> rewards)
        {
            foreach (var rewardSO in rewards)
            {
                if (rewardSO == null) continue;

                if (rewardSO.RewardType != ERewardType.Gold &&
                    rewardSO.RewardType != ERewardType.ReputationPoint)
                {
                    continue;
                }

                Transform prefab = GetPrefabByRewardType(rewardSO.RewardType);

                if (prefab == null)
                {
                    Debug.LogWarning($"No prefab defined for reward type {rewardSO.RewardType}");
                    continue;
                }

                Transform rewardTrans = PoolManager.Pools["UIRewardPreview"].Spawn(prefab, m_RewardContainer);

                SetupRewardUI(rewardTrans, rewardSO);
            }
        }

        private Transform GetPrefabByRewardType(ERewardType type)
        {
            switch (type)
            {
                case ERewardType.Gold:
                case ERewardType.Stats:
                case ERewardType.ReputationPoint:
                    return m_NumericRewardPrefab;

                case ERewardType.Item:
                    return m_ItemRewardPrefab;

                case ERewardType.Card:
                    return m_CardRewadPrefab;

                default:
                    return null;
            }
        }

        private void SetupRewardUI(Transform rewardTrans, RewardDataSO rewardSO)
        {
            UIRewardPreview preview = rewardTrans.GetComponent<UIRewardPreview>();

            if (preview == null)
            {
                Debug.LogWarning("UIRewardPreview component missing.");
                return;
            }

            preview.SetContent(rewardSO.RewardTargetId, rewardSO.RewardType, rewardSO.RewardAmount);
        }

        private void Click()
        {
            Debug.Log($"Tournament {_id} clicked.");
            _onClick?.Invoke(_id);
        }
    }
}