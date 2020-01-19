using UnityEngine;
using System.Collections.Generic;
using System;
using WarWolfWorks.Utility;
using WarWolfWorks.Interfaces;
using System.Linq;

namespace WarWolfWorks.EntitiesSystem
{
    /// <summary>
    /// Utility class based around Entities.
    /// </summary>
    public sealed class EntityManager : Singleton<EntityManager>
    {
        internal static List<Entity> InitiatedEntities = new List<Entity>();
        /// <summary>
        /// Finds an existing entity based on Predicate given.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public static Entity Find(Predicate<Entity> match) => InitiatedEntities.Find(match);

        /// <summary>
        /// Finds all entities who match the given condition.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public static List<Entity> FindAll(Predicate<Entity> match) => InitiatedEntities.FindAll(match);

        /// <summary>
        /// Returns true if an entity of given type exists inside the scene.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool Exists(Type type) => Find(e => e.EntityType == type);

        /// <summary>
        /// Gets the closest <see cref="Entity"/> to position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Entity GetClosestEntity(Vector3 position)
        {
            float lastDist = Mathf.Infinity;
            Entity toReturb = null;
            foreach (Entity e in InitiatedEntities)
            {
                float curDist = Vector3.Distance(e.Position, position);
                if (lastDist > curDist)
                {
                    lastDist = curDist;
                    toReturb = e;
                }
            }

            return toReturb;
        }

        /// <summary>
        /// Gets the closest <see cref="Entity"/> to position of specified T type.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Entity GetClosestEntity<T>(Vector3 position)
        {
            float lastDist = Mathf.Infinity;
            Entity toReturb = null;
            foreach (Entity e in InitiatedEntities)
            {
                if (!e.IsEntity(typeof(T)))
                    continue;
                float curDist = Vector3.Distance(e.Position, position);
                if (lastDist > curDist)
                {
                    lastDist = curDist;
                    toReturb = e;
                }
            }

            return toReturb;
        }

        /// <summary>
        /// Gets the closest <see cref="Entity"/> in within range from position.
        /// </summary>
        /// <param name="within"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Entity GetClosestEntity(float within, Vector3 position)
        {
            float lastDist = within;
            Entity toReturb = null;
            foreach (Entity e in InitiatedEntities)
            {
                float curDist = Vector3.Distance(e.Position, position);
                if (lastDist > curDist)
                {
                    lastDist = curDist;
                    toReturb = e;
                }
            }

            return toReturb;
        }

