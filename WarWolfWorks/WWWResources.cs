using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;
using WarWolfWorks.Internal;
using WarWolfWorks.NyuEntities;
using WarWolfWorks.NyuEntities.ProjectileSystem;
using WarWolfWorks.IO.CTS;
using WarWolfWorks.Utility;
using WarWolfWorks.Debugging;
using WarWolfWorks.Enums;
using WarWolfWorks.UI.MenusSystem.SlickMenu;

[assembly: InternalsVisibleTo("WarWolfWorks.EditorBase.Services")]
[assembly: InternalsVisibleTo("WarWolfWorks.EditorBase")]
namespace WarWolfWorks
{
    /// <summary>
    /// All constant and readonly values of the <see cref="WarWolfWorks"/> library.
    /// </summary>
    public static class WWWResources
    {
        #region Streaming
        internal const int STREAMING_FILE_ENCRYPTION_JUMPER = 85;

        /// <summary>
        /// Path to personal preferences of this library.
        /// </summary>
        public static readonly string SV_Path_Preferences =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"WarWolfWorks\",
                Application.productName,
                "Preferences.cts");
        /// <summary>
        /// Path to the WWWSettings.cts file.
        /// </summary>
        public static readonly string SV_Path_Settings = Hooks.Streaming.GetStreamingAssetsFilePath("WWWSettings.cts");
        /// <summary>
        /// Path to DefaultKeys.cts.
        /// </summary>
        internal static readonly string SV_Path_DefaultKeys = Path.Combine(Application.streamingAssetsPath.Replace("/", "\\"), "DefaultKeys.cts");
        /// <summary>
        /// Category name for keys.
        /// </summary>
        internal const string SVCN_KEYS = "Keys";
        /// <summary>
        /// Default variable split character for saving.
        /// </summary>
        public const char SV_DEFAULT_SPLIT = ';';
        #endregion

        #region Regex Expressions
        /// <summary>
        /// Matches any string value that starts with "s_" or "S_".
        /// </summary>
        public static readonly Regex Expression_NoS = new Regex(@"^[sS]_");

        /// <summary>
        /// Matches all japanese hiragana text.
        /// </summary>
        public static readonly Regex Expression_Hiragana = new Regex(@"[\u3041-\u3096]+");
        /// <summary>
        /// Matches all japanese katakana text.
        /// </summary>
        public static readonly Regex Expression_Katakana = new Regex(@"[\u30A0-\u30FF]+");
        /// <summary>
        /// Matches all kanji text.
        /// </summary>
        public static readonly Regex Expression_Kanji = new Regex(@"[\u3400-\u4DB5\u4E00-\u9FCB\uF900-\uFA6A]+");
        /// <summary>
        /// Matches all japanese text. Note: Does not match if the given text contains kanji characters only;
        /// For such an expression match, use <see cref="Expression_Japanese_Greedy"/>
        /// </summary>
        public static readonly Regex Expression_Japanese = new Regex(@"([\u3400-\u4DB5\u4E00-\u9FCB\uF900-\uFA6A]*[\u3041-\u3096]|[\u30A0-\u30FF])+");
        /// <summary>
        /// Matches katakana, hiragana and kanji; Note: Can have ambiguity with Chinese, to avoid such behavior use <see cref="Expression_Japanese"/> instead.
        /// </summary>
        public static readonly Regex Expression_Japanese_Greedy = new Regex(@"([\u3400-\u4DB5\u4E00-\u9FCB\uF900-\uFA6A]|[\u3041-\u3096]|[\u30A0-\u30FF])+");
        #endregion

        #region WarWolfWorks Settings
        /// <summary>
        /// Name of the category given to the <see cref="Settings.UtilityCanvas"/> Settings in WWWSettings.ini.
        /// </summary>
        internal const string SVCN_CORECANVAS = "ユチリテイキャンバス";
        internal const string VN_DEFAULT_CANVAS = "WWWCanvas";
        internal const string SVCN_OTHER = "その他";
        internal const string SVCN_NYUENTITIES = "ニューエンチチズ";
        internal const string SVN_SETTINGS_LANGUAGE = "言語";
        /// <summary>
        /// Name of the category given to <see cref="AdvancedDebug"/> settings in WWWSettings.ini.
        /// </summary>
        internal const string SVCN_DEBUG = "デバッグ";
        internal static readonly string[] SVS_Layers_State = new string[] { " With Active State " };
        internal const string SVN_AD_LAYER = "レイヤー";
        internal const string SVN_AD_STYLE = "STYLE";
        internal const string SVN_AD_COLOR_LOG = "COLOR_LOG";
        internal const string SVN_AD_COLOR_WARNING = "COLOR_WARNING";
        internal const string SVN_AD_COLOR_ERROR = "COLOR_ERROR";

