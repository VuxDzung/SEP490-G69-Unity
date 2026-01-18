namespace SEP490G69
{
    using UnityEngine;

    [DefaultExecutionOrder(-100)]
    public class GlobalSingleton<T> : MonoBehaviour where T : Component
    {
        [SerializeField] protected bool dontDestroyOnLoad = true;

        // create a private reference to T instance
        private static T instance;
        public static T Singleton
        {
            get
            {
                // If instance is null
                if (instance == null)
                {
                    Debug.Log($"[Singleton] Add new singleton instance for {typeof(T)}");
                    // find the generic instance
                    instance = FindFirstObjectByType<T>();

                    // if it's null again create a new object
                    // and attach the generic instance
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        instance = obj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                CreateNewInstance();
                if (dontDestroyOnLoad)
                {
                    gameObject.name = $"DDOL_{gameObject.name}";
                    DontDestroyOnLoad(instance);
                }
            }
            else
            {
                Debug.Log($"[Singleton] Destroy duplicated instance of {gameObject.name}");
                Destroy(gameObject);
            }
        }

        protected virtual void CreateNewInstance() { }
    }
}