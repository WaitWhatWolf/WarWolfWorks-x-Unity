using UnityEngine;

namespace WarWolfWorks.Interfaces.UnityMethods
{
    /// <summary>
    /// Icludes OnStart for <see cref="MonoBehaviour"/>-like Start() method.
    /// </summary>
    public interface IStart
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.Start().
        /// </summary>
        void Start();
    }
}
