using System;
using static WarWolfWorks.EditorBase.Constants;
using static WarWolfWorks.Constants;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.EditorBase.Interfaces;
using WarWolfWorks.NyuEntities.AttackSystem;
using WarWolfWorks.EditorBase.Utility;

namespace WarWolfWorks.EditorBase.NyuEntities
{
    internal sealed class NyuAttackInternalEditor : INyuComponentInternalEditor
    {
        private SerializedObject serializedObject;
        private SerializedProperty sp_Attacks;

        Type INyuComponentInternalEditor.EditorType => typeof(NyuAttack);

        void INyuComponentInternalEditor.OnEnable(SerializedObject serializedObject)
        {
            this.serializedObject = serializedObject;
            sp_Attacks = serializedObject.FindProperty("AllAttacks");
        }

        /// <summary>
        /// Draws the custom inspector.
        /// </summary>
        void INyuComponentInternalEditor.OnInspectorGUI()
        {
            serializedObject.Update();

            for (int i = 0; i < sp_Attacks.arraySize; i++)
            {
                Rect rect = EditorGUILayout.GetControlRect();
                Rect labelRect = new Rect(rect);
                float padding = rect.width / EVARV_ENTITYATTACK_ATK_LABEL_WIDTH_DIV;
                labelRect.xMin += padding;
                labelRect.xMax -= padding;
                Rect labelSeparatorRect = new Rect(rect);
                labelSeparatorRect.y = labelSeparatorRect.yMax;
                EditorHooks.SlickSeparator(labelSeparatorRect);
                Rect attackLabelRect = new Rect(labelSeparatorRect);
                attackLabelRect.y = labelSeparatorRect.yMax;
                attackLabelRect.width /= 3;
                Rect conditionLabelRect = new Rect(attackLabelRect);
                conditionLabelRect.x = attackLabelRect.xMax;
                Rect pointLabelRect = new Rect(conditionLabelRect);
                pointLabelRect.x = conditionLabelRect.xMax;
                Rect attackRect = new Rect(attackLabelRect);
                attackRect.y = attackRect.yMax;
                Rect conditionRect = new Rect(conditionLabelRect);
                conditionRect.y = conditionRect.yMax;
                Rect pointRect = new Rect(pointLabelRect);
                pointRect.y = pointRect.yMax;
                Rect foldoutRect = new Rect(rect);
                foldoutRect.y = conditionRect.yMax;

                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 4);

                SerializedProperty sp_Current = sp_Attacks.GetArrayElementAtIndex(i);
                SerializedProperty sp_attack = sp_Current.FindPropertyRelative("s_Attack");
                string attackName = (sp_attack.objectReferenceValue as Attack)?.Name ?? LS_Unknown;
                SerializedProperty sp_condition = sp_Current.FindPropertyRelative("s_Condition");
                SerializedProperty sp_point = sp_Current.FindPropertyRelative("Point");

                EditorHooks.DrawColoredSquare(labelRect, Color.gray);
                EditorGUI.LabelField(labelRect, attackName, GUIS_EntityAttack_Atk_Label);

                EditorGUI.LabelField(attackLabelRect, ELS_EntityAttack_Attack);
                EditorGUI.LabelField(conditionLabelRect, ELS_EntityAttack_Condition);
                EditorGUI.LabelField(pointLabelRect, ELS_EntityAttack_Point);
                EditorGUI.PropertyField(attackRect, sp_attack, GUIContent.none);
                EditorGUI.PropertyField(conditionRect, sp_condition, GUIContent.none);
                EditorGUI.PropertyField(pointRect, sp_point, GUIContent.none);

                sp_Current.isExpanded = EditorGUI.Foldout(foldoutRect, sp_Current.isExpanded, ELS_Additional_Settings);
                if (sp_Current.isExpanded)
                {
                    EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 2);

                    Rect activeRect = new Rect(rect);
                    activeRect.y = foldoutRect.yMax;
                    activeRect.width /= 3;
                    Rect instAttackRect = new Rect(activeRect);
                    instAttackRect.x = activeRect.xMax;
                    Rect instConditionRect = new Rect(instAttackRect);
                    instConditionRect.x = instAttackRect.xMax;

                    SerializedProperty sp_Active = sp_Current.FindPropertyRelative("Active");
                    SerializedProperty sp_InstAttack = sp_Current.FindPropertyRelative("InstantiatesAttack");
                    SerializedProperty sp_InstCondition = sp_Current.FindPropertyRelative("InstantiatesCondition");

                    EditorGUI.LabelField(activeRect, ELS_EntityAttack_Active);
                    EditorGUI.LabelField(instAttackRect, ELS_EntityAttack_InstAttack);
                    EditorGUI.LabelField(instConditionRect, ELS_EntityAttack_InstCondition);

                    EditorHooks.Rects.SetRectsYPos(activeRect.yMax, ref activeRect, ref instAttackRect, ref instConditionRect);

                    EditorGUI.PropertyField(activeRect, sp_Active, GUIContent.none);
                    EditorGUI.PropertyField(instAttackRect, sp_InstAttack, GUIContent.none);
                    EditorGUI.PropertyField(instConditionRect, sp_InstCondition, GUIContent.none);
                }
            }

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

            Rect addRect = EditorGUILayout.GetControlRect();
            addRect.width /= 2;
            Rect removeRect = new Rect(addRect);
            removeRect.x = removeRect.xMax;

            if (GUI.Button(addRect, LS_Add))
            {
                sp_Attacks.InsertArrayElementAtIndex(Mathf.Clamp(sp_Attacks.arraySize - 1, 0, sp_Attacks.arraySize));
            }

            if (sp_Attacks.arraySize > 0 && GUI.Button(removeRect, LS_Remove))
            {
                sp_Attacks.DeleteArrayElementAtIndex(sp_Attacks.arraySize - 1);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
