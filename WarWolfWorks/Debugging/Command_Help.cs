using System;
using System.Collections.Generic;
using System.Text;

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
        /// <param name="arg"></param>
        public override void OnPassed(string arg)
        {
            List<Command> commands = new (Console.GetCommands());
            commands.Remove(this);
            Console.Write();
            foreach(Command command in commands)
            {
                Console.Write(command.Recognition + '\n' + command.Description, MessageType.Info);
            }
        }
    }
}
