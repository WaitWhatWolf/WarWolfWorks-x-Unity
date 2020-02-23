using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using WarWolfWorks.Debugging;
using WarWolfWorks.Security;
using WarWolfWorks.Utility;
using static WarWolfWorks.AdvancedDebug;
using static WarWolfWorks.Utility.Hooks.Streaming;

[assembly: InternalsVisibleTo("WarWolfWorks.EditorBase")]
namespace WarWolfWorks.Internal
{
    /// <summary>
    /// General settings of the WWWLibrary.
    /// </summary>
    public static class Settings
    {
        internal const string CanvasDefaultName = "WWWCanvas";
        
        /// <summary>
        /// Path to the WWWSettings.ini file.
        /// </summary>
        public static readonly string SettingsPath = GetStreamingAssetsFilePath("WWWSettings.ini");

        #region Canvas Settings
        private static readonly string[] CanvasNames =
            new string[]
            {
                "UTILITY_CANVAS_TYPE",
                "UTILITY_CANVAS_RESOURCES_PATH",
                "UTILITY_CANVAS_NAME_LOAD"
            };

        /// <summary>
        /// Used to determine the behaviour of <see cref="Hooks.UtilityCanvas"/>.
        /// </summary>
        public enum UtilityCanvasType
        {
            /// <summary>
            /// Takes the first <see cref="Canvas"/> found inside the given scene.
            /// </summary>
            FIRST_FOUND = 0,
            /// <summary>
            /// Takes a canvas inside a Resources folder and loads it through <see cref="Resources.Load{T}(string)"/>
            /// </summary>
            PREFABBED = 1,
            /// <summary>
            /// Takes the first <see cref="GameObject"/> inside the active scene under given name.
            /// </summary>
            BY_NAME_IN_SCENE = 2,
            /// <summary>
            /// Instantiates a new canvas that will be purely used by the WWWLibrary.
            /// </summary>
            INSTANTIATE_NEW = 3,
        }

        /// <summary>
        /// Name of the category given to the <see cref="Hooks.UtilityCanvas"/> Settings in WWWSettings.ini.
        /// </summary>
        internal const string CATEGORY_CANVAS_NAME = "ユチリテイキャンバス";

        /// <summary>
        /// Gets the <see cref="UtilityCanvasType"/> read directly from the WWWSettings.ini file.
        /// </summary>
        public static UtilityCanvasType GetUtilityCanvasType()
            => Hooks.Parse<UtilityCanvasType>(Load(Catalog.Loader(SettingsPath, CATEGORY_CANVAS_NAME, CanvasNames[0], UtilityCanvasType.FIRST_FOUND.ToString(), true)));
        /// <summary>
        /// If the <see cref="GetUtilityCanvasType"/> is <see cref="UtilityCanvasType.PREFABBED"/>, it will return the path to the object it was set to.
        /// </summary>
        public static string GetUtilityCanvasResourcesPath()
            => Load(Catalog.Loader(SettingsPath, CATEGORY_CANVAS_NAME, CanvasNames[1], default, true));
        /// <summary>
        /// If the <see cref="GetUtilityCanvasType"/> is <see cref="UtilityCanvasType.BY_NAME_IN_SCENE"/>, it will return the name of the object that will be searched for in the scene.
        /// </summary>
        /// <returns></returns>
        public static string GetUtilityCanvasNameLoad()
            => Load(Catalog.Loader(SettingsPath, CATEGORY_CANVAS_NAME, CanvasNames[2], default, true));
        #endregion

        #region Debug Settings
        internal static readonly string[] DebugNames = new string[]
        {
            "STYLE",
            "COLOR_LOG",
            "COLOR_WARNING",
            "COLOR_ERROR"
        };

        /// <summary>
        /// Name of the category given to <see cref="AdvancedDebug"/> settings in WWWSettings.ini.
        /// </summary>
        internal const string CATEGORY_DEBUG_NAME = "デバッグ";
        internal static readonly string[] SEPARATOR_LAYER_STATE = new string[] { " With Active State " };
        internal const char SEPARATOR_COLOR = ';';
        internal const string STREAMING_LAYER_NAME = "レイヤー";
        
        /// <summary>
        /// Default layer used by <see cref="AdvancedDebug"/>.
        /// </summary>
        internal static string LayerToSavableString(string layer, bool to = false) => $"{layer}{SEPARATOR_LAYER_STATE[0]}{to}";
        
        /// <summary>
        /// Gives the name of the variable to be used with <see cref="Hooks.Streaming"/>'s loading.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static string ToLayerStreamingName(int index) => $"{STREAMING_LAYER_NAME}{index}";
        
