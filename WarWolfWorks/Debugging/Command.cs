using System;

namespace WarWolfWorks.Debugging
{
    /// <summary>
    /// Used with <see cref="Console"/> for commands.
    /// </summary>
    public abstract class Command : IComparable<Command>
    {
        /// <summary>
        /// The string that needs to be input for the command to be used.
        /// </summary>
        public abstract string Recognition { get; }

        /// <summary>
        /// Used in a console (usually in pair with <see cref="Console.GetClosestCommandsRecognition(string)"/>)
        /// </summary>
        public abstract string ArgumentHelper { get; }

        /// <summary>
        /// The description of the command.
        /// </summary>
        public virtual string Description => "No description available.";

        /// <summary>
        /// Compares a command to another. (<see cref="IComparable{T}"/> implementation)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Command other)
        {
            return Recognition.CompareTo(other.Recognition);
        }

        /// <summary>
        /// Called when the command is used.
        /// </summary>
        /// <param name="arg"></param>
        public abstract void OnPassed(string arg);
    }
}
