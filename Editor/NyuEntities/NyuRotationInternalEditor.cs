using System;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.EditorBase.Interfaces;
using WarWolfWorks.EditorBase.Utility;
using WarWolfWorks.Interfaces.NyuEntities;
using WarWolfWorks.NyuEntities;

namespace WarWolfWorks.EditorBase.NyuEntities
{
    internal sealed class NyuRotationInternalEditor : INyuComponentInternalEditor
    {
        Type INyuComponentInternalEditor.EditorType => typeof(NyuRotation);
        private NyuRotation nc_Rotation;

        bool INyuComponentInternalEditor.DrawDefaultEditor => false;

        void INyuComponentInternalEditor.OnEnable(SerializedObject serializedObject)
        {
            nc_Rotation = serializedObject.targetObject as NyuRotation;
        }

        void INyuComponentInternalEditor.OnInspectorGUI()
        {
            Rect transXLabelRect = EditorGUILayout.GetControlRect();
            Rect transYLabelRect = new Rect(transXLabelRect);
            Rect transZLabelRect = new Rect(transYLabelRect);

            EditorHooks.Rects.SetRectsWidth(transXLabelRect.width / 3, ref transXLabelRect, ref transYLabelRect, ref transZLabelRect);

            transYLabelRect.x = transXLabelRect.xMax;
            transZLabelRect.x = transYLabelRect.xMax;

            Rect transXRect = new Rect(transXLabelRect);
            Rect transYRect = new Rect(transYLabelRect);
            Rect transZRect = new Rect(transZLabelRect);

            EditorHooks.Rects.SetRectsYPos(transXRect.yMax, ref transXRect, ref transYRect, ref transZRect);

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 1.5f);

            EditorGUI.LabelField(transXLabelRect, "Rotated X");
            EditorGUI.LabelField(transYLabelRect, "Rotated Y");
            EditorGUI.LabelField(transZLabelRect, "Rotated Z");

            nc_Rotation.s_ToRotateX = EditorGUI.ObjectField(transXRect, GUIContent.none, nc_Rotation.ToRotateX, typeof(Transform), true) as Transform;
            nc_Rotation.s_ToRotateY = EditorGUI.ObjectField(transYRect, GUIContent.none, nc_Rotation.ToRotateY, typeof(Transform), true) as Transform;
            nc_Rotation.s_ToRotateZ = EditorGUI.ObjectField(transZRect, GUIContent.none, nc_Rotation.ToRotateZ, typeof(Transform), true) as Transform;

            nc_Rotation.s_BaseRotationSpeed = EditorGUILayout.FloatField("Speed", nc_Rotation.s_BaseRotationSpeed);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NyuRotationInternalEditor()
        {

        }
    }
}
