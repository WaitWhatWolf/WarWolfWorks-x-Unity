using UnityEngine;
using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on a <see cref="NyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has a OnCollisionEnter2D method.
    /// </summary>
    public interface INyuOnCollisionEnter2D
    {
        /// <summary>
        /// Invoked on collision enter.
        /// </summary>
        void NyuOnCollisionEnter2D(Collision2D collision);
    }
}
