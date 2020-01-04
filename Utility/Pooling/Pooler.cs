using System;
using System.Collections.Generic;

namespace WarWolfWorks.Utility.Pooling
{
    public static class Pooler
    {
        private static List<IPool> Pools = new List<IPool>();
        public static IPool[] GetAllPools() => Pools.ToArray();
        public static void AddPool(IPool pool, string 名前, int size)
        {
            pool.StartPool(名前, size);
            pool.名前 = 名前;
            Pools.Add(pool);
        }

        public static IPool GetPool(string 名前)
        {
            foreach(IPool p in Pools)
            {
                if (p.名前 == 名前) return p;
            }

            return null;
        }

        public static void RemovePool(IPool pool)
        {
            if(!ContainsInCollection(Pools, pool))
                return;

            Pools.Remove(pool);
            pool.EndPool();
        }

        private static bool ContainsInCollection<T>(ICollection<T> collection, T item)
        {
            foreach(T t in collection)
            {
                if (t.Equals(item)) return true;
            }

            return false;
        }
    }
}
