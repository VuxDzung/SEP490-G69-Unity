namespace SEP490G69.Authentication 
{ 
    using UnityEngine;

    [CreateAssetMenu(fileName = "GoogleClientConfig")]
    public class GoogleClientConfig : ScriptableObject
    {
        [SerializeField] private string m_ClientId;
        [SerializeField] private string m_RedirectUri;
        [SerializeField] private string m_Scope;

        public string ClientId => m_ClientId;
        public string RedirectUri => m_RedirectUri;
        public string Scope => m_Scope;
    }
}