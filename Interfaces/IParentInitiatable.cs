namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Used for basic initiation of an object.
    /// </summary>
    public interface IParentInitiatable<T> : IInitiated, IParentable<T>
    {
        /// <summary>
        /// Initiates it.
        /// </summary>
        void Init(T parent);
    }
}
