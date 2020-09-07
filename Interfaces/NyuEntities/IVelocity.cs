using UnityEngine;
using WarWolfWorks.NyuEntities.MovementSystemV2;

namespace WarWolfWorks.Interfaces.NyuEntities
{
    /// <summary>
    /// Interface used with <see cref="NyuMovement"/> to create a velocity.
    /// </summary>
    public interface IVelocity : IParentInitiatable<NyuMovement>, INyuReferencable
    {
        /// <summary>
        /// The final value applied to this velocity.
        /// </summary>
        /// <returns></returns>
        Vector3 GetValue();
    }
}
