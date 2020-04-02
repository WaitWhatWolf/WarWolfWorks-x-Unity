using System.Collections.Generic;
using UnityEditor;
using WarWolfWorks.EditorBase.Utility;
using WarWolfWorks.EntitiesSystem;
using System;
using WarWolfWorks.Attributes;
using UnityEngine;

namespace WarWolfWorks.EditorBase.EntitiesSystem
{
    /// <summary>
    /// Custom editor for <see cref="Entity"/>. All fields with "s_" or "S_" will be removed if the
    /// surface class is marked with <see cref="CompleteNoS"/>.
    /// </summary>
    [CustomEditor(typeof(Entity), true)]
    public sealed class EntityEditor : Editor
    {
        private bool RemovesS;

        private List<SerializedProperty> Properties;
        private List<GUIContent> PropertyContents;

        private SerializedProperty Name;

        private void OnEnable()
        {
            UpdateProperties();
        }

        private void UpdateProperties()
        {
            try { RemovesS = target.GetType().GetCustomAttributes(typeof(CompleteNoS), true).Length > 0; }
            catch { RemovesS = false; }

            if (EditorHooks.GetAllVisibleProperties(serializedObject, false, out Properties, out PropertyContents, RemovesS))
            {
                Name = Properties[0];
                Properties.RemoveAt(0);
                PropertyContents.RemoveAt(0);
            }
        }

        /// <summary>
        /// Draws the <see cref="Entity"/>'s custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            int count = 0;
            Start:
            serializedObject.Update();

            try
            {
                EditorGUILayout.PropertyField(Name);
                EditorHooks.SlickSeparatorNS();

                for (int i = 0; i < Properties.Count; i++)
                {
                    EditorGUILayout.PropertyField(Properties[i], PropertyContents[i], Properties[i].hasVisibleChildren);
                }
            }
            catch (Exception e)
            {
                if (count > 3)
                {
                    AdvancedDebug.LogError("Fatal error occured on EntityEditor's display.", 0);
                    return;
                }
                AdvancedDebug.LogException(e);
                UpdateProperties();
                count++;
                goto Start;
            }
            finally
            {
                serializedObject.ApplyModifiedProperties();
            }
            
        }
    }
}
