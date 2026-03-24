namespace SEP490G69.Training
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "TrainingAnimationConfig")]
    public class TrainingAnimationConfigSO : ScriptableObject
    {
        [SerializeField] private List<TrainingAnimData> m_AnimationList;

        public TrainingAnimData GetById(string characterId)
        {
            if (string.IsNullOrEmpty(characterId) ||
                m_AnimationList.Count == 0)
            {
                return null;
            }

            return m_AnimationList.FirstOrDefault(a => a.characterId == characterId);
        }
    }

    [System.Serializable]
    public class TrainingAnimData
    {
        public string characterId;
        public Transform prefab;
    }
}