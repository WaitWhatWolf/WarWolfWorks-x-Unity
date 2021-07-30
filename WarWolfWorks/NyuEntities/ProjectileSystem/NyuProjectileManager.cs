using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.Enums;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;
using WarWolfWorks.Interfaces.UnityMethods;
using WarWolfWorks.Security;
using WarWolfWorks.Utility;

namespace WarWolfWorks.NyuEntities.ProjectileSystem
{
    /// <summary>
    /// The manager of all <see cref="NyuProjectile"/> objects.
    /// </summary>
    [CompleteNoS]
    public abstract class NyuProjectileManager<T> : MonoBehaviour, IInitiated where T : NyuProjectile
    {
        #region Pool
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool Initiated { get; private set; }

        /// <summary>
        /// Initiates this ProjectileManager. Can be re-invoked to change the pool size; Destroys all previous projectiles in doing so.
        /// </summary>
        public void Init(int poolSize)
        {
            if (poolSize < 1)
                throw new NyuProjectileException(1);

            if (Initiated)
            {
                for (int i = 0; i < AllProjectiles.Length; i++)
                {
                    Destroy(AllProjectiles[i].gameObject);
                }
            }

            AllProjectiles = new T[poolSize];
            InactiveProjectiles = new List<T>(poolSize);
            ActiveProjectiles = new List<T>(poolSize);

            lock (AllProjectiles)
            {
                for (int i = 0; i < poolSize; i++)
                {
                    GameObject g = new GameObject("Projectile_" + (i + 1).ToString());
                    g.SetActive(false);
                    g.transform.SetParent(transform);

                    T projectile = g.AddComponent<T>();
                    AllProjectiles[i] = projectile;
                    OnProjectileCreated(projectile);
                    InactiveProjectiles.Add(AllProjectiles[i]);
                }

                DontDestroyOnLoad(this);
                Initiated = true;
            }
        }
        
        /// <summary>
        /// Invoked when a projectile is added into the pool through <see cref="Init(int)"/>.
        /// </summary>
        /// <param name="projectile"></param>
        protected abstract void OnProjectileCreated(T projectile);

        /// <summary>
        /// Use this method at the beginning of your creation method to retrieve a projectile from the pool.
        /// </summary>
        /// <param name="owner">The owner of the projectile.</param>
        /// <param name="position">Spawn position of the projectile.</param>
        /// <param name="rotation">Spawn rotation of the projectile.</param>
        /// <param name="projectile">The projectile which can be used when New returns true.</param>
        /// <param name="behaviors">All behaviors applied to this projectile.</param>
        protected bool New(Nyu owner, out T projectile, Vector3 position, Quaternion rotation, IEnumerable<NyuProjectile.Behavior> behaviors)
        {
            if (!Initiated)
                throw new NyuProjectileException(2);

            if(InactiveProjectiles.Count == 0)
            {
                projectile = null;
                return false;
            }

            projectile = InactiveProjectiles[0];
            projectile.Position = position;
            projectile.Rotation = rotation;
            projectile.NyuMain = owner;
            projectile.Behaviors = behaviors.RemoveNull().ToArray();

            InactiveProjectiles.RemoveAt(0);
            projectile.gameObject.SetActive(true);
            ActiveProjectiles.Add(projectile);

            if (projectile is INyuAwake awake)
                awake.NyuAwake();

            for (int i = 0; i < projectile.Behaviors.Length; i++)
            {
                projectile.Behaviors[i] = projectile.Behaviors[i].GetDuplicate();
                projectile.Behaviors[i].Parent = projectile;

                if (projectile.Behaviors[i] is INyuAwake bAwake)
                {
                    bAwake.NyuAwake();
                }

                projectile.RefreshUpdateLists();
            }

            if (projectile is INyuStart start)
                start.NyuStart();

            return true;
        }

