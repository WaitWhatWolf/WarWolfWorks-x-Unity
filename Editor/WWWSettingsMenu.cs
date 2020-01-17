using UnityEditor;
using UnityEngine;
using WarWolfWorks.Internal;
using static WarWolfWorks.Utility.Hooks.Streaming;
using static WarWolfWorks.AdvancedDebug;
using WarWolfWorks.Utility;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Linq;

namespace WarWolfWorks.EditorBase
{
    public class WWWSettingsMenu : EditorWindow
    {
        #region Canvas
        
        private readonly string[] CanvasNames = new string[]
        {
            "UTILITY_CANVAS_TYPE",
            "UTILITY_CANVAS_RESOURCES_PATH",
            "UTILITY_CANVAS_NAME_LOAD"
        };

        private string[] CanvasValues;

        private string[] DebugNames => Settings.DebugNames;
        private string[] DebugValues;

        private SystemLanguage Language;
        private Settings.UtilityCanvasType CanvasType;
        private string CanvasResourcesPath, CanvasNameLoad;
        #endregion

        #region Language Strings
        //new LanguageString("", ("", SystemLanguage.Polish), ("", SystemLanguage.Japanese));
        private readonly LanguageString LS_UCSettings = 
            new LanguageString("Utility Canvas Settings", ("Ustawienia Użytecznego Canvasu", SystemLanguage.Polish), ("ユーティリティーキャンバス設定", SystemLanguage.Japanese));
        private readonly LanguageString LS_UCLoadType =
            new LanguageString("Load Mode", ("Tryb Ładowania", SystemLanguage.Polish), ("ロードモード", SystemLanguage.Japanese));
        private readonly LanguageString LS_UCPath =
            new LanguageString("Resources Path", ("Ścieszka Zasobów", SystemLanguage.Polish), ("リソースフォルダ", SystemLanguage.Japanese));
        private readonly LanguageString LS_UCName =
            new LanguageString("Name", ("Nazwa", SystemLanguage.Polish), ("名", SystemLanguage.Japanese));
        private readonly LanguageString LS_ADSettings =
            new LanguageString("Advanced Debug Settings", ("Ustawienia Zaawansowanego Debugowania", SystemLanguage.Polish), ("アドバンスドデバッグ設定", SystemLanguage.Japanese));
        private readonly LanguageString LS_ADLayers =
            new LanguageString("Layers", ("Warstwy", SystemLanguage.Polish), ("レイヤーズ", SystemLanguage.Japanese));
        private readonly LanguageString LS_ADLayerCodename =
            new LanguageString("Layer", ("Warstwa", SystemLanguage.Polish), ("レイヤー", SystemLanguage.Japanese));
        private readonly LanguageString LS_ADActiveState =
            new LanguageString("Active", ("Aktywny", SystemLanguage.Polish), ("アクティブ", SystemLanguage.Japanese));
        private readonly LanguageString LS_ADStyle =
            new LanguageString("Style", ("Styl", SystemLanguage.Polish), ("スタイル", SystemLanguage.Japanese));
        private readonly LanguageString LS_ADColorLog =
            new LanguageString("Log Color", ("Kolor Logowania", SystemLanguage.Polish), ("ログ色", SystemLanguage.Japanese));
        private readonly LanguageString LS_ADColorWarning =
            new LanguageString("Warning Color", ("Kolor Ostrzeżenia", SystemLanguage.Polish), ("警告色", SystemLanguage.Japanese));
        private readonly LanguageString LS_ADColorError =
            new LanguageString("Error Color", ("Kolor Błędów", SystemLanguage.Polish), ("エラー色", SystemLanguage.Japanese));
        private readonly LanguageString LS_OSettings =
            new LanguageString("Other Settings", ("Inne Ustawienia", SystemLanguage.Polish), ("その他設定", SystemLanguage.Japanese));
        private readonly LanguageString LS_OLangReminder =
            new LanguageString("(Currently WarWolfWorks library is translated with English, Polish and Japanese)", 
                ("(Obecnie, biblioteka klas WarWolfWorks jest przetłumaczona na Angielski, Polski i Japoński)", SystemLanguage.Polish), 
                ("（今すぐ、WarWolfWorks図書館は英語、ポーランド語と日本語に翻訳されています）", SystemLanguage.Japanese));
        private readonly LanguageString LS_OLanguage =
            new LanguageString("Language", ("Język", SystemLanguage.Polish), ("言語", SystemLanguage.Japanese));
        private readonly LanguageString LS_Refresh =
            new LanguageString("Refresh", ("Odśwież", SystemLanguage.Polish), ("リフレッシュ", SystemLanguage.Japanese));
        private readonly LanguageString LS_Apply =
            new LanguageString("Apply", ("Zastosuj", SystemLanguage.Polish), ("当てはまる", SystemLanguage.Japanese));

        #endregion

        private bool LayersFoldout;
        private ReorderableList list;
        private DebugLayer[] Layers;
        private Settings.DebugStyle DebugStyle;
        private Color LogColor, WarningColor, ErrorColor;
        private Vector2 LayersPosition;

        private GUIStyle TitleStyle;
        private GUIStyle GrayedStyle;

        private GUIStyle SmallNoteStyle;

        private GUIStyle RichStyle;

        private const float LIST_CELL_PADDING_X = 10;

        [MenuItem("WarWolfWorks/Settings")]
        public static void WWWSettingsMenuInit()
        {
            GetWindow<WWWSettingsMenu>("Settings").Show();
        }

