using UnityEditor;
using WarWolfWorks.NyuEntities;
using WarWolfWorks.EditorBase.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using static WarWolfWorks.EditorBase.Constants;
using static WarWolfWorks.WWWResources;
using UnityEditorInternal;
using UnityEngine;
using WarWolfWorks.EditorBase.Interfaces;
using System.Linq;
using WarWolfWorks.Attributes;
using WarWolfWorks.Interfaces.NyuEntities;

namespace WarWolfWorks.EditorBase.NyuEntities
{
    /// <summary>
    /// Base editor for <see cref="Nyu"/>. If you wish to make a custom editor for <see cref="Nyu"/>,
    /// override PostOnInspectorGUI.
    /// </summary>
    [CustomEditor(typeof(Nyu), true)]
    public class NyuEditor : Editor
    {
        private struct SerializedGroup
        {
            public INyuComponent Parent;
            public SerializedObject s_SerializedObject;
            public List<SerializedProperty> Properties;
            public List<GUIContent> GUIContents;

            public SerializedGroup(INyuComponent component)
            {
                Parent = component;
                s_SerializedObject = new SerializedObject(Parent as UnityEngine.Object);
                EditorHooks.GetAllVisibleProperties(s_SerializedObject, false, out Properties, out GUIContents);
            }
        }

        private SerializedProperty sp_PlotArmor;
        /// <summary>
        /// Returns the target cast as <see cref="Nyu"/>.
        /// </summary>
        protected Nyu NyuTarget => (Nyu)target;

        private INyuComponent[] TargetComponents;
        private SerializedGroup[] SerializedGroupsDefault;

        private Type[] EditorTypes;
        private List<Type> NyuComponentTypes;
        private List<INyuComponentEditor> NyuEditors;
        private List<INyuSerializedEditor> NyuSerializedEditors;
        private List<INyuComponentInternalEditor> PremadeEditors = new List<INyuComponentInternalEditor>();
        private ReorderableList ReorderableTypeList;
        private List<SerializedProperty> sp_Properties;
        private List<GUIContent> sp_PropertyContents;

        /// <summary>
        /// The expanded state of the add section.
        /// </summary>
        private bool AddExpanded;

        /// <summary>
        /// Used for color-selection display.
        /// </summary>
        private int SelectedIndex;

        private void OnEnable()
        {
            RefreshTypes();
            RefreshDrawnComponents();
            RefreshReorderableTypeList();
            RefreshOtherProperties();
            OnEnabled();
        }

        private void OnDisable()
        {
            if(NyuSerializedEditors != null)
                foreach (INyuSerializedEditor serializedEditor in NyuSerializedEditors)
                    serializedEditor.OnDisable();

            OnDisabled();
        }

        /// <summary>
        /// Override this method to get the equivalent of OnDisable.
        /// </summary>
        protected virtual void OnDisabled() { }

        /// <summary>
        /// Refreshes components to be drawn on the entity's core inspector.
        /// </summary>
        protected void RefreshDrawnComponents()
        {
            if (NyuTarget.hs_SerializePlotArmor == null || NyuTarget.hs_SerializePlotArmor.Length + 1 < byte.MaxValue)
                NyuTarget.hs_SerializePlotArmor = new bool[byte.MaxValue + 1];

            sp_PlotArmor = serializedObject.FindProperty("hs_SerializePlotArmor");

            Type[] avTypes = Assembly.GetAssembly(typeof(INyuComponentInternalEditor)).GetTypes();

            PremadeEditors = new List<INyuComponentInternalEditor>();
            for (int i = 0; i < avTypes.Length; i++)
            {
                if (avTypes[i].GetInterfaces().Contains(typeof(INyuComponentInternalEditor)))
                {
                    INyuComponentInternalEditor editor = (INyuComponentInternalEditor)Activator.CreateInstance(avTypes[i]);
                    PremadeEditors.Add(editor);
                }
            }

            TargetComponents = NyuTarget.GetComponents<INyuComponent>();
            NyuEditors = new List<INyuComponentEditor>(TargetComponents.Length);
            SerializedGroupsDefault = new SerializedGroup[TargetComponents.Length];
            for (int i = 0; i < TargetComponents.Length; i++)
            {
                if (TargetComponents[i] is INyuComponentEditor editor)
                    NyuEditors.Add(editor);
                else
                {
                    NyuEditors.Add(null);

                    SerializedGroupsDefault[i] = new SerializedGroup(TargetComponents[i]);
                }
                for (int j = 0; j < PremadeEditors.Count; j++)
                {
                    if (PremadeEditors[j].EditorType.IsAssignableFrom(TargetComponents[i].GetType()))
                    {
                        PremadeEditors[j].OnEnable(new SerializedObject(TargetComponents[i] as Component));
                    }
                }
            }

            if (EditorTypes != null)
            {
                NyuSerializedEditors = new List<INyuSerializedEditor>(EditorTypes.Length);
                for (int i = 0; i < EditorTypes.Length; i++)
                {
                    if (EditorTypes[i].GetInterfaces().Contains(typeof(INyuSerializedEditor)))
                        NyuSerializedEditors.Add(Activator.CreateInstance(EditorTypes[i]) as INyuSerializedEditor);
                }

                for (int i = 0; i < NyuSerializedEditors.Count; i++)
                {
                    SerializedObject used = null;
                    for (int j = 0; j < TargetComponents.Length; j++)
                        if (NyuSerializedEditors[i].EditorOf == TargetComponents[j].GetType())
                            used = new SerializedObject(TargetComponents[j] as UnityEngine.Object);

                    NyuSerializedEditors[i].OnEnable(used);
                }
            }
        }

