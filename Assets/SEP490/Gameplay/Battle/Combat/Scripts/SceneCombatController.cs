namespace SEP490G69.Battle.Combat
{
    using UnityEngine;

    public class SceneCombatController : MonoBehaviour, ISceneContext
    {
        [SerializeField] private Transform m_PlayerContainer;
        [SerializeField] private Transform m_EnemyContainer;

        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);
        }
        private void OnDestroy()
        {
            ContextManager.Singleton.RemoveSceneContext(this);
        }

        public void InitializeBattle()
        {

        }

        public void StartBattle()
        {

        }
    }
}