using UnityEngine;

namespace WarWolfWorks.NyuEntities.ProjectileSystem
{
    /// <summary>
    /// Projectile manager for 3D bullets.
    /// </summary>
    public sealed class NyuProjectile3DManager : NyuProjectileManager<NyuProjectile3D>
    {
        private void Awake()
        {
            Instance = this;
        }

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
        }

        /// <summary>
        /// Creates a new <see cref="NyuProjectile3D"/>. (Queues an inactive projectile into the pool of active projectiles)
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="mesh"></param>
        /// <param name="materials"></param>
        /// <param name="behaviors"></param>
        /// <returns></returns>
        public static NyuProjectile3D New(Nyu owner, Vector3 position, Quaternion rotation, Mesh mesh, Material[] materials, params NyuProjectile.Behavior[] behaviors)
        {
            New(owner, out NyuProjectile3D toReturn, position, rotation, behaviors);

            toReturn.MeshFilter.mesh = mesh;
            toReturn.MeshRenderer.materials = materials;

            return toReturn;
        }
    }
}
