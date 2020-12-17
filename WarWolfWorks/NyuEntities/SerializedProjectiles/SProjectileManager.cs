using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.NyuEntities.SerializedProjectiles
{
    /// <summary>
    /// Base class for managing <see cref="SProjectile"/> objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CompleteNoS]
    public abstract class SProjectileManager<T> : MonoBehaviour, IInitiated where T : SProjectile
    {
        /// <summary>
        /// Used by <see cref="Awake"/> for <see cref="Init(int)"/>.
        /// </summary>
        [SerializeField, Tooltip("The pool size of projectiles. If at less than 1, the pool will not be initiated.")]
        protected int s_PoolSize;

        private T[] AllProjectiles;
        private List<T> InactiveProjectiles;
        private List<T> ActiveProjectiles;

        /// <summary>
        /// The initiated state of this projectile manager.
        /// </summary>
        public bool Initiated { get; private set; }
        
        /// <summary>
        /// Calls <see cref="Init(int)"/> when <see cref="s_PoolSize"/> is higher than 0.
        /// </summary>
        protected virtual void Awake()
        {
            if(s_PoolSize > 0)
            {
                Init(s_PoolSize);
            }
        }


        /// <summary>
        /// Initiates the projectile manager.
        /// </summary>
        /// <param name="poolSize"></param>
        public void Init(int poolSize)
        {
            if(Initiated)
            {
                for(int i = 0; i < AllProjectiles.Length; i++)
                {
                    Destroy(AllProjectiles[i].gameObject);
                }
            }

            AllProjectiles = new T[poolSize];
            InactiveProjectiles = new List<T>(poolSize);
            ActiveProjectiles = new List<T>(poolSize);

            for(int i = 0; i < poolSize; i++)
            {
                GameObject p = new GameObject(VN_PROJECTILE);
                p.transform.SetParent(transform);
                p.SetActive(false);
                AllProjectiles[i] = p.AddComponent<T>();
                AllProjectiles[i].SetSProjectileUpdates();
                AllProjectiles[i].OnInit(this);
                InactiveProjectiles.Add(AllProjectiles[i]);
            }
        }

        /// <summary>
        /// Call this at the beginning of your projectile instantiation method, and continue the initiation when it returns true.
        /// </summary>
        /// <param name="parent">The caster of this projectile.</param>
        /// <param name="position">Starting position of the projectile.</param>
        /// <param name="rotation">Starting rotation of the projectile.</param>
        /// <param name="projectile">Returns the projectile used.</param>
        /// <param name="behaviors">Behaviors set to the projectile. (All behaviors are instantiated before being used to avoid overriding resources.)</param>
        /// <returns></returns>
        protected bool New(Nyu parent, Vector3 position, Quaternion rotation, out T projectile, IEnumerable<Behavior> behaviors)
        {
            if(InactiveProjectiles.Count == 0)
            {
                projectile = null;
                return false;
            }

            projectile = InactiveProjectiles[0];
            projectile.transform.position = position;
            projectile.transform.rotation = rotation;
            projectile.NyuMain = parent;
            projectile.Behaviors = behaviors.RemoveNull().ToArray();

            InactiveProjectiles.RemoveAt(0);
            projectile.gameObject.SetActive(true);
            ActiveProjectiles.Add(projectile);

            if (projectile is INyuAwake awake)
                awake.NyuAwake();

            for(int i = 0; i < projectile.Behaviors.Length; i++)
            {
                if (projectile.Behaviors[i] is ICloneInstructable cloneInstructable)
                {
                    object[] instructions = cloneInstructable.GetInstructions();
                    projectile.Behaviors[i] = Instantiate(projectile.Behaviors[i]);
                    ((ICloneInstructable)projectile.Behaviors[i]).SetInstructions(instructions);
                }
                else projectile.Behaviors[i] = Instantiate(projectile.Behaviors[i]);

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
        /// Ends a projectile.
        /// </summary>
        /// <param name="projectile"></param>
        public bool End(T projectile)
        {
            if(ActiveProjectiles.Contains(projectile))
            {
                if (projectile is INyuOnDestroyQueued destroyQueued)
                    destroyQueued.NyuOnDestroyQueued();

                for(int j = 0; j < projectile.Behaviors.Length; j++)
                {
                    if(projectile.Behaviors[j] is INyuOnDestroy bDestroy)
                        bDestroy.NyuOnDestroy();

                    Destroy(projectile.Behaviors[j]);
                }
                
                if (projectile is INyuOnDestroy onDestroy)
                    onDestroy.NyuOnDestroy();   

                projectile.gameObject.SetActive(false);
                projectile.Behaviors = new Behavior[0];
                ActiveProjectiles.Remove(projectile);
                InactiveProjectiles.Add(projectile);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Ends a projectile.
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns></returns>
        public bool End(SProjectile projectile)
        {
            if (projectile is T tProj && ActiveProjectiles.Contains(tProj))
            {
                for (int j = 0; j < projectile.Behaviors.Length; j++)
                {
                    if (projectile.Behaviors[j] is INyuOnDestroy bDestroy)
                        bDestroy.NyuOnDestroy();

                    Destroy(projectile.Behaviors[j]);
                }

                projectile.gameObject.SetActive(false);
                projectile.Behaviors = new Behavior[0];
                ActiveProjectiles.Remove(tProj);
                InactiveProjectiles.Add(tProj);

                return true;
            }

            return false;
        }

        #region Calling
        /// <summary>
        /// When overriding, make sure to include base.Update() as it calls all <see cref="INyuUpdate.NyuUpdate"/> methods
        /// in applicable <see cref="Behavior"/> components.
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
        /// When overriding, make sure to include base.FixedUpdate() as it calls all <see cref="INyuFixedUpdate.NyuFixedUpdate"/> methods
        /// in applicable <see cref="Behavior"/> components.
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
        /// When overriding, make sure to include base.LateUpdate() as it calls all <see cref="INyuLateUpdate.NyuLateUpdate"/> methods
        /// in applicable <see cref="Behavior"/> components.
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
        #endregion

        #region Utility
        /// <summary>
        /// Returns an array of all active projectiles.
        /// </summary>
        /// <returns></returns>
        public T[] GetActiveProjectiles() => ActiveProjectiles.ToArray();
        /// <summary>
        /// Returns an array of all inactive projectiles.
        /// </summary>
        /// <returns></returns>
        public T[] GetInactiveProjectiles() => InactiveProjectiles.ToArray();

        /// <summary>
        /// Finds all projectiles matching the given condition.
        /// </summary>
        /// <param name="match"></param>
        /// <param name="searchIn"></param>
        /// <returns></returns>
        public T[] FindAll(Predicate<T> match, Search searchIn)
        {
            switch(searchIn)
            {
                default:
                    return Array.FindAll(AllProjectiles, match);
                case Search.ActiveProjectiles:
                    return ActiveProjectiles.FindAll(match).ToArray();
                case Search.InactiveProjectiles:
                    return InactiveProjectiles.FindAll(match).ToArray();
            }
        }

        /// <summary>
        /// Returns the first projectile matching the given condition.
        /// </summary>
        /// <param name="match"></param>
        /// <param name="searchIn"></param>
        /// <returns></returns>
        public T Find(Predicate<T> match, Search searchIn)
        {
            switch (searchIn)
            {
                default:
                    return Array.Find(AllProjectiles, match);
                case Search.ActiveProjectiles:
                    return ActiveProjectiles.Find(match);
                case Search.InactiveProjectiles:
                    return InactiveProjectiles.Find(match);
            }
        }
        #endregion
    }
}
