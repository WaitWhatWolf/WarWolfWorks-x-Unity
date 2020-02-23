    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using WarWolfWorks.Utility;

namespace WarWolfWorks.Debugging
{ 
    /// <summary>
    /// In game console (For visual display, see <see cref="WarWolfWorks.UI.ConsoleMenu"/>).
    /// </summary>
    public static class WWWConsole
    {
        private static StringBuilder CurrentInput = new StringBuilder();

        private static int inputIndx;

        internal static DefaultKeys.WKey DefaultKey = new DefaultKeys.WKey("ConsoleKey", KeyCode.F1);

        /// <summary>
        /// All non-function input keys acceptable by the <see cref="WWWConsole"/>.
        /// </summary>
        public static readonly KeyCode[] AcceptableKeys = new KeyCode[]
            {
            KeyCode.A,
            KeyCode.B,
            KeyCode.C,
            KeyCode.D,
            KeyCode.E,
            KeyCode.F,
            KeyCode.G,
            KeyCode.H,
            KeyCode.I,
            KeyCode.J,
            KeyCode.K,
            KeyCode.L,
            KeyCode.M,
            KeyCode.N,
            KeyCode.O,
            KeyCode.P,
            KeyCode.Q,
            KeyCode.R,
            KeyCode.S,
            KeyCode.T,
            KeyCode.U,
            KeyCode.V,
            KeyCode.W,
            KeyCode.X,
            KeyCode.Y,
            KeyCode.Z,
            KeyCode.Alpha0,
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
            KeyCode.Period,
            KeyCode.Underscore,
            KeyCode.Minus,
            KeyCode.Equals,
            KeyCode.Space,
            KeyCode.Slash,
            };

        /// <summary>
        /// All function keys readable by the console.
        /// </summary>
        public static readonly KeyCode[] FunctionKeys = new KeyCode[]
            {
            KeyCode.UpArrow,
            KeyCode.DownArrow,
            KeyCode.LeftArrow,
            KeyCode.RightArrow,
            KeyCode.Return,
            KeyCode.Backspace
            };

        private static int insrtOffstIndx;

        private const int BackspaceRemoveAmount = 1;

        /// <summary>
        /// What separates the command name from it's argument.
        /// </summary>
        public const char COMMAND_ARG_SEPARATOR = '=';

        /// <summary>
        /// All previous inputs confirmed inside the console.
        /// </summary>
        public static List<string> PreviousInputs { get; private set; } = new List<string>();

        private static int CurrentInputIndex
        {
            get
            {
                return inputIndx;
            }
            set
            {
                inputIndx = value;
                if (inputIndx < 0)
                {
                    inputIndx = PreviousInputs.Count;
                }
                else if (inputIndx >= PreviousInputs.Count)
                {
                    inputIndx = 0;
                }
            }
        }

        /// <summary>
        /// The entirety of text inside the console.
        /// </summary>
        public static string AllConsoleText
        {
            get;
            private set;
        }

        /// <summary>
        /// Index of the | inside the input.
        /// </summary>
        public static int InsertOffsetIndex
        {
            get
            {
                return insrtOffstIndx;
            }
            set
            {
                insrtOffstIndx = value;
                if (insrtOffstIndx < 0)
                {
                    insrtOffstIndx = 0;
                }
                else if (insrtOffstIndx > CurrentInput.Length)
                {
                    insrtOffstIndx = CurrentInput.Length;
                }
            }
        }

        /// <summary>
        /// All commands.
        /// </summary>
        private static List<Command> Commands = new List<Command>()
        {
            new Command(nameof(Help), false, "Displays information about the console.", Help),
            new Command(nameof(g_all_commands), true, "Displays all commands.", g_all_commands),
            new Command(nameof(s_console_size), true, "Allows you to change the size of the console menu. Arg{[SizeX]x[SizeY]} (0 to 1)", s_console_size),
            new Command(nameof(s_console_scroll_speed), true, "Sets the scroll sensivity of the console. Arg{[speed]}", s_console_scroll_speed),
            new Command(nameof(s_console_font_size), true, "Sets the size of the font. Arg{[size]}", s_console_font_size),
        };

        /// <summary>
        /// Adds a command to the list of commands. (Note: Cannot add two commands of the same <see cref="Command.Name"/> value)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool AddCommand(Command command)
        {
            if (Commands.FindIndex(c => c.Name == command.Name) != -1)
                return false;

            Commands.Add(command);
            return true;
        }

