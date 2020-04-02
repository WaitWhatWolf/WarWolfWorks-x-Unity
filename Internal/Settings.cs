using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using WarWolfWorks.Debugging;
using WarWolfWorks.Security;
using WarWolfWorks.Utility;
using static WarWolfWorks.AdvancedDebug;
using static WarWolfWorks.Utility.Hooks.Streaming;
using static WarWolfWorks.Constants;
using UnityEngine.UI;
using WarWolfWorks.NyuEntities.ProjectileSystem;

[assembly: InternalsVisibleTo("WarWolfWorks.EditorBase")]
namespace WarWolfWorks.Internal
{
    /// <summary>
    /// General settings of the WWWLibrary.
    /// </summary>
    public static class Settings
    {
        private static Canvas utilCanvas = null;
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
                            utilCanvas = new GameObject(VARN_DEFAULT_CANVAS).AddComponent<Canvas>();
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
        /// Gets the <see cref="UtilityCanvasType"/> read directly from the WWWSettings.ini file.
        /// </summary>
        public static UtilityCanvasType GetUtilityCanvasType()
            => Hooks.Parse<UtilityCanvasType>(Load(Catalog.Loader(Path_Settings, SVARCN_UTIL_CANVAS, SVARN_UC_TYPE, UtilityCanvasType.FIRST_FOUND.ToString(), true)));
        /// <summary>
        /// If the <see cref="GetUtilityCanvasType"/> is <see cref="UtilityCanvasType.PREFABBED"/>, it will return the path to the object it was set to.
        /// </summary>
        public static string GetUtilityCanvasResourcesPath()
            => Load(Catalog.Loader(Path_Settings, SVARCN_UTIL_CANVAS, SVARN_UC_RESOURCES_PATH, default, true));
        /// <summary>
        /// If the <see cref="GetUtilityCanvasType"/> is <see cref="UtilityCanvasType.BY_NAME_IN_SCENE"/>, it will return the name of the object that will be searched for in the scene.
        /// </summary>
        /// <returns></returns>
        public static string GetUtilityCanvasNameLoad()
            => Load(Catalog.Loader(Path_Settings, SVARCN_UTIL_CANVAS, SVARN_UC_NAME_LOAD, default, true));

        internal static void SaveUtilityCanvasType(UtilityCanvasType type)
        {
            Save(Catalog.Saver(Path_Settings, SVARCN_UTIL_CANVAS, SVARN_UC_TYPE, type.ToString()));
        }

        internal static void SaveUtilityCanvasResourcesPath(string path)
        {
            Save(Catalog.Saver(Path_Settings, SVARCN_UTIL_CANVAS, SVARN_UC_RESOURCES_PATH, path));
        }

        internal static void SaveUtilityCanvasNameLoad(string name)
        {
            Save(Catalog.Saver(Path_Settings, SVARCN_UTIL_CANVAS, SVARN_UC_NAME_LOAD, name));
        }
        #endregion

        #region Debug Settings
        /// <summary>
        /// Default layer used by <see cref="AdvancedDebug"/>.
        /// </summary>
        internal static string LayerToSavableString(string layer, bool to = false) => $"{layer}{SVARS_LAYER_STATE[0]}{to}";
        
        /// <summary>
        /// Gives the name of the variable to be used with <see cref="Hooks.Streaming"/>'s loading.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static string ToLayerStreamingName(int index) => $"{SVARN_AD_LAYER}{index}";
        
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
        public static DebugStyle GetDebugStyle() => Hooks.Parse<DebugStyle>(Load(Catalog.Loader(Path_Preferences, SVARCN_DEBUG, SVARN_AD_STYLE, DebugStyle.EDITOR_GAME_DEBUG.ToString(), true)));
        
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

                string[] splits = Load(Catalog.Loader(Path_Preferences, SVARCN_DEBUG, ToLayerStreamingName(i), LayerToSavableString(baseName), true)).Split(SVARS_LAYER_STATE, StringSplitOptions.None);
                toReturn[i] = new DebugLayer(splits[0], Convert.ToBoolean(splits[1]));
            }

