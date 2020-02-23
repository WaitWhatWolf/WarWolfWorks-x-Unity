using UnityEngine;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// Inherit from this class with generic type T being the name of your class to make a class that implements a Singleton pattern.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool m_ShuttingDown = false;

        private static object m_Lock = new object();

        private static T m_Instance;

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (m_ShuttingDown)
                {
                    AdvancedDebug.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed. Returning null.", AdvancedDebug.DEBUG_LAYER_WWW_INDEX);
                    return null;
                }
                lock (m_Lock)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = (T)FindObjectOfType(typeof(T));
                        if (m_Instance == null)
                        {
                            GameObject gameObject = new GameObject();
                            m_Instance = gameObject.AddComponent<T>();
                            gameObject.name = m_Instance.ToString() + " (Singleton)";
                            DontDestroyOnLoad(gameObject);
                        }
                    }
                    return m_Instance;
                }
            }
        }

        private void OnApplicationQuit()
        {
            m_ShuttingDown = true;
        }

        private void OnDestroy()
        {
            m_ShuttingDown = true;
        }
    }
}
