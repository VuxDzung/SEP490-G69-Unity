namespace SEP490G69.Economy
{
    using UnityEngine;

    public class AddItemEvent : IEvent
    {
        public ItemDataHolder ItemData {  get; set; }
    }
}