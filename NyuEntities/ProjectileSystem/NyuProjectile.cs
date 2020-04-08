using WarWolfWorks.Interfaces.NyuEntities;
using UnityEngine;
using System;
using WarWolfWorks.Interfaces;

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

        internal Behavior[] ns_Behaviors;
        /// <summary>
        /// All behaviors affecting this projectile.
        /// </summary>
        public Behavior[] Behaviors { get => ns_Behaviors; }

        /// <summary>
        /// The parent of this <see cref="NyuProjectile"/>.
        /// </summary>
        public Nyu NyuMain { get; internal set; }
        #endregion

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