        internal const string SVN_UC_TYPE = "UTILITY_CANVAS_TYPE";
        internal const string SVN_UC_RESOURCES_PATH = "UTILITY_CANVAS_RESOURCES_PATH";
        internal const string SVN_UC_NAME_LOAD = "SVARN_UC_NAME_LOAD";
        #endregion

        #region Language Strings
        internal static readonly LanguageString LS_Unknown = new LanguageString(
            "Unknown",
            ("Nieznany", SystemLanguage.Polish),
            ("不明", SystemLanguage.Japanese));
        internal static readonly LanguageString LS_Add = new LanguageString(
            "Add",
            ("Dodaj", SystemLanguage.Polish),
            ("追加", SystemLanguage.Japanese));
        internal static readonly LanguageString LS_Remove = new LanguageString(
            "Remove",
            ("Usuń", SystemLanguage.Polish),
            ("削除", SystemLanguage.Japanese));
        internal static readonly LanguageString LS_RemoveExpanded = new LanguageString(
            "Remove Expanded",
            ("Usuń Rozwinięte", SystemLanguage.Polish),
            ("拡したを削除", SystemLanguage.Japanese));
        internal static readonly LanguageString LS_Value = new LanguageString(
           "Value",
           ("Waluta", SystemLanguage.Polish),
           ("バリュー", SystemLanguage.Japanese));
        internal static readonly LanguageString LS_Stat_Affections = new LanguageString(
           "Affections",
           ("Afekty", SystemLanguage.Polish),
           ("アフェクションズ", SystemLanguage.Japanese));
        internal static readonly LanguageString LS_Stat_Stacking = new LanguageString(
           "Stacking",
           ("Stakowanie", SystemLanguage.Polish),
           ("スタッキング", SystemLanguage.Japanese));
        internal static readonly LanguageString LS_Confirm = new LanguageString(
            "Confirm",
            ("Potwierdz", SystemLanguage.Polish),
            ("追認", SystemLanguage.Japanese));
        internal static readonly LanguageString LS_Previous = new LanguageString("Previous", ("Poprzedni", SystemLanguage.Polish), ("前に", SystemLanguage.Japanese));
        internal static readonly LanguageString LS_Next = new LanguageString("Next", ("Następny", SystemLanguage.Polish), ("次に", SystemLanguage.Japanese));
        internal static readonly LanguageString LS_Type = new LanguageString("Type", ("Typ", SystemLanguage.Polish), ("タイプ", SystemLanguage.Japanese));
        internal static readonly LanguageString LS_Check = new LanguageString("Check", ("Sprawdź", SystemLanguage.Polish), ("確認", SystemLanguage.Japanese));
        internal static readonly LanguageString LS_Reset = new LanguageString("Reset", ("Resetuj", SystemLanguage.Polish), ("リセット", SystemLanguage.Japanese));
        #endregion

        #region Compile Time
        internal const string V_SCATALOGSAVE_OBSOLETE_MESSAGE = "The Streaming.Catalog system is replaced by WarWolfWorks.Streaming.CTS and will be removed in the very near future; Consider switching to WarWolfWorks.Streaming.CTS.";
        internal const bool V_SCATALOGSAVE_OBSOLETE_ISERROR = false;
        #endregion

        #region Unity Stuff
        /// <summary>
        /// An instance of <see cref="WaitForFixedUpdate"/>.
        /// </summary>
        public static readonly WaitForFixedUpdate FixedUpdateWaiter = new WaitForFixedUpdate();
        /// <summary>
        /// An instance of <see cref="WaitForEndOfFrame"/>.
        /// </summary>
        public static readonly WaitForEndOfFrame LateUpdateWaiter = new WaitForEndOfFrame();

        internal const string IN_ASSETMENU_WARWOLFWORKS = "WarWolfWorks/";
        internal const string IN_ASSETMENU_UTILITY = "Utility/";
        internal const string IN_ASSETMENU_UI = "UI/";
        internal const string IN_ASSETMENU_MENUSYSTEM = "Menu System/";
        internal const string IN_ASSETMENU_PHYSICS = "Physics/";
        #endregion

