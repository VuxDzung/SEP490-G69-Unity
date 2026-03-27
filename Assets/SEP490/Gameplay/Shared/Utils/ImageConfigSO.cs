namespace SEP490G69
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "image_config_")]
    public class ImageConfigSO : ScriptableObject
    {
        [SerializeField] private string m_Category;
        [SerializeField] private List<ImageData> m_Images;

        public string Category => m_Category;
        public IReadOnlyList<ImageData> Images => m_Images;

        public ImageData GetById(string imageId)
        {
            if (string.IsNullOrEmpty(imageId))
            {
                return null;
            }
            return m_Images.FirstOrDefault(img => img.id == imageId);
        }
    }

    [System.Serializable]
    public class ImageData
    {
        public string id;
        public Sprite image;
    }
}