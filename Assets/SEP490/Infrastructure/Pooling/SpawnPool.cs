namespace SEP490G69
{
    using System.Collections.Generic;
    using UnityEngine;

    [DefaultExecutionOrder(-100)]
    public class SpawnPool : MonoBehaviour
    {
        public static readonly int POOL_SIZE_MAX = 300;
        public static readonly int POOL_SIZE_STARTER = 50;

        [SerializeField] private string poolName;
        [SerializeField] private PreLoadPrefab[] preLoadPrefabs;

        //-------------------------------
        //---------- Pool Fields --------
        //-------------------------------
        private int _poolSize;

        private List<Transform> poolList = new List<Transform>();
        private List<Transform> activeList = new List<Transform>();
        public int Count => activeList.Count;

        private void Awake()
        {
            PoolManager.AddPool(poolName, this);
            SetPoolSize(POOL_SIZE_STARTER);
        }

        private void Start()
        {
            if (preLoadPrefabs.Length > 0)
            {
                foreach (var preLoad in preLoadPrefabs)
                {
                    for (int i = 0; i < preLoad.PreLoadCount; i++)
                    {
                        Spawn(preLoad.Prefab.transform, null);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            PoolManager.RemovePool(poolName);
        }

        #region Pool Methods
        public void SetPoolSize(int size)
        {
            _poolSize = Mathf.Max(size, _poolSize);
        }

        public Transform Spawn(GameObject prefab)
        {
            return Spawn(prefab.transform);
        }
        public Transform Spawn(GameObject prefab, Vector3 position = default, Quaternion rotation = default, Transform parent = null, ESpawnType spawnType = ESpawnType.Pooling)
        {
            return Spawn(prefab.transform, position, rotation, parent, spawnType);
        }
        public Transform Spawn(GameObject prefab, Transform parent = null, ESpawnType spawnType = ESpawnType.Pooling)
        {
            return Spawn(prefab.transform, parent, spawnType);
        }

        public Transform Spawn(Transform prefab)
        {
            return Spawn(prefab, null);
        }
        public Transform Spawn(Transform objectToSpawn, Vector3 position = default, Quaternion rotation = default, Transform parent = null, ESpawnType spawnType = ESpawnType.Pooling)
        {
            return spawnType == ESpawnType.Pooling ? GetPooledInstantiate(objectToSpawn, position, rotation, parent) : Instantiate(objectToSpawn, position, rotation, parent);
        }
        public Transform Spawn(Transform objectToSpawn, Transform parent = null, ESpawnType spawnType = ESpawnType.Pooling)
        {
            return spawnType == ESpawnType.Pooling ? GetPooledInstantiate(objectToSpawn, parent) : Instantiate(objectToSpawn, parent);
        }

        private Transform GetPooledInstantiate(Transform objectToSpawn, Transform parent)
        {
            Transform pooledObject = GetAvailableObject(objectToSpawn);

            if (pooledObject != null)
            {
                pooledObject.gameObject.SetActive(true);
                activeList.Add(pooledObject);

                IPooledObject[] existedObjects = pooledObject.GetComponents<IPooledObject>();

                if (existedObjects != null && existedObjects.Length > 0)
                {
                    foreach (var obj in existedObjects)
                    {
                        obj.Spawn();
                    }
                }
                return pooledObject;
            }

            if (poolList.Count >= _poolSize)
            {
                // Pool is full, return null instead of searching again.
                LoggerUtils.Logging("POOL SIZE EXCEEDED", $"Max pool size {_poolSize} exceed", TextColor.Red);
                return null;
            }

            pooledObject = Instantiate(objectToSpawn, parent);

            var identity = pooledObject.GetComponent<PooledIdentity>();
            if (identity == null)
            {
                identity = pooledObject.gameObject.AddComponent<PooledIdentity>();
                poolList.Add(pooledObject);
                identity.PrefabId = objectToSpawn.gameObject.GetInstanceID();
            }
            activeList.Add(pooledObject);

            IPooledObject[] objects = pooledObject.GetComponents<IPooledObject>();

            if (objects != null && objects.Length > 0)
            {
                foreach (var obj in objects)
                {
                    obj.Spawn();
                }
            }

            return pooledObject;
        }

        private Transform GetPooledInstantiate(Transform objectToSpawn, Vector3 pos, Quaternion rot, Transform parent)
        {
            Transform pooledObject = GetAvailableObject(objectToSpawn);

            if (pooledObject != null)
            {
                pooledObject.gameObject.SetActive(true);
                pooledObject.position = pos;
                pooledObject.rotation = rot;
                activeList.Add(pooledObject);
                IPooledObject[] existedObjects = pooledObject.GetComponents<IPooledObject>();

                if (existedObjects != null && existedObjects.Length > 0)
                {
                    foreach (var obj in existedObjects)
                    {
                        obj.Spawn();
                    }
                }
                return pooledObject;
            }

            if (poolList.Count >= _poolSize)
            {
                // Pool is full, return null instead of searching again.
                LoggerUtils.Logging("POOL SIZE EXCEEDED", $"Max pool size {_poolSize} exceed", TextColor.Red);
                return null;
            }

            pooledObject = Instantiate(objectToSpawn, pos, rot, parent);
            var identity = pooledObject.GetComponent<PooledIdentity>();
            if (identity == null)
            {
                identity = pooledObject.gameObject.AddComponent<PooledIdentity>();
                poolList.Add(pooledObject);
                identity.PrefabId = objectToSpawn.gameObject.GetInstanceID();
            }
            activeList.Add(pooledObject);

            IPooledObject[] objects = pooledObject.GetComponents<IPooledObject>();

            if (objects != null && objects.Length > 0)
            {
                foreach (var obj in objects)
                {
                    obj.Spawn();
                }
            }

            return pooledObject;
        }

        public void DespawnAll(ESpawnType despawnType = ESpawnType.Pooling)
        {
            Transform[] objects = activeList.ToArray();
            foreach (var despawned in objects)
            {
                DespawnObject(despawned, despawnType);
            }
        }

        public void DespawnObject(Transform obj, ESpawnType type = ESpawnType.Pooling)
        {
            obj.GetComponent<IPooledObject>()?.Despawn();
            switch (type)
            {
                case ESpawnType.Pooling:
                    obj.gameObject.SetActive(false);
                    activeList.Remove(obj);
                    break;
                case ESpawnType.InitDestroy:
                    activeList.Remove(obj);
                    Destroy(obj.gameObject);
                    break;
            }
        }

        private Transform GetAvailableObject(Transform prefab)
        {
            foreach (var component in poolList)
            {
                var identity = component.GetComponent<PooledIdentity>();
                if (identity != null &&
                    identity.PrefabId == prefab.gameObject.GetInstanceID() &&
                    !component.gameObject.activeSelf)
                {
                    return component;
                }
            }
            return null;
        }
        #endregion
    }
}