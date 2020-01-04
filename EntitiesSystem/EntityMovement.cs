using System.Collections.Generic;
using UnityEngine;
using WarWolfWorks.Utility;

namespace WarWolfWorks.EntitiesSystem.Movement
{
    using System;
    using UnityEngine.Serialization;
    using WarWolfWorks.Interfaces;

    /// <summary>
    /// Base class of all EntityMovement scripts. Use only this to move entites.
    /// </summary>
    public abstract class EntityMovement : EntityComponent, ILockable
    {
        /// <summary>
        /// Class which is used to add or remove velocity from the Entity.
        /// </summary>
        public class Velocity
        {
            /// <summary>
            /// Direction towards which the velocity will be applied. (read-only)
            /// </summary>
            public Vector3 Value;
            /// <summary>
            /// Time remaining for the velocity to be removed. (Only used if VelocityRunsOut is true)
            /// </summary>
            public float CurrentTimer;
            /// <summary>
            /// Initial duration of the Velocity. (read-only)
            /// </summary>
            public readonly float StartingTime;
            /// <summary>
            /// If true, Velocity's strength will become weaker as time passes. Applies only when
            /// <see cref="VelocityRunsOut"/> is true. (read-only)
            /// </summary>
            public readonly bool ValueScalesWithTime;
            /// <summary>
            /// If true, the velocity will be removed after <see cref="StartingTime"/> seconds. (read-only)
            /// </summary>
            public readonly bool VelocityRunsOut;
            /// <summary>
            /// Affection used if the Velocity will be affected by the Entity's Statistics. (read-only)
            /// </summary>
            [FormerlySerializedAs("Affection")]
            public readonly int[] Affections;
            /// <summary>
            /// Base Velocity.
            /// </summary>
            public float BaseStatValue;
            /// <summary>
            /// If true, Velocity will scale off of Entity's Statistics.
            /// </summary>
            public readonly bool HasAffection;

            /// <summary>
            /// Creates a Velocity which is affected by stats.
            /// </summary>
            /// <param name="val"></param>
            /// <param name="time"></param>
            /// <param name="scalesWithTime"></param>
            /// <param name="usesTime"></param>
            /// <param name="affections"></param>
            /// <param name="baseStatValue"></param>
            public Velocity(Vector3 val, float time, bool scalesWithTime, bool usesTime, int[] affections, float baseStatValue)
            {
                Value = val;
                CurrentTimer = StartingTime = time;
                VelocityRunsOut = usesTime;
                ValueScalesWithTime = scalesWithTime;
                Affections = affections;
                BaseStatValue = baseStatValue;
                HasAffection = true;
            }

            /// <summary>
            /// Creates a Velocity which is unaffected by stats.
            /// </summary>
            /// <param name="val"></param>
            /// <param name="time"></param>
            /// <param name="scalesWithTime"></param>
            /// <param name="usesTime"></param>
            public Velocity(Vector3 val, float time, bool scalesWithTime, bool usesTime)
            {
                Value = val;
                CurrentTimer = StartingTime = time;
                VelocityRunsOut = usesTime;
                ValueScalesWithTime = scalesWithTime;
                Affections = null;
                BaseStatValue = 0;
                HasAffection = false;
            }

            /// <summary>
            /// Returns true if all variables are the same, except for <see cref="CurrentTimer"/>.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                try
                {
                    Velocity objAsVel = ((Velocity)obj);
                    return objAsVel.HasAffection == HasAffection &&
                        objAsVel.Value == Value &&
                        objAsVel.ValueScalesWithTime == ValueScalesWithTime &&
                        objAsVel.VelocityRunsOut == VelocityRunsOut &&
                        objAsVel.StartingTime == StartingTime &&
                        objAsVel.BaseStatValue == BaseStatValue &&
                        objAsVel.Affections == Affections;
                }
                catch
                {
                    return base.Equals(obj);
                }
            }

            /// <summary>
            /// Gets the hashcode of this Velocity. (Exludes <see cref="CurrentTimer"/> from hashcode calculation)
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                var hashCode = -670195169;
                hashCode = hashCode * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(Value);
                hashCode = hashCode * -1521134295 + StartingTime.GetHashCode();
                hashCode = hashCode * -1521134295 + ValueScalesWithTime.GetHashCode();
                hashCode = hashCode * -1521134295 + VelocityRunsOut.GetHashCode();
                hashCode = hashCode * -1521134295 + Affections.GetHashCode();
                hashCode = hashCode * -1521134295 + BaseStatValue.GetHashCode();
                hashCode = hashCode * -1521134295 + HasAffection.GetHashCode();
                return hashCode;
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
        /// <summary>
        /// Every velocity stacked onto the entity's movement.
        /// </summary>
        public List<Velocity> Velocities { get; } = new List<Velocity>();
        /// <summary>
        /// Final Velocity that will be applied to this entity.
        /// </summary>
        public virtual Vector3 UsedVelocity
        {
            get
            {
                Vector3 toReturn = DefaultVelocity;

                Velocities.ForEach
                (
                    val =>
                    {
                        float spd = val.HasAffection ? EntityMain.Stats.CalculatedValue(val.BaseStatValue, val.Affections) : 1;
                        Vector3 used = val.Value * spd;
                        toReturn += val.ValueScalesWithTime && val.VelocityRunsOut ? used * (val.CurrentTimer / val.StartingTime) : used;
                    }
                );

                return Locked ? default : toReturn * Hooks.FixedFullSecond;
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
            if (Locked) OnLocked?.Invoke(this);
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