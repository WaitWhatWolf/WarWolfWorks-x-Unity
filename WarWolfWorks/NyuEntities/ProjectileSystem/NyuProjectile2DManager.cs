﻿using UnityEngine;
using WarWolfWorks.Attributes;

namespace WarWolfWorks.NyuEntities.ProjectileSystem
{
    /// <summary>
    /// Projectile manager for 2D bullets.
    /// </summary>
    [CompleteNoS]
    public sealed class NyuProjectile2DManager : NyuProjectileManager<NyuProjectile2D>
    {
        /// <summary>
        /// <see cref="MeshFilter"/>と<see cref="MeshRenderer"/>を追加。
        /// </summary>
        /// <param name="projectile"></param>
        protected override void OnProjectileCreated(NyuProjectile2D projectile)
        {
            projectile.SpriteRenderer = projectile.gameObject.AddComponent<SpriteRenderer>();
            projectile.Rigidbody = projectile.gameObject.AddComponent<Rigidbody2D>();
            projectile.Rigidbody.gravityScale = 0;
            projectile.Collider = projectile.gameObject.AddComponent<BoxCollider2D>();
            projectile.Collider.isTrigger = true;
        }

        /// <summary>
        /// Creates a new <see cref="NyuProjectile2D"/>. (Queues an inactive projectile into the pool of active projectiles)
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="sprite"></param>
        /// <param name="behaviors"></param>
        /// <returns></returns>
        public NyuProjectile2D New(Nyu owner, Vector3 position, Quaternion rotation, Sprite sprite, params NyuProjectile.Behavior[] behaviors)
        {
            if (New(owner, out NyuProjectile2D toReturn, position, rotation, behaviors))
            {
                toReturn.SpriteRenderer.sprite = sprite;
                return toReturn;
            }

            return null;
        }
    }
}
