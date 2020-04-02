using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.Interfaces;

namespace WarWolfWorks.EditorBase.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(IStat), true)]
    public class StatDrawer : PropertyDrawer
    {
        protected List<float> FinalHeights { get; set; } = new List<float>();

        protected float FinalHeight { get; set; } = EditorGUIUtility.singleLineHeight;

        protected virtual float BaseHeight => EditorGUIUtility.singleLineHeight;
        protected virtual float WidthDivider => 3;
        protected float FinalWidth(Rect position) => position.width / WidthDivider;

        private float AppliedHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUIStart(ref position);
            EditorGUI.BeginProperty(position, label, property);
            GUIDraw(ref position, property, label);
            EditorGUI.EndProperty();
            GUIEnd();
        }

        protected virtual void GUIStart(ref Rect position)
        {
            FinalHeights.Clear();

            FinalHeights.Add(FinalHeight);
            FinalHeight = position.height = EditorGUIUtility.singleLineHeight;
            
        }

        protected virtual void GUIEnd()
        {
            FinalHeight += BaseHeight;

            ApplyFinalHeights();
        }

        protected virtual void GUIDraw(ref Rect position, SerializedProperty property, GUIContent label)
        {
            DrawValue(position, property.FindPropertyRelative("value"), position.x, $"{label.text} Value", out _);

            AsEnumStatAttribute enumAttribute = null;

            try
            {
                enumAttribute = fieldInfo.GetCustomAttributes(typeof(AsEnumStatAttribute), true)[0] as AsEnumStatAttribute;
            }
            catch { }

            Type stackingAttribute = enumAttribute?.StackingType ?? WarWolfWorks.Internal.Settings.DefaultStackingType;
            Type affectionsAttribute = enumAttribute?.AffectionType ?? WarWolfWorks.Internal.Settings.DefaultAffectionsType;

            if (stackingAttribute !=  null)
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

        protected void ApplyFinalHeights()
        {
            float largest = 0;
            foreach (float f in FinalHeights)
            {
                if (f > largest)
                    largest = f;
            }

            AppliedHeight = largest;
        }

        protected void DrawValue(Rect position, SerializedProperty property, float x, string label, out Rect finalRect, float xMin = 0)
        {
            Rect valueRect = new Rect(position);
            valueRect.width = FinalWidth(valueRect);
            valueRect.x = x;
            valueRect.xMin += xMin;

            EditorGUI.LabelField(valueRect, label);
            valueRect.y += BaseHeight;

            EditorGUI.PropertyField(valueRect, property, GUIContent.none);
            finalRect = valueRect;
        }

        protected void DrawValueAsEnum(Rect position, Type enumType, SerializedProperty property, float x, string label, out Rect finalRect, float xMin = 0)
        {
            Rect valueRect = new Rect(position);
            valueRect.width = FinalWidth(valueRect);
            valueRect.x = x;
            valueRect.xMin += xMin;

            EditorGUI.LabelField(valueRect, label);
            valueRect.y += BaseHeight;

            Enum enumVal = (Enum)Enum.Parse(enumType, property.intValue.ToString());
            enumVal = EditorGUI.EnumPopup(valueRect, enumVal);
            property.intValue = (int)Enum.ToObject(enumType, enumVal);
            finalRect = valueRect;
        }

        protected void DrawAffections(Rect position, SerializedProperty property, float x)
        {
            Rect affectionRect;
            DrawValue(position, property, x, "Affections", out affectionRect, 15);

            if (property.isExpanded)
            {
                IncrementRectHeight(ref affectionRect, BaseHeight);
                if (GUI.Button(affectionRect, "Add Affection"))
                    property.InsertArrayElementAtIndex(property.arraySize == 0 ? 0 : property.arraySize - 1);


                if (property.arraySize > 0)
                {
                    IncrementRectHeight(ref affectionRect, BaseHeight);
                    if (GUI.Button(affectionRect, "Remove Affection"))
                        property.DeleteArrayElementAtIndex(property.arraySize - 1);
                }
                for (int i = 0; i < property.arraySize; i++)
                {
                    IncrementRectHeight(ref affectionRect, BaseHeight);
                    EditorGUI.PropertyField(affectionRect, property.GetArrayElementAtIndex(i), GUIContent.none);
                }
            }
        }

        protected void DrawAffectionsAsEnum(Rect position, Type enumType, SerializedProperty property, float x)
        {
            Rect affectionRect;
            DrawValue(position, property, x, "Affections", out affectionRect, 15);

            if (property.isExpanded)
            {
                IncrementRectHeight(ref affectionRect, BaseHeight);
                if (GUI.Button(affectionRect, "Add Affection"))
                    property.InsertArrayElementAtIndex(property.arraySize == 0 ? 0 : property.arraySize - 1);

                if (property.arraySize > 0)
                {
                    IncrementRectHeight(ref affectionRect, BaseHeight);
                    if (GUI.Button(affectionRect, "Remove Affection"))
                        property.DeleteArrayElementAtIndex(property.arraySize - 1);
                }
                for (int i = 0; i < property.arraySize; i++)
                {
                    IncrementRectHeight(ref affectionRect, BaseHeight);
                    SerializedProperty IndexProperty = property.GetArrayElementAtIndex(i);
                    Enum enumVal = (Enum)Enum.Parse(enumType, IndexProperty.intValue.ToString());
                    enumVal = EditorGUI.EnumPopup(affectionRect, enumVal);
                    IndexProperty.intValue = (int)Enum.ToObject(enumType, enumVal);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => AppliedHeight;

        protected void IncrementRectHeight(ref Rect rect, float amount)
        {
            rect.y += amount;

            FinalHeight += amount;
        }

        protected void IncrementRectHeight(ref Rect rect, ref float height, float amount)
        {
            rect.y += amount;

            height += amount;
        }
    }
}
