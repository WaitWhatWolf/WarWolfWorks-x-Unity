using UnityEngine;
using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on a <see cref="NyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has a OnCollisionExit method.
    /// </summary>
    public interface INyuOnCollisionExit
    {
        /// <summary>
        /// Invoked on collision exit.
        /// </summary>
        void NyuOnCollisionExit(Collision collision);
    }
}
