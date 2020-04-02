using UnityEngine;
using WarWolfWorks.Interfaces.UnityMethods;

namespace WarWolfWorks.NyuEntities.ProjectileSystem
{
    /// <summary>
    /// <see cref="NyuProjectile"/> used for 3D projectiles; Used by <see cref="NyuProjectile3DManager"/>.
    /// </summary>
    public sealed class NyuProjectile3D : NyuProjectile
    {
        /// <summary>
        /// Returns the <see cref="NyuProjectile3D"/>'s rigidbody velocity.
        /// </summary>
        public override Vector3 Velocity { get => Rigidbody.velocity; set => Rigidbody.velocity = value; }
        
        #region Additional Components
        /// <summary>
        /// The projectile's <see cref="UnityEngine.MeshFilter"/>.
        /// </summary>
        public MeshFilter MeshFilter { get; internal set; }
        /// <summary>
        /// The projectile's <see cref="UnityEngine.MeshRenderer"/>.
        /// </summary>
        public MeshRenderer MeshRenderer { get; internal set; }
        /// <summary>
        /// The projectile's <see cref="UnityEngine.Rigidbody"/>.
        /// </summary>
        public Rigidbody Rigidbody { get; internal set; }
        /// <summary>
        /// The projectile's <see cref="UnityEngine.SphereCollider"/>.
        /// </summary>
        public BoxCollider Collider { get; internal set; }
        #endregion

        #region Interface Calling
        private void OnTriggerEnter(Collider other)
        {
            foreach (Behavior behavior in Behaviors)
                if (behavior is IOnTriggerEnter behaviorTriggerEnter)
                    behaviorTriggerEnter.OnTriggerEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            foreach (Behavior behavior in Behaviors)
                if (behavior is IOnTriggerExit behaviorTriggerExit)
                    behaviorTriggerExit.OnTriggerExit(other);
        }
        #endregion
    }
}
