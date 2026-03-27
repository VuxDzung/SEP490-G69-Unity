namespace SEP490G69.Exploration
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "reward_pool_")]
    public class ExplorePoolSO : ScriptableObject
    {
        [SerializeField] private string m_PoolId;
        [SerializeField] private ERewardType m_RewardCategory;
        [SerializeField] private string[] m_RewardIdArray;

        public string PoolId => m_PoolId;
        public ERewardType RewardCategory => m_RewardCategory;
        public string[] RewardIdArray => m_RewardIdArray;

        public string GetRandomElement()
        {
            if (m_RewardIdArray.Length == 0)
            {
                return string.Empty;
            }
            return m_RewardIdArray[Random.Range(0, m_RewardIdArray.Length)];
        }

        public List<string> GetRandomUniqueElements(int count)
        {
            List<string> result = new List<string>();

            if (m_RewardIdArray == null || m_RewardIdArray.Length == 0 || count <= 0)
            {
                return result;
            }

            // Clone array để không ảnh hưởng data gốc
            List<string> tempList = new List<string>(m_RewardIdArray);

            // Fisher-Yates shuffle
            for (int i = 0; i < tempList.Count; i++)
            {
                int randIndex = Random.Range(i, tempList.Count);
                (tempList[i], tempList[randIndex]) = (tempList[randIndex], tempList[i]);
            }

            // Lấy n phần tử đầu
            int takeCount = Mathf.Min(count, tempList.Count);
            for (int i = 0; i < takeCount; i++)
            {
                result.Add(tempList[i]);
            }

            return result;
        }
    }
}