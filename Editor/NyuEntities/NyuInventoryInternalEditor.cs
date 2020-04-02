using System;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.EditorBase.Interfaces;
using WarWolfWorks.EditorBase.Utility;
using WarWolfWorks.NyuEntities.Itemization;

namespace WarWolfWorks.EditorBase.NyuEntities
{
    internal sealed class NyuInventoryInternalEditor : INyuComponentInternalEditor
    {
        Type INyuComponentInternalEditor.EditorType => typeof(NyuInventory<NyuItem>);

        private NyuInventory<NyuItem> nc_Inventory;

        void INyuComponentInternalEditor.OnEnable(SerializedObject serializedObject)
        {
            nc_Inventory = serializedObject.targetObject as NyuInventory<NyuItem>;
        }

        void INyuComponentInternalEditor.OnInspectorGUI()
        {
            if (nc_Inventory.ItemsCount > 0)
            {
                for (int i = 0; i < nc_Inventory.ItemsCount; i++)
                {
                    EditorHooks.DrawColoredSquare(EditorGUILayout.GetControlRect(), Color.gray);
                    EditorGUILayout.LabelField(nc_Inventory[i].Name, $"ID:{nc_Inventory[i].GetID()}");
                }
            }
            else
            {
                EditorGUILayout.LabelField("No items currently in the inventory.");
            }
        }
    }
}
