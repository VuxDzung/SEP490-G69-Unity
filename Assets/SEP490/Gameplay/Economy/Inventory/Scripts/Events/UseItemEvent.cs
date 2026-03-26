namespace SEP490G69.Economy
{
    using UnityEngine;

    public class UseItemEvent : IEvent
    {
        public ItemDataHolder ItemData {  get; set; }
    }
}