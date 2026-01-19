namespace SEP490G69
{
    using UnityEngine;

    /// <summary>
    /// Handle business logic of shop.
    /// Logic includes:
    /// - Get all items.
    /// - Buy item.
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        private EventManager _eventManager;

        private void Awake()
        {
            _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();
        }

        public ItemDataHolder[] GetAllItems()
        {
            return null;
        }
        public void BuyItem(string itemId, int amount)
        {

        }
        public void SellItem(string itemId, int amount)
        {

        }
    }
}