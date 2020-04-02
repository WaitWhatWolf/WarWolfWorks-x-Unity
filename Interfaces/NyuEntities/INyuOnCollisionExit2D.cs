using UnityEngine;
using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on a <see cref="NyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has a OnCollisionExit2D method.
    /// </summary>
    public interface INyuOnCollisionExit2D
    {
        /// <summary>
        /// Invoked on collision exit.
        /// </summary>
        void NyuOnCollisionExit2D(Collision2D collision);
    }
}
