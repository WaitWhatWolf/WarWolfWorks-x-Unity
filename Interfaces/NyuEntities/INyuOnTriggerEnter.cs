using UnityEngine;
using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on a <see cref="NyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has a OnTriggerEnter method.
    /// </summary>
    public interface INyuOnTriggerEnter
    {
        /// <summary>
        /// Invoked on trigger enter.
        /// </summary>
        void NyuOnTriggerEnter(Collider collider);
    }
}
