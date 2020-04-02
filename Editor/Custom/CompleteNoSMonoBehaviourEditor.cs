using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.EditorBase.Utility;

namespace WarWolfWorks.EditorBase.Custom
{
    /// <summary>
    /// Custom editor for all monobehaviours flagged with <see cref="CompleteNoS"/>.
    /// </summary>
    [CustomEditor(typeof(MonoBehaviour), true)]
    public sealed class CompleteNoSMonoBehaviourEditor : Editor
    {
        private bool displaysNoS;
        private List<SerializedProperty> Properties;
        private List<GUIContent> PropertyContents;

        private void OnEnable()
        {
            try { displaysNoS = target.GetType().GetCustomAttributes(typeof(CompleteNoS), true).Length > 0; }
            catch { displaysNoS = false; }

            if (displaysNoS)
            {
                displaysNoS = EditorHooks.GetAllVisibleProperties(serializedObject, false, out Properties, out PropertyContents);
            }
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
