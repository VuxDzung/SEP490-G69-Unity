using SEP490G69;
using System.Linq;
using UnityEngine;

public class TestDialogSystem : MonoBehaviour
{
    [SerializeField] private string m_DialogTreeId;
    [SerializeField] private string m_DialogNodeId;

    private GameDialogManager _dialogManager;
    private EventManager _eventManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _dialogManager = ContextManager.Singleton.ResolveGameContext<GameDialogManager>();
        _eventManager = ContextManager.Singleton.ResolveGameContext<EventManager>();

        if (_dialogManager != null )
        {
            _dialogManager.StartTree(m_DialogTreeId, m_DialogNodeId);
        }
        _eventManager.Subscribe<DialogEvent>(DispatchDialogEvent);
    }

    private void OnDestroy()
    {
        _eventManager.Unsubscribe<DialogEvent>(DispatchDialogEvent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DispatchDialogEvent(DialogEvent ev)
    {
        Debug.Log($"Receiver: {ev.Receiver}\nAction: {ev.Action}\nParameters: {ev.Parameters.Select(p => p.ParamName)}");
    }
}
