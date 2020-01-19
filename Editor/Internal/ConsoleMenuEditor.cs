using System;
using UnityEditor;
using WarWolfWorks.UI;

namespace WarWolfWorks.EditorBase.Internal
{
    [CustomEditor(typeof(ConsoleMenu))]
    public class ConsoleMenuEditor : Editor
    {
        private SerializedProperty[] Properties;

        private SerializedProperty CustomConsole => Properties[0];

        private void OnEnable()
        {
            Properties = new SerializedProperty[4]
            {
            serializedObject.FindProperty("CustomConsoleLayout"),
            serializedObject.FindProperty("consoleContentText"),
            serializedObject.FindProperty("consoleInputText"),
            serializedObject.FindProperty("holder")
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(CustomConsole);
            if (CustomConsole.boolValue)
            {
                for (int i = 1; i < Properties.Length; i++)
                {
                    EditorGUILayout.PropertyField(Properties[i]);
                }
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
