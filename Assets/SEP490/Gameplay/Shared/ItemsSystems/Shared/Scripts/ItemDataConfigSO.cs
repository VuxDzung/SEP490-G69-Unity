namespace SEP490G69
{
    using UnityEngine;

    [CreateAssetMenu]
    public class ItemDataConfigSO : ScriptableObject
    {
        [SerializeField] private ItemDataSO[] m_Items;

        public ItemDataSO GetItem(string itemID)
        {
            foreach (var item in m_Items)
            {
                if (item.ItemID.Equals(itemID))
                {
                    return item;
                }
            }
            return null;
        }
    }
}