        /// <summary>
        /// Removes a command under the given name from the list of commands.
        /// </summary>
        /// <param name="commandName"></param>
        /// <returns></returns>
        public static bool RemoveCommand(string commandName)
        {
            return Commands.Remove(Commands.Find(c => c.Name == commandName));
        }

        /// <summary>
        /// Invoked when Input inside the console is confirmed.
        /// </summary>
        public static event Action<string> OnInputConfirmed;
        /// <summary>
        /// Invoked when a command was successfully called.
        /// </summary>
        public static event Action<Command, string> OnCommandConfirmed;
        /// <summary>
        /// Invoked when text is inserted inside the console content.
        /// </summary>
        public static event Action<string> OnConsoleTextInsert;

        /// <summary>
        /// Returns the current input text from the console.
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentInput()
        {
            return CurrentInput.ToString();
        }

        /// <summary>
        /// Use this to display a helper message.
        /// </summary>
        /// <param name="commandListText"></param>
        public static void WriteCommandHelper(string commandListText)
        {
            InsertTextToConsole(string.Empty);
            InsertTextToConsole("All fields with Arg{} indicate that the method should be used with '" + COMMAND_ARG_SEPARATOR + "' and given an argument as described inside the curly brackets.");
            InsertTextToConsole("All fields with [text] indicate that [text] should be replaced with a number.");
            InsertTextToConsole("Example:\nExample_Command: An Example Command. Arg{[number1]x[number2]}");
            InsertTextToConsole("Should be used as follows:\nExample_Command=4x5");
            InsertTextToConsole("\nList of " + commandListText + " commands:");
        }

        private static void Help(string arg)
        {
            WriteCommandHelper("general");

            InsertTextToConsole(string.Empty);

            IEnumerable<Command> used = Commands.GetRange(0, 5);
            foreach(Command c in used)
                GetCommandNameDescriptionText(c);
        }

        private static void g_all_commands(string arg)
        {
            foreach(Command c in Commands)
            {
                GetCommandNameDescriptionText(c);
            }
        }

        /// <summary>
        /// Returns a nicely formatted text of a command.
        /// </summary>
        /// <param name="command"></param>
        public static void GetCommandNameDescriptionText(Command command)
        {
            InsertTextToConsole($"{command.Name}{(command.CaseSensitiveName ? "(Case-Sensitive)" : string.Empty)}: {command.Description}");
        }

        /// <summary>
        /// Sets the console input text.
        /// </summary>
        /// <param name="text"></param>
        public static void SetInputText(string text)
        {
            CurrentInput = new StringBuilder(text);
        }

        /// <summary>
        /// Pointer to <see cref="InsertTextToConsole(string, Color)"/> with set as <see cref="Color.green"/> * 0.6f.
        /// </summary>
        /// <param name="text"></param>
        public static void InsertTextToConsole(string text)
        {
            InsertTextToConsole(text, Color.green * 0.6f);
        }

        /// <summary>
        /// Inserts text inside the console with a specified color.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public static void InsertTextToConsole(string text, Color color)
        {
            text = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + text + "</color>";
            string text2 = "\n" + text;
            AllConsoleText += text2;
            WWWConsole.OnConsoleTextInsert?.Invoke(text2);
        }

        /// <summary>
        /// Inserts a <see cref="KeyCode"/> inside the input text.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool InsertInput(KeyCode key)
        {
            bool flag;
            if (AcceptableKeys.EnumerableContains(key))
            {
                flag = false;
            }
            else
            {
                if (!FunctionKeys.EnumerableContains(key))
                {
                    return false;
                }
                flag = true;
            }
            if (!flag)
            {
                string text;
                if (key == KeyCode.Underscore || key == KeyCode.Minus)
                {
                    text = "_";
                }
                else
                {
                    switch (key)
                    {
                        case KeyCode.Space:
                            text = " ";
                            break;
                        case KeyCode.Equals:
                            text = "=";
                            break;
                        case KeyCode.Period:
                            text = ".";
                            break;
                        default:
                            text = (Input.GetKey(KeyCode.LeftShift) ? key.ToString().ToUpper() : key.ToString().ToLower());
                            break;
                    }
                }
                if (text.Contains("alpha"))
                {
                    text = text.Remove(0, 5);
                }
                WriteToInput(text);
            }
            else
            {
                switch (key)
                {
                    case KeyCode.UpArrow:
                        CurrentInputIndex++;
                        break;
                    case KeyCode.DownArrow:
                        CurrentInputIndex--;
                        break;
                    case KeyCode.LeftArrow:
                        InsertOffsetIndex--;
                        break;
                    case KeyCode.RightArrow:
                        InsertOffsetIndex++;
                        break;
                    case KeyCode.Return:
                        ConfirmInput();
                        break;
                    case KeyCode.Backspace:
                        RemoveFromInput(1);
                        break;
                }
            }
            return true;
        }

