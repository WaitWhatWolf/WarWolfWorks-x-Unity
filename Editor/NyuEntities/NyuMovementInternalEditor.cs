using System;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.EditorBase.Interfaces;
using WarWolfWorks.EditorBase.Utility;
using WarWolfWorks.NyuEntities.MovementSystem;
using static WarWolfWorks.Constants;
using static WarWolfWorks.EditorBase.Constants;

namespace WarWolfWorks.EditorBase.NyuEntities
{
    internal sealed class NyuMovementInternalEditor : INyuComponentInternalEditor
    {
        private SerializedObject serializedObject;

        private SerializedProperty sp_Velocities;
        private NyuMovement ec_Movement;

        Type INyuComponentInternalEditor.EditorType => typeof(NyuMovement);

        private bool isExpanded;

        void INyuComponentInternalEditor.OnEnable(SerializedObject serializedObject)
        {
            this.serializedObject = serializedObject;
            sp_Velocities = serializedObject.FindProperty("hs_Velocities");

            ec_Movement = serializedObject.targetObject as NyuMovement;
        }

        void INyuComponentInternalEditor.OnInspectorGUI()
        {
            isExpanded = EditorGUILayout.Foldout(isExpanded, ELS_EntityMovement_Velocities);

            if (isExpanded)
            {
                Color drawColor = new Color(1f, 0.5f, 0f, .5f);
                for (int i = 0; i < ec_Movement.Velocities.Count; i++)
                {
                    Rect valueLabelRect = new Rect(EditorGUILayout.GetControlRect());
                    valueLabelRect.width /= 4;
                    Rect timerLabelRect = new Rect(valueLabelRect);
                    timerLabelRect.x = valueLabelRect.xMax;
                    Rect affectionsLabelRect = new Rect(timerLabelRect);
                    affectionsLabelRect.x = timerLabelRect.xMax;

                    Rect valueRect = new Rect(valueLabelRect);
                    Rect timerRect = new Rect(timerLabelRect);
                    Rect affectionsRect = new Rect(affectionsLabelRect);

                    Rect squareRect = new Rect(EditorGUILayout.GetControlRect());
                    squareRect.y = valueLabelRect.y;
                    squareRect.height *= 2;

                    EditorHooks.Rects.SetRectsYPos(valueLabelRect.yMax, ref valueRect, ref timerRect, ref affectionsRect);

                    EditorHooks.DrawColoredSquare(squareRect, drawColor);

                    EditorGUI.LabelField(valueLabelRect, LS_Value);
                    EditorGUI.LabelField(timerLabelRect, ELS_EntityMovement_Velocity_Time);
                    EditorGUI.LabelField(affectionsLabelRect, LS_Stat_Affections);

                    EditorGUI.LabelField(valueRect, ec_Movement.Velocities[i].Value.ToString());
                    EditorGUI.LabelField(timerRect, $"{ec_Movement.Velocities[i].Time}/{ec_Movement.Velocities[i].StartTime}");

                    string affectionsText;
                    if (ec_Movement.Velocities[i].Affections != null && ec_Movement.Velocities[i].Affections.Length > 0)
                    {
                        affectionsText = ec_Movement.Velocities[i].Affections[0].ToString();
                        for (int j = 1; j < ec_Movement.Velocities[i].Affections.Length; j++)
                            affectionsText += $", {ec_Movement.Velocities[i].Affections[j]}";
                    }
                    else affectionsText = "N/A";
                    EditorGUI.LabelField(affectionsRect, affectionsText);

                    EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
                }
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NyuMovementInternalEditor()
        {

        }
    }
}
