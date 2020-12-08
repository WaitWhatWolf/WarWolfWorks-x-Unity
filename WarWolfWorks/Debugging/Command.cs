namespace WarWolfWorks.Debugging
{
    /// <summary>
    /// Used with <see cref="Console"/> for commands.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// The string that needs to be input for the command to be used.
        /// </summary>
        public abstract string Recognition { get; }

        /// <summary>
        /// The description of the command.
        /// </summary>
        public virtual string Description => "No description available.";

        /// <summary>
        /// Called when the command is used.
        /// </summary>
        /// <param name="arg"></param>
        public abstract void OnPassed(string arg);
    }
}
