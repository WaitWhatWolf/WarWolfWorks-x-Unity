using UnityEditor;
using UnityEngine;
using WarWolfWorks.EditorBase.Utility;
using WarWolfWorks.Utility;

namespace WarWolfWorks.EditorBase.PropertyDrawers
{
    /// <summary>
    /// Property drawer for <see cref="LevelFloat"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(LevelFloat))]
    public sealed class LevelFloatDrawer : PropertyDrawer
    {
        /// <summary>
        /// Height applied to the drawer using <see cref="GetPropertyHeight(SerializedProperty, GUIContent)"/>.
        /// </summary>
        private float FinalHeight;

        /// <summary>
        /// Draws the <see cref="LevelFloat"/>.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float originalPos = position.y;
            position.height = FinalHeight = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(position, property.FindPropertyRelative("defaultValue"), label);

            SerializedProperty values = property.FindPropertyRelative("levelValues");

            EditorGUI.PropertyField(position, values, GUIContent.none);

            if (values.isExpanded)
            {
                SetDrawerHeight(ref position);
                if (GUI.Button(position, "Add Level"))
                    values.InsertArrayElementAtIndex(values.arraySize == 0 ? 0 : values.arraySize - 1);

                if (values.arraySize > 0)
                {
                    SetDrawerHeight(ref position);
                    if (GUI.Button(position, "Remove Level"))
                        values.DeleteArrayElementAtIndex(values.arraySize - 1);
                }
                for (int i = 0; i < values.arraySize; i++)
                {
                    SetDrawerHeight(ref position);
                    EditorHooks.SlickSeparator(position);
                    SetDrawerHeight(ref position);

                    Rect leftRect = new Rect(position);
                    Rect rightRect = new Rect(position);

                    leftRect.width /= 2;
                    rightRect.width /= 2;
                    rightRect.x = leftRect.xMax;

                    EditorGUI.LabelField(leftRect, "Value");
                    EditorGUI.PropertyField(rightRect, values.GetArrayElementAtIndex(i).FindPropertyRelative("Value"), GUIContent.none);
                    EditorHooks.Rects.SetRectsYPos(position.yMax, ref leftRect, ref rightRect);
                    SetDrawerHeight(ref position);
                    EditorGUI.LabelField(leftRect, "Level");
                    EditorGUI.PropertyField(rightRect, values.GetArrayElementAtIndex(i).FindPropertyRelative("Level"), GUIContent.none);
                }
            }

            FinalHeight = position.yMax - originalPos;
        }

        private void SetDrawerHeight(ref Rect rect)
        {
            rect.y = rect.yMax;
        }

        /// <summary>
        /// Sets the property height to the starting position minus the ending yMax position.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return FinalHeight;
        }
    }
}
