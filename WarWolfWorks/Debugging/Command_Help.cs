using System;
using System.Collections.Generic;
using System.Text;
using WarWolfWorks.Utility;

namespace WarWolfWorks.Debugging
{
    /// <summary>
    /// A class which is used as help to write all commands and all their descriptions.
    /// </summary>
    public sealed class Command_Help : Command
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string Recognition => "HELP";
        
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string ArgumentHelper => string.Empty;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string Description => "Displays a list of all usable commands.";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="arg"></param>
        public override void OnPassed(string arg)
        {
            List<Command> commands = new (Console.GetCommands());
            commands.Remove(this);
            Console.Write();
            foreach(Command command in commands)
            {
                string flavorHelper = string.IsNullOrEmpty(command.ArgumentHelper) ? string.Empty : Hooks.Text.GetRichColoredText(command.ArgumentHelper, Hooks.Colors.FerrariRed);
                Console.Write(Hooks.Text.GetRichColoredText(command.Recognition, Hooks.Colors.Crimson) + '/' +
                    flavorHelper + '\n' + command.Description, MessageType.Info);
                Console.Write();
            }
        }
    }
}
