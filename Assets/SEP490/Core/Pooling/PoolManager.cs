namespace SEP490G69
{
    using System.Collections.Generic;

    public static class PoolManager
    {
        public static readonly Dictionary<string, SpawnPool> Pools = new Dictionary<string, SpawnPool>();

        public static void AddPool(string poolName, SpawnPool pool)
        {
            if (!Pools.ContainsKey(poolName))
            {
                Pools.Add(poolName, pool);
            }
        }

        public static bool RemovePool(string poolName)
        {
            if (Pools.ContainsKey(poolName))
            {
                return Pools.Remove(poolName);
            }
            return false;
        }

        public static SpawnPool GetPool(string poolName)
        {
            if (Pools.ContainsKey(poolName))
            {
                return Pools[poolName];
            }
            return null;
        }
    }
}