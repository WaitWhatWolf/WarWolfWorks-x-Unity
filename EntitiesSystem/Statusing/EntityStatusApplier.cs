﻿using System;
using System.Collections.Generic;
using UnityEngine;
using WarWolfWorks.Interfaces;
using WarWolfWorks.Utility;

namespace WarWolfWorks.EntitiesSystem.Statusing
{
    /// <summary>
    /// The class which handles status application.
    /// </summary>
    [System.Obsolete(Constants.VAR_ENTITESSYSTEM_OBSOLETE_MESSAGE, Constants.VAR_ENTITIESSYSTEM_OBSOLETE_ISERROR)]
    public sealed class EntityStatusApplier : EntityComponent, IStatusApplier, ILockable
    {
        /// <summary>
        /// All statuses currently affecting the <see cref="EntityStatusApplier"/>.
        /// </summary>
        private List<IStatus> Statuses { get; } = new List<IStatus>();

        private List<Resistance> Resistances { get; } = new List<Resistance>();

        /// <summary>
        /// Returns all statuses affecting this <see cref="EntityStatusApplier"/>.
        /// </summary>
        public IEnumerable<IStatus> GetAllStatuses => Statuses;

        /// <summary>
        /// Returns all resistances of this <see cref="EntityStatusApplier"/>.
        /// </summary>
        public IEnumerable<Resistance> GetAllResistances => Resistances;

        /// <summary>
        /// Locked state of the <see cref="EntityStatusApplier"/>. See <see cref="ILockable"/> for more info.
        /// </summary>
        public bool Locked { get; private set; }

        /// <summary>
        /// Invoked when this <see cref="EntityStatusApplier"/> is locked.
        /// </summary>
        public event Action<ILockable> OnLocked;
        /// <summary>
        /// Invoked when this <see cref="EntityStatusApplier"/> is unlocked.
        /// </summary>
        public event Action<ILockable> OnUnlocked;

        /// <summary>
        /// Returns true if a <see cref="Resistance"/> contained in this <see cref="EntityStatusApplier"/> has <see cref="Resistance.ResistantTo"/> equal to the given value.
        /// </summary>
        /// <param name="of"></param>
        /// <returns></returns>
        public bool ContainsResistance(Type of) => Resistances.FindIndex(r => r.ResistantTo == of) != -1;

        /// <summary>
        /// Returns a <see cref="Resistance"/> of which <see cref="Resistance.ResistantTo"/> is equal to the value given.
        /// </summary>
        /// <param name="of"></param>
        /// <returns></returns>
        public Resistance GetResistance(Type of) => Resistances.Find(r => r.ResistantTo == of);

        /// <summary>
        /// Adds a single status to affect this <see cref="EntityStatusApplier"/>.
        /// </summary>
        /// <param name="status"></param>
        public bool AddStatus(IStatus status)
        {
            if (status == null || ContainsResistance(status.GetType()))
                return false;

            Type statusType = status.GetType();
            int sameTypeIndex = Statuses.FindIndex(s => s.GetType() == statusType);
            if (sameTypeIndex != -1)
            {
                switch(status.OverlapType)
                {
                    case StatusOverlapType.ignore:
                        return false;
                    case StatusOverlapType.replace:
                        RemoveStatus(Statuses[sameTypeIndex], false);
                        break;
                }
            }

            Statuses.Add(status);
            status.OnStart(this);
            status.CurrentDuration = status.MaxDuration;
            return true;
        }

        /// <summary>
        /// Adds a params of Statuses to affect this <see cref="EntityStatusApplier"/>.
        /// </summary>
        /// <param name="statuses"></param>
        public void AddStatuses(params IStatus[] statuses)
        {
            for(int i = 0; i < statuses.Length; i++)
            {
                AddStatus(statuses[i]);
            }
        }

        /// <summary>
        /// Adds a resistance of a certain <see cref="IStatus"/> to this <see cref="EntityStatusApplier"/>.
        /// </summary>
        /// <param name="resistance"></param>
        public void AddResistance(Resistance resistance)
        {
            Resistances.Add(resistance);
            resistance.OnAdded?.Invoke(this);
            for (int i = 0; i < Statuses.Count; i++)
            {
                if (Statuses[i].GetType().Equals(resistance.ResistantTo))
                    RemoveStatus(Statuses[i], false);
            }
        }

        /// <summary>
        /// Adds a range of resistances using params.
        /// </summary>
        /// <param name="resistances"></param>
        public void AddResistances(params Resistance[] resistances)
        {
            for(int i = 0; i < resistances.Length; i++)
                AddResistance(resistances[i]);
        }

        /// <summary>
        /// Removes a status which currently affects this <see cref="EntityStatusApplier"/>.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="ignoreOnEnd">If true, <see cref="IStatus.OnEnd(IStatusApplier)"/> will not be triggered.</param>
        /// <returns></returns>
        public bool RemoveStatus(IStatus status, bool ignoreOnEnd)
        {
            bool toReturn = Statuses.Remove(status);
            if (toReturn && !ignoreOnEnd) status.OnEnd(this);

            return toReturn;
        }

        /// <summary>
        /// Removes statuses which currently affect this <see cref="EntityStatusApplier"/>.
        /// </summary>
        /// <param name="ignoreOnEnd"></param>
        /// <param name="statuses">If true, <see cref="IStatus.OnEnd(IStatusApplier)"/> will not be triggered.</param>
        public void RemoveStatuses(bool ignoreOnEnd, params IStatus[] statuses)
        {
            for (int i = 0; i < statuses.Length; i++)
            {
                bool toReturn = Statuses.Remove(statuses[i]);
                if (toReturn && !ignoreOnEnd) statuses[i].OnEnd(this);
            }
        }

        /// <summary>
        /// Removes a resistance from this <see cref="EntityStatusApplier"/>.
        /// </summary>
        /// <param name="resistance"></param>
        /// <returns></returns>
        public bool RemoveResistance(Resistance resistance)
        {
            bool toReturn = Resistances.Remove(resistance);
            if (toReturn) resistance.OnRemoved?.Invoke(this);
            return toReturn;
        }

        /// <summary>
        /// Removes a range of Resistances using params.
        /// </summary>
        /// <param name="resistances"></param>
        public void RemoveResistances(params Resistance[] resistances)
        {
            for (int i = 0; i < resistances.Length; i++)
                if (Resistances.Remove(resistances[i])) resistances[i].OnRemoved?.Invoke(this);
        }

        /// <summary>
        /// Calls <see cref="IStatus.OnTick(IStatusApplier)"/> for all statuses currently applied to this <see cref="EntityStatusApplier"/>.
        /// </summary>
        public override void OnUpdate()
        {
            if (Locked)
                return;

            for (int i = 0; i < Statuses.Count; i++)
            {
                Statuses[i].OnTick(this);
                Statuses[i].CurrentDuration = Mathf.Clamp(Statuses[i].CurrentDuration - Time.deltaTime, 0, Statuses[i].MaxDuration);
                if (Statuses[i].CurrentDuration == 0)
                {
                    RemoveStatus(Statuses[i], false);
                }
            }
        }

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

            Statuses.ForEach(s =>
            {
                if (s is ILockable)
                    ((ILockable)s).SetLock(Locked);
            });
        }
    }
}
