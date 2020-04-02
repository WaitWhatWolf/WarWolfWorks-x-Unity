using UnityEngine;

namespace WarWolfWorks.Interfaces.UnityMethods
{
    /// <summary>
    /// Icludes LateUpdate for a <see cref="MonoBehaviour"/>-like LateUpdate() method.
    /// </summary>
    public interface ILateUpdate
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.LateUpdate().
        /// </summary>
        void LateUpdate();
    }
}
