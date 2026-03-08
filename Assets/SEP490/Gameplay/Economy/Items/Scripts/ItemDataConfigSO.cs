namespace SEP490G69.Economy
{
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ItemConfig", menuName = OrganizationConstants.NAMESPACE + "/Items/Item config")]
    public class ItemDataConfigSO : ScriptableObject
    {
        [SerializeField] private ItemDataSO[] m_Items;

        public ItemDataSO[] Items => m_Items;

        public ItemDataSO GetItemById(string itemID)
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

        public ItemDataSO[] GetItemsByType(EItemType itemType)
        {
            if (!m_Items.Any())
            {
                return null;
            }
            return m_Items.Where(itm => itm.ItemType == itemType).ToArray();
        }
    }
}