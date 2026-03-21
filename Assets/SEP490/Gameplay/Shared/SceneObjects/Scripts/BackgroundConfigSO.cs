namespace SEP490G69
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "BackgroundConfig")]
    public class BackgroundConfigSO : ScriptableObject
    {
        [SerializeField] private List<BackgroundData> m_BgList = new List<BackgroundData>();

        public IReadOnlyList<BackgroundData> BackgroundList => m_BgList;

        public BackgroundData GetById(string bgId)
        {
            if (string.IsNullOrEmpty(bgId))
            {
                return null;
            }
            return m_BgList.FirstOrDefault(bg => bg.BGId == bgId);
        }
    }

    [System.Serializable]
    public class BackgroundData
    {
        [SerializeField] private string backgroundId;
        [SerializeField] private Sprite backgroundSprite;
        [SerializeField] private Vector3 bgScale;

        public string BGId => backgroundId;
        public Sprite BgSprite => backgroundSprite;
        public Vector3 BGScale => bgScale;
    }
}