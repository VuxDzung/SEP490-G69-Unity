namespace SEP490G69
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "CharacterVFXConfig")]
    public class CharacterVFXConfigSO : ScriptableObject
    {
        [SerializeField] private List<VFXData> m_VfxList = new List<VFXData>();

        public IReadOnlyList<VFXData> VFXList => m_VfxList;

        public VFXData GetById(string vfxId)
        {
            if (string.IsNullOrEmpty(vfxId)) return null;

            return m_VfxList.FirstOrDefault(vfx => vfx.vfxId == vfxId);
        }
    }

    [System.Serializable]
    public class VFXData
    {
        public string vfxId;
        public Transform vfxTransform;
    }
}