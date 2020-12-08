namespace WarWolfWorks.Interfaces
{
    /// <summary>
    /// Indicates a focusable object.
    /// </summary>
    public interface IFocusable
    {
        /// <summary>
        /// The focused state of this <see cref="IFocusable"/>.
        /// </summary>
        bool IsFocused { get; }
    }
}
