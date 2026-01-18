namespace SEP490G69
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "GameContextConfig", menuName = OrganizationConstants.NAMESPACE + "/Context Framework/Game Context Config")]
    public class GameContextConfigSO : ScriptableObject
    {
        [SerializeField] private GameObject[] m_GameContexts;
        [SerializeField] private ScriptableObject[] m_DataConfig;
        public GameObject[] GameContexts => m_GameContexts;
        public ScriptableObject[] DataConfig => m_DataConfig;
    }
}