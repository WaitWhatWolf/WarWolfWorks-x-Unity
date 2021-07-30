using UnityEngine;
using WarWolfWorks.Interfaces.NyuEntities;

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
            for (int i = 0; i < Behaviors.Length; i++)
            {
                if (Behaviors[i] is INyuOnTriggerEnter nyuEnter)
                    nyuEnter.NyuOnTriggerEnter(other);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            for (int i = 0; i < Behaviors.Length; i++)
            {
                if (Behaviors[i] is INyuOnTriggerExit nyuExit)
                    nyuExit.NyuOnTriggerExit(other);
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            for (int i = 0; i < Behaviors.Length; i++)
            {
                if (Behaviors[i] is INyuOnCollisionEnter nyuEnter)
                    nyuEnter.NyuOnCollisionEnter(collision);
            }
        }
        private void OnCollisionExit(Collision collision)
        {
            for (int i = 0; i < Behaviors.Length; i++)
            {
                if (Behaviors[i] is INyuOnCollisionExit nyuExit)
                    nyuExit.NyuOnCollisionExit(collision);
            }
        }
        #endregion
    }
}
