namespace SEP490G69
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "GameContextConfig", menuName = OrganizationConstants.NAMESPACE + "/Context Framework/Game Context Config")]
    public class GameContextConfigSO : ScriptableObject
    {
        [SerializeField] private GameObject[] m_GameContexts;

        public GameObject[] GameContexts => m_GameContexts;
    }
}