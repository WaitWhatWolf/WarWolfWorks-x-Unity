using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.Internal;
using WarWolfWorks.IO.CTS;
using WarWolfWorks.Security;
using WarWolfWorks.Utility;
using static WarWolfWorks.WWWResources;

namespace WarWolfWorks.Debugging
{
    /// <summary>
    /// A class used for an in-game console and commands.
    /// </summary>
    public static class Console
    {
        /// <summary>
        /// The active state of the console.
        /// </summary>
        public static bool On { get; private set; }

        /// <summary>
        /// Used by commands to determine if a command was destined to be controlled exclusively by the developper, not the player.
        /// </summary>
        public static bool DeveloperMode;

        /// <summary>
        /// If true, the auto-handled console input will be disabled so you can make your own custom input.
        /// </summary>
        public static bool AllowCustomInput = false;

        /// <summary>
        /// Console key used to active/deactivate the console; A gameobject under the name "ConsoleManager"
        /// handles listening for the key.
        /// </summary>
        public static DefaultKeys.WKey ConsoleKey { get; set; } = new("ConsoleKey", KeyCode.Tilde);

        /// <summary>
        /// Text currently inside the input of the console.
        /// </summary>
        public static string Input
        {
            get => pv_Input;
            private set
            {
                if (value != pv_Input)
                    OnInputChanged?.Invoke();

                pv_Input = value;
            }
        }

        /// <summary>
        /// The previously confirmed input.
        /// </summary>
        public static List<string> PreviousInputs { get; private set; } = new();

        /// <summary>
        /// Full text of the console.
        /// </summary>
        public static string FullText { get; private set; }

        /// <summary>
        /// Invoked when text is added to <see cref="FullText"/>.
        /// </summary>
        public static event Action<string> OnWrite;
        /// <summary>
        /// Invoked when the console <see cref="Input"/> is confirmed. String value is the string value of the confirmed input.
        /// </summary>
        public static event Action<string> OnInputConfirm;
        /// <summary>
        /// Invoked when the console is cleared.
        /// </summary>
        public static event Action OnClear;
        /// <summary>
        /// Invoked when a command is successfully passed.
        /// </summary>
        public static event Action<Command> OnPassed;
        /// <summary>
        /// Invoked when the console is turned on.
        /// </summary>
        public static event Action OnTurnOn;
        /// <summary>
        /// Invoked when the console is turned off.
        /// </summary>
        public static event Action OnTurnOff;
        /// <summary>
        /// Invoked when <see cref="Input"/>'s text changes in any way.
        /// </summary>
        public static event Action OnInputChanged;

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
            if (string.IsNullOrEmpty(command.Recognition))
                throw new ArgumentException("The given command's Recognition is empty or null; This is not allowed.");

            if (command.Recognition.Any(char.IsLower))
                throw new ArgumentException("The given command's Recognition contains lower case letters; This is not allowed.");

            foreach(Command commandCheck in pv_Commands)
            {
                if (commandCheck.Recognition == command.Recognition)
                    throw new ArgumentException("The given command's Recognition already exists in the list of servicable commands; Either remove the existing command or change the given command's Recognition.");
            }

            pv_Commands.Add(command);

            pv_Commands.Sort();
        }

        /// <summary>
        /// Adds multiple commands.
        /// </summary>
        /// <param name="commands"></param>
        public static void AddCommands(params Command[] commands)
        {
            foreach (Command command in commands)
                AddCommand(command);
        }

        /// <summary>
        /// Removes a command from the list of available commands.
        /// </summary>
        /// <param name="command"></param>
        public static void RemoveCommand(Command command)
        {
            pv_Commands.Remove(command);
        }

        /// <summary>
        /// Removes multiple commands.
        /// </summary>
        /// <param name="commands"></param>
        public static void RemoveCommands(params Command[] commands)
        {
            foreach (Command command in commands)
                RemoveCommand(command);
        }
        #endregion

        #region public text stuff
        /// <summary>
        /// Adds text to the console.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="messageType"></param>
        /// <remarks>This method does not directly process commands; Input the command into <see cref="Input"/> and call <see cref="ConfirmInput"/> to
        /// use commands.</remarks>
        public static void Write(object text, MessageType messageType = MessageType.Info)
        {
            string actText = text.ToString();

            FullText += '\n';
            FullText += $"<color=#{Hooks.Text.GetConsoleHexColor(messageType)}>{actText}</color>";
            OnWrite?.Invoke(actText);
        }

