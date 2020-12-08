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
        
        private static readonly IntRange LayerRange = new IntRange(0, 32);

        internal static DebugLayer[] DebugLayers = new DebugLayer[33];

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
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void RefreshDebugger()
        {
            DebugLayers = Settings.GetDebugLayers();
            Settings.LoadDebug();
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="exception"></param>
        public static void LogException(Exception exception)
        {
            Debug.LogErrorFormat("<color=#{0}>{1}</color>\n{2}", Hooks.Text.GetConsoleHexColor(MessageType.Error),exception.Message, exception.StackTrace);
        }

        private static void InternalLog(object message, int layer, MessageType messageType)
        {
            if (!LayerIsActive(layer))
                return;

            switch (Settings.AdvancedDebugStyle)
            {
                case Settings.DebugStyle.DISABLED:
                    break;
                case Settings.DebugStyle.EDITOR_DEBUG_ONLY:
                    UnityDebug(message, messageType);
                    break;
                case Settings.DebugStyle.IN_GAME_DEBUG_ONLY:
                    WarWolfWorks.Debugging.Console.Write(message, messageType);
                    break;
                case Settings.DebugStyle.EDITOR_GAME_DEBUG:
                    UnityDebug(message, messageType);
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
        public static void Log(object message, int layer = 2) => InternalLog(message, layer, MessageType.Info);
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
        public static void LogWarning(object message, int layer = 3) => InternalLog(message, layer, MessageType.Warning);
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
        public static void LogError(object message, int layer = 0) => InternalLog(message, layer, MessageType.Error);
        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="layer"></param>
        public static void LogError(object message, string layer) => InternalLog(message, layer, MessageType.Error);

        private static void UnityDebug(object message, MessageType messageType)
        {
            string toUse = $"<color=#{Hooks.Text.GetConsoleHexColor(messageType)}>{message}</color>";
            
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

        private static void InternalFormatLog(string message, int layer, MessageType messageType, params object[] args)
        {
            if (!LayerIsActive(layer))
                return;

            switch (Settings.AdvancedDebugStyle)
            {
                case Settings.DebugStyle.DISABLED:
                    break;
                case Settings.DebugStyle.IN_GAME_DEBUG_ONLY:
                    break;
                case Settings.DebugStyle.EDITOR_DEBUG_ONLY:
                    UnityFormatDebug(message, messageType, args);
                    break;
                case Settings.DebugStyle.EDITOR_GAME_DEBUG:
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
        public static void LogFormat(string message, int layer = 2, params object[] args) => InternalFormatLog(message, layer, MessageType.Info, args);
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
        public static void LogWarningFormat(string message, int layer = 3, params object[] args) => InternalFormatLog(message, layer, MessageType.Warning, args);
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
        public static void LogErrorFormat(string message, int layer = 0, params object[] args) => InternalFormatLog(message, layer, MessageType.Error, args);
        /// <summary>
        /// Logs an error message using <see cref="string.Format(string, object[])"/>-like formatting.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="layer"></param>
        /// <param name="args"></param>
        public static void LogErrorFormat(string message, string layer, params object[] args) => InternalFormatLog(message, layer, MessageType.Error, args);

        private static void UnityFormatDebug(string message, MessageType messageType, params object[] args)
        {
            string toUse = $"<color=#{Hooks.Text.GetConsoleHexColor(messageType)}>{message}</color>";
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