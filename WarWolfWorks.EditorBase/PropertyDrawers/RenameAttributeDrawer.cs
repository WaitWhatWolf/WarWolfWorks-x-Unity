using UnityEditor;
using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.EditorBase.Utility;

namespace WarWolfWorks.EditorBase.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(RenameAttribute))]
    public class RenameAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            try
            {
                var propertyAttribute = attribute as RenameAttribute;
                if (!EditorHooks.PropertyIsArray(property))
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
    }
}
