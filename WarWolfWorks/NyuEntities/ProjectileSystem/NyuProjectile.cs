using WarWolfWorks.Interfaces.NyuEntities;
using UnityEngine;
using System;
using WarWolfWorks.Interfaces;
using System.Collections.Generic;

namespace WarWolfWorks.NyuEntities.ProjectileSystem
{
    /// <summary>
    /// The core class of the <see cref="ProjectileSystem"/>.
    /// </summary>
    public abstract partial class NyuProjectile : MonoBehaviour, INyuReferencable, ILockable
    {
        #region Runtime and Initializing
        /// <summary>
        /// The current tag of this projectile.
        /// </summary>
        public string Tag { get; private set; } = string.Empty;

        /// <summary>
        /// Sets the <see cref="Tag"/> to the given value.
        /// </summary>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool SetTag(string to)
        {
            if (string.IsNullOrEmpty(to) || to == Tag)
                return false;

            Tag = to;
            return true;
        }

        /// <summary>
        /// Removes the <see cref="Tag"/>.
        /// </summary>
        public void RemoveTag()
        {
            Tag = string.Empty;
        }
        #endregion

        #region Projectile Implementation
        /// <summary>
        /// Pointer to the <see cref="NyuProjectile"/>'s transform.position.
        /// </summary>
        public Vector3 Position { get => transform.position; set => transform.position = value; }
        /// <summary>
        /// Pointer to the <see cref="NyuProjectile"/>'s transform.eulerAngles.
        /// </summary>
        public Vector3 Euler { get => transform.eulerAngles; set => transform.eulerAngles = value; }
        /// <summary>
        /// Pointer to the <see cref="NyuProjectile"/>'s transform.rotation.
        /// </summary>
        public Quaternion Rotation { get => transform.rotation; set => transform.rotation = value; }

        /// <summary>
        /// The velocity of the projectile; Usually assigned to a rigidbody's velocity.
        /// </summary>
        public abstract Vector3 Velocity { get; set; }

        /// <summary>
        /// The parent of this <see cref="NyuProjectile"/>.
        /// </summary>
        public Nyu NyuMain { get; internal set; }
        #endregion

        /// <summary>
        /// All behaviors this <see cref="NyuProjectile"/> is currently using.
        /// When setting this array or any of it's elements to a new object, make sure to call <see cref="RefreshUpdateLists"/>
        /// to update all update lists, otherwise any <see cref="Behavior"/> implementations of <see cref="INyuUpdate"/>,
        /// <see cref="INyuFixedUpdate"/> or <see cref="INyuLateUpdate"/> will not work.
        /// </summary>
        public Behavior[] Behaviors = new Behavior[0];


        #region Internal
        internal List<INyuUpdate> Behaviors_Updates = new List<INyuUpdate>();
        internal List<INyuFixedUpdate> Behaviors_FixedUpdates = new List<INyuFixedUpdate>();
        internal List<INyuLateUpdate> Behaviors_LateUpdates = new List<INyuLateUpdate>();

        internal bool IsUpdate;
        internal bool IsFixedUpdate;
        internal bool IsLateUpdate;

        internal INyuUpdate AsUpdate;
        internal INyuFixedUpdate AsFixedUpdate;
        internal INyuLateUpdate AsLateUpdate;

        internal void SetSProjectileUpdates()
        {
            if (this is INyuUpdate update)
            {
                AsUpdate = update;
                IsUpdate = true;
            }
            if (this is INyuFixedUpdate fixedUpdate)
            {
                AsFixedUpdate = fixedUpdate;
                IsFixedUpdate = true;
            }
            if (this is INyuLateUpdate lateUpdate)
            {
                AsLateUpdate = lateUpdate;
                IsLateUpdate = true;
            }
        }
        #endregion

        /// <summary>
        /// Refreshes all behavior update lists.
        /// </summary>
        public void RefreshUpdateLists()
        {
            Behaviors_Updates.Clear();
            Behaviors_FixedUpdates.Clear();
            Behaviors_LateUpdates.Clear();

            for (int i = 0; i < Behaviors.Length; i++)
            {
                if (Behaviors[i] is INyuUpdate nyuUpdate)
                    Behaviors_Updates.Add(nyuUpdate);
                if (Behaviors[i] is INyuFixedUpdate nyuFixedUpdate)
                    Behaviors_FixedUpdates.Add(nyuFixedUpdate);
                if (Behaviors[i] is INyuLateUpdate nyuLateUpdate)
                    Behaviors_LateUpdates.Add(nyuLateUpdate);
            }
        }

        #region ILockable Implementation
        /// <summary>
        /// <see cref="ILockable"/> implementation; Invoked when <see cref="SetLock(bool)"/> is successfully set to true.
        /// </summary>
        public event Action<ILockable> OnLocked;

        /// <summary>
        /// <see cref="ILockable"/> implementation; Invoked when <see cref="SetLock(bool)"/> is successfully set to false.
        /// </summary>
        public event Action<ILockable> OnUnlocked;

        /// <summary>
        /// The locked state of this <see cref="NyuProjectile"/>. (<see cref="ILockable"/> implementation)
        /// </summary>
        public bool Locked { get; private set; }

        /// <summary>
        /// Attempts to set the locked state of this <see cref="NyuProjectile"/>. (<see cref="ILockable"/> implementation)
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
