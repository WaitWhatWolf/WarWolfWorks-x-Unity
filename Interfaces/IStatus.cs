using WarWolfWorks.EntitiesSystem.Statusing;

namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Core class used for all statuses.
    /// </summary>
    public interface IStatus : IEntity
    {
        /// <summary>
        /// Determines how this <see cref="IStatus"/> will be treated when it is added to an <see cref="IStatusApplier"/>
        /// when an <see cref="IStatus"/> of the same type is detected.
        /// </summary>
        StatusOverlapType OverlapType { get; }
        /// <summary>
        /// The duration at which the status will start with.
        /// </summary>
        float MaxDuration { get; }
        /// <summary>
        /// The current countdown for this <see cref="IStatus"/>' duration.
        /// </summary>
        float CurrentDuration { get; set; }
        /// <summary>
        /// Invokes when the status is first added to a <see cref="IStatusApplier"/>.
        /// </summary>
        /// <param name="of"></param>
        void OnStart(IStatusApplier of);
        /// <summary>
        /// Invoked when this <see cref="IStatus"/> has stayed inside a <see cref="IStatusApplier"/> for <see cref="MaxDuration"/>. 
        /// </summary>
        /// <param name="of"></param>
        void OnEnd(IStatusApplier of);
        /// <summary>
        /// Invoked every in-game physics frame.
        /// </summary>
        /// <param name="of"></param>
        void OnTick(IStatusApplier of);
    }
}
