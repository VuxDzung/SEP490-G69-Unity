namespace SEP490G69.Authentication 
{ 
    using UnityEngine;

    [CreateAssetMenu(fileName = "GoogleClientConfig")]
    public class GoogleClientConfig : ScriptableObject
    {
        [SerializeField] private string m_ClientId;

        public string ClientId => m_ClientId;
    }
}