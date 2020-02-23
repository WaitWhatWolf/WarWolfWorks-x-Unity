namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Interface to be implemented for an initiatable component/object.
    /// </summary>
    public interface IInitiated
    {
        /// <summary>
        /// Returns the initiated state of this <see cref="IInitiated"/>.
        /// </summary>
        bool Initiated { get; }
    }
}
