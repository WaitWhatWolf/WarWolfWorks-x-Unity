using System;

namespace WarWolfWorks.Debugging
{
    /// <summary>
    /// Used with <see cref="WWWConsole"/> to add commands.
    /// </summary>
    public sealed class Command
    {
        /// <summary>
        /// Name of the command.
        /// </summary>
        public string Name;
        /// <summary>
        /// If true, all characters must be of the same case to be accepted.
        /// </summary>
        public bool CaseSensitiveName;
        /// <summary>
        /// Description of the command.
        /// </summary>
        public string Description;
        /// <summary>
        /// What happens when the command is entered; String value is the text after the '='.
        /// </summary>
        public Action<string> OnCommandUsed;

        /// <summary>
        /// Creates a new <see cref="Command"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="caseSensitiveName"></param>
        /// <param name="description"></param>
        /// <param name="onCommandUsed"></param>
        public Command(string name, bool caseSensitiveName, string description, Action<string> onCommandUsed)
        {
            Name = name;
            CaseSensitiveName = caseSensitiveName;
            Description = description;
            OnCommandUsed = onCommandUsed;
        }
    }
}
