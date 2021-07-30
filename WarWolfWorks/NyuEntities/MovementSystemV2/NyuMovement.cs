using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.MovementSystemV2
{
    /// <summary>
    /// Base class for movement of an entity.
    /// </summary>
    public abstract class NyuMovement : NyuComponent, INyuUpdate, INyuFixedUpdate, INyuLateUpdate, ILockable
    {
        /// <summary>
        /// Invoked when a velocity is added.
        /// </summary>
        public event Action<IVelocity> OnVelocityAdded;
        /// <summary>
        /// Invoked when a velocity is removed.
        /// </summary>
        public event Action<IVelocity> OnVelocityRemoved;

        /// <summary>
        /// The default velocity to be applies to this <see cref="Nyu"/>. (<see cref="Vector3.zero"/> by default)
        /// </summary>
        public virtual Vector3 DefaultVelocity => Vector3.zero;

        private readonly List<INyuUpdate> velocityUpdates = new List<INyuUpdate>();
        private readonly List<INyuFixedUpdate> velocityFixedUpdates = new List<INyuFixedUpdate>();
        private readonly List<INyuLateUpdate> velocityLateUpdates = new List<INyuLateUpdate>();

        [HideInInspector]
        private readonly List<IVelocity> ns_Velocities = new List<IVelocity>();
        /// <summary>
        /// Every velocity stacked onto the entity's movement.
        /// </summary>
        public IEnumerable<IVelocity> Velocities => ns_Velocities;

        /// <summary>
        /// Adds a velocity to this <see cref="NyuMovement"/>.
        /// </summary>
        /// <param name="velocity"></param>
        public void AddVelocity(IVelocity velocity)
        {
            ns_Velocities.Add(velocity);

            velocity.Init(this);

            if (velocity is INyuUpdate update)
                velocityUpdates.Add(update);
            if (velocity is INyuFixedUpdate @fixed)
                velocityFixedUpdates.Add(@fixed);
            if (velocity is INyuLateUpdate late)
                velocityLateUpdates.Add(late);

            OnVelocityAdded?.Invoke(velocity);
        }

        /// <summary>
        /// Adds a list of velocities to this <see cref="NyuMovement"/>.
        /// </summary>
        /// <param name="velocities"></param>
        public void AddVelocities(params IVelocity[] velocities)
        {
            for(int i = 0; i < velocities.Length; i++)
            {
                AddVelocity(velocities[i]);
            }
        }

        /// <summary>
        /// Adds an enumerable of velocities.
        /// </summary>
        /// <param name="velocities"></param>
        public void AddVelocities(IEnumerable<IVelocity> velocities)
        {
            foreach(IVelocity velocity in velocities)
            {
                AddVelocity(velocity);
            }
        }

        /// <summary>
        /// Removes an existing velocity.
        /// </summary>
        /// <param name="velocity"></param>
        /// <returns></returns>
        public bool RemoveVelocity(IVelocity velocity)
        {
            if(ns_Velocities.Remove(velocity))
            {
                if (velocity is INyuUpdate update)
                    velocityUpdates.Remove(update);
                if (velocity is INyuFixedUpdate @fixed)
                    velocityFixedUpdates.Remove(@fixed);
                if (velocity is INyuLateUpdate late)
                    velocityLateUpdates.Remove(late);

                OnVelocityRemoved?.Invoke(velocity);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a list of velocities.
        /// </summary>
        /// <param name="velocities"></param>
        /// <returns></returns>
        public bool[] RemoveVelocities(params IVelocity[] velocities)
        {
            bool[] toReturn = new bool[velocities.Length];

            for(int i = 0; i < velocities.Length; i++)
            {
                toReturn[i] = RemoveVelocity(velocities[i]);
            }

            return toReturn;
        }

        /// <summary>
        /// Removes a IEnumerable of velocities.
        /// </summary>
        /// <param name="velocities"></param>
        /// <returns></returns>
        public bool[] RemoveVelocities(IEnumerable<IVelocity> velocities)
        {
            IVelocity[] used = velocities.ToArray();

            return RemoveVelocities(used);
        }

        /// <summary>
        /// Returns true if the given velocity is contained within this <see cref="NyuMovement"/>.
        /// </summary>
        /// <returns></returns>
        public bool ContainsVelocity(IVelocity velocity)
        {
            return ns_Velocities.Contains(velocity);
        }

        /// <summary>
        /// Returns all velocities of given generic type.
        /// </summary>
        public List<T> GetAllVelocities<T>() where T : IVelocity
        {
            List<T> toReturn = new List<T>();
            foreach(IVelocity velocity in ns_Velocities)
            {
                if (velocity is T asT)
                    toReturn.Add(asT);
            }

            return toReturn;
        }

        /// <summary>
        /// Finds a velocity.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public IVelocity FindVelocity(Predicate<IVelocity> match)
            => ns_Velocities.Find(match);
        
        /// <summary>
        /// Finds all velocities that match the given condition.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public List<IVelocity> FindAllVelocities(Predicate<IVelocity> match)
            => ns_Velocities.FindAll(match);

        /// <summary>
        /// Removes all velocities matching the given predicate.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public int RemoveAllVelocities(Predicate<IVelocity> match)
            => ns_Velocities.RemoveAll(match);

        /// <summary>
        /// Final Velocity that will be applied to this entity.
        /// </summary>
        public virtual Vector3 UsedVelocity
        {
            get
            {
                if (Locked)
                    return default;

                Vector3 toReturn = DefaultVelocity;

                for (int i = 0; i < ns_Velocities.Count; i++)
                {
                    try 
                    {
                        Vector3 toAdd = ns_Velocities[i].GetValue();
                        toReturn += toAdd; 
                    } 
                    catch(Exception e) 
                    { 
                        AdvancedDebug.LogWarning("Skipped the calculation of a velocity as it generated an exception.");
                        e.LogException();
                    }
                }

                return toReturn;
            }
        }

        /// <summary>
        /// Moves the <see cref="Nyu"/> to the given position.
        /// </summary>
        /// <param name="position">World position to move the <see cref="Nyu"/> to.</param>
        /// <param name="respectPhysics">If true, this method will try to respect the world physics in your game. (E.G not move inside another collider but next to it, etc..)</param>
        public abstract void MovePosition(Vector3 position, bool respectPhysics);

        /// <summary>
        /// When overriding this class, make sure to call "base.NyuUpdate();" inside it as it
        /// calls all <see cref="INyuUpdate"/> velocities. (<see cref="INyuUpdate"/> implementation)
        /// </summary>
        public virtual void NyuUpdate()
        {
            if (Locked)
                return;

            for (int i = 0; i < velocityUpdates.Count; i++)
            {
                velocityUpdates[i].NyuUpdate();
            }
        }

        /// <summary>
        /// When overriding this class, make sure to call "base.NyuFixedUpdate();" inside it as it
        /// calls all <see cref="INyuFixedUpdate"/> velocities. (<see cref="INyuFixedUpdate"/> implementation)
        /// </summary>
        public virtual void NyuFixedUpdate()
        {
            if (Locked)
                return;

            for (int i = 0; i < velocityFixedUpdates.Count; i++)
            {
                velocityFixedUpdates[i].NyuFixedUpdate();
            }
        }

        /// <summary>
        /// When overriding this class, make sure to call "base.NyuLateUpdate();" inside it as it
        /// calls all <see cref="INyuLateUpdate"/> velocities. (<see cref="INyuLateUpdate"/> implementation)
        /// </summary>
        public virtual void NyuLateUpdate()
        {
            if (Locked)
                return;

            for (int i = 0; i < velocityLateUpdates.Count; i++)
            {
                velocityLateUpdates[i].NyuLateUpdate();
            }
        }

        #region ILockable implementation
        /// <summary>
        /// The object's Lock state; See <see cref="ILockable"/> for more info.
        /// </summary>
        public bool Locked { get; private set; }

        /// <summary>
        /// Called when the object is locked (<see cref="ILockable"/> implementation).
        /// </summary>
        public event Action<ILockable> OnLocked;
        /// <summary>
        /// Called when the object is unlocked (<see cref="ILockable"/> implementation).
        /// </summary>
        public event Action<ILockable> OnUnlocked;

        /// <summary>
        /// Locks or Unlocks this object (<see cref="ILockable"/> implementation).
        /// </summary>
        /// <param name="to"></param>
        public void SetLock(bool to)
        {
            if (to == Locked)
                return;

            Locked = to;
            if (Locked)
                OnLocked?.Invoke(this);
            else OnUnlocked?.Invoke(this);
        }
        #endregion
    }
}
