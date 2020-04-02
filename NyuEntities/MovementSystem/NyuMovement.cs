using System;
using System.Collections.Generic;
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
        public List<Velocity> Velocities => hs_Velocities;

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

                for (int i = 0; i < Velocities.Count; i++)
                {
                    float multi = Velocities[i].UsesStat ? NyuMain.Stats.CalculatedValue(1, Velocities[i].Affections) : 1;
                    toReturn += Velocities[i].Value * multi;
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

            for (int i = 0; i < Velocities.Count; i++)
            {
                Velocities[i].Time = Mathf.Clamp(Velocities[i].Time - Time.deltaTime, 0, Velocities[i].StartTime);
                if (Velocities[i].Time <= 0 && Velocities[i].DeleteOnCount0)
                    Velocities.RemoveAt(i);
            }
        }
    }
}
