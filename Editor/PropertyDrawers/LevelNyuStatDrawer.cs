using System;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.EditorBase.Utility;
using WarWolfWorks.NyuEntities.Statistics;

namespace WarWolfWorks.EditorBase.PropertyDrawers
{
    /// <summary>
    /// Custom property drawer for <see cref="LevelStat"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(LevelStat))]
    internal sealed class LevelNyuStatDrawer : NyuStatDrawer
    {
        private float CurrentHeight;
        private float ListHeight => CurrentHeight;

        protected override void GUIStart(ref Rect position)
        {
            FinalHeights.Clear();
            FinalHeights.Add(ListHeight);
            FinalHeights.Add(FinalHeight);

            CurrentHeight = FinalHeight = position.height = EditorGUIUtility.singleLineHeight;
        }

        protected override void GUIDraw(ref Rect position, SerializedProperty property, GUIContent label)
        {
            DrawLevelValue(position, property.FindPropertyRelative("value"), position.x, $"{label.text} Value", out _);

            AsEnumStatAttribute enumAttribute = null;

            try
            {
                enumAttribute = fieldInfo.GetCustomAttributes(typeof(AsEnumStatAttribute), true)[0] as AsEnumStatAttribute;
            }
            catch { }

            Type stackingAttribute = enumAttribute?.StackingType ?? WarWolfWorks.Internal.Settings.DefaultStackingType;
            Type affectionsAttribute = enumAttribute?.AffectionType ?? WarWolfWorks.Internal.Settings.DefaultAffectionsType;

            if (stackingAttribute != null)
            {
                DrawValueAsEnum(position, stackingAttribute, property.FindPropertyRelative("stacking"),
                    position.x + FinalWidth(position), "Stacking", out _);
            }
            else
            {
                DrawValue(position, property.FindPropertyRelative("stacking"), position.x + FinalWidth(position), "Stacking", out _);
            }

            if (affectionsAttribute != null)
            {
                DrawAffectionsAsEnum(position, affectionsAttribute, property.FindPropertyRelative("affections"),
                    position.x + FinalWidth(position) * 2);
            }
            else
            {
                DrawAffections(position, property.FindPropertyRelative("affections"), position.x + FinalWidth(position) * 2);
            }
        }

        private void DrawLevelValue(Rect position, SerializedProperty property, float x, string label, out Rect finalRect, float xMin = 0)
        {
            Rect levelRect = new Rect(position);
            levelRect.width = FinalWidth(levelRect);
            levelRect.x = x;
            levelRect.xMin += xMin;

            EditorGUI.LabelField(levelRect, label);
            levelRect.y += BaseHeight;

            EditorGUI.PropertyField(levelRect, property.FindPropertyRelative("defaultValue"), GUIContent.none);

            SerializedProperty values = property.FindPropertyRelative("levelValues");

            EditorGUI.PropertyField(levelRect, values, GUIContent.none);

            if(values.isExpanded)
            {
                IncrementRectHeight(ref levelRect, ref CurrentHeight, BaseHeight);
                if (GUI.Button(levelRect, "Add Level"))
                    values.InsertArrayElementAtIndex(values.arraySize == 0 ? 0 : values.arraySize - 1);

                if (values.arraySize > 0)
                {
                    IncrementRectHeight(ref levelRect, ref CurrentHeight, BaseHeight);
                    if (GUI.Button(levelRect, "Remove Level"))
                        values.DeleteArrayElementAtIndex(values.arraySize - 1);
                }
                for (int i = 0; i < values.arraySize; i++)
                {
                    IncrementRectHeight(ref levelRect, ref CurrentHeight, BaseHeight);
                    EditorHooks.SlickSeparator(levelRect);
                    IncrementRectHeight(ref levelRect, ref CurrentHeight, BaseHeight);

                    Rect leftRect = new Rect(levelRect);
                    Rect rightRect = new Rect(levelRect);

                    leftRect.width /= 2;
                    rightRect.width /= 2;
                    rightRect.x = leftRect.xMax;

                    EditorGUI.LabelField(leftRect, "Value");
                    EditorGUI.PropertyField(rightRect, values.GetArrayElementAtIndex(i).FindPropertyRelative("Value"), GUIContent.none);
                    IncrementRectHeight(ref levelRect, ref CurrentHeight, BaseHeight);
                    leftRect.y = rightRect.y = levelRect.y;
                    EditorGUI.LabelField(leftRect, "Level");
                    EditorGUI.PropertyField(rightRect, values.GetArrayElementAtIndex(i).FindPropertyRelative("Level"), GUIContent.none);
                }
            }

            finalRect = levelRect;
        }

        protected override void GUIEnd()
        {
            CurrentHeight += BaseHeight;
            base.GUIEnd();
        }
    }
}
