namespace SEP490G69.Tournament
{
    using UnityEngine;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = "EnemySO", menuName = OrganizationConstants.NAMESPACE + "/Characters/Enemy data")]
    public class EnemySO : BaseCharacterSO 
    {
        [Header("Enemy Specifics")]
        [Header("Combat Deck")]
        //[SerializeField] private List<string> m_Deck;
        [SerializeField] private EnemyDeckSO m_EnemyDeck;

        public IReadOnlyList<string> Deck => m_EnemyDeck.CardIdList;

        public float TotalStats => BaseVit + BasePow + BaseAgi + BaseInt + BaseSta;
    }
}