namespace SEP490G69
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AudioConfig", menuName = OrganizationConstants.NAMESPACE + "/Shared/Audio config")]
    public class AudioConfigSO : ScriptableObject
    {
        [SerializeField] private List<AudioData> m_AudioList = new List<AudioData>();

        public IReadOnlyList<AudioData> AudioList => m_AudioList;

        public AudioClip GetById(string id)
        {
            if (m_AudioList.Count == 0)
            {
                return null;
            }
            return m_AudioList.FirstOrDefault(a => a.id.Equals(id))?.clip;
        }
    }

    [System.Serializable]
    public class AudioData
    {
        public string id;
        public AudioClip clip;
    }
}