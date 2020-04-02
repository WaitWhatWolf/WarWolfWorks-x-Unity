using UnityEngine;

namespace WarWolfWorks.Interfaces.UnityMethods
{
    /// <summary>
    /// Icludes OnTriggerExit for <see cref="MonoBehaviour"/>-like OnTriggerExit(Collider) method.
    /// </summary>
    public interface IOnTriggerExit
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.OnTriggerExit(Collider).
        /// </summary>
        /// <param name="collider"></param>
        void OnTriggerExit(Collider collider);
    }
}
