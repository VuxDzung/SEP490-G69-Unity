namespace SEP490G69
{
    public interface INarrativeActionHandler
    {
        string ActionId { get; }
        void Execute(DialogEvent ev);
    }
}