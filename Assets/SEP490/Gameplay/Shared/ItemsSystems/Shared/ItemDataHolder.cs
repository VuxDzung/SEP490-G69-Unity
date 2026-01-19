namespace SEP490G69
{
    using UnityEngine;

    /// <summary>
    /// This is a holder for both hard-data (in-editor data) and runtime data
    /// </summary>
    [System.Serializable]
    public class ItemDataHolder 
    {
        private ItemData _data;
        private ItemDataSO _dataSO;

        public ItemDataHolder(ItemData data, ItemDataSO dataSO)
        {
            _data = data;
            _dataSO = dataSO;
        }
    }
}