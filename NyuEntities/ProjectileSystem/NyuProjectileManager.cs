using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WarWolfWorks.Interfaces.UnityMethods;
using WarWolfWorks.Security;
using static WarWolfWorks.Constants;

namespace WarWolfWorks.NyuEntities.ProjectileSystem
{
    /// <summary>
    /// The manager of all <see cref="NyuProjectile"/> objects.
    /// </summary>
    public abstract class NyuProjectileManager<T> : MonoBehaviour where T : NyuProjectile
    {
        #region Pool
        /// <summary>
        /// Singleton instance of the manager; Set in inheriting classes.
        /// </summary>
        protected static NyuProjectileManager<T> Instance;

        /// <summary>
        /// Returns true if <see cref="Initiate(int)"/> was previously called.
        /// </summary>
        public static bool Populated { get; private set; }

        /// <summary>
        /// The transform which groups all projectiles together.
        /// </summary>
        public static Transform ProjectileHolder { get; private set; }

        /// <summary>
        /// Initiates this ProjectileManager. Can be re-invoked to change the pool size; Destroys all previous projectiles in doing so.
        /// </summary>
        public static void Initiate(int population)
        {
            if (population < 1)
                throw new NyuProjectileException(1);

            if (Instance == null)
                throw new NyuProjectileException(2);

            if (Populated)
            {
                for (int i = 0; i < Projectiles.Length; i++)
                {
                    Destroy(Projectiles[i].gameObject);
                }

                EnumeratorReset();
            }

            Projectiles = new T[population];
            InactiveProjectiles = new List<T>(population);
            ActiveProjectiles = new List<T>(population);

            lock (Projectiles)
            {
                if (!Populated) ProjectileHolder = new GameObject(VARN_PROJECTILE_HOLDER).transform;
                for (int i = 0; i < population; i++)
                {
                    GameObject g = new GameObject("Projectile_" + (i + 1).ToString());
                    g.SetActive(false);
                    g.transform.SetParent(ProjectileHolder);

                    T projectile = g.AddComponent<T>();
                    Projectiles[i] = projectile;
                    Instance.OnProjectileCreated(projectile);
                    InactiveProjectiles.Add(Projectiles[i]);
                }

                EnumeratorReset();
                DontDestroyOnLoad(Instance);
                Populated = true;
            }
        }
        
        /// <summary>
        /// Invoked when a projectile is added into the pool through <see cref="Initiate(int)"/>.
        /// </summary>
        /// <param name="projectile"></param>
        protected abstract void OnProjectileCreated(T projectile);

        /// <summary>
        /// Use this method at the beginning of your creation method to retrieve a projectile from the pool.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="projectile"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="behaviors"></param>
        protected static void New(Nyu owner, out T projectile, Vector3 position, Quaternion rotation, IEnumerable<NyuProjectile.Behavior> behaviors)
        {
            if (!Populated)
                throw new NyuProjectileException(3);

            Start:
            if (!EnumeratorMoveNext())
            {
                EnumeratorReset();
                goto Start;
            }

            projectile = Projectiles[EnumeratorIndex];
            projectile.NyuMain = owner;
            projectile.Position = position;
            projectile.Rotation = rotation;
            projectile.ns_Behaviors = behaviors.ToArray();

            InactiveProjectiles.Remove(projectile);
            ActiveProjectiles.Add(projectile);

            foreach(NyuProjectile.Behavior behavior in projectile.Behaviors)
            {
                if (behavior is IAwake behaviorAwake)
                    behaviorAwake.Awake();
            }
        }

        /// <summary>
        /// Removes a given projectile from the pool of active projectiles.
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns></returns>
        public static bool End(T projectile)
        {
            if (!Populated)
                throw new NyuProjectileException(3);

            if (ActiveProjectiles.Remove(projectile))
            {
                if (projectile is IOnDestroy projectileOnDestroy)
                    projectileOnDestroy.OnDestroy();

                InactiveProjectiles.Add(projectile);
                return true;
            }

            return false;
        }
        #endregion

        #region Calling
        /// <summary>
        /// When overriding, make sure to include base.Update() as it calls all <see cref="IUpdate.Update"/> methods
        /// in applicable <see cref="NyuProjectile.Behavior"/> components.
        /// </summary>
        protected virtual void Update()
        {
            for(int i = 0; i < Projectiles.Length; i++)
            {
                if (Projectiles[i] is IUpdate projectileUpdate)
                    projectileUpdate.Update();
            }
        }

        /// <summary>
        /// When overriding, make sure to include base.FixedUpdate() as it calls all <see cref="IFixedUpdate.FixedUpdate"/> methods
        /// in applicable <see cref="NyuProjectile.Behavior"/> components.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            for (int i = 0; i < Projectiles.Length; i++)
            {
                if (Projectiles[i] is IFixedUpdate projectileFixedUpdate)
                    projectileFixedUpdate.FixedUpdate();
            }
        }

        /// <summary>
        /// When overriding, make sure to include base.LateUpdate() as it calls all <see cref="ILateUpdate.LateUpdate"/> methods
        /// in applicable <see cref="NyuProjectile.Behavior"/> components.
        /// </summary>
        protected virtual void LateUpdate()
        {
            for (int i = 0; i < Projectiles.Length; i++)
            {
                if (Projectiles[i] is ILateUpdate projectileLateUpdate)
                    projectileLateUpdate.LateUpdate();
            }
        }
        #endregion

        #region Enumeration
        /// <summary>
        /// All projectiles.
        /// </summary>
        private static T[] Projectiles;
        /// <summary>
        /// List of all active projectiles.
        /// </summary>
        private static List<T> ActiveProjectiles;
        /// <summary>
        /// List of all inactive projectiles.
        /// </summary>
        private static List<T> InactiveProjectiles;
        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> of all active projectiles.
        /// </summary>
        /// <returns></returns>
        public static T[] GetActiveProjectiles() => ActiveProjectiles.ToArray();
        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> of all inactive projectiles.
        /// </summary>
        /// <returns></returns>
        public static T[] GetInactiveProjectiles() => InactiveProjectiles.ToArray();

        /// <summary>
        /// Finds the first projectile that matches the given condition.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public static T Find(Predicate<T> match)
        {
            foreach (T projectile in Projectiles)
                if (match(projectile))
                    return projectile;

            return null;
        }

        /// <summary>
        /// Returns all projectiles which match the given condition.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public static List<T> FindAll(Predicate<T> match)
        {
            List<T> toReturn = new List<T>(Projectiles.Length);

            foreach (T projectile in Projectiles)
                if (match(projectile))
                    toReturn.Add(projectile);

            return toReturn;
        }
        #endregion

        #region Enumerator Simulation
        private static int EnumeratorIndex { get; set; }

        /// <summary>
        /// Enumerator-like implementation for getting a new projectile for instantiation.
        /// </summary>
        /// <returns></returns>
        private static bool EnumeratorMoveNext()
        {
            EnumeratorIndex++;

            if (EnumeratorIndex < Projectiles.Length)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Enumerator-like implementation to reset the itteration.
        /// </summary>
        private static void EnumeratorReset()
        {
            EnumeratorIndex = -1;
        }
        #endregion
    }
}
