namespace SEP490G69.Tournament
{
    using UnityEngine;
    using System.Collections.Generic;
    using SEP490G69.Battle.Combat;

    [CreateAssetMenu(fileName = "EnemySO", menuName = OrganizationConstants.NAMESPACE + "/Characters/Enemy data")]
    public class EnemySO : BaseCharacterSO 
    {
        [Header("Enemy Specifics")]
        [Header("Enemy Stats")]
        [SerializeField] private float m_EnemyAttackStat;
        [SerializeField] private float m_EnemyMaxHP;
        [SerializeField] private float m_EnemyINT;
        [SerializeField] private float m_EnemyDEF;

        [Header("Enemy Intent Pattern")]
        [SerializeField] private EnemyIntentPatternSO m_IntentPattern;

        [Header("Deprecated Fields")]
        [Header("Combat Deck")]
        [SerializeField] private EnemyDeckSO m_EnemyDeck;

        public IReadOnlyList<string> Deck => m_EnemyDeck.CardIdList;

        public float TotalStats => BaseVit + BasePow + BaseAgi + BaseInt + BaseSta;

        public float EnemyAttack => m_EnemyAttackStat;
        public float EnemyMaxHP => m_EnemyMaxHP;
        public float EnemyINT => m_EnemyINT;
        public float EnemyDEF => m_EnemyDEF;

        public IReadOnlyList<EnemyIntentSO> IntentList => m_IntentPattern.Intents;
    }
}