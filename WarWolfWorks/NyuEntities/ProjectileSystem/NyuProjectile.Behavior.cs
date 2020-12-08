using WarWolfWorks.Interfaces;
using UnityEngine;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.ProjectileSystem
{
    /// <summary>
    /// Class which handles projectile behaviors.
    /// </summary>
    public abstract partial class NyuProjectile : MonoBehaviour
    {
        /// <summary>
        /// Class used to make a projectile behave in a specified way.
        /// (Supported interfaces: <see cref="INyuAwake"/>,
        /// <see cref="INyuUpdate"/>, <see cref="INyuFixedUpdate"/>, <see cref="INyuLateUpdate"/>,
        /// <see cref="INyuOnTriggerEnter"/> (conditional), <see cref="INyuOnTriggerExit"/> (conditional),
        /// <see cref="INyuOnTriggerExit2D"/> (conditional), <see cref="INyuOnTriggerExit2D"/> (conditional),
        /// and <see cref="INyuOnDestroy"/>)
        /// </summary>
        public abstract class Behavior : IParentable<NyuProjectile>
        {
            /// <summary>
            /// The parent of this behavior.
            /// </summary>
            public NyuProjectile Parent { get; internal set; }
        }
    }
}
