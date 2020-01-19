using UnityEditor;
using UnityEngine;
using WarWolfWorks.Debugging;

namespace WarWolfWorks.EditorBase.PropertyDrawers
{

    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Change check is needed to prevent values being overwritten during multiple-selection
            EditorGUI.BeginChangeCheck();
            int newValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }
        }
    }
}