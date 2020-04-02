using UnityEngine;

namespace WarWolfWorks.Interfaces.UnityMethods
{
    /// <summary>
    /// Icludes OnDestroy for a <see cref="MonoBehaviour"/>-like OnDestroy() method.
    /// </summary>
    public interface IOnDestroy
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.OnDestroy().
        /// </summary>
        void OnDestroy();
    }
}
