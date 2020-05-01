namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Interface used for custom effect when a <see cref="IAdvancedHealth"/> object enters immunity.
    /// </summary>
    public interface IImmunityEffect<T> : IParentable<T> where T : IAdvancedHealth
    {
        /// <summary>
        /// Invoked when this immunity effect is added to a <see cref="IAdvancedHealth"/>, or when it is initiated with it.
        /// </summary>
        void OnAdded();
        /// <summary>
        /// This is invoked when immunity is first triggered.
        /// </summary>
        void OnTrigger();
        /// <summary>
        /// Invoked for as long as the immunity is active.
        /// </summary>
        void WhileTrigger();
        /// <summary>
        /// This is invoked when immunity ends.
        /// </summary>
        void OnEnd();
    }
}
