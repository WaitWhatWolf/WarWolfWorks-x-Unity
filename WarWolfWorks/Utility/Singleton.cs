using System;
using System.Collections.Generic;
using System.Text;
using WarWolfWorks.Security;

namespace WarWolfWorks.Utility
{
    /// <summary>
    /// A class which represents a singleton pattern.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : IDisposable where T : class
    { 
        /// <summary>
        /// Creates the singleton.
        /// </summary>
        public Singleton()
        {
            if (pv_Instance != null)
                throw new SingletonException();

            pv_Instance = this as T;
        }

        /// <summary>
        /// The singleton instance of the object.
        /// </summary>
        public static T Instance
        {
            get
            {
                lock(pv_Instance)
                {
                    return pv_Instance;
                }
            }
        }

        /// <summary>
        /// Disposes of the static T Instance.
        /// </summary>
        public virtual void Dispose()
        {
            pv_Instance = null;
        }

        /// <summary>
        /// Calls it's <see cref="Dispose"/> method.
        /// </summary>
        ~Singleton()
        {
            Dispose();
        }

        private static T pv_Instance;
    }
}
