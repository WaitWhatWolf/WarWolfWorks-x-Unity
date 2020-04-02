using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.Attributes;
using WarWolfWorks.EditorBase.Utility;
using WarWolfWorks.EntitiesSystem.Movement;
using static WarWolfWorks.Constants;
using static WarWolfWorks.EditorBase.Constants;
using static WarWolfWorks.EntitiesSystem.Movement.EntityMovement;

namespace WarWolfWorks.EditorBase.EntitiesSystem.Movement
{
    /// <summary>
    /// Custom editor for <see cref="EntityMovement"/>.
    /// </summary>
    [CustomEditor(typeof(EntityMovement), true)]
    public sealed class EntityMovementEditor : Editor
    {
        private List<SerializedProperty> Properties;
        private List<GUIContent> PropertyContents;

        private SerializedProperty sp_Velocities;
        private List<Velocity> ns_Velocities;
        private bool RemovesS;

        private void OnEnable()
        {
            try { RemovesS = target.GetType().GetCustomAttributes(typeof(CompleteNoS), true).Length > 0; }
            catch { RemovesS = false; }

            if (EditorHooks.GetAllVisibleProperties(serializedObject, false, out Properties, out PropertyContents, RemovesS))
            {
                sp_Velocities = Properties[0];
                Properties.RemoveAt(0);
                PropertyContents.RemoveAt(0);
            }

            ns_Velocities = (target as EntityMovement).Velocities;
        }

        /// <summary>
        /// Draws the custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            sp_Velocities.isExpanded = EditorGUILayout.Foldout(sp_Velocities.isExpanded, ELS_EntityMovement_Velocities);

            if(sp_Velocities.isExpanded)
            {
                Color drawColor = new Color(1f,0.5f, 0f, .5f);
                for (int i = 0; i < ns_Velocities.Count; i++)
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

                    EditorGUI.LabelField(valueRect, ns_Velocities[i].Value.ToString());
                    EditorGUI.LabelField(timerRect, $"{ns_Velocities[i].Time}/{ns_Velocities[i].StartTime}");


                    string affectionsText;
                    if (ns_Velocities[i].Affections != null && ns_Velocities[i].Affections.Length > 0)
                    {
                        affectionsText = ns_Velocities[i].Affections[0].ToString();
                        for (int j = 1; j < ns_Velocities[i].Affections.Length; j++)
                            affectionsText += $", {ns_Velocities[i].Affections[j]}";
                    }
                    else affectionsText = "N/A";
                    EditorGUI.LabelField(affectionsRect, affectionsText);

                    EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
                }
            }

            EditorHooks.SlickSeparatorNS();

            for(int i = 0; i < Properties.Count; i++)
            {
                EditorGUILayout.PropertyField(Properties[i], PropertyContents[i], Properties[i].hasChildren);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
