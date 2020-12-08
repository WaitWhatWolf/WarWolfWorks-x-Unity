using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.Interfaces.NyuEntities;
using static WarWolfWorks.EditorBase.Constants;

namespace WarWolfWorks.EditorBase.PropertyDrawers
{
    /// <summary>
    /// Custom property drawer for the <see cref="INyuStat"/> value.
    /// </summary>
    [CustomPropertyDrawer(typeof(INyuStat), true)]
    public class NyuStatDrawer : PropertyDrawer
    {
        /// <summary>
        /// List of heights stored; Uses the highest value in the list as reference.
        /// </summary>
        protected List<float> FinalHeights { get; set; } = new List<float>();

        /// <summary>
        /// The final height applied; Used by <see cref="PropertyDrawer.GetPropertyHeight(SerializedProperty, GUIContent)"/>.
        /// </summary>
        protected float FinalHeight { get; set; } = EditorGUIUtility.singleLineHeight;

        /// <summary>
        /// The base height of each line in a stat.
        /// </summary>
        protected virtual float BaseHeight => EditorGUIUtility.singleLineHeight;
        /// <summary>
        /// Width of each cell.
        /// </summary>
        protected virtual float WidthDivider => 3;

        /// <summary>
        /// Gets the width of a position for horizontal scaling.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        protected float FinalWidth(Rect position) => position.width / WidthDivider;

        private float AppliedHeight;

        /// <summary>
        /// The OnGUI override.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUIStart(ref position);
            EditorGUI.BeginProperty(position, label, property);
            GUIDraw(ref position, property, label);
            EditorGUI.EndProperty();
            GUIEnd();
        }

        /// <summary>
        /// Very first method executed in <see cref="OnGUI(Rect, SerializedProperty, GUIContent)"/>.
        /// </summary>
        /// <param name="position"></param>
        protected virtual void GUIStart(ref Rect position)
        {
            FinalHeights.Clear();

            FinalHeights.Add(FinalHeight);
            FinalHeight = position.height = EditorGUIUtility.singleLineHeight;
            
        }

        /// <summary>
        /// Very last method executed in <see cref="OnGUI(Rect, SerializedProperty, GUIContent)"/>.
        /// </summary>
        protected virtual void GUIEnd()
        {
            FinalHeight += BaseHeight;

            ApplyFinalHeights();
        }

        /// <summary>
        /// Executed after <see cref="GUIStart(ref Rect)"/> and before <see cref="GUIEnd"/>.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        protected virtual void GUIDraw(ref Rect position, SerializedProperty property, GUIContent label)
        {
            DrawValue(position, property.FindPropertyRelative("s_Value"), position.x, $"{label.text} Value", out _);

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
                DrawValueAsEnum(position, stackingAttribute, property.FindPropertyRelative("s_Stacking"),
                    position.x + FinalWidth(position), ELS_NyuStat_Stacking, out _);
            }
            else
            {
                DrawValue(position, property.FindPropertyRelative("s_Stacking"), position.x + FinalWidth(position), ELS_NyuStat_Stacking, out _);
            }

            if (affectionsAttribute != null)
            {
                DrawAffectionsAsEnum(position, affectionsAttribute, property.FindPropertyRelative("s_Affections"),
                    position.x + FinalWidth(position) * 2);
            }
            else
            {
                DrawAffections(position, property.FindPropertyRelative("s_Affections"), position.x + FinalWidth(position) * 2);
            }
        }

        /// <summary>
        /// Applies the final height.
        /// </summary>
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

        /// <summary>
        /// Draws a stat value.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="x"></param>
        /// <param name="label"></param>
        /// <param name="finalRect"></param>
        /// <param name="xMin"></param>
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

        /// <summary>
        /// Draws a stat value, where the value is attempted to be drawn as an enum field.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="enumType"></param>
        /// <param name="property"></param>
        /// <param name="x"></param>
        /// <param name="label"></param>
        /// <param name="finalRect"></param>
        /// <param name="xMin"></param>
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

        /// <summary>
        /// Draws affections.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="x"></param>
        protected void DrawAffections(Rect position, SerializedProperty property, float x)
        {
            Rect affectionRect;
            DrawValue(position, property, x, ELS_NyuStat_Affections, out affectionRect, 15);

            if (property.isExpanded)
            {
                IncrementRectHeight(ref affectionRect, BaseHeight);
                if (GUI.Button(affectionRect, ELS_NyuStat_Affections_Add))
                    property.InsertArrayElementAtIndex(property.arraySize == 0 ? 0 : property.arraySize - 1);


                if (property.arraySize > 0)
                {
                    IncrementRectHeight(ref affectionRect, BaseHeight);
                    if (GUI.Button(affectionRect, ELS_NyuStat_Affections_Remove))
                        property.DeleteArrayElementAtIndex(property.arraySize - 1);
                }
                for (int i = 0; i < property.arraySize; i++)
                {
                    IncrementRectHeight(ref affectionRect, BaseHeight);
                    EditorGUI.PropertyField(affectionRect, property.GetArrayElementAtIndex(i), GUIContent.none);
                }
            }
        }

        /// <summary>
        /// Draws affections, where the values are drawn as an enum field.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="enumType"></param>
        /// <param name="property"></param>
        /// <param name="x"></param>
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

        /// <summary>
        /// Points to <see cref="AppliedHeight"/>.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => AppliedHeight;

        /// <summary>
        /// Increments a rect's height by amount and adds it to <see cref="AppliedHeight"/>.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="amount"></param>
        protected void IncrementRectHeight(ref Rect rect, float amount)
        {
            rect.y += amount;

            FinalHeight += amount;
        }

        /// <summary>
        /// Increments a rect's height by amount and increases the height value given by that same amount.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="height"></param>
        /// <param name="amount"></param>
        protected void IncrementRectHeight(ref Rect rect, ref float height, float amount)
        {
            rect.y += amount;

            height += amount;
        }
    }
}
