using UnityEngine;

namespace WarWolfWorks.Interfaces.UnityMethods
{
    /// <summary>
    /// Icludes OnTriggerEnter for <see cref="MonoBehaviour"/>-like OnTriggerEnter(Collider) method.
    /// </summary>
    public interface IOnTriggerEnter
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.OnTriggerEnter(Collider).
        /// </summary>
        /// <param name="collider"></param>
        void OnTriggerEnter(Collider collider);
    }
}
