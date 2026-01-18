using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GameContextManager : MonoBehaviour
{
    private List<ISceneContext> _contextList = new List<ISceneContext>();

    private void Awake()
    {
        gameObject.name = "DDOL_ContextManager";
        DontDestroyOnLoad(gameObject);
    }

    public void AddContext<T>(T context) where T : ISceneContext
    {
        if (_contextList.Contains(context)) return;

        _contextList.Add(context);
    }
    public void RemoveContext<T>(T context) where T : ISceneContext
    {
        if (!_contextList.Contains(context))
        {
            return;
        }
        _contextList.Remove(context);
    }
    public bool TryGetContext<T>(out T context) where T : ISceneContext
    {
        foreach (var c in _contextList)
        {
            if (c is T _context)
            {
                context = _context;
                return true;
            }
        }
        context = default(T);
        return false;
    }
}
