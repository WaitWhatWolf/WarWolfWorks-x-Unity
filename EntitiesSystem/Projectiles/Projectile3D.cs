using System;
using UnityEngine;
using WarWolfWorks.Security;

namespace WarWolfWorks.EntitiesSystem.Projectiles
{
    /// <summary>
    /// 3D version of <see cref="Projectile"/>.
    /// </summary>
    public class Projectile3D : Projectile
    {
        /// <summary>
        /// 3D version of <see cref="Projectile.Behavior"/>.
        /// </summary>
        public class Behavior3D : Behavior
        {
            /// <summary>
            /// Called when <see cref="Projectile.Behavior.Parent"/> collides with an object.
            /// </summary>
            /// <param name="collider"></param>
            public virtual void OnTriggerEnter(Collider collider) { }

            /// <summary>
            /// Called when <see cref="Projectile.Behavior.Parent"/> stops colliding with an object.
            /// </summary>
            /// <param name="collider"></param>
            public virtual void OnTriggerExit(Collider collider) { }
        }

        /// <summary>
        /// The <see cref="UnityEngine.MeshFilter"/> attached to this projectile.
        /// </summary>
        public MeshFilter MeshFilter { get; private set; }
        /// <summary>
        /// The <see cref="UnityEngine.MeshRenderer"/> attached to this projectile.
        /// </summary>
        public MeshRenderer MeshRenderer { get; private set; }
        /// <summary>
        /// The <see cref="UnityEngine.Rigidbody"/> attached to this projectile.
        /// </summary>
        public Rigidbody Rigidbody { get; private set; }

        /// <summary>
        /// Populates the projectile pool.
        /// </summary>
        /// <param name="population"></param>
        public static void Populate(int population)
            => Populate<Projectile3D>(population, PopulateAction);

        private static void PopulateAction(Projectile3D projectile)
        {
            projectile.MeshFilter = projectile.gameObject.AddComponent<MeshFilter>();
            projectile.MeshRenderer = projectile.gameObject.AddComponent<MeshRenderer>();
            projectile.Rigidbody = projectile.gameObject.AddComponent<Rigidbody>();
        }

        /// <summary>
        /// Instantiates a new <see cref="Projectile3D"/>.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="mesh"></param>
        /// <param name="meshMaterials"></param>
        /// <param name="behaviors"></param>
        /// <returns></returns>
        public static Projectile3D New(Entity owner, Vector3 position, Quaternion rotation, Mesh mesh, Material[] meshMaterials, params Behavior3D[] behaviors)
        {
            if (!Populated)
                throw new ProjectileException(ProjectileExceptionType.UnpopulatedProjectileNew);

            Projectile3D toReturn;
            Start:
            if (EnumeratorMoveNext())
            {
                EnumeratorCurrent.Position = position;
                EnumeratorCurrent.Rotation = rotation;
                (EnumeratorCurrent as Projectile3D).MeshFilter.mesh = mesh;
                (EnumeratorCurrent as Projectile3D).MeshRenderer.materials = meshMaterials;
                EnumeratorCurrent.gameObject.SetActive(true);

                NewArgsSet(EnumeratorCurrent, owner, behaviors);

                toReturn = EnumeratorCurrent as Projectile3D;
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
        /// Calls behaviors accordingly to their 3D functionality.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        protected override void CallBehaviors(BehaviorCall type, object data = null)
        {
            if (!Active)
                return;

            try
            {
                foreach (Behavior3D b in Behaviors)
                {
                    switch (type)
                    {
                        case BehaviorCall.Initiate: b.Init(this); break;
                        case BehaviorCall.Update: b.Update(); break;
                        case BehaviorCall.Fixed: b.FixedUpdate(); break;
                        case BehaviorCall.CollideEnter: b.OnTriggerEnter((Collider)data); break;
                        case BehaviorCall.CollideExit: b.OnTriggerExit((Collider)data); break;
                        case BehaviorCall.Destroy: b.OnDestroy(); break;
                    }
                }
            }
            catch (Exception e)
            {
                AdvancedDebug.LogException(e);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            CallBehaviors(BehaviorCall.CollideEnter, other);
        }

        private void OnTriggerExit(Collider other)
        {
            CallBehaviors(BehaviorCall.CollideExit, other);
        }
    }
}
