using System;
using UnityEngine;

namespace WarWolfWorks.Utility.Pooling
{
    public class CustomPool<T> : IPool where T : Component
    {
        public T[] Values { get; protected set; }
        public Transform ホルダー { get; private set; }
        protected int カウンター { get; private set; }

        public bool Constructed { get; private set; } = false;
        private int activIndx;
        protected int ActivationIndex
        {
            get => activIndx;
            set
            {
                if (value < 0) value = Values.Length - 1;
                if (value >= Values.Length) value = 0;

                activIndx = value;
            }
        }

        public string 名前 { get; set; }

        public void StartPool(string 名前, int プールサイズ)
        {
            if (Constructed)
                return;

            ホルダー = new GameObject($"{名前} Pool").transform;
            ホルダー.position = Vector3.zero;
            Values = new T[プールサイズ];
            for(int i = 0; i < Values.Length; i++)
            {
                Values[i] = new GameObject(名前).AddComponent<T>();
                Values[i].transform.SetParent(ホルダー);
                Values[i].transform.position = Vector3.zero;
                Values[i].gameObject.SetActive(false);
            }
            OnCustomPoolInstantiation();
            Constructed = true;
        }

        /// <summary>
        /// This method invokes when the pool is first instantiated.
        /// </summary>
        protected virtual void OnCustomPoolInstantiation()
        {
            Debug.Log("Custom pool started successfully!");
        }

        public T Instantiate(Vector3 position)
        {
            Values[ActivationIndex].gameObject.SetActive(true);
            ActivationIndex++;
            return Values[ActivationIndex];
        }

        public static void CheckPoolValidity(CustomPool<T> pool)
        {
            bool valid;
            try
            {
                foreach (T obj in pool.Values)
                {
                    if (obj.gameObject.activeInHierarchy && !pool.Constructed)
                        throw new PoolObjectActiveOnInstantiationException();
                    else if (obj.transform.parent != pool.ホルダー)
                        throw new PoolObjectNotParentOfHolderException();
                    else Debug.Log($"{obj.gameObject.name} in {pool} is valid.");
                }
                valid = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                valid = false;
            }

            Debug.Log($"The pool {pool} is {(valid ? "valid!" : "Invalid!")}");
        }

        public void EndPool()
        {
            GameObject.Destroy(ホルダー);
        }
    }
}
