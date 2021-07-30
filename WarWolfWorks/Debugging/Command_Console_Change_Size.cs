using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WarWolfWorks.UI.ConsoleMenu;
using WarWolfWorks.UI.MenusSystem;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Debugging
{
    /// <summary>
    /// Changes the size of <see cref="ConsoleUI"/>.
    /// </summary>
    public sealed class Command_Console_Change_Size : Command
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override string Recognition => CONVAR_UI + "CONSOLE_CHANGE_SIZE";
        
        /// <summary>
        /// Displays that it accepts an int value.
        /// </summary>
        public override string ArgumentHelper => "<int>";

        /// <summary>
        /// Displays all possible index values.
        /// </summary>
        public override string Description { get; } = "Changes the size and position of the console based on a given number:" +
            "\n0 - Bottom (default)" +
            "\n1 - Top" +
            "\n2 - Left" +
            "\n3 - Right" +
            "\n4 - Top Left" +
            "\n5 - Top Right" +
            "\n6 - Bottom Left" +
            "\n7 - Bottom Right";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="arg"></param>
        public override void OnPassed(string arg)
        {
            ConsoleUI consoleUI = Menu.GetMenu<ConsoleUI>();
            Vector4 val = consoleUI.GetConsoleAnchoredSize();
            switch(arg)
            {
                case "0": val = new Vector4(0f, 0f, 1f, 0.25f); break;
                case "1": val = new Vector4(0f, 0.75f, 1f, 1f); break;
                case "2": val = new Vector4(0f, 0f, 0.25f, 1f); break;
                case "3": val = new Vector4(0.75f, 0f, 1f, 1f); break;
                case "4": val = new Vector4(0f, .75f, 0.35f, 1f); break;
                case "5": val = new Vector4(0.65f, 0.75f, 1f, 1f); break;
                case "6": val = new Vector4(0f, 0f, 0.35f, 0.25f); break;
                case "7": val = new Vector4(0.65f, 0f, 1f, 0.25f); break;
                default: Console.Write("Unknown argument", MessageType.Error); break;
            }

            consoleUI.SetConsoleSize(val);
        }
    }
}
