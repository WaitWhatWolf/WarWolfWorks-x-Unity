namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Indicates a component that can be indexed.
    /// </summary>
    public interface IIndexable
    {
        /// <summary>
        /// The index of this <see cref="IIndexable"/> component.
        /// </summary>
        int Index { get; }
    }
}
