using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.EditorBase.Utility;
using static WarWolfWorks.Constants;

namespace WarWolfWorks.EditorBase.Custom
{
    /// <summary>
    /// Custom editor for all scriptable objects flagged with <see cref="CompleteNoS"/>.
    /// </summary>
    [CustomEditor(typeof(ScriptableObject), true)]
    public sealed class CompleteNoSScriptableObjectEditor : Editor
    {
        private bool displaysNoS;
        private List<SerializedProperty> Properties;
        private List<GUIContent> PropertyContents;

        private void OnEnable()
        {
            try { displaysNoS = target.GetType().GetCustomAttributes(typeof(CompleteNoS), true).Length > 0; }
            catch { displaysNoS = false; }

            displaysNoS = EditorHooks.GetAllVisibleProperties(serializedObject, false, out Properties, out PropertyContents, displaysNoS);
        }

        /// <summary>
        /// Draws an inspector with all fields that start with "s_" or "S_" if the <see cref="MonoBehaviour"/> was marked with <see cref="CompleteNoS"/>.
        /// </summary>
        public override void OnInspectorGUI()
        {
            if(displaysNoS)
            {
                serializedObject.Update();
                for (int i = 0; i < Properties.Count; i++)
                    EditorGUILayout.PropertyField(Properties[i], PropertyContents[i]);
                serializedObject.ApplyModifiedProperties();
            }
            else
                base.OnInspectorGUI();
        }
    }
}