        /// <summary>
        /// Refreshes all reflected types addable to the Nyu entity.
        /// </summary>
        protected void RefreshTypes()
        {
            Assembly usedAssembly = null;
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if(assembly.GetName().Name == "Assembly-CSharp-Editor")
                {
                    usedAssembly = assembly;
                    break;
                }
            }

            EditorTypes = usedAssembly?.GetTypes();

            if (TargetComponents == null)
                RefreshDrawnComponents();

            List<Type> retrievedTypes = new List<Type>(Assembly.GetAssembly(typeof(INyuComponent)).GetTypes());
            retrievedTypes.AddRange(serializedObject.targetObject.GetType().Assembly.GetTypes());

            NyuComponentTypes = new List<Type>(retrievedTypes.Count);
            for (int i = 0; i < retrievedTypes.Count; i++)
            {
                Type[] interfaces = retrievedTypes[i].GetInterfaces();
                if (Array.Exists(interfaces, t => t == typeof(INyuComponent))
                    && !retrievedTypes[i].IsAbstract 
                    && !Array.Exists(TargetComponents, c => c.GetType() == retrievedTypes[i]))
                    NyuComponentTypes.Add(retrievedTypes[i]);
            }
        }

        /// <summary>
        /// Refreshes the reorderable type list.
        /// </summary>
        protected void RefreshReorderableTypeList()
        {
            ReorderableTypeList = new ReorderableList(NyuComponentTypes, typeof(Type), false, false, false, false);
            ReorderableTypeList.drawElementCallback += Event_DrawList;
            ReorderableTypeList.drawHeaderCallback += Event_DrawListHeader;
        }

        /// <summary>
        /// Refreshes all properties to be drawn in the base <see cref="PostOnInspectorGUI"/>.
        /// </summary>
        protected void RefreshOtherProperties()
        {
            bool propertiesNoS;
            try { propertiesNoS = serializedObject.targetObject.GetType().GetCustomAttributes(typeof(CompleteNoS), true).Length > 0; }
            catch { propertiesNoS = false; }
            EditorHooks.GetAllVisibleProperties(serializedObject, false, out sp_Properties, out sp_PropertyContents, propertiesNoS);
        }