        /// <summary>
        /// Writes an exception inside the console with a premade message pointing the user to a helper command.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="commandTip"></param>
        public static void DebugException(Exception e, string commandTip = "Help")
        {
            InsertTextToConsole(e.Message, Color.red);
            InsertTextToConsole("Type " + commandTip + " to get a list of commands you can use.", new Color(0.5f, 0f, 0f, 1f));
        }

        private static void WriteToInput(string key)
        {
            CurrentInput.Insert(InsertOffsetIndex, key);
            InsertOffsetIndex += key.Length;
        }

        private static void RemoveFromInput(int amount)
        {
            if (InsertOffsetIndex > 0)
            {
                CurrentInput.Remove(InsertOffsetIndex - amount, amount);
                InsertOffsetIndex -= amount;
            }
        }

        /// <summary>
        /// Confirms the current input of the console.
        /// </summary>
        public static void ConfirmInput()
        {
            string actInput = CurrentInput.ToString();
            if (!string.IsNullOrEmpty(actInput))
            {
                PreviousInputs.Add(actInput);
                InsertTextToConsole(actInput);
                InsertOffsetIndex = 0;
                CurrentInputIndex = 0;
                CurrentInput = new StringBuilder(string.Empty);
                int argSepIndex = actInput.IndexOf(COMMAND_ARG_SEPARATOR);
                foreach (Command c in Commands)
                {
                    bool passed = false;
                    if (c.CaseSensitiveName)
                    {
                        if (actInput.Contains(c.Name))
                        {
                            passed = true;
                        }
                    }
                    else if (actInput.ToUpper().Contains(c.Name.ToUpper()))
                        passed = true;

                    if (passed)
                    {
                        string arg = actInput.Substring(argSepIndex + 1);
                        c.OnCommandUsed?.Invoke(argSepIndex == -1 ? string.Empty : arg);
                        OnCommandConfirmed?.Invoke(c, arg);
                        break;
                    }
                }

                OnInputConfirmed?.Invoke(actInput);
            }
        }

        #region UI
        private static int inptFldIndx = 0;

        /// <summary>
        /// Index of the Console's UI input field.
        /// </summary>
        public static int InputFieldIndex
        {
            get
            {
                return inptFldIndx;
            }
            internal set
            {
                inptFldIndx = value;
                if (inptFldIndx < 0)
                {
                    inptFldIndx = 0;
                }
                if (inptFldIndx >= PreviousInputs.Count)
                {
                    inptFldIndx = PreviousInputs.Count - 1;
                }
            }
        }

        /// <summary>
        /// Scroll speed of the console's UI.
        /// </summary>
        public static float ScrollSpeed { get; set; }

        /// <summary>
        /// Font size of the console's UI.
        /// </summary>
        public static float FontSize { get; set; }

        internal static Vector4 AnchoredPosition { get; private set; }

        internal static void s_console_scroll_speed(string arg)
        {
            ScrollSpeed = Convert.ToSingle(arg);
        }

        internal static void s_console_font_size(string arg)
        {
            FontSize = Convert.ToSingle(arg);
        }

        internal static void s_console_size(string arg)
        {
            InputFieldIndex = PreviousInputs.Count - 1;
            try
            {
                string[] array = arg.Split(new char[1]
                {
                    'x'
                });
                float x = Convert.ToSingle(array[0]);
                float num = Convert.ToSingle(array[1]);
                AnchoredPosition = new Vector4(0f, 1f - num, x, 1f);
            }
            catch (Exception e)
            {
                DebugException(e);
            }
        }
        #endregion
    }

}
