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

#pragma warning disable IDE0051
        private const int BackspaceRemoveAmount = 1;
#pragma warning restore IDE0051

        /// <summary>
        /// All previous inputs confirmed inside the console.
        /// </summary>
        public static List<StringBuilder> PreviousInputs { get; private set; } = new List<StringBuilder>();

#pragma warning disable IDE0051
        private static int CurrentInputIndex
#pragma warning restore IDE0051
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
        /// Called when Input inside the console is confirmed; Use this to add commands.
        /// </summary>
        public static event Action<StringBuilder> OnConfirmInput = ConsoleHelper;
        /// <summary>
        /// Called when text is inserted inside the console content.
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
            InsertTextToConsole("All fields with [text] indicate that [text] should be replaced with a number, for example: \nExample_Command = [number] \nShould be: \nExample_Command = 4");
            InsertTextToConsole("\nList of " + commandListText + " commands:");
        }

        private static void ConsoleHelper(StringBuilder sb)
        {
            if (sb.ToString().ToUpper() == "HELP")
            {
                WriteCommandHelper("general");
            }
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
            if (AcceptableKeys.ContainsInCollection(key))
            {
                flag = false;
            }
            else
            {
                if (!FunctionKeys.ContainsInCollection(key))
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
            if (CurrentInput.ToString() != string.Empty)
            {
                StringBuilder obj = new StringBuilder(CurrentInput.ToString());
                PreviousInputs.Add(CurrentInput);
                InsertTextToConsole(CurrentInput.ToString());
                InsertOffsetIndex = 0;
                CurrentInputIndex = 0;
                CurrentInput = new StringBuilder(string.Empty);
                WWWConsole.OnConfirmInput?.Invoke(obj);
            }
        }
    }

}
