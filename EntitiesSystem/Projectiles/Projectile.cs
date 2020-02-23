﻿using System;
using System.Collections.Generic;
using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Security;

namespace WarWolfWorks.EntitiesSystem.Projectiles
{
    /// <summary>
    /// Core class of the projectile system.
    /// </summary>
    public abstract class Projectile : MonoBehaviour, IEntity
    {
        /// <summary>
        /// Used with <see cref="CallBehaviors(BehaviorCall, object)"/>; It calls a method inside a <see cref="Behavior"/>
        /// according to it's name.
        /// </summary>
        protected enum BehaviorCall
        {
            /// <summary>
            /// Calls <see cref="Behavior.Init(Projectile)"/>.
            /// </summary>
            Initiate,
            /// <summary>
            /// Calls <see cref="Behavior.Update"/>.
            /// </summary>
            Update,
            /// <summary>
            /// Calls <see cref="Behavior.FixedUpdate"/>.
            /// </summary>
            Fixed,
            /// <summary>
            /// Calls a custom method which triggers inside a <see cref="Projectile"/> parent's OnTriggerEnter method (2D or 3D).
            /// </summary>
            CollideEnter,
            /// <summary>
            /// Calls a custom method which triggers inside a <see cref="Projectile"/> parent's OnTriggerExit method (2D or 3D).
            /// </summary>
            CollideExit,
            /// <summary>
            /// Called inside <see cref="Projectile.Destroy(Projectile)"/>, calls <see cref="Behavior.OnDestroy"/>.
            /// </summary>
            Destroy
        }

        /// <summary>
        /// Base behavioural class for <see cref="Projectile"/> behaviour.
        /// </summary>
        public abstract class Behavior : IParentInitiatable<Projectile>
        {
            /// <summary>
            /// Initiated state of this <see cref="Behavior"/>. (<see cref="IParentInitiatable{T}"/> implementation)
            /// </summary>
            public bool Initiated { get; private set; }

            /// <summary>
            /// Parent of this <see cref="Behavior"/>.
            /// </summary>
            public Projectile Parent { get; private set; }

            /// <summary>
            /// Initiates this <see cref="Behavior"/>.
            /// </summary>
            /// <param name="parent"></param>
            public void Init(Projectile parent)
            {
                if (Initiated)
                    return;

                Parent = parent;
                Initiated = true;

                OnInitiate();
            }

            /// <summary>
            /// Called when the behavior is added to a <see cref="Projectile"/>.
            /// </summary>
            protected virtual void OnInitiate() { }

            /// <summary>
            /// Called every in-game frame.
            /// </summary>
            public virtual void Update() { }

            /// <summary>
            /// Called every in-game physics frame.
            /// </summary>
            public virtual void FixedUpdate() { }

            /// <summary>
            /// Called when a <see cref="Projectile"/> is destroyed.
            /// </summary>
            public virtual void OnDestroy() { }
        }

        private static Projectile[] Projectiles;

        /// <summary>
        /// List of all active projectiles.
        /// </summary>
        private static List<Projectile> ActiveProjectiles = new List<Projectile>();
        /// <summary>
        /// List of all inactive projectiles.
        /// </summary>
        private static List<Projectile> InactiveProjectiles = new List<Projectile>();

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> of all active projectiles.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Projectile> GetActiveProjectiles() => ActiveProjectiles;
        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> of all inactive projectiles.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Projectile> GetInactiveProjectiles() => InactiveProjectiles;

        /// <summary>
        /// Invoked when a <see cref="Projectile"/>.New method is called.
        /// </summary>
        public static event Action<Projectile> OnProjectileNew;
        /// <summary>
        /// Invoked when <see cref="Projectile.Destroy(Projectile)"/> is succesfully called.
        /// </summary>
        public static event Action<Projectile> OnProjectileDestroy;

        /// <summary>
        /// Returns true if <see cref="Populate{T}(int, Action{T})"/> was previously called.
        /// </summary>
        protected static bool Populated { get; private set; }

        /// <summary>
        /// What type the pool population was instantiated as.
        /// </summary>
        protected static Type PopulationType { get; private set; }

        /// <summary>
        /// Owner of this projectile.
        /// </summary>
        public Entity EntityMain { get; protected set; }

        /// <summary>
        /// Active state of this <see cref="Projectile"/>.
        /// </summary>
        public bool Active { get; private set; }

        private bool PostNewInvoked { get; set; }

        /// <summary>
        /// All behaviors of this <see cref="Projectile"/>.
        /// </summary>
        public IEnumerable<Behavior> Behaviors { get; private set; }

