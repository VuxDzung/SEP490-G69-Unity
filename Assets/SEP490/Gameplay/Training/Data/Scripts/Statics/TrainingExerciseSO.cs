namespace SEP490G69
{
    using System.Collections.Generic;
    using Unity.VisualScripting.Antlr3.Runtime.Misc;
    using UnityEngine;

    [System.Serializable]
    public struct TrainingRewardConfig
    {
        public StatusModifierSO Modifier;
        public float BonusPerLevel;
    }

    [CreateAssetMenu(fileName = "TrainingEx_", menuName = OrganizationConstants.NAMESPACE + "/Training Exercises/Exercise data")]
    public class TrainingExerciseSO : ScriptableObject
    {
        [SerializeField] private string exerciseId;
        [SerializeField] private string exerciseName;
        [SerializeField] private string exerciseDescription;
        [SerializeField] private Sprite exerciseIcon;
        [SerializeField] private bool canShowOnUI = true;

        [SerializeField] private List<TrainingRewardConfig> m_SuccessModifiers;
        [SerializeField] private List<TrainingRewardConfig> m_FailedModifiers;

        public string ExerciseId => exerciseId;
        public string ExerciseName => exerciseName;
        public string ExerciseDescription => exerciseDescription;
        public Sprite ExerciseIcon => exerciseIcon;
        public List<TrainingRewardConfig> SuccessModifiers => m_SuccessModifiers;
        public List<TrainingRewardConfig> FailedModifiers => m_FailedModifiers;
        public bool CanShowOnUI => canShowOnUI; 
    }
}