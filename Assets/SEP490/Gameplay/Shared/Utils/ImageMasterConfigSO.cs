namespace SEP490G69
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ImageMasterConfig")]
    public class ImageMasterConfigSO : ScriptableObject
    {
        [SerializeField] private List<ImageConfigSO> m_Categories;

        public IReadOnlyList<ImageConfigSO> Categories => m_Categories;

        public ImageConfigSO GetCategoryConfig(string category)
        {
            if (string.IsNullOrEmpty(category) || m_Categories.Count == 0) return null;

            return m_Categories.FirstOrDefault(c => c.Category == category);
        }

        public ImageData GetImage(string category, string id)
        {
            ImageConfigSO config = GetCategoryConfig(category);

            if (config == null) return null;

            return config.GetById(id);
        }
    }
}