        /// <summary>
        /// The <see cref="ReorderableTypeList"/>'s header.
        /// </summary>
        /// <param name="rect"></param>
        private void Event_DrawListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, ELS_NyuEntity_ListHeader);
        }

        /// <summary>
        /// Used to draw a <see cref="ReorderableTypeList"/>'s cell.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void Event_DrawList(Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorGUI.LabelField(rect, NyuComponentTypes[index].FullName);
        }

        /// <summary>
        /// Override this method to get the equivalent of an editor's OnEnable method.
        /// </summary>
        protected virtual void OnEnabled() { }

        /// <summary>
        /// Draws <see cref="Nyu"/>'s core inspector.
        /// </summary>
        public override sealed void OnInspectorGUI()
        {
            serializedObject.Update();

            sp_PlotArmor.isExpanded = EditorGUILayout.Foldout(sp_PlotArmor.isExpanded, ELS_NyuEntity_Components);
            if(sp_PlotArmor.isExpanded)
            {
                for (int i = 0; i < TargetComponents.Length; i++)
                {
                    string labelName = TargetComponents[i].GetType().Name;
                    Rect use = EditorGUILayout.GetControlRect();

                    Repaint();
                    if (EditorHooks.EventMouseInRect(use))
                    {
                        SelectedIndex = i;
                        if (Event.current.rawType == EventType.MouseDown)
                            NyuTarget.hs_SerializePlotArmor[i] = !NyuTarget.hs_SerializePlotArmor[i];
                    }
                    else
                        SelectedIndex = -1;

                    EditorHooks.DrawColoredSquare(use, i == SelectedIndex ? NyuEntity_Component_Selected : NyuEntity_Component_Deselected);
                    EditorGUI.LabelField(use, "<color=#f9f9f9>" + labelName + "</color>", GUIS_Nyu_Component_Foldout_Style);

                    if (NyuTarget.hs_SerializePlotArmor[i])
                    {
                        bool drawDefaultEditor = true;

                        for (int j = 0; j < PremadeEditors.Count; j++)
                        {
                            if (PremadeEditors[j].EditorType.IsAssignableFrom(TargetComponents[i].GetType()))
                            {
                                PremadeEditors[j].OnInspectorGUI();
                                drawDefaultEditor = PremadeEditors[j].DrawDefaultEditor;
                            }
                        }

                        int serializedEditorIndex = NyuSerializedEditors?.FindIndex(se => se.EditorOf == TargetComponents[i].GetType()) ?? -1;
                        if (serializedEditorIndex != -1)
                        {
                            NyuSerializedEditors[serializedEditorIndex].OnInspectorGUI();
                            drawDefaultEditor = false;
                        }
                        else if (NyuEditors[i] != null)
                        {
                            NyuEditors[i].NyuOnInspectorGUI();
                            drawDefaultEditor = false;
                        }

                        if (drawDefaultEditor)
                        {
                            //EditorGUILayout.LabelField(
                            //    string.Format(ELS_NyuEntity_NoEditor, labelName, typeof(INyuComponentEditor).Name,
                            //    typeof(INyuSerializedEditor).Name), GUILayout.Height(EditorGUIUtility.singleLineHeight * 3));

                            if (SerializedGroupsDefault[i].Properties.Count > 0)
                            {
                                SerializedGroupsDefault[i].s_SerializedObject.Update();

                                for (int j = 0; j < SerializedGroupsDefault[i].Properties.Count; j++)
                                {
                                    EditorGUILayout.PropertyField(SerializedGroupsDefault[i].Properties[j],
                                        SerializedGroupsDefault[i].GUIContents[j]);
                                }

                                SerializedGroupsDefault[i].s_SerializedObject.ApplyModifiedProperties();
                            }
                            else
                            {
                                EditorGUILayout.LabelField(ELS_NyuEntity_NoSerializedObj, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                            }
                        }
                    }
                }

                Rect addRect = EditorGUILayout.GetControlRect();
                Rect removeRect = new Rect(addRect);
                EditorHooks.Rects.SetRectsWidth(addRect.width / 2, ref addRect, ref removeRect);
                removeRect.x = addRect.xMax;

                EditorGUILayout.Space(removeRect.height);

                if (GUI.Button(addRect, LS_Add))
                    AddExpanded = !AddExpanded;

                if (AddExpanded)
                {
                    //ListScrollView = EditorGUILayout.BeginScrollView(ListScrollView);
                    //EditorGUILayout.BeginVertical();
                    ReorderableTypeList.DoLayoutList();
                    //EditorGUILayout.EndVertical();
                    //EditorGUILayout.EndScrollView();

                    if (ReorderableTypeList.index >= 0 && GUILayout.Button(LS_Confirm))
                    {
                        if (EditorApplication.isPlaying)
                            NyuTarget.ANC(NyuComponentTypes[ReorderableTypeList.index]);
                        else
                            NyuTarget.gameObject.AddComponent(NyuComponentTypes[ReorderableTypeList.index]);
                        RefreshDrawnComponents();
                        RefreshTypes();
                        RefreshReorderableTypeList();
                        AddExpanded = false;
                    }
                }
                if (TargetComponents.Length > 0 && GUI.Button(removeRect, LS_RemoveExpanded))
                {
                    bool refreshes = false;
                    for (int i = 0; i < TargetComponents.Length; i++)
                    {
                        if (NyuTarget.hs_SerializePlotArmor[i])
                        {
                            if (EditorApplication.isPlaying)
                                NyuTarget.RNC(TargetComponents[i]);
                            else
                                DestroyImmediate(TargetComponents[i] as Component);

                            refreshes = true;
                        }
                    }

                    if (refreshes)
                    {
                        RefreshDrawnComponents();
                        RefreshTypes();
                        RefreshReorderableTypeList();
                    }
                }
            }

            EditorHooks.SlickSeparator();

            PostOnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Override this method to create a custom inspector.
        /// </summary>
        protected virtual void PostOnInspectorGUI()
        {
            for (int i = 0; i < sp_Properties.Count; i++)
            {
                EditorGUILayout.PropertyField(sp_Properties[i], sp_PropertyContents[i]);
            }
        }
    }
}
