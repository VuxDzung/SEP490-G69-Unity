namespace SEP490G69
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "PlayerCharacterSO_", menuName = OrganizationConstants.NAMESPACE + "/Characters/Playable Character data")]
    public class PlayerCharacterDataSO : BaseCharacterSO
    {
        [SerializeField] private List<StatusModifierSO> m_TrainingModifiers;

        [SerializeField] private List<string> m_UniqueCardId;

        public IReadOnlyList<StatusModifierSO> TrainingModifiers;

        public IReadOnlyList<string> UniqueCardId => m_UniqueCardId;

        public StatusModifierSO GetModifierByStatType(EStatusType statType)
        {
            if (m_TrainingModifiers == null || m_TrainingModifiers.Count == 0)
            {
                return null;
            }
            return m_TrainingModifiers.FirstOrDefault(mod => mod.StatType == statType); 
        }
    }
}