using WarWolfWorks.Interfaces;
using UnityEngine;
using WarWolfWorks.Interfaces.NyuEntities;
using System;

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
        public abstract class Behavior : IParentable<NyuProjectile>, INyuReferencable
        {
            /// <summary>
            /// Pointer to Parent.NyuMain.
            /// </summary>
            public Nyu NyuMain => Parent.NyuMain;

            /// <summary>
            /// The parent of this behavior.
            /// </summary>
            public NyuProjectile Parent { get; internal set; }

            /// <summary>
            /// Returns a copy of this <see cref="Behavior"/>.
            /// </summary>
            /// <returns></returns>
            public abstract Behavior GetDuplicate();

            /// <summary>
            /// Calls <see cref="INyuOnDestroy"/> if implemented.
            /// </summary>
            ~Behavior()
            {
                if (this is INyuOnDestroy dest)
                    dest.NyuOnDestroy();
            }
        }
    }
}
