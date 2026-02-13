namespace SEP490G69
{
    using UnityEngine;
    public abstract class NarrativeActionHandlerBase : INarrativeActionHandler
    {
        protected readonly ContextManager _contextManager;

        protected NarrativeActionHandlerBase(ContextManager contextManager)
        {
            this._contextManager = contextManager;
        }

        public abstract string ActionId { get; }
        public abstract void Execute(DialogEvent ev);
    }
}