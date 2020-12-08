using System;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.EditorBase.Interfaces;
using WarWolfWorks.EditorBase.Utility;
using WarWolfWorks.Internal;
using WarWolfWorks.NyuEntities.HealthSystem;
using static WarWolfWorks.EditorBase.Constants;

namespace WarWolfWorks.EditorBase.NyuEntities
{
    internal sealed class NyuHealthInternalEditor : INyuComponentInternalEditor
    {
        Type INyuComponentInternalEditor.EditorType => typeof(NyuHealth);
        private SerializedObject serializedObject;

        private SerializedProperty sp_Health;
        private SerializedProperty sp_ImmunityTime;
        private SerializedProperty sp_DamageParent;
        private SerializedProperty sp_DestroyOnDeath;
        private SerializedProperty sp_UsesImmunity;
        private SerializedProperty sp_Calculator;
        private SerializedProperty sp_ImmunityEffect;

        bool INyuComponentInternalEditor.DrawDefaultEditor => false;

        void INyuComponentInternalEditor.OnEnable(SerializedObject serializedObject)
        {
            this.serializedObject = serializedObject;

            sp_Health = serializedObject.FindProperty("s_MaxHealth");
            sp_ImmunityTime = serializedObject.FindProperty("s_ImmnunityDuration");
            sp_DamageParent = serializedObject.FindProperty("s_DamageParent");
            sp_DestroyOnDeath = serializedObject.FindProperty("s_DestroyOnDeath");
            sp_UsesImmunity = serializedObject.FindProperty("s_UsesImmunity");
            sp_Calculator = serializedObject.FindProperty("s_Calculator");
            sp_ImmunityEffect = serializedObject.FindProperty("s_ImmunityEffect");
        }

        void INyuComponentInternalEditor.OnInspectorGUI()
        {
            serializedObject.Update();
            EditorHooks.Drawers.StatField(EditorGUILayout.GetControlRect(), sp_Health, Settings.DefaultStackingType, Settings.DefaultAffectionsType);
            EditorHooks.Drawers.StatField(EditorGUILayout.GetControlRect(), sp_ImmunityTime, Settings.DefaultStackingType, Settings.DefaultAffectionsType);

            Rect calculatorRect = EditorGUILayout.GetControlRect();
            calculatorRect.width /= 2;
            Rect immunityRect = new Rect(calculatorRect);
            immunityRect.x = immunityRect.xMax;

            EditorGUI.LabelField(calculatorRect, "Calculator");
            EditorGUI.LabelField(immunityRect, "Immunity");

            EditorHooks.Rects.SetRectsYPos(calculatorRect.yMax, ref calculatorRect, ref immunityRect);

            EditorGUI.PropertyField(calculatorRect, sp_Calculator, GUIContent.none);
            EditorGUI.PropertyField(immunityRect, sp_ImmunityEffect, GUIContent.none);

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 1.1f);

            sp_DamageParent.isExpanded = EditorGUILayout.Foldout(sp_DamageParent.isExpanded, ELS_Additional_Settings);
            if(sp_DamageParent.isExpanded)
            {
                Rect editorRect = new Rect(EditorGUILayout.GetControlRect());
                editorRect.xMin += 10;

                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 1.5f);
                EditorGUI.PropertyField(editorRect, sp_DamageParent, new GUIContent(sp_DamageParent.displayName.Remove(0, 2)));
                EditorHooks.RectSpace(ref editorRect);
                EditorGUI.PropertyField(editorRect, sp_DestroyOnDeath, new GUIContent(sp_DestroyOnDeath.displayName.Remove(0, 2)));
                EditorHooks.RectSpace(ref editorRect);
                EditorGUI.PropertyField(editorRect, sp_UsesImmunity, new GUIContent(sp_UsesImmunity.displayName.Remove(0, 2)));
                EditorGUILayout.Space();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
