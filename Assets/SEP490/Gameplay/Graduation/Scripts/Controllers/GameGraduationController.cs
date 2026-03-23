namespace SEP490G69
{
    using UnityEngine;

    public class GameGraduationController : MonoBehaviour, IGameContext
    {
        private IGraduationService _service;
        private ContextManager _contextManager;

        private EventManager _eventManager;

        public void SetManager(ContextManager manager)
        {
            _contextManager = manager;

            _eventManager = _contextManager.ResolveGameContext<EventManager>();
        }

        public void Graduate()
        {
            _eventManager.Publish(new GraduationEvent
            {
                FinalScore = 0
            });
        }
    }

    public class GraduationEvent : IEvent
    {
        public int FinalScore { get; set; }
    }
}