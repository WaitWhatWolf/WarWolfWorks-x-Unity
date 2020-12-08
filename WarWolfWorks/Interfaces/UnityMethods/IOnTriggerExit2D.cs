using UnityEngine;

namespace WarWolfWorks.Interfaces.UnityMethods
{
    /// <summary>
    /// Icludes OnTriggerExit2D for <see cref="MonoBehaviour"/>-like OnTriggerExit2D(Collider2D) method.
    /// </summary>
    public interface IOnTriggerExit2D
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.OnTriggerEnter2D(Collider2D).
        /// </summary>
        /// <param name="collider"></param>
        void OnTriggerExit2D(Collider2D collider);
    }
}
