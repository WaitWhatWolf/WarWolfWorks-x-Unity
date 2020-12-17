using UnityEngine;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Inherit from this class with generic type T being the name of your class to make a class that implements a Singleton pattern.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (pv_Destroying)
                {
                    AdvancedDebug.LogWarning("[Singleton] Instance '" + typeof(T) + "is being destroyed, returning null.", DEBUG_LAYER_WWW_INDEX);
                    return null;
                }
                lock (pv_Lock)
                {
                    if (pv_Instance == null)
                    {
                        pv_Instance = (T)FindObjectOfType(typeof(T));
                        if (pv_Instance == null)
                        {
                            GameObject gameObject = new GameObject();
                            pv_Instance = gameObject.AddComponent<T>();
                            gameObject.name = pv_Instance.ToString() + " (Singleton)";
                            DontDestroyOnLoad(gameObject);
                        }
                    }
                    return pv_Instance;
                }
            }
        }

        private void OnApplicationQuit()
        {
            pv_Destroying = true;
        }

        private void OnDestroy()
        {
            pv_Destroying = true;
        }

        private static bool pv_Destroying = false;
        private static object pv_Lock = new object();
        private static T pv_Instance;
    }
}