        /// <summary>
        /// Gets the closest <see cref="Entity"/> in within range from position of given type.
        /// </summary>
        /// <param name="within"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Entity GetClosestEntity<T>(float within, Vector3 position)
        {
            float lastDist = within;
            Entity toReturn = null;
            foreach (Entity e in InitiatedEntities)
            {
                if (!e.IsEntity(typeof(T)))
                    continue;
                float curDist = Vector3.Distance(e.Position, position);
                if (lastDist > curDist)
                {
                    lastDist = curDist;
                    toReturn = e;
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Gets the closest <see cref="Entity"/> in within range from position of given type.
        /// </summary>
        /// <param name="within"></param>
        /// <param name="position"></param>
        /// <param name="of"></param>
        /// <returns></returns>
        public static Entity GetClosestEntity(float within, Vector3 position, Type of)
        {
            float lastDist = within;
            Entity toReturn = null;
            foreach (Entity e in InitiatedEntities)
            {
                if (!e.IsEntity(of))
                    continue;
                float curDist = Vector3.Distance(e.Position, position);
                if (lastDist > curDist)
                {
                    lastDist = curDist;
                    toReturn = e;
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Gets the closest <see cref="Entity"/> in within range from position of given type.
        /// </summary>
        /// <param name="within"></param>
        /// <param name="position"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static Entity GetClosestEntity(float within, Vector3 position, IEnumerable<Type> types)
        {
            float lastDist = within;
            Entity toReturn = null;
            foreach (Entity e in InitiatedEntities)
            {
                if (!types.Contains(e.EntityType))
                    continue;
                float curDist = Vector3.Distance(e.Position, position);
                if (lastDist > curDist)
                {
                    lastDist = curDist;
                    toReturn = e;
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Gets all entities in within range from position.
        /// </summary>
        /// <param name="within"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static List<Entity> GetClosestEntities(float within, Vector3 position)
        {
            var entities = InitiatedEntities.FindAll(t =>
            Vector3.Distance(position, t.Position) <= within);
            return entities;
        }

        /// <summary>
        /// Gets all entities in within range from position of given type.
        /// </summary>
        /// <param name="within"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static List<Entity> GetClosestEntities<T>(float within, Vector3 position)
        {
            var entities = InitiatedEntities.FindAll(t =>
            Vector3.Distance(position, t.Position) <= within && t.IsEntity(typeof(T)));
            return entities;
        }

        /// <summary>
        /// Gets all entities in within range from position of given type.
        /// </summary>
        /// <param name="within"></param>
        /// <param name="position"></param>
        /// <param name="of"></param>
        /// <returns></returns>
        public static List<Entity> GetClosestEntities(float within, Vector3 position, Type of)
        {
            var entities = InitiatedEntities.FindAll(t =>
            Vector3.Distance(position, t.Position) <= within && t.IsEntity(of));
            return entities;
        }

        /// <summary>
        /// Gets all entities in within range from position of given types.
        /// </summary>
        /// <param name="within"></param>
        /// <param name="position"></param>
        /// <param name="ofTypes"></param>
        /// <returns></returns>
        public static List<Entity> GetClosestEntities(float within, Vector3 position, IEnumerable<Type> ofTypes)
        {
            var entities = InitiatedEntities.FindAll(t =>
            Vector3.Distance(position, t.Position) <= within &&
            ofTypes.FirstOrDefault(ty => ty == t.EntityType) != null);

            return entities;
        }


        /// <summary>
        /// Returns all entities visible to a camera. Only works on entities with a renderer component.
        /// Rather slow, avoid using on any Update method.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static List<Entity> GetAllWithinView(Camera from)
        {
            List<Entity> toReturn = new List<Entity>();
            foreach (Entity e in InitiatedEntities)
            {
                if (!Hooks.Rendering.IsVisibleFrom(e.GetComponent<Renderer>(), from))
                    continue;

                toReturn.Add(e);
            }

            return toReturn;
        }

        /// <summary>
        /// Returns all entities of given type visible to a camera. Only works on entities with a renderer component.
        /// Rather slow, avoid using on any Update method.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="of"></param>
        /// <returns></returns>
        public static List<Entity> GetAllWithinView(Camera from, Type of)
        {
            List<Entity> toReturn = new List<Entity>();
            foreach (Entity e in InitiatedEntities)
            {
                if (!Hooks.Rendering.IsVisibleFrom(e.GetComponent<Renderer>(), from))
                    continue;

                if (e.IsEntity(of))
                    toReturn.Add(e);
            }

            return toReturn;
        }

        /// <summary>
        /// Returns all entities of given T type visible to a camera. Only works on entities with a renderer component.
        /// Rather slow, avoid using on any Update method.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static List<Entity> GetAllWithinView<T>(Camera from)
        {
            List<Entity> toReturn = new List<Entity>();
            foreach (Entity e in InitiatedEntities)
            {
                if (!Hooks.Rendering.IsVisibleFrom(e.GetComponent<Renderer>(), from))
                    continue;

                if (e.IsEntity(typeof(T)))
                    toReturn.Add(e);
            }

            return toReturn;
        }

        /// <summary>
        /// Gets all entities inside of bounds.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IEnumerable<Entity> GetAllWithinBounds(Vector3 center, Vector3 size)
            => FindAll(e => Hooks.Vectors.IsInsideBounds(e.Position, center, size));

        /// <summary>
        /// Gets all entities of given generic type inside of bounds.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IEnumerable<Entity> GetAllWithinBounds<T>(Vector3 center, Vector3 size)
            => FindAll(e => Hooks.Vectors.IsInsideBounds(e.Position, center, size) && e.IsEntity(typeof(T)));

        /// <summary>
        /// Gets all entities of given type inside of bounds.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="of"></param>
        /// <returns></returns>
        public static IEnumerable<Entity> GetAllWithinBounds(Vector3 center, Vector3 size, Type of)
            => FindAll(e => Hooks.Vectors.IsInsideBounds(e.Position, center, size) && e.EntityType == of);

        /// <summary>
        /// Gets all entities of given types inside of bounds.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="of"></param>
        /// <returns></returns>
        public static IEnumerable<Entity> GetAllWithinBounds(Vector3 center, Vector3 size, IEnumerable<Type> of)
            => FindAll(e => Hooks.Vectors.IsInsideBounds(e.Position, center, size) && of.Contains(e.EntityType));

        /// <summary>
        /// Gets all entities currently inside the scene (Includes inactive/disabled Entities).
        /// </summary>
        public static Entity[] GetAllEntities => InitiatedEntities.ToArray();

        /// <summary>
        /// Gets all entities currently inside the scene into a <see cref="IList{T}"/> (Includes inactive/disabled Entities).
        /// </summary>
        public static IList<Entity> GetAllEntitiesList => InitiatedEntities;

        /// <summary>
        /// Gets all disabled entites inside the scene.
        /// </summary>
        public static Entity[] GetAllDisabledEntities => InitiatedEntities.FindAll(e => !e.enabled || !e.gameObject.activeSelf).ToArray();

        /// <summary>
        /// Gets all enabled entites inside the scene.
        /// </summary>
        public static Entity[] GetAllEnabledEntities => InitiatedEntities.FindAll(e => e.enabled && e.gameObject.activeSelf).ToArray();

        /// <summary>
        /// Returns the oldest parent of an Entity with <see cref="IEntityParentable"/> interface.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includeNonParentableParent"></param>
        /// <returns></returns>
        public static Entity OldestOf(Entity entity, bool includeNonParentableParent)
        {
            Entity toReturn = entity;
            while(toReturn is IEntityParentable)
            {
                IEntityParentable par = (IEntityParentable)toReturn;
                if (par.Parent == null || (!includeNonParentableParent && !(par.Parent is IEntityParentable)))
                    break;

                toReturn = par.Parent;
            }

            return toReturn;
        }
    }
}