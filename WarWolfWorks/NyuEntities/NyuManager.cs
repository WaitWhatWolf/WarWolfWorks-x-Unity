using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WarWolfWorks.WWWResources;
using WarWolfWorks.Interfaces.NyuEntities;
using WarWolfWorks.Security;
using WarWolfWorks.Utility;
using WarWolfWorks.NyuEntities.Statistics;

namespace WarWolfWorks.NyuEntities
{
    /// <summary>
    /// Manages all entities, as well as provides a substantial amount of utility methods to retrieve entities.
    /// </summary>
    public static class NyuManager
    {
        #region Multi-Threading
        private static void WarningDebugNRE(string interfaceName, string objectName, Exception exception)
        {
            AdvancedDebug.LogWarningFormat("Couldn't call {0} of {1} as there was an exception; Aborting...", DEBUG_LAYER_WWW_INDEX
                , interfaceName, objectName, exception);
            AdvancedDebug.LogException(exception);
        }

        /// <summary>
        /// Invokes the update method of all entities.
        /// </summary>
        private static void Event_NyuUpdate()
        {
            for (int i = 0; i < AllUpdates.Count; i++)
            {
                if (!AllUpdates[i].Item1.enabled || AllUpdates[i].Item1.ns_DestroyedCorrectly)
                    continue;

                try
                {
                    AllUpdates[i].Item2.NyuUpdate();
                }
                catch (Exception e)
                {
                    WarningDebugNRE(nameof(INyuUpdate), AllEntities[i].GetType().Name, e);
                }
            }

            for (int j = 0; j < ComponentsUpdate.Count; j++)
            {
                try
                {
                    ComponentsUpdate[j].NyuUpdate();
                }
                catch (Exception e)
                {
                    WarningDebugNRE(nameof(INyuUpdate), ComponentsUpdate[j].GetType().Name, e);
                }
            }
        }

