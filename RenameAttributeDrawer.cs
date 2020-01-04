using UnityEditor;
using UnityEngine;
using WarWolfWorks.Debugging;

namespace WarWolfWorks.EditorBase
{
    [CustomPropertyDrawer(typeof(RenameAttribute))]
    public class RenameAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            try
            {
                var propertyAttribute = attribute as RenameAttribute;
                if (!IsItBloodyArrayTho(property))
                    label.text = propertyAttribute.label;
                else
                {
                    Debug.LogWarningFormat(
                        "{0}(\"{1}\") Rename attribute is not supported for Enumerable values.",
                        typeof(RenameAttribute).Name,
                        propertyAttribute.label
                    );
                }
                EditorGUI.PropertyField(position, property, label);
            }
            catch (System.Exception ex) { Debug.LogException(ex); }
        }

        bool IsItBloodyArrayTho(SerializedProperty property)
        {
            string path = property.propertyPath;
            int idot = path.IndexOf('.');
            if (idot == -1) return false;
            string propName = path.Substring(0, idot);
            SerializedProperty p = property.serializedObject.FindProperty(propName);
            return p.isArray;
        }
    }
}
