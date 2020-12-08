using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.Utility;

namespace WarWolfWorks.EditorBase.PropertyDrawers
{
    /// <summary>
    /// Property drawer for <see cref="NoSAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(NoSAttribute))]
    public sealed class NoSPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Detects a string that starts with "s_" or "S_".
        /// </summary>
        public static readonly Regex NoS_Detector = new Regex(@"^[S|s]_\w{1,99}");

        /// <summary>
        /// Sets the height to appropriate value based on if the property is of array type.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float increment = EditorGUIUtility.singleLineHeight;
            float toReturn = increment;
            if (property.isExpanded)
            {
                property.NextVisible(true);

                while (property.NextVisible(false))
                {
                    toReturn += increment;
                }

                toReturn += ((NoSAttribute)attribute).Padding;
            }
            return toReturn;
        }

        /// <summary>
        /// Changes the name of the label if <see cref="NoS_Detector"/> is a match.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (NoS_Detector.IsMatch(property.displayName)) label.text = property.displayName.Substring(2, property.displayName.Length - 2);
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}
