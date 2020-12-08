using UnityEngine;
using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on a <see cref="NyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has a OnTriggerEnter2D method.
    /// </summary>
    public interface INyuOnTriggerEnter2D
    {
        /// <summary>
        /// Invoked on trigger enter.
        /// </summary>
        void NyuOnTriggerEnter2D(Collider2D collider);
    }
}
