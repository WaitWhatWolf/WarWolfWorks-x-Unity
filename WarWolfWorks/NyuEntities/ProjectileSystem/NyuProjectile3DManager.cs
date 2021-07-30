using UnityEngine;
using WarWolfWorks.Utility;

namespace WarWolfWorks.NyuEntities.ProjectileSystem
{
    /// <summary>
    /// Projectile manager for 3D bullets.
    /// </summary>
    public sealed class NyuProjectile3DManager : NyuProjectileManager<NyuProjectile3D>
    {
        /// <summary>
        /// <see cref="MeshFilter"/>と<see cref="MeshRenderer"/>を追加。
        /// </summary>
        /// <param name="projectile"></param>
        protected override void OnProjectileCreated(NyuProjectile3D projectile)
        {
            projectile.MeshFilter = projectile.gameObject.AddComponent<MeshFilter>();
            projectile.MeshRenderer = projectile.gameObject.AddComponent<MeshRenderer>();
            projectile.Rigidbody = projectile.gameObject.AddComponent<Rigidbody>();
            projectile.Rigidbody.useGravity = false;
            projectile.Collider = projectile.gameObject.AddComponent<BoxCollider>();
            projectile.Collider.isTrigger = true;
        }

        /// <summary>
        /// Creates a new <see cref="NyuProjectile3D"/>. (Queues an inactive projectile into the pool of active projectiles)
        /// </summary>
        /// <param name="owner">The owner of the projectile.</param>
        /// <param name="position">Spawn position of the projectile.</param>
        /// <param name="rotation">Spawn rotation of the projectile.</param>
        /// <param name="mesh">The physical mesh of the projectile.</param>
        /// <param name="materials">Materials applied to the mesh of the projectile.</param>
        /// <param name="behaviors">All behaviors applied to this projectile.</param>
        /// <returns></returns>
        public NyuProjectile3D New(Nyu owner, Vector3 position, Quaternion rotation, Mesh mesh, Material[] materials, params NyuProjectile.Behavior[] behaviors)
        {
            if (New(owner, out NyuProjectile3D toReturn, position, rotation, behaviors))
            {
                toReturn.MeshFilter.mesh = mesh;
                toReturn.MeshRenderer.materials = materials;

                return toReturn;
            }
            return null;
        }
    }
}