            return toReturn;
        }

        internal static void SaveDebugLayers(DebugLayer[] layers)
        {
            if (layers == null || layers.Length != 33)
                throw new Exception("Cannot save layers as the value given is either null, or is not 33 in length.");
            
            for(int i = 0; i < layers.Length; i++)
            {
                string toUse = LayerToSavableString(layers[i].Name, layers[i].Active);
                switch (i)
                {
                    case DEBUG_LAYER_EXCEPTIONS_INDEX: toUse = LayerToSavableString(DEBUG_LAYER_EXCEPTIONS_NAME, true); break;
                    case DEBUG_LAYER_WWW_INDEX: toUse = LayerToSavableString(DEBUG_LAYER_WWW_NAME, layers[i].Active); break;
                }
                Save(Catalog.Saver(Path_Preferences, SVARCN_DEBUG, ToLayerStreamingName(i), toUse));
            }
        }

        internal static string GetDebugColorSaveFormat(Color color)
        {
            return string.Format("{1}{0}{2}{0}{3}", SVARS_DEFAULT_SPLIT, color.r, color.g, color.b);
        }

        internal static void SaveDebugColor(MessageType @for, Color color)
        {
            Save(Catalog.Saver(Path_Preferences, SVARCN_DEBUG, GetColorVarName(@for), GetDebugColorSaveFormat(color)));
        }

        internal static string GetColorVarName(MessageType @for)
        {
            switch (@for)
            {
                default:
                    return SVARN_AD_COLOR_LOG;
                case MessageType.Warning:
                    return SVARN_AD_COLOR_WARNING;
                case MessageType.Error:
                    return SVARN_AD_COLOR_ERROR;
            }
        }

        private static Color GetDebugColor(MessageType @for)
        {
            float r, g, b;
            string[] split;

            split = Load(Catalog.Loader(Path_Preferences, SVARCN_DEBUG, GetColorVarName(@for), GetDebugColorSaveFormat(Color.white), true)).Split(SVARS_DEFAULT_SPLIT);

            r = float.Parse(split[0]);
            g = float.Parse(split[1]);
            b = float.Parse(split[2]);

            return new Color(r, g, b);
        }
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
        #endregion

        #region Nyu Settings
        /*private static bool i_ProjectileCount;
        private static int sub_ProjectileCount = 1000;
        /// <summary>
        /// The projectile pool size of <see cref="NyuProjectile"/>.
        /// </summary>
        public static int NyuProjectilesPoolSize
        {
            get
            {
                if(!i_ProjectileCount)
                {
                    string countSaved = Load(Catalog.Loader(Path_Settings, SVARNCN_NYUENTITIES, nameof(sub_DefaultStackingType).ToUpper()));
                    if(!string.IsNullOrEmpty(countSaved))
                        sub_ProjectileCount = Convert.ToInt32(countSaved);

                    i_ProjectileCount = true;
                }

                return sub_ProjectileCount;
            }
            set
            {
                if(value != sub_ProjectileCount)
                {
                    Save(Catalog.Saver(Path_Settings, SVARNCN_NYUENTITIES, nameof(sub_ProjectileCount).Substring(4).ToUpper(), sub_ProjectileCount.ToString()));
                }
            }
        }*/
        #endregion

        #region Other
        private static bool i_DefaultStackingType;
        private static Type sub_DefaultStackingType = null;
        /// <summary>
        /// The default type of stacking used by Stat drawers and custom editors from the <see cref="WarWolfWorks"/> library.
        /// </summary>
        public static Type DefaultStackingType
        {
            get
            {
                if(!i_DefaultStackingType)
                {
                    string typeName = Load(Catalog.Loader(Path_Settings, SVARNCN_OTHER, nameof(sub_DefaultStackingType).ToUpper()));
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
                        Save(Catalog.Saver(Path_Settings, SVARNCN_OTHER, nameof(sub_DefaultStackingType).ToUpper(), value.ToString()));
                    else Remove(Catalog.Loader(Path_Settings, SVARNCN_OTHER, nameof(sub_DefaultStackingType).ToUpper()));
                }
            }
        }

        private static bool i_DefaultAffectionsType;
        private static Type sub_DefaultAffectionsType = null;
        /// <summary>
        /// The default type of affection used by Stat drawers and custom editors from the <see cref="WarWolfWorks"/> library.
        /// </summary>
        public static Type DefaultAffectionsType
        {
            get
            {
                if(!i_DefaultAffectionsType)
                {
                    string typeName = Load(Catalog.Loader(Path_Settings, SVARNCN_OTHER, nameof(sub_DefaultAffectionsType).ToUpper()));
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
                        Save(Catalog.Saver(Path_Settings, SVARNCN_OTHER, nameof(sub_DefaultAffectionsType).ToUpper(), value.ToString()));
                    else Remove(Catalog.Loader(Path_Settings, SVARNCN_OTHER, nameof(sub_DefaultAffectionsType).ToUpper()));
                }
            }
        }


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
                    libraryLanguage = Hooks.Parse<SystemLanguage>(Load(Catalog.Loader(Path_Preferences, SVARNCN_OTHER, SVARN_SETTINGS_LANGUAGE, Application.systemLanguage.ToString(), true)));
                    languageInit = true;
                }
                return libraryLanguage;
            }
            set
            {
                libraryLanguage = value;
                Save(Catalog.Saver(Path_Preferences, SVARNCN_OTHER, SVARN_SETTINGS_LANGUAGE, value.ToString()));
            }
        }
        #endregion
    }
}
