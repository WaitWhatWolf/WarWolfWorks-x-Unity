using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.SerializedProjectiles
{
    /// <summary>
    /// The base class for projectile behaviors in <see cref="SerializedProjectiles"/>.
    /// (Supported interfaces: <see cref="INyuAwake"/>,
    /// <see cref="INyuUpdate"/>, <see cref="INyuFixedUpdate"/>, <see cref="INyuLateUpdate"/>,
    /// <see cref="INyuOnTriggerEnter"/>, <see cref="INyuOnTriggerExit"/>,
    /// <see cref="INyuOnTriggerEnter2D"/>, <see cref="INyuOnTriggerExit2D"/>,
    /// <see cref="INyuOnCollisionEnter"/>, <see cref="INyuOnCollisionExit"/>,
    /// <see cref="INyuOnCollisionEnter2D"/>, <see cref="INyuOnCollisionExit2D"/>,
    /// and <see cref="INyuOnDestroy"/>)
    /// </summary>
    public abstract class Behavior : ScriptableObject, IParentable<SProjectile>
    {
        /// <summary>
        /// The projectile which uses this behavior.
        /// </summary>
        public SProjectile Parent { get; internal set; }
    }
}
