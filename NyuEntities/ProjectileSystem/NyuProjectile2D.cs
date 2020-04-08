using UnityEngine;
using WarWolfWorks.Interfaces.NyuEntities;

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
        private void OnTriggerEnter2D(Collider2D collision)
        {
            for (int i = 0; i < ns_Behaviors.Length; i++)
            {
                if (ns_Behaviors[i] is INyuOnTriggerEnter2D nyuEnter2D)
                    nyuEnter2D.NyuOnTriggerEnter2D(collision);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            for (int i = 0; i < ns_Behaviors.Length; i++)
            {
                if (ns_Behaviors[i] is INyuOnTriggerExit2D nyuExit2D)
                    nyuExit2D.NyuOnTriggerExit2D(collision);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            for (int i = 0; i < ns_Behaviors.Length; i++)
            {
                if (ns_Behaviors[i] is INyuOnCollisionEnter2D nyuEnter2D)
                    nyuEnter2D.NyuOnCollisionEnter2D(collision);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            for (int i = 0; i < ns_Behaviors.Length; i++)
            {
                if (ns_Behaviors[i] is INyuOnCollisionExit2D nyuExit2D)
                    nyuExit2D.NyuOnCollisionExit2D(collision);
            }
        }
        #endregion
    }
}
