
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.EditorBase.Utility;
using WarWolfWorks.NyuEntities.Itemization;

namespace WarWolfWorks.EditorBase.NyuEntities.Itemization
{
    /// <summary>
    /// Custom editor for <see cref="ItemEditor"/>; It is also what sets a <see cref="NyuItem"/>'s ID when not set.
    /// </summary>
    [CustomEditor(typeof(NyuItem), true)]
    public sealed class ItemEditor : Editor
    {
        private NyuItem TargetItem => (NyuItem)target;

        private SerializedProperty sp_Name;
        private SerializedProperty sp_ID;
        private SerializedProperty sp_Description;
        private SerializedProperty sp_Sprite;

        private GUIContent guic_Name = new GUIContent("Name");
        private GUIContent guic_Description = new GUIContent("Description");
        private GUIContent guic_Sprite = new GUIContent("Sprite");

        private List<SerializedProperty> Properties;
        private List<GUIContent> PropertyContents;
        private bool RemovesS;

        private bool IsInResources = true;
        private static NyuItem[] Items => Resources.LoadAll<NyuItem>(string.Empty);
        /*{
            get
            {
                if (pvsub_Items == null)
                {
                    pvsub_Items = Resources.LoadAll<NyuItem>(string.Empty);
                }

                return pvsub_Items;
            }
        }

        private static NyuItem[] pvsub_Items;
        */

        private void OnEnable()
        {
            sp_Name = serializedObject.FindProperty("s_Name");
            sp_ID = serializedObject.FindProperty("s_ID");
            sp_Description = serializedObject.FindProperty("s_Description");
            sp_Sprite = serializedObject.FindProperty("s_Sprite");
            IsInResources = Array.Exists(Items, item => item.GetInstanceID() == target.GetInstanceID());

            if (sp_ID.intValue == -1 && IsInResources)
            {
                if (Items.Length == 0)
                    sp_ID.intValue = 0;
                else
                    for (int i = 0; i < Items.Length; i++)
                    {
                        if (Array.Exists(Items, itm => itm.GetID() == i))
                            continue;

                        sp_ID.intValue = i;
                        break;
                    }

                serializedObject.ApplyModifiedProperties();
            }

            try { RemovesS = target.GetType().GetCustomAttributes(typeof(CompleteNoS), true).Length > 0; }
            catch { RemovesS = false; }
            EditorHooks.GetAllVisibleProperties(serializedObject, false, out Properties, out PropertyContents, RemovesS);
            Properties.RemoveRange(0, 4);
            PropertyContents.RemoveRange(0, 4);
        }

        /// <summary>
        /// Draws the custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!IsInResources)
            {
                EditorGUILayout.HelpBox("This item is not inside a resources folder; This is not allowed.", MessageType.Error);
                return;
            }
            #region Name and ID
            Rect nameRect = EditorGUILayout.GetControlRect();
            float idxMax = nameRect.xMax;
            nameRect.width /= Constants.EV_ITEM_NAME_WIDTH_DIV;
            EditorGUI.PropertyField(nameRect, sp_Name, guic_Name);

            nameRect.xMin = nameRect.xMax;
            nameRect.xMax = idxMax;
            EditorGUI.LabelField(nameRect, $"(ID: {sp_ID.intValue})");

            EditorGUILayout.Space(nameRect.height);
            #endregion

            EditorGUILayout.PropertyField(sp_Description, guic_Description);
            EditorGUILayout.PropertyField(sp_Sprite, guic_Sprite);

            EditorHooks.SlickSeparator();

            for (int i = 0; i < Properties.Count; i++)
            {
                EditorGUILayout.PropertyField(Properties[i], PropertyContents[i], Properties[i].hasVisibleChildren);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}