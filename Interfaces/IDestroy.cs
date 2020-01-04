using UnityEngine;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Icludes OnDestroyed for <see cref="MonoBehaviour"/>-like OnDestroy() method.
    /// </summary>
    public interface IDestroy
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.OnDestroy().
        /// </summary>
        void OnDestroyed();
    }
}
