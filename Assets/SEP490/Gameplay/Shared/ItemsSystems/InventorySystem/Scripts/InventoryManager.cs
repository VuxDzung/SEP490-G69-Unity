namespace SEP490G69
{
    using UnityEngine;

    /// <summary>
    /// Handle business logic of inventory.
    /// Logic includes:
    /// - Add item
    /// - Use item
    /// - Sell item
    /// </summary>
    public class InventoryManager : MonoBehaviour, ISceneContext
    {
        private EventManager _eventManager;
        private void Awake()
        {
            ContextManager.Singleton.AddSceneContext(this);
            _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();
        }

        public void AddItem(string itemId, int amount)
        {

        }
        public bool UseItem(string itemId, int amount)
        {
            return false;
        }
        public bool RemoveItem(string itemId, int amount)
        {
            return false;
        }
    }
}