        /// <summary>
        /// Determines how <see cref="AdvancedDebug"/> will behave.
        /// </summary>
        public enum DebugStyle
        {
            /// <summary>
            /// Will display using both <see cref="WWWConsole"/> and editor debugging.
            /// </summary>
            EDITOR_GAME_DEBUG = 0,
            /// <summary>
            /// Will only display messages inside the editor.
            /// </summary>
            EDITOR_DEBUG_ONLY,
            /// <summary>
            /// Will only display messages using <see cref="WWWConsole"/>.
            /// </summary>
            IN_GAME_DEBUG_ONLY,
            /// <summary>
            /// Disables <see cref="AdvancedDebug"/> debugging.
            /// </summary>
            DISABLED,
        }
        
        /// <summary>
        /// Gets the <see cref="DebugStyle"/> read directly from WWWSettings.ini.
        /// </summary>
        /// <returns></returns>
        public static DebugStyle GetDebugStyle() => Hooks.Parse<DebugStyle>(Load(Catalog.Loader(SettingsPath, CATEGORY_DEBUG_NAME, DebugNames[0], DebugStyle.EDITOR_GAME_DEBUG.ToString(), true)));
        
        /// <summary>
        /// Gets the debug layer read directly from WWWSettings.ini.
        /// </summary>
        /// <returns></returns>
        internal static DebugLayer[] GetDebugLayers()
        {
            DebugLayer[] toReturn = new DebugLayer[33];
            for (int i = 0; i < toReturn.Length; i++)
            {
                string baseName = $"LAYER_{i}";
                switch(i)
                {
                    case DEBUG_LAYER_EXCEPTIONS_INDEX:
                        baseName = DEBUG_LAYER_EXCEPTIONS_NAME;
                        break;
                    case DEBUG_LAYER_WWW_INDEX:
                        baseName = DEBUG_LAYER_WWW_NAME;
                        break;
                }

                string[] splits = Load(Catalog.Loader(SettingsPath, CATEGORY_DEBUG_NAME, ToLayerStreamingName(i), LayerToSavableString(baseName), true)).Split(SEPARATOR_LAYER_STATE, StringSplitOptions.None);
                toReturn[i] = new DebugLayer(splits[0], Convert.ToBoolean(splits[1]));
            }

            return toReturn;
        }

        internal static void SaveDebugLayers(DebugLayer[] layers)
        {
            if (layers == null || layers.Length != 33)
                throw new WWWException("Cannot save layers as the value given is either null, or is not 33 in length.");
            
            for(int i = 0; i < layers.Length; i++)
            {
                string toUse = LayerToSavableString(layers[i].Name, layers[i].Active);
                switch (i)
                {
                    case DEBUG_LAYER_EXCEPTIONS_INDEX: toUse = LayerToSavableString(DEBUG_LAYER_EXCEPTIONS_NAME, true); break;
                    case DEBUG_LAYER_WWW_INDEX: toUse = LayerToSavableString(DEBUG_LAYER_WWW_NAME, layers[i].Active); break;
                }
                Save(Catalog.Saver(SettingsPath, CATEGORY_DEBUG_NAME, ToLayerStreamingName(i), toUse));
            }
        }

        private static Color GetDebugColor(int index)
        {
            float r, g, b;
            string[] split;

            split = Load(Catalog.Loader(SettingsPath, CATEGORY_DEBUG_NAME, DebugNames[index + 1], "1;1;1", true)).Split(';');

            r = float.Parse(split[0]);
            g = float.Parse(split[1]);
            b = float.Parse(split[2]);

            return new Color(r, g, b);
        }
        /// <summary>
        /// Gets the Log <see cref="Color"/> read directly from WWWSettings.ini.
        /// </summary>
        /// <returns></returns>
        public static Color GetDebugLogColor() => GetDebugColor(0);
        /// <summary>
        /// Gets the LogWarning <see cref="Color"/> read directly from WWWSettings.ini.
        /// </summary>
        /// <returns></returns>
        public static Color GetDebugWarningColor() => GetDebugColor(1);
        /// <summary>
        /// Gets the LogError <see cref="Color"/> read directly from WWWSettings.ini.
        /// </summary>
        /// <returns></returns>
        public static Color GetDebugErrorColor() => GetDebugColor(2);
        #endregion

        #region Other
        internal const string CATEGORY_OTHER_NAME = "その他";
        internal const string VAR_LANGUAGE_NAME = "言語";

        private static bool languageInit;
        private static SystemLanguage libraryLanguage;
        /// <summary>
        /// Language of this library.
        /// </summary>
        public static SystemLanguage LibraryLanguage
        {
            get
            {
                if (!languageInit)
                {
                    libraryLanguage = Hooks.Parse<SystemLanguage>(Load(Catalog.Loader(SettingsPath, CATEGORY_CANVAS_NAME, VAR_LANGUAGE_NAME, Application.systemLanguage.ToString(), true)));
                    languageInit = true;
                }
                return libraryLanguage;
            }
            set
            {
                libraryLanguage = value;
                Save(Catalog.Saver(SettingsPath, CATEGORY_CANVAS_NAME, VAR_LANGUAGE_NAME, value.ToString()));
            }
        }
        #endregion
    }
}