        /// <summary>
        /// Adds text to the console.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="messageType"></param>
        /// <param name="args"></param>
        /// <remarks>This method does not directly process commands; Input the command into <see cref="Input"/> and call <see cref="ConfirmInput"/> to
        /// use commands.</remarks>
        public static void WriteFormat(object text, MessageType messageType, params object[] args)
        {
            string actText = text.ToString();

            FullText += '\n';
            string formatted = string.Format(actText, args);
            FullText += $"<color=#{Hooks.Text.GetConsoleHexColor(messageType)}>{formatted}</color>";
            OnWrite?.Invoke(formatted);
        }

        /// <summary>
        /// Adds a new line to the console text.
        /// </summary>
        /// <param name="fromInput">If true, the input will be written first before attempting to write a new line.</param>
        public static void Write(bool fromInput = false)
        {
            if(fromInput)
            {
                if (string.IsNullOrEmpty(Input))
                    FullText += '\n';
                else
                {
                    ConfirmInput();
                }
            }
            else
                FullText += '\n';
        }

        internal static void Write(object text)
        {
            string actText = text.ToString();

            int splitIndex = actText.IndexOf('/');
            if (splitIndex != -1)
            {
                FullText += '\n';
                FullText += actText;

                string commandRec = actText.Substring(0, splitIndex).ToUpper();
                foreach (Command command in pv_Commands)
                {
                    if (command.Recognition == commandRec)
                    {
                        Type commandType = command.GetType();
                        if (commandType.GetCustomAttribute<DeveloperCommandAttribute>() != null && !DeveloperMode)
                        {
                            AdvancedDebug.LogWarningFormat("The {0} command can only be used by developers.", WWWResources.DEBUG_LAYER_EXCEPTIONS_INDEX, command.Recognition + '/');
                        }
                        else
                        {
                            command.OnPassed(actText.Substring(splitIndex + 1));
                            OnPassed?.Invoke(command);
                        }

                        break;
                    }
                }

                OnWrite?.Invoke(actText);
            }
            else
            {
                Write(text, MessageType.Info);
            }
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
        /// Sets <see cref="Input"/> to <paramref name="to"/> but returns it to <paramref name="from"/> beforehand.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        public static void ForceSetInput(string to, out string from)
        {
            from = Input;
            Input = to;

            in_PreviousInputSelectionIndex = -1;
        }

        /// <summary>
        /// Confirms the <see cref="Input"/>, putting it in <see cref="FullText"/> as well as calling <see cref="OnInputConfirm"/>.
        /// </summary>
        public static void ConfirmInput()
        {
            string prevInput = Input;
            Input = string.Empty;
            Write(prevInput);
            OnInputConfirm?.Invoke(prevInput);

            pv_PrevEntVariables.Add(new Variable(PreviousInputs.Count.ToString(), prevInput));
            PreviousInputs.Insert(0, prevInput);
            CTS_Preferences_Console.Override(pv_PrevEntVariables);
            CTS_Preferences_Console.Apply();

            in_PreviousInputSelectionIndex = -1;
        }

        /// <summary>
        /// Turns the console on.
        /// </summary>
        public static void TurnOn()
        {
            if (On)
                return;

            On = true;
            OnTurnOn?.Invoke();
        }
        
        /// <summary>
        /// Turns the console off.
        /// </summary>
        public static void TurnOff()
        {
            if (!On)
                return;

            On = false;
            OnTurnOff?.Invoke();
        }

        /// <summary>
        /// Removes all <see cref="PreviousInputs"/> and removes all saved inputs from the disk.
        /// </summary>
        public static void ResetPreviousInputs()
        {
            PreviousInputs.Clear();
            pv_PrevEntVariables.Clear();
            CTS_Preferences_Console.Override(pv_PrevEntVariables);
            CTS_Preferences_Console.Apply();
        }

        /// <summary>
        /// Returns the command with the closest recognition to the given text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>
        /// Returns the command with the closest <see cref="Command.Recognition"/> to the given <paramref name="text"/>;
        /// If none were found to be any close, null is returned.
        /// </returns>
        public static SortedDictionary<int, Command> GetClosestCommandsRecognition(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("The given text is null or empty; This is not allowed.");

            text = text.ToUpper();
            SortedDictionary<int, Command> toReturn = new SortedDictionary<int, Command>();

            for (int i = 0; i < pv_Commands.Count; i++)
            {
                int curIndex = Hooks.Text.LevenshteinDistance(text, pv_Commands[i].Recognition);

                if (curIndex <= 2 || pv_Commands[i].Recognition.Contains(text))
                {
                    while (toReturn.ContainsKey(curIndex))
                    {
                        curIndex++;
                    }
                    toReturn.Add(curIndex, pv_Commands[i]);
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Calls <see cref="GetClosestCommandsRecognition(string)"/> where the argument is <see cref="Input"/>.
        /// </summary>
        /// <returns></returns>
        public static SortedDictionary<int, Command> GetClosestCommandsRecognition()
            => GetClosestCommandsRecognition(Input);
        #endregion

        private static List<Command> pv_Commands = new()
        {
            new Command_Help(),
            new Command_DefaultKeys_Apply(),
            new Command_DefaultKeys_Key_Change_Value(),
            new Command_DefaultKeys_Key_ForceChange_Value(),
            new Command_DefaultKeys_Key_Add(),
            new Command_DefaultKeys_Key_ForceAdd(),
            new Command_DefaultKeys_Key_Remove(),
            new Command_DefaultKeys_Key_ForceRemove(),
            new Command_DefaultKeys_Key_Change_Name(),
            new Command_DefaultKeys_OptimizedState_Change(),
            new Command_Object_Tagged_Move(),
            new Command_Object_Named_Move(),
            new Command_Resources_Load(),
        };

        private static void Event_OnUpdate()
        {
            if(DefaultKeys.GetKeyDown(ConsoleKey.Name))
            {
                Action toInvoke = On ? TurnOff : TurnOn;
                toInvoke();
            }

            if(!AllowCustomInput && On && UnityEngine.Input.anyKeyDown)
            {
                KeyCode code = Hooks.GetKeyStroke(1);

                if ((string.IsNullOrEmpty(Input) || in_PreviousInputSelectionIndex != -1))
                {
                    if (code == KeyCode.UpArrow)
                    {
                        if (in_PreviousInputSelectionIndex >= PreviousInputs.Count || in_PreviousInputSelectionIndex == -1)
                            in_PreviousInputSelectionIndex = 0;

                        Input = PreviousInputs[in_PreviousInputSelectionIndex++];
                    }
                    else if (code == KeyCode.DownArrow)
                    {
                        if (in_PreviousInputSelectionIndex <= 0)
                            in_PreviousInputSelectionIndex = PreviousInputs.Count - 1;
                        Input = PreviousInputs[in_PreviousInputSelectionIndex--];
                    }
                }

                int codeInt = (int)code;
                if (codeInt >= 97 && codeInt <= 122)
                    Input += code.ToString();
                else if ((codeInt >= 48 && codeInt <= 57))
                    Input += code.ToString().ToUpper().Replace("ALPHA", string.Empty);
                else if (codeInt >= 256 && codeInt <= 265)
                    Input += code.ToString().ToUpper().Replace("KEYPAD", string.Empty);
                else
                {
                    switch(code)
                    {
                        case KeyCode.Slash: Input += '/'; break;
                        case KeyCode.Backspace:
                            if (Input.Length > 0)
                                Input = Input.Remove(Input.Length - 1, 1);
                            break;
                        case KeyCode.Comma: Input += ','; break;
                        case KeyCode.Period: Input += '.'; break;
                        case KeyCode.Return: Write(true); break;
                    }
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            if(!DefaultKeys.KeyExists(ConsoleKey.Name))
                DefaultKeys.ForceAddKey(ConsoleKey);

            //InitAutoMaker();

            pv_Commands.Sort();

            IReadOnlyList<Variable> variables = CTS_Preferences_Console.GetAllLines();

            foreach(Variable variable in variables)
            {
                PreviousInputs.Insert(0, variable.Value);
                pv_PrevEntVariables.Add(variable);
            }

            MonoManager_OnUpdate += Event_OnUpdate;
        }

        [Obsolete]
        private static void InitAutoMaker()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type type in types)
            {
                MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance);

                foreach (MethodInfo info in methodInfos)
                {
                    ConsoleMethodMakerAttribute consoleMethodMaker = info.GetCustomAttribute<ConsoleMethodMakerAttribute>();
                    if (consoleMethodMaker != null)
                    {
                        Command_AutoMaker maker = new Command_AutoMaker((Action<object>)info.CreateDelegate(typeof(Action)),
                            consoleMethodMaker.Type,
                            consoleMethodMaker.Name,
                            consoleMethodMaker.Description,
                            consoleMethodMaker.ArgumentHelper);
                        AddCommand(maker);
                    }
                }

            }
        }

        internal static int in_PreviousInputSelectionIndex = -1;
        private static List<Variable> pv_PrevEntVariables = new List<Variable>();
        private static string pv_Input = string.Empty;
    }
}
