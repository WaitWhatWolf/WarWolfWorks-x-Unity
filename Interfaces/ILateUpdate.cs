using UnityEngine;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Icludes OnLate for <see cref="MonoBehaviour"/>-like LateUpdate() method.
    /// </summary>
    public interface ILateUpdate
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.LateUpdate().
        /// </summary>
        void OnLate();
    }
}