        /// <summary>
        /// Pointer to transform.position.
        /// </summary>
        public Vector3 Position { get => transform.position; set => transform.position = value; }
        /// <summary>
        /// Pointer to transform.rotation.
        /// </summary>
        public Quaternion Rotation { get => transform.rotation; set => transform.rotation = value; }

        /// <summary>
        /// Populates the <see cref="Projectile"/> pool.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="population">How many projectiles will be instantiated in the pool.</param>
        /// <param name="OnInstantiate">This action is invoked when a Projectile is instantiated; Useful for adding components into the projectile.</param>
        protected static void Populate<T>(int population, Action<T> OnInstantiate) where T : Projectile
        {
            if (population < 1)
                throw new ProjectileException(ProjectileExceptionType.PopulationLessThanOne);

            lock (Projectiles)
            {
                if (Populated)
                {
                    for (int i = 0; i < Projectiles.Length; i++)
                    {
                        Destroy(Projectiles[i].gameObject);
                    }
                }

                Projectiles = new Projectile[population];

                for (int i = 0; i < population; i++)
                {
                    GameObject g = new GameObject("Projectile_" + (i + 1).ToString());
                    Projectiles[i] = g.AddComponent<T>();
                    OnInstantiate((T)Projectiles[i]);
                    InactiveProjectiles.Add(Projectiles[i]);
                }

                EnumeratorReset();
                PopulationType = typeof(T);
                Populated = true;
            }
        }

        /// <summary>
        /// Sets protected behaviors un-settable throught inherited members for any New/Instantiate methods.
        /// </summary>
        /// <param name="for"></param>
        /// <param name="entityMain"></param>
        /// <param name="behaviors"></param>
        protected static void NewArgsSet(Projectile @for, Entity entityMain, IEnumerable<Behavior> behaviors)
        {
            @for.EntityMain = entityMain;
            @for.SetBehaviors(behaviors);
        }

        /// <summary>
        /// You are required to call this method at the end of a New/Instantiate method, as it set all required variables that are inaccessible through inherited members;
        /// Creating a projectile without calling this method will result in an exception.
        /// </summary>
        /// <param name="for"></param>
        protected static void PostNew(Projectile @for)
        {
            if (@for)
            {
                ActiveProjectiles.Add(@for);
                @for.PostNewInvoked = true;
                OnProjectileNew?.Invoke(@for);
            }
        }

        /// <summary>
        /// Call a behavior method of all behaviors with optional data.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        protected abstract void CallBehaviors(BehaviorCall type, object data = null);
        
        /// <summary>
        /// Sets behaviors of this projectile, then calls <see cref="CallBehaviors(BehaviorCall, object)"/> with <see cref="BehaviorCall.Initiate"/>.
        /// </summary>
        /// <param name="behaviors"></param>
        protected void SetBehaviors(IEnumerable<Behavior> behaviors)
        {
            if (behaviors == null)
                return;

            Behaviors = behaviors;

            CallBehaviors(BehaviorCall.Initiate);
        }

        private void OnEnable()
        {
            if (!PostNewInvoked)
            {
                gameObject.SetActive(false);
                throw new ProjectileException(ProjectileExceptionType.InstantiatedWithoutPostNew);
            }

            OnEnabled();
        }

        /// <summary>
        /// <see cref="MonoBehaviour"/>'s OnEnable equivalent.
        /// </summary>
        protected virtual void OnEnabled() { }

        private void Update() => CallBehaviors(BehaviorCall.Update);

        private void FixedUpdate() => CallBehaviors(BehaviorCall.Fixed);

        /// <summary>
        /// Destroys a projectile.
        /// </summary>
        /// <param name="projectile"></param>
        public static void Destroy(Projectile projectile)
        {
            if (projectile == null)
                return;

            lock (projectile)
            {
                projectile.CallBehaviors(BehaviorCall.Destroy);
                projectile.gameObject.SetActive(false);
                ActiveProjectiles.Remove(projectile);
                OnProjectileDestroy?.Invoke(projectile);
                projectile.PostNewInvoked = false;
            }
        }

        private static int EnumeratorIndex { get; set; }

        /// <summary>
        /// The current itteration of the Enumerator-like implementation.
        /// </summary>
        protected static Projectile EnumeratorCurrent => Projectiles[EnumeratorIndex];

        /// <summary>
        /// Enumerator-like implementation for getting a new projectile for instantiation.
        /// </summary>
        /// <returns></returns>
        protected static bool EnumeratorMoveNext()
        {
            if (EnumeratorIndex < Projectiles.Length)
            {
                EnumeratorIndex++;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Enumerator-like implementation to reset the itteration.
        /// </summary>
        protected static void EnumeratorReset()
        {
            EnumeratorIndex = -1;
        }
    }
}
