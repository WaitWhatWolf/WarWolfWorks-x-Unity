using UnityEngine;
using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Used on a <see cref="NyuComponent"/>, a <see cref="Nyu"/> entity or sub-component to indicate that it has a OnTriggerExit method.
    /// </summary>
    public interface INyuOnTriggerExit
    {
        /// <summary>
        /// Invoked on trigger exit.
        /// </summary>
        void NyuOnTriggerExit(Collider collider);
    }
}
