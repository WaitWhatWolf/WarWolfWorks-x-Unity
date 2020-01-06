using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.Internal;
using WarWolfWorks.Utility;
using static WarWolfWorks.Utility.Hooks.Streaming;

namespace WarWolfWorks.EditorBase
{
    public class EnumListDisplayer : EditorWindow
    {
        [MenuItem("WarWolfWorks/Enum Displayer")]
        public static void Enable()
        {
            GetWindow<EnumListDisplayer>("Enum List Displayer").Show();
        }

        private Type CurrentType = null;
        private string ParsedType = "";

        bool flag;

        private bool ActivatedWindow;

        public (string[] EnumName, int[] EnumVal) EnumValues;
        public (string[] EnumName, short[] EnumVal) EnumValuesShort;
        public (string[] EnumName, long[] EnumVal) EnumValuesLong;

        private Vector3 ScrollPosition;

        enum IntType : short
        {
            int16,
            int32,
            int64
        }

        private string Category = "EnumListEntries";
        private IntType CurrentIntType;

        private string[] PreviousEntries;
        private int entryIndx = 0;
        private int EntryIndex
        {
            get => entryIndx;
            set
            {
                entryIndx = value;
                if (entryIndx >= PreviousEntries.Length)
                    entryIndx = 0;
                else if (entryIndx < 0)
                    entryIndx = PreviousEntries.Length - 1;

                ParsedType = PreviousEntries[entryIndx];
            }
        }

        private void OnEnable() => SetEntries();

        private void SetEntries()
        {
            try { PreviousEntries = LoadAll(Catalog.Loader(Settings.SettingsPath, Category, null), false).ToArray(); }
            catch { goto DefaultSetter; }

            if (PreviousEntries != null)
                return;

        DefaultSetter:
            PreviousEntries = new string[] { "WWW.EntitiesSystem.Statistics.Stacking" };
        }

        private void OnGUI()
        {
            try
            {
                if (!ActivatedWindow)
                {
                    if (PreviousEntries == null) SetEntries();
                    ParsedType = EditorGUILayout.TextField("Type", ParsedType);
                    flag = GUILayout.Button("Check", GUILayout.Height(40));
                    CurrentType = ParsedType.Length > 0 ? Hooks.ParseType(ParsedType) : null;

                    if (CurrentType != null && CurrentType.BaseType == typeof(Enum))
                    {
                        if (flag)
                        {
                            ActivatedWindow = true;
                            Type underlyingType = Enum.GetUnderlyingType(CurrentType);
                            CurrentIntType = underlyingType == typeof(int) ? IntType.int32 :
                                underlyingType == typeof(short) ? IntType.int16 : IntType.int64;
                            if(PreviousEntries == null || !PreviousEntries.Contains(ParsedType)) Save(Catalog.Saver(Settings.SettingsPath, Category, $"Entry{PreviousEntries.Length}", ParsedType));
                            switch (CurrentIntType)
                            {
                                default:
                                    EnumValues = (Enum.GetNames(CurrentType), (int[])Enum.GetValues(CurrentType));
                                    break;
                                case IntType.int16:
                                    EnumValuesShort = (Enum.GetNames(CurrentType), (short[])Enum.GetValues(CurrentType));
                                    break;
                                case IntType.int64:
                                    EnumValuesLong = (Enum.GetNames(CurrentType), (long[])Enum.GetValues(CurrentType));
                                    break;
                            }
                        }
                    }
                    else
                        EditorGUILayout.HelpBox("Parsed type does not exist! Make sure you include the namespace of the type and the type is an Enum.", UnityEditor.MessageType.Warning);

                    if(GUILayout.Button("Previous Entry", GUILayout.Height(20)))
                    {
                        EntryIndex--;
                    }
                }
                else
                {
                    EditorGUILayout.LabelField($"Currently displaying: {CurrentType.FullName}");

                    EditorGUILayout.BeginVertical();
                    ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);

                    if (CurrentIntType == IntType.int32)
                    {
                        for (short i = 0; i < EnumValues.EnumName.Length; i++)
                        {
                            EditorGUILayout.LabelField($"{EnumValues.EnumName[i]} = {EnumValues.EnumVal[i]}");
                        }
                    }
                    else if (CurrentIntType == IntType.int16)
                    {
                        for (short i = 0; i < EnumValuesShort.EnumName.Length; i++)
                        {
                            EditorGUILayout.LabelField($"{EnumValuesShort.EnumName[i]} = {EnumValuesShort.EnumVal[i]}");
                        }
                    }
                    else
                    {
                        for (short i = 0; i < EnumValuesLong.EnumName.Length; i++)
                        {
                            EditorGUILayout.LabelField($"{EnumValuesLong.EnumName[i]} = {EnumValuesLong.EnumVal[i]}");
                        }
                    }
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();

                    if (GUILayout.Button("Reset"))
                    {
                        ActivatedWindow = false;
                        PreviousEntries = null;
                    }
                }
            }
            catch
            {
                ActivatedWindow = false;
                PreviousEntries = null;
            }
        }

    }
}
