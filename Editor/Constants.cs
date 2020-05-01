using UnityEngine;
using WarWolfWorks.EditorBase.Interfaces;
using WarWolfWorks.EditorBase.Services;
using WarWolfWorks.IO.CTS;
using WarWolfWorks.Utility;
using static WarWolfWorks.Constants;

namespace WarWolfWorks.EditorBase
{
    /// <summary>
    /// All constant values of the <see cref="WarWolfWorks.EditorBase"/> namespace.
    /// </summary>
    public static class Constants
    {
        #region Streaming
        internal const string SVCN_ENUMENTRIES = "Enumリストエントリー";
        #endregion

        #region Service Menu
        internal static readonly IService[] Settings_Tab_Menus = new IService[]
        {
            new DefaultKeysService(),
            new UtilityCanvasService(),
            new AdvancedDebugService(),
            new MiscService()
        };

        internal static readonly Color Settings_Tab_Color_Selected = new Color(0.4f, 0.4f, 0.4f, 0.4f);
        internal static readonly Color Settings_Tab_Color_Default = new Color(0.25f, 0.25f, 0.25f, 0.5f);
        internal const float V_SERVICES_TAB_SEPARATOR_WIDTH = 2;
        internal const float V_SERVICES_TAB_HEIGHT = 30;
        internal static readonly Vector2 Services_Window_Size_Min = new Vector2(525, 300);
        internal static readonly LanguageString ELS_DefaultKeys = new LanguageString("Default Keys", ("domyślne Przyciski", SystemLanguage.Polish), ("デフォルト・キーズ", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_AdvancedDebug = new LanguageString("Advanced Debug", ("Zaawansowane Debugowanie", SystemLanguage.Polish), ("アドバンスドデバッグ", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_UtilityCanvas = new LanguageString("Utility Canvas", ("Użyteczny Canvas", SystemLanguage.Polish), ("ユーティリティーキャンバス", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_Misc = new LanguageString("Misc.", ("Inne Ustawienia", SystemLanguage.Polish), ("その他設定", SystemLanguage.Japanese));

        internal static readonly LanguageString LS_Refresh =
            new LanguageString("Refresh", ("Odśwież", SystemLanguage.Polish), ("リフレッシュ", SystemLanguage.Japanese));
        internal static readonly LanguageString LS_Apply =
            new LanguageString("Apply", ("Zastosuj", SystemLanguage.Polish), ("当てはまる", SystemLanguage.Japanese));
        internal const float EV_AD_CELL_CONTENT_PADDING_X = 10;
        #endregion

        #region Properties
        /// <summary>
        /// Display name of a <see cref="IStat"/>'s Stacking.
        /// </summary>
        public const string EVN_STAT_STACKING = "Stacking";
        /// <summary>
        /// Display name of a <see cref="IStat"/>'s Affections.
        /// </summary>
        public const string EVN_STAT_AFFECTIONS = "Affections";
        #endregion

        #region readonly
        #endregion

        #region Custom Editors
        /// <summary>
        /// The amount by which the width gets divided for <see cref="ItemEditor"/>'s name field.
        /// </summary>
        internal const float EV_ITEM_NAME_WIDTH_DIV = 1.2f;

        /// <summary>
        /// The divider of the with of the attack label.
        /// </summary>
        internal const float EV_ENTITYATTACK_ATK_LABEL_WIDTH_DIV = 3f;

        internal static readonly GUIStyle GUIS_EntityAttack_Atk_Label = new GUIStyle()
        {
            alignment = TextAnchor.MiddleCenter,
            richText = false,
            fontSize = 12
        };

        internal static readonly GUIStyle GUIS_Nyu_Component_Foldout_Style = new GUIStyle()
        {
            alignment = TextAnchor.MiddleCenter,
            richText = true,
            fontSize = 12
        };

        /// <summary>
        /// A title <see cref="GUIStyle"/> used to separate variables into different categories.
        /// </summary>
        internal static readonly GUIStyle GUIS_DefaultTitle_0 = new GUIStyle()
        {
            alignment = TextAnchor.MiddleCenter,
            richText = true,
            fontSize = 20,
            fontStyle = FontStyle.BoldAndItalic
        };

        internal static readonly Color NyuEntity_Component_Selected = new Color(0.8f, 0.8f, 0.8f, 0.6f);
        internal static readonly Color NyuEntity_Component_Deselected = new Color(0.4f, 0.4f, 0.4f, 0.5f);
        #endregion

        #region Editor Language Strings
        internal static readonly LanguageString ELS_Additional_Settings = new LanguageString(
            "Additional Settings",
            ("Dodatkowe Ustawienia", SystemLanguage.Polish),
            ("追加の設定", SystemLanguage.Japanese));

        internal static readonly LanguageString ELS_EntityAttack_Attack = new LanguageString(
            "Attack",
            ("Atak", SystemLanguage.Polish),
            ("アタック", SystemLanguage.Japanese));

        internal static readonly LanguageString ELS_EntityAttack_Condition = new LanguageString(
            "Condition",
            ("Kondycja", SystemLanguage.Polish),
            ("コンディション", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_EntityAttack_Active = new LanguageString(
           "Active",
           ("Aktywny", SystemLanguage.Polish),
           ("アクティブ", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_EntityAttack_InstAttack = new LanguageString(
           "Use duplicate attack",
           ("Użyj zduplikowanego ataku", SystemLanguage.Polish),
           ("アタックを複写使用する", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_EntityAttack_InstCondition = new LanguageString(
           "Use duplicate condition",
           ("Użyj zduplikowanej kondycji", SystemLanguage.Polish),
           ("コンディションを複写使用する", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_EntityAttack_Point = new LanguageString(
           "Point",
           ("Punkt", SystemLanguage.Polish),
           ("ポイント", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_EntityMovement_Velocities = new LanguageString(
           "Show Velocities",
           ("Pokaż Prędkości", SystemLanguage.Polish),
           ("ヴェロシティ示す", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_EntityMovement_Velocity_Time = new LanguageString(
           "Time",
           ("Czas", SystemLanguage.Polish),
           ("時間", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_NyuEntity_Tag = new LanguageString(
            "Tag",
            ("タグ", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_NyuEntity_Components = new LanguageString(
            "Components",
            ("Komponenty", SystemLanguage.Polish),
            ("コンポーネント", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_NyuEntity_NoEditor = new LanguageString(
            "If you wish to serialize values, make {0} implement \n{1} or create a class that implements \n{2}.",
            ("Jeżeli chcesz serializować waluty {0}, \nzaimplementój {1} do niego lub stwórz klasę \nktura implementuje {2}.", SystemLanguage.Polish),
            ("aaa {0}\n{1}\n{2}", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_NyuEntity_ListHeader = new LanguageString(
            "Available Components",
            ("Dostępne Komponenty", SystemLanguage.Polish));
        internal static readonly LanguageString ELS_NyuStat_Countdown = new LanguageString(
            "Countdown",
            ("Odliczanie", SystemLanguage.Polish),
            ("カウントダウン", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_NyuStat_Stacking = new LanguageString(
            "Stacking",
            ("Stakowanie", SystemLanguage.Polish),
            ("スタッキング", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_NyuStat_Affections_Add = new LanguageString(
            "Add Affection",
            ("Dodaj Afekcję", SystemLanguage.Polish),
            ("アフェクションを追加", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_NyuStat_Affections_Remove = new LanguageString(
            "Remove Affection",
            ("Usuń Afekcję", SystemLanguage.Polish),
            ("アフェクションを削除", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_NyuStat_Affections = new LanguageString(
            "Affections",
            ("Afekcje", SystemLanguage.Polish),
            ("アフェクション", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_NyuStat_Level = new LanguageString(
            "Level",
            ("Poziom", SystemLanguage.Polish),
            ("レヴェル", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_Transition_Color = new LanguageString(
            "Color Transition Settings",
            ("Opcje tranzycji koloru", SystemLanguage.Polish),
            ("色の推移設定", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_Transition_Anchors = new LanguageString(
            "Anchor Transition Settings",
            ("Opcje tranzycji kotwic", SystemLanguage.Polish),
            ("アンカーの推移設定", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_Transition_Color_Unfocused = new LanguageString(
            "Color: Unfocused",
            ("Kolor: Nieskupiony", SystemLanguage.Polish),
            ("色: 中心ない", SystemLanguage.Japanese));
        internal static readonly LanguageString ELS_Transition_Color_Focused = new LanguageString(
            "Color: Focused",
            ("Kolor: Skupiony", SystemLanguage.Polish),
            ("色: 中心", SystemLanguage.Japanese));
        #endregion

        #region Default Keys Editor Window
        /// <summary>
        /// Name of the <see cref="DefaultKeys"/> editor window.
        /// </summary>
        public const string EV_DEFKEYS_NAME = "Default Keys Customizer";

        /// <summary>
        /// The Y size of key elements.
        /// </summary>
        public const float EV_DEFKEYS_KEY_CELL_SIZE_Y = 20;
        /// <summary>
        /// The padding between keys.
        /// </summary>
        public const float EV_DEFKEYS_KEY_CELL_PADDING_Y = 5;
        /// <summary>
        /// The default name of a new key.
        /// </summary>
        public const string EV_DEFKEYS_KEY_NEW = "New Key";
        #endregion

        #region CTS
        internal static readonly Catalog CTS_Preferences_EnumEntries = new Catalog(SV_Path_Settings, SVCN_ENUMENTRIES);
        #endregion
    }
}
