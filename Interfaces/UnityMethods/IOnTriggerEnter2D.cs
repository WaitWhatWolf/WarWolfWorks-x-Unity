using UnityEngine;

namespace WarWolfWorks.Interfaces.UnityMethods
{
    /// <summary>
    /// Icludes OnTriggerEnter2D for <see cref="MonoBehaviour"/>-like OnTriggerEnter2D(Collider2D) method.
    /// </summary>
    public interface IOnTriggerEnter2D
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.OnTriggerEnter2D(Collider2D).
        /// </summary>
        /// <param name="collider"></param>
        void OnTriggerEnter2D(Collider2D collider);
    }
}
