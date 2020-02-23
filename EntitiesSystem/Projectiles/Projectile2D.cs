using System;
using UnityEngine;
using WarWolfWorks.Security;

namespace WarWolfWorks.EntitiesSystem.Projectiles
{
    /// <summary>
    /// 2D version of <see cref="Projectile"/>.
    /// </summary>
    public class Projectile2D : Projectile
    {
        /// <summary>
        /// 2D version of <see cref="Projectile.Behavior"/>.
        /// </summary>
        public class Behavior2D : Behavior
        {
            /// <summary>
            /// Called when <see cref="Projectile.Behavior.Parent"/> collides with an object.
            /// </summary>
            /// <param name="collider"></param>
            public virtual void OnTriggerEnter(Collider2D collider) { }

            /// <summary>
            /// Called when <see cref="Projectile.Behavior.Parent"/> stops colliding with an object.
            /// </summary>
            /// <param name="collider"></param>
            public virtual void OnTriggerExit(Collider2D collider) { }
        }

        /// <summary>
        /// The <see cref="UnityEngine.SpriteRenderer"/> attached to this projectile.
        /// </summary>
        public SpriteRenderer SpriteRenderer { get; private set; }
        /// <summary>
        /// The <see cref="UnityEngine.Rigidbody2D"/> attached to this projectile.
        /// </summary>
        public Rigidbody2D Rigidbody2D { get; private set; }

        /// <summary>
        /// Populates the projectile pool.
        /// </summary>
        /// <param name="population"></param>
        public static void Populate(int population)
            => Populate<Projectile2D>(population, PopulateAction);

        private static void PopulateAction(Projectile2D projectile)
        {
            projectile.SpriteRenderer = projectile.gameObject.AddComponent<SpriteRenderer>();
            projectile.Rigidbody2D = projectile.gameObject.AddComponent<Rigidbody2D>();
        }

        /// <summary>
        /// Instantiates a new <see cref="Projectile2D"/>.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="sprite"></param>
        /// <param name="behaviors"></param>
        /// <returns></returns>
        public static Projectile2D New(Entity owner, Vector2 position, Quaternion rotation, Sprite sprite, params Behavior2D[] behaviors)
        {
            if (!Populated)
                throw new ProjectileException(ProjectileExceptionType.UnpopulatedProjectileNew);

            Projectile2D toReturn;
            Start:
            if(EnumeratorMoveNext())
            {
                EnumeratorCurrent.Position = position;
                EnumeratorCurrent.Rotation = rotation;
                (EnumeratorCurrent as Projectile2D).SpriteRenderer.sprite = sprite;

                EnumeratorCurrent.gameObject.SetActive(true);

                NewArgsSet(EnumeratorCurrent, owner, behaviors);

                toReturn = EnumeratorCurrent as Projectile2D;
            }
            else
            {
                EnumeratorReset();
                goto Start;
            }

            PostNew(toReturn);

            return toReturn;
        }

        /// <summary>
        /// Calls behaviors accordingly to their 2D functionality.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        protected override void CallBehaviors(BehaviorCall type, object data = null)
        {
            if (!Active)
                return;

            try
            {
                foreach (Behavior2D b in Behaviors)
                {
                    switch (type)
                    {
                        case BehaviorCall.Initiate: b.Init(this); break;
                        case BehaviorCall.Update: b.Update(); break;
                        case BehaviorCall.Fixed: b.FixedUpdate(); break;
                        case BehaviorCall.CollideEnter: b.OnTriggerEnter((Collider2D)data); break;
                        case BehaviorCall.CollideExit: b.OnTriggerExit((Collider2D)data); break;
                        case BehaviorCall.Destroy: b.OnDestroy(); break;
                    }
                }
            }
            catch (Exception e)
            {
                AdvancedDebug.LogException(e);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CallBehaviors(BehaviorCall.CollideEnter, other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            CallBehaviors(BehaviorCall.CollideExit, other);
        }
    }
}
