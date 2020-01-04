using System;
using System.Collections.Generic;
using UnityEngine;

namespace WarWolfWorks.CollisionSystem
{
    /// <summary>
    /// Used to detect 2D collisions using collision2D methods.
    /// </summary>
    [AddComponentMenu("WWW/Collision/2D/ColliderDetector")]
    public sealed class ColliderDetector2D : Detector<Collider2D>
    {
        /// <summary>
        /// Returns true if <see cref="AttachedCollider"/> and <see cref="AttachedRigidbody"/> are not null.
        /// </summary>
        protected override Predicate<Collidable> コンディション => c => AttachedRigidbody && AttachedCollider;

        /// <summary>
        /// Rigidbody attached to this Detector.
        /// </summary>
        public Rigidbody2D AttachedRigidbody { get; private set; }

        /// <summary>
        /// Collider attached to this Detector. 
        /// (Gets the first collider in the hierarchy, doesn't have function by itself)
        /// </summary>
        public Collider2D AttachedCollider { get; private set; }

        /// <summary>
        /// Unity's awake function, used to get rigidbody and base collider.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            AttachedCollider = GetComponentInChildren<Collider2D>();
            AttachedRigidbody = GetComponentInChildren<Rigidbody2D>();
        }

#pragma warning disable IDE0051 // 使用されていないプライベート メンバーを削除する
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(IsUsable)
            {
                エンター呼び出す(collision.collider, false);
                グループ追加(collision.collider, false);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (IsUsable)
            {
                出口呼び出す(collision.collider, false);
                グループ削除(collision.collider);
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (IsUsable)
            {
                エンター呼び出す(collider, true);
                グループ追加(collider, true);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if(IsUsable)
            {
                出口呼び出す(collider, true);
                グループ削除(collider);
            }
        }
#pragma warning restore IDE0051 // 使用されていないプライベート メンバーを削除の終わりする
    }
}
