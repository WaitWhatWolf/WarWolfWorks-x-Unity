using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using WarWolfWorks.Debugging;
using WarWolfWorks.Internal;
using WarWolfWorks.Utility;

[assembly: InternalsVisibleTo("WarWolfWorks.EditorBase")]
namespace WarWolfWorks
{
    /// <summary>
    /// An advanced way to debug code information.
    /// </summary>
    public static class AdvancedDebug
    {
        [Serializable]
        internal struct DebugLayer
        {
            public string Name;
            public bool Active;

            public DebugLayer(string name, bool active)
            {
                Name = name;
                Active = active;
            }
        }
        /// <summary>
        /// Color which will be used for Log information.
        /// </summary>
        public static Color InfoColor { get; private set; } = Color.white;

        /// <summary>
        /// Color which will be used for LogWarning information.
        /// </summary>
        public static Color WarningColor { get; private set; } = Color.magenta;

        /// <summary>
        /// Color which will be used for LogError information.
        /// </summary>
        public static Color ErrorColor { get; private set; } = Color.red;

        /// <summary>
        /// Current <see cref="Settings.DebugStyle"/> of <see cref="AdvancedDebug"/>.
        /// </summary>
        public static Settings.DebugStyle DebugStyle { get; private set; } = default;

        private static readonly IntRange LayerRange = new IntRange(0, 32);

        internal static DebugLayer[] DebugLayers = new DebugLayer[33];

        /// <summary>
        /// Layer at which exceptions are handled.
        /// </summary>
        public const int DEBUG_LAYER_EXCEPTIONS_INDEX = 0;
        /// <summary>
        /// Layer at which exceptions are handled.
        /// </summary>
        public const string DEBUG_LAYER_EXCEPTIONS_NAME = "Exceptions";
        /// <summary>
        /// Layer at which WWWLibrary gives simple debug information.
        /// </summary>
        public const int DEBUG_LAYER_WWW_INDEX = 1;
        /// <summary>
        /// Layer at which WWWLibrary gives simple debug information.
        /// </summary>
        public const string DEBUG_LAYER_WWW_NAME = "WWWInfo";
        /// <summary>
        /// Returns true if the given layer is active.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static bool LayerIsActive(int layer)
        {
            if (!LayerRange.IsWithinRange(layer))
                return false;

            return DebugLayers[layer].Active;
        }

        /// <summary>
        /// Returns true if the given layer is active.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static bool LayerIsActive(string layer)
        {
            return Array.Find(DebugLayers, dl => dl.Name == layer).Active;
        }

