namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Class for a parenting system.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IParentable<T> 
    {
        /// <summary>
        /// The parent.
        /// </summary>
        T Parent { get; }
    }
}
