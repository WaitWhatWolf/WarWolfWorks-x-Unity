using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WarWolfWorks.Utility;

namespace WarWolfWorks.Debugging
{
    /// <summary>
    /// A class used for an in-game console and commands.
    /// </summary>
    public static class Console
    {
        /// <summary>
        /// Full text of the console.
        /// </summary>
        public static string FullText { get; private set; }

        /// <summary>
        /// Invoked when text is added to the console.
        /// </summary>
        public static event Action<string> OnWrite;
        /// <summary>
        /// Invoked when the console is cleared.
        /// </summary>
        public static event Action OnClear;
        /// <summary>
        /// Invoked when a command is successfully passed.
        /// </summary>
        public static event Action<Command> OnPassed;

        #region Public Command stuff
        /// <summary>
        /// Returns a collection of all available commands.
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyList<Command> GetCommands() => pv_Commands;

        /// <summary>
        /// Adds a command to the list of available commands.
        /// </summary>
        /// <param name="command"></param>
        public static void AddCommand(Command command)
        {
            pv_Commands.Add(command);
        }

        /// <summary>
        /// Removes a command from the list of available commands.
        /// </summary>
        /// <param name="command"></param>
        public static void RemoveCommand(Command command)
        {
            pv_Commands.Remove(command);
        }
        #endregion

        #region public text stuff
        /// <summary>
        /// Adds text to the console.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="messageType"></param>
        public static void Write(object text, MessageType messageType = MessageType.Info)
        {
            string actText = text.ToString();

            FullText += '\n';
            FullText += $"<color=#{Hooks.Text.GetConsoleHexColor(messageType)}>{actText}</color>";
            OnWrite?.Invoke(actText);

            int splitIndex = actText.IndexOf('/');
            if (splitIndex != -1)
            {
                string commandRec = actText.Substring(0, splitIndex).ToUpper();
                foreach (Command command in pv_Commands)
                {
                    if (command.Recognition == commandRec)
                    {
                        command.OnPassed(actText.Substring(splitIndex + 1));
                        OnPassed?.Invoke(command);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a new line to the console text.
        /// </summary>
        public static void Write()
        {
            FullText += '\n';
        }

        /// <summary>
        /// Clears all console text.
        /// </summary>
        public static void Clear()
        {
            FullText = string.Empty;
            OnClear?.Invoke();
        }

        /// <summary>
        /// Returns the command with the closest recognition to the given text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>
        /// Returns the command with the closest <see cref="Command.Recognition"/> to the given <paramref name="text"/>;
        /// If none were found to be any close, null is returned.
        /// </returns>
        public static string GetClosestCommandRecognition(string text)
        {
            int bestIndex = -1;
            for(int i = 0; i < pv_Commands.Count; i++)
            {
                int curIndex = Hooks.Text.LevenshteinDistance(text, pv_Commands[i].Recognition);
                if (curIndex > bestIndex)
                    bestIndex = curIndex;
            }

            return bestIndex <= 0 ? null : pv_Commands[bestIndex].Recognition;
        }
        #endregion

        private static List<Command> pv_Commands = new()
        {
            new Command_Help(),
        };
    }
}
