namespace SEP490G69
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "TestEnemyIdSO")]
    public class TestEnemyIdSO : ScriptableObject
    {
        [SerializeField] private string m_EnemyId;

        public string EnemyId => m_EnemyId;
    }
}