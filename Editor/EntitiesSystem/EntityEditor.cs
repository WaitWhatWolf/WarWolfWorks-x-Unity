using System.Collections.Generic;
using UnityEditor;
using WarWolfWorks.EditorBase.Utility;
using WarWolfWorks.EntitiesSystem;
using UnityEngine;
using System.Linq;
using System;

namespace WarWolfWorks.EditorBase.EntitiesSystem
{
    [CustomEditor(typeof(Entity), true)]
    public class EntityEditor : Editor
    {
        private List<SerializedProperty> Properties;
        private SerializedProperty Name;
        private SerializedProperty Stats;

        private void OnEnable()
        {
            UpdateProperties();
        }

        private void UpdateProperties()
        {
            Properties = EditorHooks.GetAllVisibleProperties(serializedObject, false);
            Name = Properties.Find(sp => sp.name == "名前");
            Properties.Remove(Name);
        }

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
                    bool disp = EditorGUILayout.PropertyField(Properties[i], Properties[i].hasVisibleChildren);
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
