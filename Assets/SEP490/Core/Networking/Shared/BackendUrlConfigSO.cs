namespace SEP490G69.Addons.Networking
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "BackendUrlConfig", menuName = OrganizationConstants.NAMESPACE + "/Addons/Networking/Backend url config")]
    public class BackendUrlConfigSO : ScriptableObject
    {
        [SerializeField] private string m_BaseUrl;
        [SerializeField] private List<EndpointData> m_EndPoints;

        public string BaseUrl => m_BaseUrl;
        public EndpointData[] Endpoints => m_EndPoints.ToArray();
        public string GetEndpoint(string name)
        {
            EndpointData endpoint = m_EndPoints.FirstOrDefault(end => end.EndpointName.Equals(name));
            if (endpoint == null)
            {
                return "";
            }
            return endpoint.Url;
        }
    }

    [System.Serializable]
    public class EndpointData
    {
        [SerializeField] private string m_EndPointName;
        [SerializeField] private string m_EndPointUrl;
        public string EndpointName => m_EndPointName;
        public string Url => m_EndPointUrl;
    }
}