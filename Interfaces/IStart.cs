using UnityEngine;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Icludes OnStart for <see cref="MonoBehaviour"/>-like Start() method.
    /// </summary>
    public interface IStart
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.Start().
        /// </summary>
        void OnStart();
    }
}
