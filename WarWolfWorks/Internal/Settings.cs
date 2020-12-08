using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using WarWolfWorks.Debugging;
using WarWolfWorks.Utility;
using static WarWolfWorks.AdvancedDebug;
using static WarWolfWorks.Constants;

[assembly: InternalsVisibleTo("WarWolfWorks.EditorBase")]
namespace WarWolfWorks.Internal
{
    /// <summary>
    /// General settings of the WWWLibrary.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Applies all changes made to their respective files (CTS).
        /// </summary>
        public static void Apply()
        {
            CTS_Preferences_AdvancedDebug.Apply();
            CTS_Preferences_Misc.Apply();
            CTS_Settings_CoreCanvas.Apply();
            CTS_Settings_AdvancedDebug.Apply();
            CTS_Settings_Misc.Apply();
        }

        /// <summary>
        /// Canvas which is created under the name UtilitiesCanvas if no canvas was present on the scene.
        /// </summary>
        public static Canvas UtilityCanvas
        {
            get
            {
                if (!utilCanvas)
                {
                    switch (GetUtilityCanvasType())
                    {
                        default:
                            return utilCanvas;

                        case UtilityCanvasType.FIRST_FOUND:
                            utilCanvas = UnityEngine.Object.FindObjectOfType<Canvas>();
                            break;
                        case UtilityCanvasType.PREFABBED:
                            utilCanvas = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(GetUtilityCanvasResourcesPath())).GetComponent<Canvas>();
                            break;
                        case UtilityCanvasType.BY_NAME_IN_SCENE:
                            utilCanvas = GameObject.Find(GetUtilityCanvasNameLoad())?.GetComponent<Canvas>();
                            break;
                        case UtilityCanvasType.INSTANTIATE_NEW:
                            utilCanvas = new GameObject(VN_DEFAULT_CANVAS).AddComponent<Canvas>();
                            utilCanvas.gameObject.AddComponent<CanvasScaler>();
                            utilCanvas.gameObject.AddComponent<GraphicRaycaster>();
                            break;
                    }

                    UnityEngine.Object.DontDestroyOnLoad(utilCanvas);
                }

                return utilCanvas;
            }
            set
            {
                utilCanvas = value;
            }
        }

        #region Canvas Settings
        /// <summary>
        /// Used to determine the behaviour of <see cref="UtilityCanvas"/>.
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
            /// <summary>
            /// The canvas will not load itself; Instead, the user is in charge of setting it with using <see cref="UtilityCanvas"/>.
            /// </summary>
            MANUAL_SET = 4,
        }

        /// <summary>
        /// Gets the <see cref="UtilityCanvasType"/> from it's CTS save.
        /// </summary>
        public static UtilityCanvasType GetUtilityCanvasType()
            => Hooks.Parse<UtilityCanvasType>(CTS_Settings_CoreCanvas.GetSafe(SVN_UC_TYPE, UtilityCanvasType.FIRST_FOUND.ToString()));
        /// <summary>
        /// If the <see cref="GetUtilityCanvasType"/> is <see cref="UtilityCanvasType.PREFABBED"/>, it will return the path to the object it was set to.
        /// </summary>
        public static string GetUtilityCanvasResourcesPath()
            => CTS_Settings_CoreCanvas[SVN_UC_RESOURCES_PATH];
        /// <summary>
        /// If the <see cref="GetUtilityCanvasType"/> is <see cref="UtilityCanvasType.BY_NAME_IN_SCENE"/>, it will return the name of the object that will be searched for in the scene.
        /// </summary>
        /// <returns></returns>
        public static string GetUtilityCanvasNameLoad()
            => CTS_Settings_CoreCanvas[SVN_UC_NAME_LOAD];

        internal static void SaveUtilityCanvasType(UtilityCanvasType type)
            => CTS_Settings_CoreCanvas[SVN_UC_TYPE] = type.ToString();

        internal static void SaveUtilityCanvasResourcesPath(string path)
            => CTS_Settings_CoreCanvas[SVN_UC_RESOURCES_PATH] = path;

        internal static void SaveUtilityCanvasNameLoad(string name)
            => CTS_Settings_CoreCanvas[SVN_UC_NAME_LOAD] = name;
        #endregion

        #region Debug Settings
        /// <summary>
        /// Current <see cref="DebugStyle"/> of <see cref="AdvancedDebug"/>.
        /// </summary>
        public static Settings.DebugStyle AdvancedDebugStyle { get; private set; } = default;

        /// <summary>
        /// Color which will be used for Log information.
        /// </summary>
        public static Color DebugInfoColor { get; private set; } = Color.white;

        /// <summary>
        /// Color which will be used for LogWarning information.
        /// </summary>
        public static Color DebugWarningColor { get; private set; } = Color.magenta;

        /// <summary>
        /// Color which will be used for LogError information.
        /// </summary>
        public static Color DebugErrorColor { get; private set; } = Color.red;

        /// <summary>
        /// Gets the Log <see cref="Color"/> read directly from WWWSettings.ini.
        /// </summary>
        /// <returns></returns>
        public static Color GetDebugLogColor() => GetDebugColor(MessageType.Info);
        /// <summary>
        /// Gets the LogWarning <see cref="Color"/> read directly from WWWSettings.ini.
        /// </summary>
        /// <returns></returns>
        public static Color GetDebugWarningColor() => GetDebugColor(MessageType.Warning);
        /// <summary>
        /// Gets the LogError <see cref="Color"/> read directly from WWWSettings.ini.
        /// </summary>
        /// <returns></returns>
        public static Color GetDebugErrorColor() => GetDebugColor(MessageType.Error);

        /// <summary>
        /// Determines how <see cref="AdvancedDebug"/> will behave.
        /// </summary>
        public enum DebugStyle
        {
            /// <summary>
            /// Will display using both console and editor debugging.
            /// </summary>
            EDITOR_GAME_DEBUG = 0,
            /// <summary>
            /// Will only display messages inside the editor.
            /// </summary>
            EDITOR_DEBUG_ONLY,
            /// <summary>
            /// Will only display messages using the console.
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
        public static DebugStyle GetDebugStyle() => Hooks.Parse<DebugStyle>(CTS_Preferences_AdvancedDebug.GetSafe(SVN_AD_STYLE, DebugStyle.EDITOR_GAME_DEBUG.ToString()));
        
        /// <summary>
        /// Default layer used by <see cref="AdvancedDebug"/>.
        /// </summary>
        internal static string LayerToSavableString(string layer, bool to = false) => $"{layer}{SVS_Layers_State[0]}{to}";
        
        /// <summary>
        /// Gives the name of the variable to be used with <see cref="Hooks.Streaming"/>'s loading.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static string ToLayerStreamingName(int index) => $"{SVN_AD_LAYER}{index}";
        
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

                string name = CTS_Settings_AdvancedDebug.GetSafe(ToLayerStreamingName(i), baseName);
                bool value = Convert.ToBoolean(CTS_Preferences_AdvancedDebug.GetSafe(ToLayerStreamingName(i), false.ToString()));
                toReturn[i] = new DebugLayer(name, value);
            }

            return toReturn;
        }

        internal static void LoadDebug()
        {
            AdvancedDebugStyle = GetDebugStyle();
            DebugInfoColor = GetDebugLogColor();
            DebugWarningColor = GetDebugWarningColor();
            DebugErrorColor = GetDebugErrorColor();
        }

        internal static void SaveDebugLayers(DebugLayer[] layers)
        {
            if (layers == null || layers.Length != 33)
                throw new Exception("Cannot save layers as the value given is either null, or is not 33 in length.");
            
            for(int i = 0; i < layers.Length; i++)
            {
                string name = layers[i].Name;
                bool value = layers[i].Active;
                switch (i)
                {
                    case DEBUG_LAYER_EXCEPTIONS_INDEX:
                        name = DEBUG_LAYER_EXCEPTIONS_NAME;
                        value = true;
                        break;
                    case DEBUG_LAYER_WWW_INDEX:
                        name = DEBUG_LAYER_WWW_NAME;
                        break;
                }

                CTS_Settings_AdvancedDebug[ToLayerStreamingName(i)] = name;
                CTS_Preferences_AdvancedDebug[ToLayerStreamingName(i)] = value.ToString();
            }
        }

        internal static string GetDebugColorSaveFormat(Color color)
        {
            return string.Format("{1}{0}{2}{0}{3}", SV_DEFAULT_SPLIT, color.r, color.g, color.b);
        }

        internal static void SaveDebugColor(MessageType @for, Color color)
        {
            CTS_Preferences_AdvancedDebug[GetColorVarName(@for)] = GetDebugColorSaveFormat(color);
        }

        internal static string GetColorVarName(MessageType @for)
        {
            switch (@for)
            {
                default:
                    return SVN_AD_COLOR_LOG;
                case MessageType.Warning:
                    return SVN_AD_COLOR_WARNING;
                case MessageType.Error:
                    return SVN_AD_COLOR_ERROR;
            }
        }

        private static Color GetDebugColor(MessageType @for)
        {
            float r, g, b;
            string[] split;

            split = CTS_Preferences_AdvancedDebug.GetSafe(GetColorVarName(@for), GetDebugColorSaveFormat(Color.white)).Split(SV_DEFAULT_SPLIT);

            r = float.Parse(split[0]);
            g = float.Parse(split[1]);
            b = float.Parse(split[2]);

            return new Color(r, g, b);
        }
        #endregion

        #region Other
        /// <summary>
        /// The default type of stacking used by Stat drawers and custom editors from the <see cref="WarWolfWorks"/> library.
        /// </summary>
        public static Type DefaultStackingType
        {
            get
            {
                if(!i_DefaultStackingType)
                {
                    string typeName = CTS_Settings_Misc[nameof(sub_DefaultStackingType).ToUpper()];
                    if(!string.IsNullOrEmpty(typeName))
                        sub_DefaultStackingType = Hooks.ParseType(typeName);
                    i_DefaultStackingType = true;
                }
                return sub_DefaultStackingType;
            }
            set
            {
                if (value.IsSubclassOf(typeof(Enum)))
                {
                    sub_DefaultStackingType = value;
                    if (sub_DefaultStackingType != null)
                        CTS_Settings_Misc[nameof(sub_DefaultStackingType).ToUpper()] = value.ToString();
                    else CTS_Settings_Misc.Remove(nameof(sub_DefaultStackingType).ToUpper());
                }
            }
        }

        /// <summary>
        /// The default type of affection used by Stat drawers and custom editors from the <see cref="WarWolfWorks"/> library.
        /// </summary>
        public static Type DefaultAffectionsType
        {
            get
            {
                if(!i_DefaultAffectionsType)
                {
                    string typeName = CTS_Settings_Misc[nameof(sub_DefaultAffectionsType).ToUpper()];
                    if(!string.IsNullOrEmpty(typeName))
                        sub_DefaultAffectionsType = Hooks.ParseType(typeName);
                    i_DefaultAffectionsType = true;
                }
                return sub_DefaultAffectionsType;
            }
            set
            {
                if (value.IsSubclassOf(typeof(Enum)))
                {
                    sub_DefaultAffectionsType = value;
                    if (sub_DefaultAffectionsType != null)
                        CTS_Settings_Misc[nameof(sub_DefaultAffectionsType).ToUpper()] = value.ToString();
                    else CTS_Settings_Misc.Remove(nameof(sub_DefaultAffectionsType).ToUpper());
                }
            }
        }

        /// <summary>
        /// Language of this library.
        /// </summary>
        public static SystemLanguage LibraryLanguage
        {
            get
            {
                if (!pv_LanguageInit)
                {
                    pv_LibraryLanguage = Hooks.Parse<SystemLanguage>(CTS_Preferences_Misc.GetSafe(SVN_SETTINGS_LANGUAGE, Application.systemLanguage.ToString()));
                    pv_LanguageInit = true;
                }
                return pv_LibraryLanguage;
            }
            set
            {
                pv_LibraryLanguage = value;
                CTS_Preferences_Misc[SVN_SETTINGS_LANGUAGE] = value.ToString();
            }
        }

        private static bool i_DefaultAffectionsType;
        private static Type sub_DefaultAffectionsType = null;
        private static bool pv_LanguageInit;
        private static SystemLanguage pv_LibraryLanguage;
        private static bool i_DefaultStackingType;
        private static Type sub_DefaultStackingType = null;
        #endregion

        private static Canvas utilCanvas = null;
    }
}
