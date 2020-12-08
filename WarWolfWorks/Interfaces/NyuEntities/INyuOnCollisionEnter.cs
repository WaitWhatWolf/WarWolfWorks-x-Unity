using UnityEngine;
using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on a <see cref="NyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has a OnCollisionEnter method.
    /// </summary>
    public interface INyuOnCollisionEnter
    {
        /// <summary>
        /// Invoked on collision enter.
        /// </summary>
        void NyuOnCollisionEnter(Collision collision);
    }
}
