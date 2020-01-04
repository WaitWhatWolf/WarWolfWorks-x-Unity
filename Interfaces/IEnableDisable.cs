using UnityEngine;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Icludes OnEnabled and OnDisabled for <see cref="MonoBehaviour"/>-like OnEnable() and OnDisable() method.
    /// </summary>
    public interface IEnableDisable
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.OnEnable().
        /// </summary>
        void OnEnabled();
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.OnDisable().
        /// </summary>
        void OnDisabled();
    }
}
