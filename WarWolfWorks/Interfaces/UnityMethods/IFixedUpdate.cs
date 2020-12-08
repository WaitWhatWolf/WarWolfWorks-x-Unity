using UnityEngine;

namespace WarWolfWorks.Interfaces.UnityMethods
{
    /// <summary>
    /// Icludes FixedUpdate for a <see cref="MonoBehaviour"/>-like FixedUpdate() method.
    /// </summary>
    public interface IFixedUpdate
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.FixedUpdate().
        /// </summary>
        void FixedUpdate();
    }
}
