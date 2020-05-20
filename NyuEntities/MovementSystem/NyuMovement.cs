using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.NyuEntities.MovementSystem
{
    /// <summary>
    /// Base class for movement of an entity.
    /// </summary>
    public abstract class NyuMovement : NyuComponent, INyuFixedUpdate, ILockable
    {
        /// <summary>
        /// The default velocity to be applies to this <see cref="Nyu"/>. (<see cref="Vector3.zero"/> by default)
        /// </summary>
        public virtual Vector3 DefaultVelocity => Vector3.zero;

        /// <summary>
        /// Boolean which returns true if the <see cref="Nyu"/> is moving. (Doesn't have any function  by itself, if you want it to be functional you would need to give it the functionality yourself).
        /// </summary>
        public bool IsMoving { get; protected set; }

        [SerializeField][HideInInspector]
        private List<Velocity> hs_Velocities = new List<Velocity>();
        /// <summary>
        /// Every velocity stacked onto the entity's movement.
        /// </summary>
        public IEnumerable<Velocity> Velocities => hs_Velocities;

        /// <summary>
        /// Adds a velocity to this <see cref="NyuMovement"/>.
        /// </summary>
        /// <param name="velocity"></param>
        public void AddVelocity(Velocity velocity)
        {
            hs_Velocities.Add(velocity);
            OnVelocityAdded?.Invoke(velocity);
        }

        /// <summary>
        /// Adds a list of velocities to this <see cref="NyuMovement"/>.
        /// </summary>
        /// <param name="velocities"></param>
        public void AddVelocities(params Velocity[] velocities)
        {
            hs_Velocities.AddRange(velocities);
            for(int i = 0; i < velocities.Length; i++)
            {
                OnVelocityAdded?.Invoke(velocities[i]);
            }
        }

        /// <summary>
        /// Adds an enumerable of velocities.
        /// </summary>
        /// <param name="velocities"></param>
        public void AddVelocities(IEnumerable<Velocity> velocities)
        {
            hs_Velocities.AddRange(velocities);
            foreach(Velocity velocity in velocities)
            {
                OnVelocityAdded?.Invoke(velocity);
            }
        }

        /// <summary>
        /// Removes an existing velocity.
        /// </summary>
        /// <param name="velocity"></param>
        /// <returns></returns>
        public bool RemoveVelocity(Velocity velocity)
        {
            if(hs_Velocities.Remove(velocity))
            {
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
        public bool[] RemoveVelocities(params Velocity[] velocities)
        {
            bool[] toReturn = new bool[velocities.Length];

            for(int i = 0; i < velocities.Length; i++)
            {
                Velocity toRemove = velocities[i];
                if(toReturn[i] = hs_Velocities.Remove(toRemove))
                    OnVelocityRemoved?.Invoke(toRemove);
            }

            return toReturn;
        }

        /// <summary>
        /// Removes a IEnumerable of velocities.
        /// </summary>
        /// <param name="velocities"></param>
        /// <returns></returns>
        public bool[] RemoveVelocities(IEnumerable<Velocity> velocities)
        {
            Velocity[] used = velocities.ToArray();
            bool[] toReturn = new bool[used.Length];

            for (int i = 0; i < used.Length; i++)
            {
                Velocity toRemove = used[i];
                if(toReturn[i] = hs_Velocities.Remove(toRemove))
                    OnVelocityRemoved?.Invoke(toRemove);
            }

            return toReturn;
        }

        /// <summary>
        /// Returns true if the given velocity is contained within this <see cref="NyuMovement"/>.
        /// </summary>
        /// <returns></returns>
        public bool ContainsVelocity(Velocity velocity)
        {
            return hs_Velocities.Contains(velocity);
        }

        /// <summary>
        /// Finds a velocity.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public Velocity FindVelocity(Predicate<Velocity> match)
            => hs_Velocities.Find(match);
        
        /// <summary>
        /// Finds all velocities that match the given condition.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public List<Velocity> FindAllVelocities(Predicate<Velocity> match)
            => hs_Velocities.FindAll(match);

        /// <summary>
        /// Removes all velocities matching the given predicate.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public int RemoveAllVelocities(Predicate<Velocity> match)
            => hs_Velocities.RemoveAll(match);

        /// <summary>
        /// Invoked when a velocity is added.
        /// </summary>
        public event Action<Velocity> OnVelocityAdded;
        /// <summary>
        /// Invoked when a velocity is removed.
        /// </summary>
        public event Action<Velocity> OnVelocityRemoved;

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

                for (int i = 0; i < hs_Velocities.Count; i++)
                {
                    float multi = hs_Velocities[i].UsesStat ? NyuMain.Stats.CalculatedValue(1, hs_Velocities[i].Affections) : 1;
                    toReturn += hs_Velocities[i].Value * multi;
                }

                return toReturn;
            }
        }

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


        /// <summary>
        /// Moves the <see cref="Nyu"/> to the given position.
        /// </summary>
        /// <param name="position">World position to move the <see cref="Nyu"/> to.</param>
        /// <param name="respectPhysics">If true, this method will try to respect the world physics in your game. (E.G not move inside another collider but next to it, etc..)</param>
        public abstract void MovePosition(Vector3 position, bool respectPhysics);

        /// <summary>
        /// When overriding this class, make sure to call base.OnFixed() inside it as it is what 
        /// calculates the time left in Velocities through <see cref="Velocity.Time"/>. (<see cref="INyuFixedUpdate"/> implementation)
        /// </summary>
        public virtual void NyuFixedUpdate()
        {
            if (Locked)
                return;

            for (int i = 0; i < hs_Velocities.Count; i++)
            {
                hs_Velocities[i].Time = Mathf.Clamp(hs_Velocities[i].Time - Time.deltaTime, 0, hs_Velocities[i].StartTime);
                if (hs_Velocities[i].Time <= 0 && hs_Velocities[i].DeleteOnCount0)
                    RemoveVelocity(hs_Velocities[i]);
            }
        }
    }
}
