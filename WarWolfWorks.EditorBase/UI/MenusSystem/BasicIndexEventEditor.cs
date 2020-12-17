using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.EditorBase.Utility;
using WarWolfWorks.UI.MenusSystem.Assets;
using static WarWolfWorks.EditorBase.Constants;
using static WarWolfWorks.WWWResources;
using System.Linq;

namespace WarWolfWorks.EditorBase.UI.MenusSystem
{
    /// <summary>
    /// Custom editor for the <see cref="BasicIndexEvent"/> class.
    /// </summary>
    [CustomEditor(typeof(BasicIndexEvent), true)]
    [CanEditMultipleObjects]
    public sealed class BasicIndexEventEditor : Editor
    {
        private SerializedProperty[] AnchorsProperties;
        private SerializedProperty[] ColorProperties;

        private SerializedProperty[] OtherProperties;

        private GUIContent ColorContent;
        private GUIContent AnchorsContent;

        //V_DefaultTitle_0
        private void OnEnable()
        {
            ColorContent = new GUIContent(ELS_Transition_Color);
            AnchorsContent = new GUIContent(ELS_Transition_Anchors);

            ColorProperties = new SerializedProperty[]
            {
                serializedObject.FindProperty("s_TColor"),
                serializedObject.FindProperty("UnfocusedColor"),
                serializedObject.FindProperty("FocusedColor"),
                serializedObject.FindProperty("ColorTransitionSpeed"),
            };

            AnchorsProperties = new SerializedProperty[]
            {
                serializedObject.FindProperty("s_TAnchors"),
                serializedObject.FindProperty("UnfocusedAnchors"),
                serializedObject.FindProperty("FocusedAnchors"),
                serializedObject.FindProperty("SizeTransitionSpeed"),
            };

            List<SerializedProperty> used = EditorHooks.GetAllVisibleProperties(serializedObject, false);
            used.RemoveAll(sp => Array.Exists(ColorProperties, asp => sp.name == asp.name));
            used.RemoveAll(sp => Array.Exists(AnchorsProperties, asp => sp.name == asp.name));
            OtherProperties = used.ToArray();
        }

        /// <summary>
        /// The <see cref="BasicIndexEvent"/> inspector drawing.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawCategory(ColorProperties, ELS_Transition_Color, true, ColorContent);
            DrawCategory(AnchorsProperties, ELS_Transition_Anchors, true, AnchorsContent);
            DrawCategory(OtherProperties, ELS_Misc);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawCategory(SerializedProperty[] toDraw, string titleText, bool firstIsToggle = false, GUIContent expandContent = null)
        {
            EditorGUILayout.LabelField("<color=#EEEEEE>" + titleText + "</color>", GUIS_DefaultTitle_0);
            EditorHooks.SlickSeparatorNS();
            if (firstIsToggle)
            {
                EditorGUILayout.PropertyField(toDraw[0], expandContent);
                if(toDraw[0].boolValue)
                    for (int i = 1; i < toDraw.Length; i++)
                        EditorGUILayout.PropertyField(toDraw[i]);
            }
            else
            {
                for (int i = 0; i < toDraw.Length; i++)
                    EditorGUILayout.PropertyField(toDraw[i]);
            }
        }
    }
}