        /// <summary>
        /// Invokes the FixedUpdate method of all entities.
        /// </summary>
        /// <returns></returns>
        private static void Event_NyuFixedUpdate()
        {
            for (int i = 0; i < AllFixedUpdates.Count; i++)
            {
                if (!AllFixedUpdates[i].Item1.enabled)
                    continue;

                try
                {
                    AllFixedUpdates[i].Item2.NyuFixedUpdate();
                }
                catch (Exception e)
                {
                    WarningDebugNRE(nameof(INyuFixedUpdate), AllEntities[i].GetType().Name, e);
                }
            }

            for (int j = 0; j < ComponentsFixedUpdate.Count; j++)
            {
                try
                {
                    ComponentsFixedUpdate[j].NyuFixedUpdate();
                }
                catch (Exception e)
                {
                    WarningDebugNRE(nameof(INyuFixedUpdate), ComponentsFixedUpdate[j].GetType().Name, e);
                }
            }
        }
        /// <summary>
        /// Invokes the LateUpdate method of all entities.
        /// </summary>
        /// <returns></returns>
        private static void Event_NyuLateUpdate()
        {
            for (int i = 0; i < AllLateUpdates.Count; i++)
            {
                if (!AllLateUpdates[i].Item1.enabled)
                    continue;

                try
                {
                    AllLateUpdates[i].Item2.NyuLateUpdate();
                }
                catch (Exception e)
                {
                    WarningDebugNRE(nameof(INyuLateUpdate), AllEntities[i].GetType().Name, e);
                }
            }

            for (int j = 0; j < ComponentsLateUpdate.Count; j++)
            {
                try
                {
                    ComponentsLateUpdate[j].NyuLateUpdate();
                }
                catch (Exception e)
                {
                    WarningDebugNRE(nameof(INyuLateUpdate), ComponentsLateUpdate[j].GetType().Name, e);
                }
            }
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Returns true if any <see cref="Nyu"/> of given T type exists in the scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Exists<T>()
        {
            return AllEntities.FindIndex(c => c is T) != -1;
        }

        /// <summary>
        /// Returns true if any <see cref="Nyu"/> of given type exists in the scene.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool Exists(Type type)
        {
            return AllEntities.FindIndex(c => c.GetType().IsAssignableFrom(type)) != -1;
        }

        /// <summary>
        /// Finds an entity by match.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public static Nyu Find(Predicate<Nyu> match)
        {
            for (int i = 0; i < AllEntities.Count; i++)
                if (match(AllEntities[i]))
                    return AllEntities[i];

            return null;
        }

        /// <summary>
        /// Finds all entities that match the given condition.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public static List<Nyu> FindAll(Predicate<Nyu> match)
        {
            List<Nyu> toReturn = new List<Nyu>(AllEntities.Count);
            for (int i = 0; i < AllEntities.Count; i++)
                if (match(AllEntities[i]))
                    toReturn.Add(AllEntities[i]);

            return toReturn;
        }

        /// <summary>
        /// Returns an array of all entities in game.
        /// </summary>
        /// <returns></returns>
        public static Nyu[] GetAll() => AllEntities.ToArray();

        /// <summary>
        /// Returns true if an entity is of a given non-generic type.
        /// </summary>
        /// <param name="nyu"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsType(Nyu nyu, Type type)
            => type.IsAssignableFrom(nyu.GetType());

        /// <summary>
        /// Returns true if the entity is of any given non-generic types.
        /// </summary>
        /// <param name="nyu"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static bool IsTypeAny(Nyu nyu, IEnumerable<Type> types)
        {
            foreach (Type type in types)
                if (type.IsAssignableFrom(nyu.GetType()))
                    return true;

            return false;
        }

        #region Get Visible
        /// <summary>
        /// Gets all visible <see cref="Nyu"/> entities to a given camera.
        /// </summary>
        /// <param name="to"></param>
        /// <returns></returns>
        public static List<Nyu> GetAllVisible(Camera to)
        {
            List<Nyu> toReturn = new List<Nyu>(AllEntities.Count);
            for (int i = 0; i < AllEntities.Count; i++)
            {
                Renderer[] renderers = AllEntities[i].GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    if (Hooks.Rendering.IsVisibleFrom(renderer, to))
                    {
                        toReturn.Add(AllEntities[i]);
                        break;
                    }
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Gets all visible <see cref="Nyu"/> entities to a given camera within the specified max distance.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="within"></param>
        /// <returns></returns>
        public static List<Nyu> GetAllVisible(Camera to, float within)
        {
            List<Nyu> toReturn = new List<Nyu>(AllEntities.Count);
            for (int i = 0; i < AllEntities.Count; i++)
            {
                if (Vector3.Distance(to.transform.position, AllEntities[i].Position) < within)
                {
                    Renderer[] renderers = AllEntities[i].GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in renderers)
                    {
                        if (Hooks.Rendering.IsVisibleFrom(renderer, to))
                        {
                            toReturn.Add(AllEntities[i]);
                            break;
                        }
                    }
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Gets all visible <see cref="Nyu"/> entities of T type to a given camera.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="to"></param>
        /// <returns></returns>
        public static List<T> GetAllVisible<T>(Camera to) where T : Nyu
        {
            List<T> toReturn = new List<T>(AllEntities.Count);
            for (int i = 0; i < AllEntities.Count; i++)
            {
                if (AllEntities[i] is T tNyu)
                {
                    Renderer[] renderers = AllEntities[i].GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in renderers)
                    {
                        if (Hooks.Rendering.IsVisibleFrom(renderer, to))
                        {
                            toReturn.Add(tNyu);
                            break;
                        }
                    }
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Gets all visible <see cref="Nyu"/> entities of T type to a given camera within a specified max range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="to"></param>
        /// <param name="within"></param>
        /// <returns></returns>
        public static List<T> GetAllVisible<T>(Camera to, float within) where T : Nyu
        {
            List<T> toReturn = new List<T>(AllEntities.Count);
            for (int i = 0; i < AllEntities.Count; i++)
            {
                if (AllEntities[i] is T tNyu && Vector3.Distance(to.transform.position, tNyu.Position) <= within)
                {
                    Renderer[] renderers = AllEntities[i].GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in renderers)
                    {
                        if (Hooks.Rendering.IsVisibleFrom(renderer, to))
                        {
                            toReturn.Add(tNyu);
                            break;
                        }
                    }
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Gets all visible <see cref="Nyu"/> entities of T type to a given camera within a specified max range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="to"></param>
        /// <param name="within"></param>
        /// <returns></returns>
        public static T GetClosestVisible<T>(Camera to, float within) where T : Nyu
        {
            T toReturn = null;
            float previousDist = float.PositiveInfinity;

            for (int i = 0; i < AllEntities.Count; i++)
            {
                if (AllEntities[i] is T tNyu)
                {
                    float curDist = Vector3.Distance(to.transform.position, tNyu.Position);
                    if (curDist <= within && curDist < previousDist)
                    {
                        Renderer[] renderers = tNyu.GetComponentsInChildren<Renderer>();
                        foreach (Renderer renderer in renderers)
                        {
                            if (Hooks.Rendering.IsVisibleFrom(renderer, to))
                            {
                                toReturn = tNyu;
                                break;
                            }
                        }
                    }
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Gets all visible <see cref="Nyu"/> entities of T type to a given camera within a specified max range.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="within"></param>
        /// <param name="acceptedType"></param>
        /// <returns></returns>
        public static Nyu GetClosestVisible(Camera to, float within, Type acceptedType)
        {
            Nyu toReturn = null;
            float previousDist = float.PositiveInfinity;

            for (int i = 0; i < AllEntities.Count; i++)
            {
                if(IsType(AllEntities[i], acceptedType))
                {
                    float curDist = Vector3.Distance(to.transform.position, AllEntities[i].Position);
                    if (curDist <= within && curDist < previousDist)
                    {
                        Renderer[] renderers = AllEntities[i].GetComponentsInChildren<Renderer>();
                        foreach (Renderer renderer in renderers)
                        {
                            if (Hooks.Rendering.IsVisibleFrom(renderer, to))
                            {
                                toReturn = AllEntities[i];
                                break;
                            }
                        }
                    }
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Gets all visible <see cref="Nyu"/> entities of T type to a given camera within a specified max range.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="within"></param>
        /// <param name="acceptedTypes"></param>
        /// <returns></returns>
        public static Nyu GetClosestVisible(Camera to, float within, IEnumerable<Type> acceptedTypes)
        {
            Nyu toReturn = null;
            float previousDist = float.PositiveInfinity;

            for (int i = 0; i < AllEntities.Count; i++)
            {
                if (IsTypeAny(AllEntities[i], acceptedTypes))
                {
                    float curDist = Vector3.Distance(to.transform.position, AllEntities[i].Position);
                    if (curDist <= within && curDist < previousDist)
                    {
                        Renderer[] renderers = AllEntities[i].GetComponentsInChildren<Renderer>();
                        foreach (Renderer renderer in renderers)
                        {
                            if (Hooks.Rendering.IsVisibleFrom(renderer, to))
                            {
                                toReturn = AllEntities[i];
                                break;
                            }
                        }
                    }
                }
            }

            return toReturn;
        }
        #endregion

        #region Non-Generic Get Closest Methods
        /// <summary>
        /// Returns the closest entity to a given position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Nyu GetClosest(Vector3 position)
        {
            int index = -1;
            float lastDist = float.PositiveInfinity;
            for(int i = 0; i < AllEntities.Count; i++)
            {
                float curDist = Vector3.Distance(AllEntities[i].Position, position);
                if (curDist < lastDist)
                    index = i;
            }

            return index < 0 ? null : AllEntities[index];
        }

        /// <summary>
        /// Gets the closest <see cref="Nyu"/> to a given position within a specified max range.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="within"></param>
        /// <returns></returns>
        public static Nyu GetClosest(Vector3 position, float within)
        {
            int index = -1;
            float lastDist = within;
            for (int i = 0; i < AllEntities.Count; i++)
            {
                float curDist = Vector3.Distance(AllEntities[i].Position, position);
                if (curDist < lastDist)
                    index = i;
            }

            return index < 0 ? null : AllEntities[index];
        }

        /// <summary>
        /// Gets the closest <see cref="Nyu"/> of given type to a given position within a specified max range.
        /// (Note: <see cref="Nyu"/> entities assignable from the given type are also counted.)
        /// </summary>
        /// <param name="position"></param>
        /// <param name="within"></param>
        /// <param name="compareType"></param>
        /// <returns></returns>
        public static Nyu GetClosest(Vector3 position, float within, Type compareType)
        {
            int index = -1;
            float lastDist = within;
            for (int i = 0; i < AllEntities.Count; i++)
            {
                if(IsType(AllEntities[i], compareType))
                {
                    float curDist = Vector3.Distance(AllEntities[i].Position, position);
                    if (curDist < lastDist)
                        index = i;
                }
            }

            return index < 0 ? null : AllEntities[index];
        }

        /// <summary>
        /// Gets the closest <see cref="Nyu"/> of given types to the given position within a specified max range.
        /// (Note: <see cref="Nyu"/> entities assignable from the given types are also counted.)
        /// </summary>
        /// <param name="position"></param>
        /// <param name="within"></param>
        /// <param name="compareTypes"></param>
        /// <returns></returns>
        public static Nyu GetClosest(Vector3 position, float within, params Type[] compareTypes)
        {
            int index = -1;
            float lastDist = within;
            for (int i = 0; i < AllEntities.Count; i++)
            {
                if (IsTypeAny(AllEntities[i], compareTypes))
                {
                    float curDist = Vector3.Distance(AllEntities[i].Position, position);
                    if (curDist < lastDist)
                        index = i;
                }
            }

            return index < 0 ? null : AllEntities[index];
        }

        /// <summary>
        /// Gets the closest <see cref="Nyu"/> of given types to the given position within a specified max range.
        /// (Note: <see cref="Nyu"/> entities assignable from the given types are also counted.)
        /// </summary>
        /// <param name="position"></param>
        /// <param name="within"></param>
        /// <param name="compareTypes"></param>
        /// <returns></returns>
        public static Nyu GetClosest(Vector3 position, float within, IEnumerable<Type> compareTypes)
        {
            int index = -1;
            float lastDist = within;
            for (int i = 0; i < AllEntities.Count; i++)
            {
                if (IsTypeAny(AllEntities[i], compareTypes))
                {
                    float curDist = Vector3.Distance(AllEntities[i].Position, position);
                    if (curDist < lastDist)
                        index = i;
                }
            }

            return index < 0 ? null : AllEntities[index];
        }

        /// <summary>
        /// Returns all entities within the given distance. Only returns nyu entities of given type.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="within"></param>
        /// <param name="compareType"></param>
        /// <returns></returns>
        public static List<Nyu> GetAllWithin(Vector3 position, float within, Type compareType)
        {
            List<Nyu> toReturn = new List<Nyu>();
            for (int i = 0; i < AllEntities.Count; i++)
            {
                if (IsType(AllEntities[i], compareType))
                {
                    float curDist = Vector3.Distance(AllEntities[i].Position, position);
                    if (curDist <= within)
                        toReturn.Add(AllEntities[i]);
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Returns all entities within the given distance. Only returns nyu entities of given type.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="within"></param>
        /// <param name="compareTypes"></param>
        /// <returns></returns>
        public static List<Nyu> GetAllWithin(Vector3 position, float within, params Type[] compareTypes)
        {
            List<Nyu> toReturn = new List<Nyu>();
            for (int i = 0; i < AllEntities.Count; i++)
            {
                if (IsTypeAny(AllEntities[i], compareTypes))
                {
                    float curDist = Vector3.Distance(AllEntities[i].Position, position);
                    if (curDist <= within)
                        toReturn.Add(AllEntities[i]);
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Returns all entities within the given distance. Only returns nyu entities of given type.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="within"></param>
        /// <param name="compareTypes"></param>
        /// <returns></returns>
        public static List<Nyu> GetAllWithin(Vector3 position, float within, IEnumerable<Type> compareTypes)
        {
            List<Nyu> toReturn = new List<Nyu>();
            for (int i = 0; i < AllEntities.Count; i++)
            {
                if (IsTypeAny(AllEntities[i], compareTypes))
                {
                        float curDist = Vector3.Distance(AllEntities[i].Position, position);
                        if (curDist <= within)
                            toReturn.Add(AllEntities[i]);
                }
            }

            return toReturn;
        }
        #endregion

        #region Generic Get Closest Methods
        /// <summary>
        /// Returns the closest <see cref="Nyu"/> of T type to a given position.
        /// </summary>
        /// <typeparam name="T"><see cref="Nyu"/>'s type searched.</typeparam>
        /// <param name="position"></param>
        /// <returns></returns>
        public static T GetClosest<T>(Vector3 position) where T : Nyu
        {
            T toReturn = null;
            float lastDist = float.PositiveInfinity;
            for (int i = 0; i < AllEntities.Count; i++)
            {
                if (AllEntities[i] is T tNyu)
                {
                    float curDist = Vector3.Distance(tNyu.Position, position);
                    if (curDist < lastDist)
                        toReturn = tNyu;
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Gets the closest <see cref="Nyu"/> of T type to a given position within a specified max distance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <param name="within"></param>
        /// <returns></returns>
        public static T GetClosest<T>(Vector3 position, float within) where T : Nyu
        {
            T toReturn = null;
            float lastDist = within;
            for (int i = 0; i < AllEntities.Count; i++)
            {
                if (AllEntities[i] is T tNyu)
                {
                    float curDist = Vector3.Distance(tNyu.Position, position);
                    if (curDist < lastDist)
                        toReturn = tNyu;
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Gets all <see cref="Nyu"/> enitities of given T type within the given range of position.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <param name="within"></param>
        /// <returns></returns>
        public static List<T> GetAllWithin<T>(Vector3 position, float within) where T : Nyu
        {
            List<T> toReturn = new List<T>();
            for (int i = 0; i < AllEntities.Count; i++)
            {
                if (AllEntities[i] is T tNyu)
                {
                    float curDist = Vector3.Distance(tNyu.Position, position);
                    if (curDist <= within)
                        toReturn.Add(tNyu);
                }
            }

            return toReturn;
        }
        #endregion

        /// <summary>
        /// Returns the oldest parent of an Entity with <see cref="INyuEntityParentable"/> interface.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includeNonParentableParent"></param>
        /// <returns></returns>
        public static Nyu OldestOf(Nyu entity, bool includeNonParentableParent)
        {
            Nyu toReturn = entity;
            while (toReturn is INyuEntityParentable)
            {
                INyuEntityParentable par = (INyuEntityParentable)toReturn;
                if (par.NyuParent == null || (!includeNonParentableParent && !(par.NyuParent is INyuEntityParentable)))
                    break;

                toReturn = par.NyuParent;
            }

            return toReturn;
        }
        #endregion

        #region Instantiating / Destroying
        internal static List<Nyu> AllEntities = new List<Nyu>();
        internal static List<(Nyu, INyuUpdate)> AllUpdates = new List<(Nyu, INyuUpdate)>();
        internal static List<(Nyu, INyuFixedUpdate)> AllFixedUpdates = new List<(Nyu, INyuFixedUpdate)>();
        internal static List<(Nyu, INyuLateUpdate)> AllLateUpdates = new List<(Nyu, INyuLateUpdate)>();

        internal static List<INyuUpdate> ComponentsUpdate = new List<INyuUpdate>();
        internal static List<INyuFixedUpdate> ComponentsFixedUpdate = new List<INyuFixedUpdate>();
        internal static List<INyuLateUpdate> ComponentsLateUpdate = new List<INyuLateUpdate>();
        
        internal static void ManageNyuEntityUpdatesInternal(Nyu nyu, bool add)
        {
            if (nyu is INyuUpdate update)
            {
                if (add) AllUpdates.Add((nyu, update));
                else AllUpdates.Remove((nyu, update));
            }
            if (nyu is INyuFixedUpdate fixedUpdate)
            {
                if(add) AllFixedUpdates.Add((nyu, fixedUpdate));
                else AllFixedUpdates.Remove((nyu, fixedUpdate));
            }
            if (nyu is INyuLateUpdate lateUpdate)
            {
                if(add) AllLateUpdates.Add((nyu, lateUpdate));
                else AllLateUpdates.Remove((nyu, lateUpdate));
            }
        }

        /// <summary>
        /// Invoked when an entity is instantiated through any <see cref="NyuManager"/>.New method.
        /// </summary>
        public static event Action<Nyu> OnNyuBegin;

        /// <summary>
        /// Invoked when an entity is destroyed through <see cref="Destroy(Nyu)"/>. 
        /// (Does not get invoked when the entity is destroyed unofficially.)
        /// </summary>
        public static event Action<Nyu> OnNyuEnd;
        /// <summary>
        /// Invoked when an entity is destroyed unofficially.
        /// </summary>
        public static event Action<Nyu> OnNyuEndUnofficial;

        internal static void CallEntityBegin(Nyu of) => OnNyuBegin?.Invoke(of);

        /// <summary>
        /// Official method to instantiate a new <see cref="Nyu"/>.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Nyu New(Nyu prefab, Vector3 position, Quaternion rotation)
        {
            bool reenable = prefab.gameObject.activeSelf;
            prefab.gameObject.SetActive(false);

            Nyu toReturn = UnityEngine.Object.Instantiate(prefab, position, rotation);

            toReturn.Stats = new Stats(toReturn);
            toReturn.hs_Components = new List<INyuComponent>(toReturn.GetComponents<INyuComponent>());

            toReturn.CallInit();
            AllEntities.Add(toReturn);

            if (reenable)
            {
                prefab.gameObject.SetActive(true);
                toReturn.gameObject.SetActive(true);
            }

            if (toReturn.gameObject.activeInHierarchy)
                toReturn.CallAwake();

            ManageNyuEntityUpdatesInternal(toReturn, true);

            OnNyuBegin?.Invoke(toReturn);

            return toReturn;
        }

        /// <summary>
        /// Official method to instantiate a new <see cref="Nyu"/>.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static T New<T>(T prefab, Vector3 position, Quaternion rotation) where T : Nyu
        {
            bool reenable = prefab.gameObject.activeSelf;
            prefab.gameObject.SetActive(false);

            T toReturn = UnityEngine.Object.Instantiate(prefab, position, rotation);

            toReturn.Stats = new Stats(toReturn);
            toReturn.hs_Components = new List<INyuComponent>(toReturn.GetComponents<INyuComponent>());

            toReturn.CallInit();
            AllEntities.Add(toReturn);

            if (reenable)
            {
                prefab.gameObject.SetActive(true);
                toReturn.gameObject.SetActive(true);
            }

            if(toReturn.gameObject.activeInHierarchy)
                toReturn.CallAwake();

            ManageNyuEntityUpdatesInternal(toReturn, true);

            OnNyuBegin?.Invoke(toReturn);

            return toReturn;
        }

        /// <summary>
        /// Checks if an entity implements <see cref="INyuOnEnable"/> and/or <see cref="INyuOnDisable"/> and throws and exception if it does.
        /// </summary>
        /// <param name="entity"></param>
        internal static void Exception3Check(Nyu entity)
        {
            if (entity is INyuOnEnable || entity is INyuOnDisable)
                throw new NyuEntityException(3);
        }

        /// <summary>
        /// Official method to destroy an entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Destroy(Nyu entity)
        {
            if (entity == null)
                return false;

            entity.BeginDestroy();

            if (entity is INyuOnDestroyQueued entityDestroyQueued)
                entityDestroyQueued.NyuOnDestroyQueued();


            for (int i = 0; i < entity.hs_Components.Count; i++)
            {
                if (entity.hs_Components[i] is INyuOnDestroyQueued queue)
                    queue.NyuOnDestroyQueued();
            }

            ManageNyuEntityUpdatesInternal(entity, false);
            AllEntities.Remove(entity);

            for (int i = entity.hs_Components.Count - 1; i >= 0; i--)
            {
                if (entity.hs_Components[i] is INyuOnDestroy destroy)
                    destroy.NyuOnDestroy();

                entity.InternalManageNyuComponentRemoval(entity.hs_Components[i]);
            }

            if (entity is INyuOnDestroy entityDestroy)
                entityDestroy.NyuOnDestroy();

            entity.ns_DestroyedCorrectly = true;
            OnNyuEnd?.Invoke(entity);
            UnityEngine.Object.Destroy(entity.gameObject);

            return true;
        }

        /// <summary>
        /// Destroys the given entity without triggering any of it's destroy methods, including it's components.
        /// (Useful to despawn entities)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool DestroyUnofficially(Nyu entity)
        {
            if (entity == null)
                return false;

            ManageNyuEntityUpdatesInternal(entity, false);
            AllEntities.Remove(entity);

            entity.hs_Components.Clear();

            entity.ns_DestroyedCorrectly = true;
            OnNyuEndUnofficial?.Invoke(entity);
            UnityEngine.Object.Destroy(entity.gameObject);

            return true;
        }
        #endregion

        static NyuManager()
        {
            WWWResources.MonoManager_OnUpdate += Event_NyuUpdate;
            WWWResources.MonoManager_OnFixedUpdate += Event_NyuFixedUpdate;
            WWWResources.MonoManager_OnLateUpdate += Event_NyuLateUpdate;
        }
    }
}