        #region Projectiles
        /// <summary>
        /// Name of the <see cref="NyuProjectile"/> game object.
        /// </summary>
        public const string VN_PROJECTILE_HOLDER = "Projectiles";

        /// <summary>
        /// Name of projectile objects in the scene.
        /// </summary>
        public const string VN_PROJECTILE = "Projectile_";
        #endregion

        #region Nyu Entities
        /// <summary>
        /// Name of the <see cref="NyuManager"/>.
        /// </summary>
        public const string VN_NYUMANAGER = "NyuManager";
        #endregion

        #region Misc
        internal const int IN_COLORMANAGER_LAYER_MAX = 8;

        internal const int V_AUDIOMANAGER_MIN_POOLSIZE = 15;
        internal const int V_TRANSITIONMANAGER_TRANSITIONS_SIZE = 32;
        #endregion

        #region Debugging
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
        #endregion

        #region CTS
        internal static readonly Catalog CTS_Settings_CoreCanvas = new Catalog(SV_Path_Settings, SVCN_CORECANVAS);
        internal static readonly Catalog CTS_Settings_AdvancedDebug = new Catalog(SV_Path_Settings, SVCN_DEBUG);
        internal static readonly Catalog CTS_Settings_Misc = new Catalog(SV_Path_Settings, SVCN_OTHER);
        internal static readonly Catalog CTS_Preferences_AdvancedDebug = new Catalog(SV_Path_Preferences, SVCN_DEBUG);
        internal static readonly Catalog CTS_Preferences_Misc = new Catalog(SV_Path_Preferences, SVCN_OTHER);

        internal static readonly Catalog CTS_DefaultKeys = new Catalog(SV_Path_DefaultKeys, SVCN_KEYS);
        #endregion

        #region UI
        #region Slick UI
#pragma warning disable 1591
        public static Vector4 UI_Slick_Rect_Left = new Vector4(0, 0, 0f, 1f);
        public static Vector4 UI_Slick_Rect_Right = new Vector4(1f, 0f, 1f, 1f);
        public static Vector4 UI_Slick_Rect_Top = new Vector4(0f, 1f, 1f, 1f);
        public static Vector4 UI_Slick_Rect_Bot = new Vector4(0f, 0f, 1f, 0f);
        public const float UI_SLICK_OFFSET_X = 1f;
        public const float UI_SLICK_OFFSET_Y = 1f;
#pragma warning restore 1591
        /// <summary>
        /// The default transparency of slick UI elements.
        /// </summary>
        public const float UI_SLICK_BACK_TRANSPARENCY = 0.25f;

        /// <summary>
        /// The default color of slick UI elements.
        /// </summary>
        public static readonly Color UI_Slick_Color_Default = Hooks.Colors.Tangelo;
        #endregion
        #endregion

        #region Runtime Resource Methods
        /// <summary>
        /// Used to apply offset to <see cref="SlickBorder"/> based on given flags.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="rectTransform"></param>
        public static void AdaptSlickAnchors(SlickBorderFlags flags, RectTransform rectTransform)
        {
            switch (flags)
            {
                case SlickBorderFlags.Left:
                    rectTransform.offsetMin = new Vector2(-UI_SLICK_OFFSET_X, -UI_SLICK_OFFSET_Y);
                    rectTransform.offsetMax = new Vector2(0, UI_SLICK_OFFSET_Y);
                    break;
                case SlickBorderFlags.Right:
                    rectTransform.offsetMin = new Vector2(0, -UI_SLICK_OFFSET_Y);
                    rectTransform.offsetMax = new Vector2(UI_SLICK_OFFSET_X, UI_SLICK_OFFSET_Y);
                    break;
                case SlickBorderFlags.Top:
                    rectTransform.offsetMin = new Vector2(-UI_SLICK_OFFSET_X, 0);
                    rectTransform.offsetMax = new Vector2(UI_SLICK_OFFSET_X, UI_SLICK_OFFSET_Y);
                    break;
                case SlickBorderFlags.Bot:
                    rectTransform.offsetMin = new Vector2(-UI_SLICK_OFFSET_X, -UI_SLICK_OFFSET_Y);
                    rectTransform.offsetMax = new Vector2(UI_SLICK_OFFSET_X, 0);
                    break;
            }
        }
        #endregion
    }
}
