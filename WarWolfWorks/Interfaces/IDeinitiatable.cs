namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Indicates an object that can be deinitiated; Usually paired with <see cref="IInitiatable"/> or <see cref="IParentInitiatable{T}"/>.
    /// </summary>
    public interface IDeinitiatable
    {
        /// <summary>
        /// Deinitiates this object.
        /// </summary>
        /// <returns></returns>
        bool Deinit();
    }
}
