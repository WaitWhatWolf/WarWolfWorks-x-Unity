using System;
using UnityEditor;
using WarWolfWorks.EditorBase.Interfaces;
using WarWolfWorks.NyuEntities.YharonSystem;

namespace WarWolfWorks.EditorBase.NyuEntities
{
    internal sealed class NyuYharonInternalEditor : INyuComponentInternalEditor
    {
        Type INyuComponentInternalEditor.EditorType => typeof(NyuYharon);

        private NyuYharon nc_Yharon;
        private bool yharonsExpand;
        private bool resistancesExpand;

        void INyuComponentInternalEditor.OnEnable(SerializedObject serializedObject)
        {
            nc_Yharon = serializedObject.targetObject as NyuYharon;
        }

        void INyuComponentInternalEditor.OnInspectorGUI()
        {
            yharonsExpand = EditorGUILayout.Foldout(yharonsExpand, "Yharons");

            if (yharonsExpand)
            {
                if (nc_Yharon.ns_Yharons.Count > 0)
                {
                    for (int i = 0; i < nc_Yharon.ns_Yharons.Count; i++)
                    {
                        string secondLabel = (nc_Yharon.ns_Yharons[i] is CountdownYharon ctYharon) ? $"{ctYharon.GetCountdown()}/{ctYharon.StartCountdown}" : nc_Yharon.ns_Yharons[i].YharonType.ToString();
                        EditorGUILayout.LabelField(nc_Yharon.ns_Yharons[i].YharonType.Name, secondLabel);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("No yharons found.");
                }
            }

            resistancesExpand = EditorGUILayout.Foldout(resistancesExpand, "TaCx1s");
            if (resistancesExpand)
            {
                if (nc_Yharon.ns_TaCx1s.Count > 0)
                {
                    for (int i = 0; i < nc_Yharon.ns_Yharons.Count; i++)
                    {
                        EditorGUILayout.LabelField(nc_Yharon.ns_TaCx1s[i].Resistance.Name, (nc_Yharon.ns_TaCx1s[i].Percent * 100).ToString() + "%");
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("No resistances found.");
                }
            }
        }
    }
}
