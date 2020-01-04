using System;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Interface used for "locking" a component.
    /// </summary>
    public interface ILockable
    {
#if !WWW2_5_OR_HIGHER
        /// <summary>
        /// Determines how the values should behave based on Locks.
        /// </summary>
        float ValueMultiplier { get; }
        /// <summary>
        /// Locks put on this <see cref="ILockable"/>. The value determines how strong the lock is.
        /// All Locks on all <see cref="ILockable"/>s used should scale with the same value.
        /// </summary>
        List<float> Locks { get; }
#else
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
#endif
    }
}
