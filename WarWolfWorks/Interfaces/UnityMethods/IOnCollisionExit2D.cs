using UnityEngine;

namespace WarWolfWorks.Interfaces.UnityMethods
{
    /// <summary>
    /// Icludes OnCollisionExit2D for <see cref="MonoBehaviour"/>-like OnCollisionExit2D(Collision2D) method.
    /// </summary>
    public interface IOnCollisionExit2D
    {
        /// <summary>
        /// Equivalent to <see cref="MonoBehaviour"/>.OnCollisionExit2D(Collision2D).
        /// </summary>
        /// <param name="collision"></param>
        void OnCollisionExit2D(Collision2D collision);
    }
}
