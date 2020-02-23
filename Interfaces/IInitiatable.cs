namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Used for basic initiation of an object.
    /// </summary>
    public interface IInitiatable : IInitiated
    {
        /// <summary>
        /// Initiates it.
        /// </summary>
        void Init();
    }
}