        private void OnEnable()
        {
            TitleStyle = new GUIStyle() { fontSize = 20, richText = true };
            GrayedStyle = new GUIStyle() { richText = true };
            SmallNoteStyle = new GUIStyle() { fontSize = 10, richText = true };

            LoadVars();
        }

        private void LoadVars()
        {
            CanvasType = Settings.GetUtilityCanvasType();
            CanvasResourcesPath = Settings.GetUtilityCanvasResourcesPath();
            CanvasNameLoad = Settings.GetUtilityCanvasNameLoad();

            Layers = Settings.GetDebugLayers();
            list = new ReorderableList(new List<DebugLayer>(Layers), typeof(DebugLayer), false, false, false, false);
            UpdateLayersEvent();

            DebugStyle = Settings.GetDebugStyle();
            LogColor = Settings.GetDebugLogColor();
            WarningColor = Settings.GetDebugWarningColor();
            ErrorColor = Settings.GetDebugErrorColor();

            Language = Settings.LibraryLanguage;
        }

        private void UpdateLayersEvent()
        {
            list.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float rectThird = rect.width / 3;
                    EditorGUI.LabelField(new Rect(rect.x + LIST_CELL_PADDING_X, rect.y, rectThird - LIST_CELL_PADDING_X, EditorGUIUtility.singleLineHeight), $"<color=#D2D2D2>{LS_ADLayerCodename} {index}</color>", GrayedStyle);

                    Rect NameRect = new Rect(rect.x + rectThird + LIST_CELL_PADDING_X, rect.y, rectThird - LIST_CELL_PADDING_X, EditorGUIUtility.singleLineHeight);
                    string Name = Layers[index].Name;
                    if (index == 0 || index == 1) EditorGUI.LabelField(NameRect, Name, GrayedStyle);
                    else Name = EditorGUI.TextField(NameRect, Name);

                    bool Active = Layers[index].Active;
                    if(index != 0) Active = EditorGUI.Toggle(new Rect(rect.x + (rectThird * 2) + LIST_CELL_PADDING_X, rect.y, rectThird - LIST_CELL_PADDING_X, EditorGUIUtility.singleLineHeight), LS_ADActiveState, Active);

                    Layers[index] = new DebugLayer(Name, Active);

                    rect.y += 2;
                };
        }

        private DebugLayer[] GetSortedArray()
            => Hooks.Enumeration.ToGeneric<DebugLayer>(list.list).ToArray();

        private string ColorSaver(Color c) => $"{c.r};{c.g};{c.b}";

        private void OnGUI()
        {
            EditorHooks.SpacedLabel($"<color=#D6D6FF>{LS_UCSettings}</color>", TitleStyle);
            EditorGUILayout.Space();

            CanvasType = (Settings.UtilityCanvasType)EditorGUILayout.EnumPopup(LS_UCLoadType, CanvasType);
            switch (CanvasType)
            {
                case Settings.UtilityCanvasType.PREFABBED:
                    CanvasResourcesPath = EditorGUILayout.TextField(LS_UCPath, CanvasResourcesPath);
                    break;
                case Settings.UtilityCanvasType.BY_NAME_IN_SCENE:
                    CanvasNameLoad = EditorGUILayout.TextField(LS_UCName, CanvasNameLoad);
                    break;
            }

            EditorHooks.SpacedLabel($"<color=#D6D6FF>{LS_ADSettings}</color>", TitleStyle);

            LayersFoldout = EditorGUILayout.Foldout(LayersFoldout, LS_ADLayers);

            if (LayersFoldout)
            { 
                LayersPosition = EditorGUILayout.BeginScrollView(LayersPosition);

                list.DoLayoutList();

                EditorGUILayout.EndScrollView();
            }

            DebugStyle = (Settings.DebugStyle)EditorGUILayout.EnumPopup(LS_ADStyle, DebugStyle);

            LogColor = EditorGUILayout.ColorField(new GUIContent(LS_ADColorLog), LogColor, true, false, false);
            WarningColor = EditorGUILayout.ColorField(new GUIContent(LS_ADColorWarning), WarningColor, true, false, false);
            ErrorColor = EditorGUILayout.ColorField(new GUIContent(LS_ADColorError), ErrorColor, true, false, false);

            EditorHooks.SpacedLabel($"<color=#D6D6FF>{LS_OSettings}</color>", TitleStyle);

            EditorGUILayout.LabelField($"<color=#808080>{LS_OLangReminder}</color>", SmallNoteStyle, GUILayout.Height(10));
            Language = (SystemLanguage)EditorGUILayout.EnumPopup(LS_OLanguage, Language);

            EditorHooks.SlickSeparator();

            if (GUILayout.Button(LS_Refresh))
            {
                LoadVars();
            }

            if (GUILayout.Button(LS_Apply))
            {
                CanvasValues = new string[] { CanvasType.ToString(), CanvasResourcesPath, CanvasNameLoad };

                Catalog[] canvasSavers = Catalog.Savers(Settings.SettingsPath, Settings.CATEGORY_CANVAS_NAME, CanvasNames, CanvasValues);
                SaveAll(canvasSavers);

                Settings.SaveDebugLayers(GetSortedArray());

                DebugValues = new string[] { DebugStyle.ToString(),
                    ColorSaver(LogColor), ColorSaver(WarningColor), ColorSaver(ErrorColor) };


                Catalog[] debugSavers = Catalog.Savers(Settings.SettingsPath, Settings.CATEGORY_DEBUG_NAME, DebugNames, DebugValues);

                SaveAll(debugSavers);
                RefreshDebugger();

                Settings.LibraryLanguage = Language; //Saved automatically.
            }
        }
    }
}
