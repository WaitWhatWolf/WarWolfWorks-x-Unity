using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace WarWolfWorks.Utility.Editor
{
    [CustomPropertyDrawer(typeof(IntToEnumFlagsAttribute))]
    public class IntToEnumDrawer : PropertyDrawer
    {
        public bool expand;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            try
            {
                expand = EditorGUILayout.Foldout(expand, label.text);
                if (expand)
                {
                    SerializedProperty valueProperty = property.FindPropertyRelative("Value");
                    SerializedProperty calculusProperty = property.FindPropertyRelative("Stacks");
                    SerializedProperty affectionProperty = property.FindPropertyRelative("Affects");

                    IntToEnumFlagsAttribute affectionDisplayer = attribute as IntToEnumFlagsAttribute;
                    Enum enumval = (Enum)Enum.Parse(affectionDisplayer.EnumUse, affectionProperty.intValue.ToString());

                    EditorGUILayout.PropertyField(valueProperty);
                    EditorGUILayout.PropertyField(calculusProperty);
                    enumval = EditorGUILayout.EnumPopup("Affects", enumval);
                    affectionProperty.intValue = (int)Enum.Parse(enumval.GetType(), enumval.ToString());
                }
            }
            catch
            {
                base.OnGUI(position, property, label);
            }
        }
    }
}
