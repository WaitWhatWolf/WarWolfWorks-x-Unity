using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.EditorBase.Utility;
using WarWolfWorks.EntitiesSystem.Itemization;

namespace WarWolfWorks.EditorBase.EntitiesSystem.Itemization
{
    /// <summary>
    /// Custom editor for <see cref="ItemEditor"/>; It is also what sets an <see cref="Item"/>'s ID when not set.
    /// </summary>
    [CustomEditor(typeof(Item), true)]
    public sealed class ItemEditor : Editor
    {
        private Item TargetItem => (Item)target;

        private SerializedProperty sp_Name;
        private SerializedProperty sp_ID;
        private SerializedProperty sp_Description;
        private SerializedProperty sp_Sprite;

        private GUIContent guic_Name = new GUIContent("Name");
        private GUIContent guic_Description = new GUIContent("Description");
        private GUIContent guic_Sprite = new GUIContent("Sprite");

        private List<SerializedProperty> Properties;

        private bool IsInResources = true;

        private void OnEnable()
        {
            sp_Name = serializedObject.FindProperty("s_Name");
            sp_ID = serializedObject.FindProperty("s_ID");
            sp_Description = serializedObject.FindProperty("s_Description");
            sp_Sprite = serializedObject.FindProperty("s_Sprite");
            Item[] items = Resources.LoadAll<Item>("/");
            IsInResources = Array.Exists(items, item => item.GetInstanceID() == target.GetInstanceID());

            if (sp_ID.intValue == -1 && IsInResources)
            {
                if (items.Length == 0)
                    sp_ID.intValue = 0;
                else
                    for(int i = 0; i < items.Length; i++)
                    {
                        if (Array.Exists(items, itm => itm.ID == i))
                            continue;

                        sp_ID.intValue = i;
                        break;
                    }

                serializedObject.ApplyModifiedProperties();
            }

            Properties = EditorHooks.GetAllVisibleProperties(serializedObject, false);
            Properties = Properties.GetRange(3, Properties.Count - 3);
        }

        /// <summary>
        /// Draws the custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            if(!IsInResources)
            {
                EditorGUILayout.HelpBox("This item is not inside a resources folder; This is not allowed.", MessageType.Error);
                return;
            }
            #region Name and ID
            Rect nameRect = EditorGUILayout.GetControlRect();
            float idxMax = nameRect.xMax;
            nameRect.width /= Preferences.EDITOR_ITEM_NAME_WIDTH_DIV;
            EditorGUI.PropertyField(nameRect, sp_Name, guic_Name);

            nameRect.xMin = nameRect.xMax;
            nameRect.xMax = idxMax;
            EditorGUI.LabelField(nameRect, $"(ID: {sp_ID.intValue})");

            EditorGUILayout.Space(nameRect.height);
            #endregion

            EditorGUILayout.PropertyField(sp_Description, guic_Description);
            EditorGUILayout.PropertyField(sp_Sprite, guic_Sprite);

            EditorHooks.SlickSeparator();

            foreach(SerializedProperty sp in Properties)
            {
                EditorGUILayout.PropertyField(sp);
            }
        }
    }
}
