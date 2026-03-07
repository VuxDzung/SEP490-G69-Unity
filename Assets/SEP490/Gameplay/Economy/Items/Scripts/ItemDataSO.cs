namespace SEP490G69.Economy
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ItemData_",menuName = OrganizationConstants.NAMESPACE + "/Items/Item data")]
    public class ItemDataSO : ScriptableObject
    {
        [Header("General Information")]
        [SerializeField] private string m_ItemID;
        [SerializeField] private string m_ItemNameKey;
        [SerializeField] private string m_ItemDecsKey;
        [SerializeField] private Sprite m_ItemImage;
        [SerializeField] private EItemType m_ItemType;

        [Header("Currency Settings")]
        [SerializeField] private string m_CurrencyID;
        [SerializeField] private int m_Cost;

        [Header("Relic fields")]
        [Tooltip("When the player equip the relic, all the stats in combat changes based on the relic modifiers. " +
                 "When unequip the relic, those stats changes are reverted as well")]
        [SerializeField] private List<StatusModifierSO> m_RelicModifiers;

        public string ItemID => m_ItemID;
        public string ItemNameKey => m_ItemNameKey;
        public string ItemDescKey => m_ItemDecsKey;
        public Sprite ItemImage => m_ItemImage;
        public EItemType ItemType => m_ItemType;

        public string CurrencyID => m_CurrencyID;
        public int Cost => m_Cost;

        /// <summary>
        /// When the player equip the relic, all the stats in combat changes based on the relic modifiers.
        /// When unequip the relic, those stats changes are reverted as well.
        /// </summary>
        public IReadOnlyList<StatusModifierSO> RelicModifiers => m_RelicModifiers;
    }
}