using UnityEngine;
using WarWolfWorks.Interfaces.UnityMethods;

namespace WarWolfWorks.NyuEntities.ProjectileSystem
{
    /// <summary>
    /// <see cref="NyuProjectile"/> used for 3D projectiles; Used by <see cref="NyuProjectile3DManager"/>.
    /// </summary>
    public sealed class NyuProjectile2D : NyuProjectile
    {
        /// <summary>
        /// Returns the <see cref="NyuProjectile3D"/>'s rigidbody velocity.
        /// </summary>
        public override Vector3 Velocity { get => Rigidbody.velocity; set => Rigidbody.velocity = value; }

        #region Additional Components
        /// <summary>
        /// The projectile's <see cref="UnityEngine.SpriteRenderer"/>.
        /// </summary>
        public SpriteRenderer SpriteRenderer { get; internal set; }
        /// <summary>
        /// The projectile's <see cref="UnityEngine.Rigidbody2D"/>.
        /// </summary>
        public Rigidbody2D Rigidbody { get; internal set; }
        /// <summary>
        /// The projectile's <see cref="UnityEngine.SphereCollider"/>.
        /// </summary>
        public BoxCollider2D Collider { get; internal set; }
        #endregion

        #region Interface Calling
        private void OnTriggerEnter2D(Collider2D other)
        {
            foreach (Behavior behavior in Behaviors)
                if (behavior is IOnTriggerEnter2D behaviorTriggerEnter)
                    behaviorTriggerEnter.OnTriggerEnter2D(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            foreach (Behavior behavior in Behaviors)
                if (behavior is IOnTriggerExit2D behaviorTriggerExit)
                    behaviorTriggerExit.OnTriggerExit2D(other);
        }
        #endregion
    }
}
