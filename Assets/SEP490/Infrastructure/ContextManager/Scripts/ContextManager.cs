namespace SEP490G69
{
    using System.Collections.Generic;
    using System.Collections;
    using UnityEngine;

    [DefaultExecutionOrder(-1000)]
    public class ContextManager : GlobalSingleton<ContextManager>
    {
        [SerializeField] private GameContextConfigSO m_GameContextConfig;

        
        private List<IGameContext> _gameContextList = new List<IGameContext>();
        private List<ISceneContext> _sceneContextList = new List<ISceneContext>();

        protected override void CreateNewInstance()
        {
            base.CreateNewInstance();

            if (m_GameContextConfig == null)
            {
                m_GameContextConfig = Resources.Load<GameContextConfigSO>("Contexts/GameContextConfig");
            }

            foreach (var context in m_GameContextConfig.GameContexts)
            {
                GameObject contextGO = Instantiate(context, transform);
                IGameContext _context = contextGO.GetComponent<IGameContext>();

                if (_context == null) continue;
                contextGO.name = $"[Context] {context.name}";
                _gameContextList.Add(_context);
                _context.SetManager(this);
            }
        }

        public void AddSceneContext<T>(T context) where T : ISceneContext
        {
            if (_sceneContextList.Contains(context)) return;

            _sceneContextList.Add(context);
        }
        public void RemoveSceneContext<T>(T context) where T : ISceneContext
        {
            if (!_sceneContextList.Contains(context))
            {
                return;
            }
            _sceneContextList.Remove(context);
        }
        public T GetSceneContext<T>() where T : ISceneContext
        {
            foreach (var context in _sceneContextList)
            {
                if (context.GetType() == typeof(T))
                {
                    return (T)context;
                }
            }
            return default(T);
        }
        public bool TryResolveSceneContext<T>(out T context) where T : ISceneContext
        {
            foreach (var c in _sceneContextList)
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

        public void RegisterGameContext<T>(GameObject contextObj) where T : IGameContext
        {
            T existed = ResolveGameContext<T>();
            if (existed != null)
            {
                return;
            }
            existed = contextObj.GetComponent<T>();
            if (existed == null)
            {
                return;
            }
            contextObj.transform.SetParent(this.transform, false);
            contextObj.transform.localPosition = Vector3.zero;
            contextObj.name = $"[Context] {contextObj.name}";
            _gameContextList.Add(existed);
        }

        public T ResolveGameContext<T>() where T : IGameContext
        {
            foreach (var context in _gameContextList)
            {
                if (context.GetType() == typeof(T))
                {
                    return (T)context;
                }
            }
            return default(T);
        }

        public T GetDataSO<T>() where T : ScriptableObject
        {
            return m_GameContextConfig.GetDataSO<T>();
        }
    }
}