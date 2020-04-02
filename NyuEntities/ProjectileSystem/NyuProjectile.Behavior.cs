using WarWolfWorks.Interfaces.UnityMethods;
using WarWolfWorks.Interfaces;
using UnityEngine;

namespace WarWolfWorks.NyuEntities.ProjectileSystem
{
    /// <summary>
    /// Class which handles projectile behaviors.
    /// </summary>
    public abstract partial class NyuProjectile : MonoBehaviour
    {
        /// <summary>
        /// Class used to make a projectile behave in a specified way.
        /// (Supported interfaces: <see cref="IAwake"/>,
        /// <see cref="IUpdate"/>, <see cref="IFixedUpdate"/>, <see cref="ILateUpdate"/>,
        /// <see cref="IOnTriggerEnter"/> (conditional), <see cref="IOnTriggerExit"/> (conditional),
        /// <see cref="IOnTriggerExit2D"/> (conditional), <see cref="IOnTriggerExit2D"/> (conditional),
        /// and <see cref="IOnDestroy"/>)
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
