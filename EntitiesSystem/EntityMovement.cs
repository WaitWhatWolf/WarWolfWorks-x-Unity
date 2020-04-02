using System.Collections.Generic;
using UnityEngine;
using System;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.EntitiesSystem.Movement
{

    /// <summary>
    /// Base class of all EntityMovement scripts. Use only this to move entites.
    /// </summary>
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
    public abstract class EntityMovement : EntityComponent, ILockable
    {
        /// <summary>
        /// Class which is used to add or remove velocity from the Entity.
        /// </summary>
        [Serializable]
        public class Velocity
        {
            /// <summary>
            /// Value of the velocity.
            /// </summary>
            public Vector3 Value;
            /// <summary>
            /// When the velocity timer reaches 0, it will be removed.
            /// </summary>
            public bool DeleteOnCount0;
            /// <summary>
            /// Affections. See <see cref="IStat"/> for more info.
            /// </summary>
            public int[] Affections;

            [SerializeField]
            private float s_StartTime;
            /// <summary>
            /// Time which was set for this velocity.
            /// </summary>
            public float StartTime { get => s_StartTime; set => s_StartTime = value; }
            /// <summary>
            /// Current countdown time.
            /// </summary>
            public float Time { get; set; }

            /// <summary>
            /// Returns the percent of the velocity runout. (Time / StartTime).
            /// </summary>
            public float Time01 => Time / StartTime;

            internal bool UsesStat;

            /// <summary>
            /// Creates a simple velocity.
            /// </summary>
            /// <param name="value"></param>
            public Velocity(Vector3 value)
            {
                Value = value;
                DeleteOnCount0 = false;
                StartTime = Time = 0;
                UsesStat = false;
                Affections = null;
            }

            /// <summary>
            /// Creates a velocity with a countdown.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="time"></param>
            /// <param name="deleteOn0"></param>
            public Velocity(Vector3 value, float time, bool deleteOn0)
            {
                Value = value;
                DeleteOnCount0 = deleteOn0;
                StartTime = Time = time;
                UsesStat = false;
                Affections = null;
            }

            /// <summary>
            /// Creates a velocity with affections.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="affections"></param>
            public Velocity(Vector3 value, params int[] affections)
            {
                Value = value;
                DeleteOnCount0 = false;
                UsesStat = true;
                StartTime = Time = 0;
                Affections = affections;
            }

            /// <summary>
            /// Creates a complete velocity.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="time"></param>
            /// <param name="deleteOn0"></param>
            /// <param name="affections"></param>
            public Velocity(Vector3 value, float time, bool deleteOn0, params int[] affections)
            {
                Value = value;
                DeleteOnCount0 = deleteOn0;
                StartTime = Time = time;
                UsesStat = true;
                Affections = affections;
            }
        }

        /// <summary>
        /// The default velocity to be applies to this <see cref="Entity"/>. (<see cref="Vector3.zero"/> by default)
        /// </summary>
        public virtual Vector3 DefaultVelocity => Vector3.zero;

        /// <summary>
        /// Boolean which returns true if the <see cref="Entity"/> is moving. (Doesn't have any function  by itself, if you want it to be functional you would need to give it the functionality yourself).
        /// </summary>
        public bool IsMoving { get; protected set; }

        [SerializeField]
        private List<Velocity> s_Velocities = new List<Velocity>();
        /// <summary>
        /// Every velocity stacked onto the entity's movement.
        /// </summary>
        public List<Velocity> Velocities => s_Velocities;

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

                for(int i = 0; i < Velocities.Count; i++)
                {
                    float multi = Velocities[i].UsesStat ? EntityMain.Stats.CalculatedValue(1, Velocities[i].Affections) : 1;
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
        /// Moves the <see cref="Entity"/> to the given position.
        /// </summary>
        /// <param name="position">World position to move the <see cref="Entity"/> to.</param>
        /// <param name="respectPhysics">If true, this method will try to respect the world physics in your game. (E.G not move inside another collider but next to it, etc..)</param>
        public abstract void MovePosition(Vector3 position, bool respectPhysics);
    }
}