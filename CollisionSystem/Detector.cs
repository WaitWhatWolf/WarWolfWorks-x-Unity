using System;
using System.Collections.Generic;
using UnityEngine;
using WarWolfWorks.Debugging;

namespace WarWolfWorks.CollisionSystem
{
    /// <summary>
    /// Contains collision triggers to be detected.
    /// </summary>
    public abstract class Detector<T> : Collidable where T : Behaviour
    {
        /// <summary>
        /// Group used to manage collisions. (しょうとつ)
        /// </summary>
        protected internal struct 衝突グループ
        {
            /// <summary>
            /// Collider of the group.
            /// </summary>
            public readonly T コリダー;

            /// <summary>
            /// If the collider was drawn from an OnTrigger collision.
            /// </summary>
            public readonly bool トリガーは;

            /// <summary>
            /// Creates a new CollisionGroup.
            /// </summary>
            /// <param name="collider"></param>
            /// <param name="isTrigger"></param>
            public 衝突グループ(T collider, bool isTrigger)
            {
                コリダー = collider;
                トリガーは = isTrigger;
            }
        }

        /// <summary>
        /// Event is triggered when ColliderDetector detects an Enter collision.
        /// Collider is the collider which was detected,
        /// Boolean is if the collision was an OnTrigger type collision.
        /// </summary>
        public event Action<T, bool> OnEnter;

        /// <summary>
        /// Event is triggered as long as previously entered collision did not exit.
        /// Collider is the collider which was detected,
        /// Boolean is if the collision was an OnTrigger type collision.
        /// </summary>
        public event Action<T, bool> OnStay;

        /// <summary>
        /// Event is triggered when ColliderDetector detects an Exit collision.
        /// Collider is the collider which was detected,
        /// Boolean is if the collision was an OnTrigger type collision.
        /// </summary>
        public event Action<T, bool> OnExit;

        /// <summary>
        /// Invokes the <see cref="OnEnter"/> event action. (よびだす)
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        protected internal void エンター呼び出す(T arg1, bool arg2) => OnEnter?.Invoke(arg1, arg2);
        /// <summary>
        /// Invokes the <see cref="OnStay"/> event action.(よびだす)
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        protected internal void ソテー呼び出す(T arg1, bool arg2) => OnStay?.Invoke(arg1, arg2);
        /// <summary>
        /// Invokes the <see cref="OnExit"/> event action.(でぐちよびだす)
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        protected internal void 出口呼び出す(T arg1, bool arg2) => OnExit?.Invoke(arg1, arg2);

        /// <summary>
        /// Unity's FixedUpdate method.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            ホーイチ(か => ソテー呼び出す(か.コリダー, か.トリガーは));
        }

        private List<衝突グループ> すべてグループ { get; set; } = new List<衝突グループ>();

        /// <summary>
        /// Adds a group to CollisionGroups.
        /// </summary>
        /// <param name="ち"></param>
        /// <param name="てい"></param>
        protected internal bool グループ追加(T ち, bool てい)
        {
            if (!CollisionGroupExists(ち))
            {
                すべてグループ.Add(new 衝突グループ(ち, てい));
                return true;
            }

            return false;
        }
        /// <summary>
        /// Removes a group containing ち as collider.
        /// </summary>
        /// <param name="ち"></param>
        protected internal void グループ削除(T ち) => すべてグループ.RemoveAll(cg => cg.コリダー == ち);
        /// <summary>
        /// Returns true if a group contains the given collider.
        /// </summary>
        /// <param name="groupCollider"></param>
        /// <returns></returns>
        public bool CollisionGroupExists(T groupCollider) => すべてグループ.FindIndex(col => groupCollider == col.コリダー) != -1;

        /// <summary>
        /// Executes the action given on CollisionGroups.ForEach.
        /// </summary>
        /// <param name="アクション"></param>
        protected internal void ホーイチ(Action<衝突グループ> アクション) => すべてグループ.ForEach(アクション);
    }

}
