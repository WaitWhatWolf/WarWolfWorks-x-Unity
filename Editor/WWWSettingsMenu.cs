using UnityEditor;
using UnityEngine;
using WarWolfWorks.Internal;
using static WarWolfWorks.Utility.Hooks.Streaming;
using static WarWolfWorks.AdvancedDebug;

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

        private Settings.UtilityCanvasType CanvasType;
        private string CanvasResourcesPath, CanvasNameLoad;
        #endregion

        private string[] DebugNames => Settings.DebugNames;
        private string[] DebugValues;

        private bool LayersFoldout;
        private DebugLayer[] Layers;
        private Settings.DebugStyle DebugStyle;
        private Color LogColor, WarningColor, ErrorColor;
        private Vector2 LayersPosition;

        private GUIStyle TitleStyle;
        private GUIStyle GrayedStyle;

        [MenuItem("WarWolfWorks/Settings")]
        public static void WWWSettingsMenuInit()
        {
            GetWindow<WWWSettingsMenu>("Settings").Show();
        }

        private void OnEnable()
        {
            TitleStyle = new GUIStyle() { fontSize = 20, richText = true };
            GrayedStyle = new GUIStyle() { richText = true };
            CanvasType = Settings.GetUtilityCanvasType();
            CanvasResourcesPath = Settings.GetUtilityCanvasResourcesPath();
            CanvasNameLoad = Settings.GetUtilityCanvasNameLoad();

            Layers = Settings.GetDebugLayers();
            DebugStyle = Settings.GetDebugStyle();
            LogColor = Settings.GetDebugLogColor();
            WarningColor = Settings.GetDebugWarningColor();
            ErrorColor = Settings.GetDebugErrorColor();
        }

        private string ColorSaver(Color c) => $"{c.r};{c.g};{c.b}";

        private void OnGUI()
        {
            EditorGUILayout.LabelField("<color=#D6D6FF>UtilityCanvas Settings</color>", TitleStyle);
            EditorGUILayout.Space();

            CanvasType = (Settings.UtilityCanvasType)EditorGUILayout.EnumPopup("Utility Canvas Load Type", CanvasType);
            switch (CanvasType)
            {
                case Settings.UtilityCanvasType.PREFABBED:
                    CanvasResourcesPath = EditorGUILayout.TextField("Canvas Resources Path", CanvasResourcesPath);
                    break;
                case Settings.UtilityCanvasType.BY_NAME_IN_SCENE:
                    CanvasNameLoad = EditorGUILayout.TextField("Canvas Name", CanvasNameLoad);
                    break;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("<color=#D6D6FF>AdvancedDebug Settings</color>", TitleStyle);
            EditorGUILayout.Space();

            LayersFoldout = EditorGUILayout.Foldout(LayersFoldout, "Layers");
            EditorGUILayout.BeginVertical();
            LayersPosition = EditorGUILayout.BeginScrollView(LayersPosition, false, false);
            if (LayersFoldout)
            {
                for (int i = 0; i < Layers.Length; i++)
                {
                    EditorGUILayout.LabelField($"LAYER_{i}");
                    switch (i)
                    {
                        default:
                            Layers[i].Name = EditorGUILayout.DelayedTextField(Layers[i].Name);
                            Layers[i].Active = EditorGUILayout.Toggle("Active State:", Layers[i].Active);
                            break;
                        case ExceptionLayerIndex:
                            EditorGUILayout.LabelField($"<color=#{ColorUtility.ToHtmlStringRGB(Color.gray)}>{ExceptionLayerName}</color>", GrayedStyle);
                            break;
                        case WWWInfoLayerIndex:
                            EditorGUILayout.LabelField($"<color=#{ColorUtility.ToHtmlStringRGB(Color.gray)}>{WWWInfoLayerName}</color>", GrayedStyle);
                            Layers[i].Active = EditorGUILayout.Toggle("Active State:", Layers[i].Active);
                            break;
                    }
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            DebugStyle = (Settings.DebugStyle)EditorGUILayout.EnumPopup("Debug Style", DebugStyle);

            LogColor = EditorGUILayout.ColorField(new GUIContent("Log Color"), LogColor, true, false, false);
            WarningColor = EditorGUILayout.ColorField(new GUIContent("Warning Color"), WarningColor, true, false, false);
            ErrorColor = EditorGUILayout.ColorField(new GUIContent("Error Color"), ErrorColor, true, false, false);

            if (GUILayout.Button("Apply Changes"))
            {
                CanvasValues = new string[] { CanvasType.ToString(), CanvasResourcesPath, CanvasNameLoad };
                SaveAll(Catalog.Savers(Settings.SettingsPath, Settings.CanvasCategoryName, CanvasNames, CanvasValues));

                for(int i = 0; i < Layers.Length; i++)
                {
                    string toUse = Settings.LayerToSavableString(Layers[i].Name, Layers[i].Active);
                    switch(i)
                    {
                        case ExceptionLayerIndex: toUse = Settings.LayerToSavableString(ExceptionLayerName, Layers[i].Active); break;
                        case WWWInfoLayerIndex: toUse = Settings.LayerToSavableString(WWWInfoLayerName, Layers[i].Active); break;
                    }
                    Save(Catalog.Saver(Settings.SettingsPath, Settings.DebugCategoryName, $"{DebugNames[0]}_{i}", toUse));
                }
                DebugValues = new string[] { "Kept for compatibility, ignore plz", DebugStyle.ToString(),
                    ColorSaver(LogColor), ColorSaver(WarningColor), ColorSaver(ErrorColor) };

                SaveAll(Catalog.Savers(Settings.SettingsPath, Settings.DebugCategoryName, DebugNames, DebugValues));
                RefreshDebugger();
            }
        }
    }
}
