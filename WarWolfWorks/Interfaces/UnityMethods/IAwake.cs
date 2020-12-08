using UnityEngine;

namespace WarWolfWorks.Interfaces.UnityMethods
{
    /// <summary>
    /// Icludes Awake for a <see cref="MonoBehaviour"/>-like Awake() method.
    /// </summary>
    public interface IAwake
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.Awake().
        /// </summary>
        void Awake();
    }
}