        /// <summary>
        /// Removes a given projectile from the pool of active projectiles.
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns></returns>
        public bool End(NyuProjectile projectile)
        {
            if (!Initiated)
                throw new NyuProjectileException(2);

            if (projectile is T asT && ActiveProjectiles.Remove(asT))
            {
                projectile.Behaviors = new NyuProjectile.Behavior[0];

                if (projectile is INyuOnDestroy projectileOnDestroy)
                    projectileOnDestroy.NyuOnDestroy();

                projectile.gameObject.SetActive(false);
                InactiveProjectiles.Add(asT);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a given projectile from the pool of active projectiles.
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns></returns>
        public bool End(T projectile)
        {
            if (!Initiated)
                throw new NyuProjectileException(2);

            if (ActiveProjectiles.Remove(projectile))
            {
                projectile.Behaviors = new NyuProjectile.Behavior[0];

                if (projectile is INyuOnDestroy projectileOnDestroy)
                    projectileOnDestroy.NyuOnDestroy();

                projectile.gameObject.SetActive(false);
                InactiveProjectiles.Add(projectile);
                return true;
            }

            return false;
        }
        #endregion

        #region Calling
        /// <summary>
        /// When overriding, make sure to include "base.Update();" as it calls all <see cref="INyuUpdate.NyuUpdate"/> methods
        /// in applicable <see cref="NyuProjectile.Behavior"/> components.
        /// </summary>
        protected virtual void Update()
        {
            for (int i = 0; i < ActiveProjectiles.Count; i++)
            {
                if (ActiveProjectiles[i].Locked)
                    continue;

                if (ActiveProjectiles[i].IsUpdate)
                    ActiveProjectiles[i].AsUpdate.NyuUpdate();

                for (int j = 0; j < ActiveProjectiles[i].Behaviors_Updates.Count; j++)
                    ActiveProjectiles[i].Behaviors_Updates[j].NyuUpdate();
            }
        }

        /// <summary>
        /// When overriding, make sure to include "base.FixedUpdate();" as it calls all <see cref="INyuFixedUpdate.NyuFixedUpdate"/> methods
        /// in applicable <see cref="NyuProjectile.Behavior"/> components.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            for (int i = 0; i < ActiveProjectiles.Count; i++)
            {
                if (ActiveProjectiles[i].Locked)
                    continue;

                if (ActiveProjectiles[i].IsFixedUpdate)
                    ActiveProjectiles[i].AsFixedUpdate.NyuFixedUpdate();

                for (int j = 0; j < ActiveProjectiles[i].Behaviors_FixedUpdates.Count; j++)
                    ActiveProjectiles[i].Behaviors_FixedUpdates[j].NyuFixedUpdate();
            }
        }

        /// <summary>
        /// When overriding, make sure to include "base.LateUpdate();" as it calls all <see cref="INyuLateUpdate.NyuLateUpdate"/> methods
        /// in applicable <see cref="NyuProjectile.Behavior"/> components.
        /// </summary>
        protected virtual void LateUpdate()
        {
            for (int i = 0; i < ActiveProjectiles.Count; i++)
            {
                if (ActiveProjectiles[i].Locked)
                    continue;

                if (ActiveProjectiles[i].IsLateUpdate)
                    ActiveProjectiles[i].AsLateUpdate.NyuLateUpdate();

                for (int j = 0; j < ActiveProjectiles[i].Behaviors_LateUpdates.Count; j++)
                    ActiveProjectiles[i].Behaviors_LateUpdates[j].NyuLateUpdate();
            }
        }

        /// <summary>
        /// Calls <see cref="Init(int)"/> using <see cref="s_PoolSize"/> as argument.
        /// </summary>
        protected virtual void Start()
        {
            if (s_PoolSize > 0)
                Init(s_PoolSize);
        }
        #endregion

        #region Utility
        /// <summary>
        /// Returns a collection of all projectiles.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<T> GetAllProjectiles() => AllProjectiles;
        /// <summary>
        /// Returns a collection of all active projectiles.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<T> GetActiveProjectiles() => ActiveProjectiles.ToArray();
        /// <summary>
        /// Returns a collection of all inactive projectiles.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<T> GetInactiveProjectiles() => InactiveProjectiles.ToArray();

        /// <summary>
        /// Finds all projectiles matching the given condition.
        /// </summary>
        /// <param name="match"></param>
        /// <param name="searchIn"></param>
        /// <returns></returns>
        public T[] FindAll(Predicate<T> match, ProjectileSearch searchIn)
        {
            return searchIn switch
            {
                ProjectileSearch.ActiveProjectiles => ActiveProjectiles.FindAll(match).ToArray(),
                ProjectileSearch.InactiveProjectiles => InactiveProjectiles.FindAll(match).ToArray(),
                _ => Array.FindAll(AllProjectiles, match),
            };
        }

        /// <summary>
        /// Returns the first projectile matching the given condition.
        /// </summary>
        /// <param name="match"></param>
        /// <param name="searchIn"></param>
        /// <returns></returns>
        public T Find(Predicate<T> match, ProjectileSearch searchIn)
        {
            return searchIn switch
            {
                ProjectileSearch.ActiveProjectiles => ActiveProjectiles.Find(match),
                ProjectileSearch.InactiveProjectiles => InactiveProjectiles.Find(match),
                _ => Array.Find(AllProjectiles, match),
            };
        }
        #endregion

        /// <summary>
        /// Used by <see cref="Start"/> for <see cref="Init(int)"/>.
        /// </summary>
        [SerializeField, Tooltip("The pool size of projectiles."), Range(1, 10000)]
        private int s_PoolSize;

        /// <summary>
        /// All projectiles.
        /// </summary>
        private static T[] AllProjectiles;
        /// <summary>
        /// List of all active projectiles.
        /// </summary>
        private static List<T> ActiveProjectiles;
        /// <summary>
        /// List of all inactive projectiles.
        /// </summary>
        private static List<T> InactiveProjectiles;
    }
}
