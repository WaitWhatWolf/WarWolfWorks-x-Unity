using UnityEngine;

namespace WarWolfWorks.Interfaces.UnityMethods
{
    /// <summary>
    /// Icludes OnCollisionExit for <see cref="MonoBehaviour"/>-like OnCollisionExit(Collision) method.
    /// </summary>
    public interface IOnCollisionExit
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.OnCollisionExit(Collision).
        /// </summary>
        /// <param name="collision"></param>
        void OnCollisionExit(Collision collision);
    }
}
