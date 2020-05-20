using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using WarWolfWorks.EditorBase.Interfaces;
using WarWolfWorks.Internal;
using WarWolfWorks.Utility;
using static WarWolfWorks.AdvancedDebug;
using static WarWolfWorks.EditorBase.Constants;

namespace WarWolfWorks.EditorBase.Services
{
    internal sealed class AdvancedDebugService : IService
    {
        public string Name => ELS_AdvancedDebug;

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

        private ReorderableList list;
        private DebugLayer[] Layers;
        private Settings.DebugStyle DebugStyle;
        private Color LogColor, WarningColor, ErrorColor;
        private Vector2 LayersPosition;

        private GUIStyle GrayedStyle;

        public void OnEnable()
        {
            LoadVars();
        }

        private void LoadVars()
        {
            Layers = Settings.GetDebugLayers();

            list = new ReorderableList(new List<DebugLayer>(Layers), typeof(DebugLayer), false, false, false, false);
            UpdateLayersEvent();

            GrayedStyle = new GUIStyle() { richText = true };

            DebugStyle = Settings.GetDebugStyle();
            LogColor = Settings.GetDebugLogColor();
            WarningColor = Settings.GetDebugWarningColor();
            ErrorColor = Settings.GetDebugErrorColor();
        }

        private void UpdateLayersEvent()
        {
            list.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    Rect drawer = new Rect(rect);
                    float itemWidth = drawer.width / 3;
                    drawer.width = itemWidth;
                    drawer.height = EditorGUIUtility.singleLineHeight;

                    EditorGUI.LabelField(drawer, $"<color=#D2D2D2>{LS_ADLayerCodename} {index}</color>", GrayedStyle);

                    drawer.xMin = itemWidth;
                    drawer.xMax = itemWidth * 2;

                    string Name = Layers[index].Name;
                    if (index == 0 || index == 1) EditorGUI.LabelField(drawer, Name, GrayedStyle);
                    else Name = EditorGUI.TextField(drawer, Name);

                    drawer.xMin = itemWidth * 2;
                    drawer.xMax = itemWidth * 3;

                    bool Active = Layers[index].Active;
                    if (index != 0) Active = EditorGUI.Toggle(drawer, LS_ADActiveState, Active);

                    Layers[index] = new DebugLayer(Name, Active);

                    rect.y += 2;
                };
        }

        public void Draw()
        {
            EditorGUILayout.LabelField(LS_ADLayers);
            LayersPosition = EditorGUILayout.BeginScrollView(LayersPosition);

            list.DoLayoutList();

            EditorGUILayout.EndScrollView();

            DebugStyle = (Settings.DebugStyle)EditorGUILayout.EnumPopup(LS_ADStyle, DebugStyle);

            LogColor = EditorGUILayout.ColorField(new GUIContent(LS_ADColorLog), LogColor, true, false, false);
            WarningColor = EditorGUILayout.ColorField(new GUIContent(LS_ADColorWarning), WarningColor, true, false, false);
            ErrorColor = EditorGUILayout.ColorField(new GUIContent(LS_ADColorError), ErrorColor, true, false, false);
        }

        public void OnDisable()
        {
            Settings.SaveDebugColor(WarWolfWorks.Internal.MessageType.Info, LogColor);
            Settings.SaveDebugColor(WarWolfWorks.Internal.MessageType.Warning, WarningColor);
            Settings.SaveDebugColor(WarWolfWorks.Internal.MessageType.Error, ErrorColor);
            Settings.SaveDebugLayers(Layers);
            RefreshDebugger();
        }
    }
}
