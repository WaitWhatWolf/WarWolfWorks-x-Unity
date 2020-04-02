using UnityEngine;

namespace WarWolfWorks.Interfaces.UnityMethods
{
    /// <summary>
    /// Includes OnDisable for a <see cref="MonoBehaviour"/>-like OnDisable() method.
    /// </summary>
    public interface IOnDisable
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.OnDisable().
        /// </summary>
        void OnDisable();
    }
}
