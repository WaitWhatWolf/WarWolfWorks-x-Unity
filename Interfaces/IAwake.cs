using UnityEngine;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Icludes OnAwake for <see cref="MonoBehaviour"/>-like Awake() method.
    /// </summary>
    public interface IAwake
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.Awake().
        /// </summary>
        void OnAwake();
    }
}
