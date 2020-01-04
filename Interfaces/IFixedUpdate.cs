using UnityEngine;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Icludes OnFixed for <see cref="MonoBehaviour"/>-like FixedUpdate() method.
    /// </summary>
    public interface IFixedUpdate
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.FixedUpdate().
        /// </summary>
        void OnFixed();
    }
}
