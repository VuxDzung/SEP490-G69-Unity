namespace SEP490G69
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ItemData_",menuName = OrganizationConstants.NAMESPACE + "/Items/Item data")]
    public class ItemDataSO : ScriptableObject
    {
        [SerializeField] private string m_ItemID;
        [SerializeField] private Sprite m_ItemImage;

        public string ItemID => m_ItemID;
        public Sprite ItemImage => m_ItemImage;
    }
}