        /// <summary>
        /// Refreshes all variables to be up-to-date with WWWSettings.ini file.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RefreshDebugger()
        {
            DebugStyle = Settings.GetDebugStyle();
            DebugLayers = Settings.GetDebugLayers();
            InfoColor = Settings.GetDebugLogColor();
            WarningColor = Settings.GetDebugWarningColor();
            ErrorColor = Settings.GetDebugErrorColor();
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="exception"></param>
        public static void LogException(Exception exception)
        {
            Debug.LogErrorFormat("<color=#{0}>{1}</color>\n{2}", GetConsoleHexColor(MessageType.Error),exception.Message, exception.StackTrace);
        }

        private static void InternalLog(object message, int layer, MessageType messageType)
        {
            if (!LayerIsActive(layer))
                return;

            switch (DebugStyle)
            {
                case Settings.DebugStyle.DISABLED:
                    break;
                case Settings.DebugStyle.EDITOR_DEBUG_ONLY:
                    UnityDebug(message, messageType);
                    break;
                case Settings.DebugStyle.IN_GAME_DEBUG_ONLY:
                    if (Application.isPlaying)
                    {
                        ConsoleDebug(message, messageType);
                    }
                    break;
                case Settings.DebugStyle.EDITOR_GAME_DEBUG:
                    UnityDebug(message, messageType);
                    if (Application.isPlaying)
                    {
                        ConsoleDebug(message, messageType);
                    }
                    break;
            }
        }

        private static void InternalLog(object message, string layer, MessageType messageType)
        {
            int layerIndex = DebugLayers.FindIndex(l => l.Name.Equals(layer));
            if(layerIndex == -1)
            {
                AdvancedDebug.LogError($"Couldn't find layer under name {layer}!", 0);
                return;
            }

            InternalLog(message, layerIndex, messageType);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="layer"></param>
        public static void Log(object message, int layer) => InternalLog(message, layer, MessageType.Info);
        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="layer"></param>
        public static void Log(object message, string layer) => InternalLog(message, layer, MessageType.Info);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="layer"></param>
        public static void LogWarning(object message, int layer) => InternalLog(message, layer, MessageType.Warning);
        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="layer"></param>
        public static void LogWarning(object message, string layer) => InternalLog(message, layer, MessageType.Warning);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="layer"></param>
        public static void LogError(object message, int layer) => InternalLog(message, layer, MessageType.Error);
        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="layer"></param>
        public static void LogError(object message, string layer) => InternalLog(message, layer, MessageType.Error);

        private static void UnityDebug(object message, MessageType messageType)
        {
            string toUse = $"<color=#{GetConsoleHexColor(messageType)}>{message.ToString()}</color>";
            
            switch (messageType)
            {
                case MessageType.Info:
                    Debug.Log(toUse);
                    break;
                case MessageType.Warning:
                    Debug.LogWarning(toUse);
                    break;
                case MessageType.Error:
                    Debug.LogError(toUse);
                    break;
            }
        }

        private static string GetConsoleHexColor(MessageType messageType)
        {
            Color toUse;
            switch (messageType)
            {
                default: toUse = InfoColor; break;
                case MessageType.Warning: toUse = WarningColor; break;
                case MessageType.Error: toUse = ErrorColor; break;
            }

            return ColorUtility.ToHtmlStringRGB(toUse);
        }

        private static void ConsoleDebug(object message, MessageType messageType)
        {
            string hexColor = GetConsoleHexColor(messageType);
            WWWConsole.InsertTextToConsole($"<color=#{hexColor}>{message}</color>");
        }

        private static void InternalFormatLog(string message, int layer, MessageType messageType, params object[] args)
        {
            if (!LayerIsActive(layer))
                return;

            switch (DebugStyle)
            {
                case Settings.DebugStyle.DISABLED:
                    break;
                case Settings.DebugStyle.IN_GAME_DEBUG_ONLY:
                    ConsoleFormatDebug(message, messageType, args);
                    break;
                case Settings.DebugStyle.EDITOR_DEBUG_ONLY:
                    UnityFormatDebug(message, messageType, args);
                    break;
                case Settings.DebugStyle.EDITOR_GAME_DEBUG:
                    ConsoleFormatDebug(message, messageType, args);
                    UnityFormatDebug(message, messageType, args);
                    break;
            }
        }
        private static void InternalFormatLog(string message, string layer, MessageType messageType, params object[] args)
        {
            int layerIndex = DebugLayers.FindIndex(l => l.Name.Equals(layer));
            if (layerIndex == -1)
            {
                LogError($"Couldn't find layer under name {layer}!", 0);
                return;
            }

            InternalFormatLog(message, layerIndex, messageType);
        }

        /// <summary>
        /// Logs a message using <see cref="string.Format(string, object[])"/>-like formatting.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="layer"></param>
        /// <param name="args"></param>
        public static void LogFormat(string message, int layer, params object[] args) => InternalFormatLog(message, layer, MessageType.Info, args);
        /// <summary>
        /// Logs a message using <see cref="string.Format(string, object[])"/>-like formatting.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="layer"></param>
        /// <param name="args"></param>
        public static void LogFormat(string message, string layer, params object[] args) => InternalFormatLog(message, layer, MessageType.Info, args);

        /// <summary>
        /// Logs a warning message using <see cref="string.Format(string, object[])"/>-like formatting.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="layer"></param>
        /// <param name="args"></param>
        public static void LogWarningFormat(string message, int layer, params object[] args) => InternalFormatLog(message, layer, MessageType.Warning, args);
        /// <summary>
        /// Logs a warning message using <see cref="string.Format(string, object[])"/>-like formatting.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="layer"></param>
        /// <param name="args"></param>
        public static void LogWarningFormat(string message, string layer, params object[] args) => InternalFormatLog(message, layer, MessageType.Warning, args);

        /// <summary>
        /// Logs an error message using <see cref="string.Format(string, object[])"/>-like formatting.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="layer"></param>
        /// <param name="args"></param>
        public static void LogErrorFormat(string message, int layer, params object[] args) => InternalFormatLog(message, layer, MessageType.Error, args);
        /// <summary>
        /// Logs an error message using <see cref="string.Format(string, object[])"/>-like formatting.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="layer"></param>
        /// <param name="args"></param>
        public static void LogErrorFormat(string message, string layer, params object[] args) => InternalFormatLog(message, layer, MessageType.Error, args);

        private static void UnityFormatDebug(string message, MessageType messageType, params object[] args)
        {
            string toUse = $"<color=#{GetConsoleHexColor(messageType)}>{message.ToString()}</color>";
            switch (messageType)
            {
                case MessageType.Info:
                    Debug.LogFormat(toUse, args);
                    break;
                case MessageType.Warning:
                    Debug.LogWarningFormat(toUse, args);
                    break;
                case MessageType.Error:
                    Debug.LogErrorFormat(toUse, args);
                    break;
            }
        }
        private static void ConsoleFormatDebug(string message, MessageType messageType, params object[] args)
        {
            string hexColor = GetConsoleHexColor(messageType);
            
            WWWConsole.InsertTextToConsole($"<color=#{hexColor}>{string.Format(message, args)}</color>");
        }

        #region Utility/Various methods
        /// <summary>
        /// Uses <see cref="Log(object, int)"/> to debug all delegates inside a <see cref="Delegate"/>.
        /// </summary>
        /// <param name="delegate"></param>
        public static void LogDelegate(this Delegate @delegate)
        {
            if (@delegate != null)
            {
                Delegate[] array = @delegate?.GetInvocationList();
                foreach (Delegate delegate2 in array)
                {
                    Log(delegate2.Method.Name, 0);
                }
            }
        }

        /// <summary>
        /// Uses <see cref="Log(object, int)"/> to debug all elements inside a IEnumerable using T.ToString().
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="layer"></param>
        public static void LogIEnumerable<T>(IEnumerable<T> enumerable, int layer)
        {
            foreach (T item in enumerable)
            {
                Log(item.ToString(), layer);
            }
        }

        /// <summary>
        /// Uses <see cref="Log(object, string)"/> to debug all elements inside a IEnumerable using T.ToString().
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="layer"></param>
        public static void LogIEnumerable<T>(IEnumerable<T> enumerable, string layer)
        {
            foreach (T item in enumerable)
            {
                Log(item.ToString(), layer);
            }
        }
        #endregion
    }
}