namespace SEP490G69.Tournament
{
    using UnityEngine;
    using System.Collections.Generic;
    using SEP490G69.Battle.Cards;

    [CreateAssetMenu(fileName = "EnemySO", menuName = OrganizationConstants.NAMESPACE + "/Tournaments/Enemy data")]
    public class EnemySO : BaseCharacterSO 
    {

        [Header("Enemy Specifics")]
        [Header("Combat Deck")]
        [SerializeField] private List<string> m_Deck;

        public IReadOnlyList<string> Deck => m_Deck;

        public float TotalStats => BaseVit + BasePow + BaseAgi + BaseInt + BaseSta;
    }
}