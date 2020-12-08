using UnityEngine;

namespace WarWolfWorks.Interfaces.UnityMethods
{
    /// <summary>
    /// Includes OnEnable for a <see cref="MonoBehaviour"/>-like OnEnable() method.
    /// </summary>
    public interface IOnEnable
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.OnEnable().
        /// </summary>
        void OnEnable();
    }
}
