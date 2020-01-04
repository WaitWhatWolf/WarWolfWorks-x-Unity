using UnityEngine;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Icludes OnUpdate for <see cref="MonoBehaviour"/>-like Update() method.
    /// </summary>
    public interface IUpdate
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.Update().
        /// </summary>
        void OnUpdate();
    }
}
