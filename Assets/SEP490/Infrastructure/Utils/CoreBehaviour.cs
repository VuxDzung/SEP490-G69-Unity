namespace SEP490G69
{
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class CoreBehaviour : MonoBehaviour
    {
        public string GameObjectName
        {
            get
            {
                return gameObject.name;
            }
            set
            {
                gameObject.name = value;
            }
        }

        public void Log(string logTitle, string logMessage = "", TextColor titleColor = TextColor.Green, TextColor messageColor = TextColor.White)
        {
            LoggerUtils.Logging(logTitle, logMessage, titleColor, messageColor);
        }

        //
        // Summary:
        //     Wrapper for Unity's GameObject.AddComponent()
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T AddBehaviour<T>() where T : Behaviour
        {
            return base.gameObject.AddComponent<T>();
        }

        //
        // Summary:
        //     Wrapper for Unity's GameObject.TryGetComponent()
        public bool TryGetBehaviour<T>(out T behaviour) where T : Behaviour
        {
            return base.gameObject.TryGetComponent<T>(out behaviour);
        }

        //
        // Summary:
        //     Wrapper for Unity's GameObject.GetComponentInChildren()
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetBehaviour<T>() where T : Behaviour
        {
            return base.gameObject.GetComponentInChildren<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] GetBehaviours<T>() where T : Behaviour
        {
            return base.gameObject.GetComponentsInChildren<T>();
        }

        //
        // Summary:
        //     Wrapper for Unity's GameObject.Destroy()
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DestroyBehaviour(Behaviour behaviour)
        {
            Object.Destroy(behaviour);
        }

        #region Unity Life Cycle Subscribtions
        protected void SubscribeUpdate(bool isSubscribe)
        {
            if (isSubscribe)
            {
                TimerManager.SubscribeUpdate(ProcessUpdate);
            }
            else
            {
                TimerManager.UnsubscribeUpdate(ProcessUpdate);
            }
        }

        protected void SubscribeFixedUpdate(bool isSubscribe)
        {
            if (isSubscribe)
            {
                TimerManager.SubscribeFixedUpdate(ProcessFixedUpdate);
            }
            else
            {
                TimerManager.UnsubscribeFixedUpdate(ProcessFixedUpdate);
            }
        }

        protected void SubscribeLateUpdate(bool isSubscribe)
        {
            if (isSubscribe)
            {
                TimerManager.SubscribeLateUpdate(ProcessLateUpdate);
            }
            else
            {
                TimerManager.UnsubscribeLateUpdate(ProcessLateUpdate);
            }
        }

        protected virtual void ProcessFixedUpdate() { }
        protected virtual void ProcessUpdate() { }
        protected virtual void ProcessLateUpdate() { }
        #endregion

        #region Spawn/Despawn
        public Transform Spawn(string poolName, Transform prefab)
        {
            return PoolManager.Pools[poolName].Spawn(prefab);
        }
        public Transform Spawn(string poolName, Transform prefab, Transform parent)
        {
            return PoolManager.Pools[poolName].Spawn(prefab, parent);
        }
        public Transform Spawn(string poolName, Transform prefab, Vector3 position, Quaternion rotation)
        {
            return PoolManager.Pools[poolName].Spawn(prefab, position, rotation);
        }
        public Transform Spawn(string poolName, Transform prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            return PoolManager.Pools[poolName].Spawn(prefab, position, rotation, parent);
        }

        public Transform Spawn(string poolName, GameObject prefab)
        {
            return PoolManager.Pools[poolName].Spawn(prefab);
        }
        public Transform Spawn(string poolName, GameObject prefab, Transform parent)
        {
            return PoolManager.Pools[poolName].Spawn(prefab, parent);
        }
        public Transform Spawn(string poolName, GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return PoolManager.Pools[poolName].Spawn(prefab, position, rotation);
        }
        public Transform Spawn(string poolName, GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            return PoolManager.Pools[poolName].Spawn(prefab, position, rotation, parent);
        }

        public void Despawn(string poolName, Transform instance)
        {
            PoolManager.Pools[poolName].DespawnObject(instance);
        }
        public void DespawnAll(string poolName)
        {
            PoolManager.Pools[poolName].DespawnAll();
        }
        #endregion

        protected T GetSceneContext<T>() where T : ISceneContext
        {
            return ContextManager.Singleton.GetSceneContext<T>();
        }
    }
}