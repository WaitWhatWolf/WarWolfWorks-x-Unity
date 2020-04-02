using System;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Interface used for "locking" a component.
    /// </summary>
    public interface ILockable
    {
        /// <summary>
        /// Determimes if the current object is locked.
        /// </summary>
        bool Locked { get; }
        /// <summary>
        /// Sets the lock.
        /// </summary>
        /// <param name="to"></param>
        void SetLock(bool to);
        /// <summary>
        /// Called when SetLock was called as true.
        /// </summary>
        event Action<ILockable> OnLocked;
        /// <summary>
        /// Called when SetLock was called as false.
        /// </summary>
        event Action<ILockable> OnUnlocked;
    }
}
