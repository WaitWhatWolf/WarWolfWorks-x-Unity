using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.IO.CTS;
using WarWolfWorks.Utility;
using static WarWolfWorks.Constants;
using static WarWolfWorks.EditorBase.Constants;

namespace WarWolfWorks.EditorBase.Custom
{
    /// <summary>
    /// Use this editor to display an <see cref="Enum"/>'s value in a list fashion.
    /// </summary>
    public sealed class EnumListDisplayer : EditorWindow
    {
        /// <summary>
        /// Shows a <see cref="EnumListDisplayer"/> menu.
        /// </summary>
        [MenuItem("WarWolfWorks/Enum Displayer")]
        public static void Enable()
        {
            GetWindow<EnumListDisplayer>("Enum List Displayer").Show();
        }

        private Type CurrentType = null;
        private string ParsedType = "";

        bool flag;

        private bool ActivatedWindow;

        /// <summary>
        /// Currently displayed Enum values (Only used for int-type enums)
        /// </summary>
        public (string[] EnumName, int[] EnumVal) EnumValues;
        /// <summary>
        /// Currently displayed Enum values (Only used for short-type enums)
        /// </summary>
        public (string[] EnumName, short[] EnumVal) EnumValuesShort;
        /// <summary>
        /// Currently displayed Enum values (Only used for long-type enums)
        /// </summary>
        public (string[] EnumName, long[] EnumVal) EnumValuesLong;

        private LanguageString LS_Remove = new LanguageString("Remove Entry", ("Usuń wejście", SystemLanguage.Polish), ("エントリーを削除", SystemLanguage.Japanese));

        private LanguageString LS_WarningDoesntExist = new LanguageString("Parsed type does not exist! Make sure you include the namespace of the type and the type is an Enum.",
            ("Podany typ nie istnieję! Upewnij się że podany typ zawiera namespace oraz jest on typem Enum.", SystemLanguage.Polish),
            ("タイプは存在しません！ネームスペースを含め、タイプがEnumであることを確認してください", SystemLanguage.Japanese));
        private LanguageString LS_CurrentDisplay = new LanguageString("Currently Displaying", ("Obecnie Wyświetlane", SystemLanguage.Polish), ("現在表示", SystemLanguage.Japanese));

        private Vector3 ScrollPosition;

        enum IntType : byte
        {
            int16,
            int32,
            int64
        }

        private IntType CurrentIntType;

        private Variable[] PreviousEntries;
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

                ParsedType = PreviousEntries[entryIndx].Value;
            }
        }

        private void OnEnable() => SetEntries();

        private void SetEntries()
        {
            try { PreviousEntries = CTS_Preferences_EnumEntries.GetAllLines().ToArray(); }
            catch { goto DefaultSetter; }

            if (PreviousEntries != null && PreviousEntries.Length > 0)
                return;

        DefaultSetter:
            PreviousEntries = new Variable[] { new Variable(GetSaveName(0), "WarWolfWorks.EntitiesSystem.Statistics.Stacking") };
        }

        private string GetSaveName(int index)
        {
            return $"Entry{index}";
        }

        private void OnDisable()
        {
            CTS_Preferences_EnumEntries.Apply();
        }

        private void OnGUI()
        {
            try
            {
                if (!ActivatedWindow)
                {
                    if (PreviousEntries == null) SetEntries();
                    ParsedType = EditorGUILayout.TextField(LS_Type, ParsedType);
                    flag = GUILayout.Button(LS_Check, GUILayout.Height(40));
                    CurrentType = ParsedType.Length > 0 ? Hooks.ParseType(ParsedType) : null;

                    if (CurrentType != null && CurrentType.BaseType == typeof(Enum))
                    {
                        if (flag)
                        {
                            ActivatedWindow = true;
                            Type underlyingType = Enum.GetUnderlyingType(CurrentType);
                            CurrentIntType = underlyingType == typeof(int) ? IntType.int32 :
                                underlyingType == typeof(short) ? IntType.int16 : IntType.int64;

                            if(PreviousEntries == null || PreviousEntries.FindIndex(v => v.Value == ParsedType) == -1)
                                CTS_Preferences_EnumEntries[GetSaveName(PreviousEntries.Length)] = ParsedType;

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
                        EditorGUILayout.HelpBox(LS_WarningDoesntExist, UnityEditor.MessageType.Warning);


                    if (GUILayout.Button(LS_Next))
                    {
                        EntryIndex++;
                    }
                    if(GUILayout.Button(LS_Previous))
                    {
                        EntryIndex--;
                    }
                    if (GUILayout.Button(LS_Remove))
                    {
                        CTS_Preferences_EnumEntries.Remove(PreviousEntries[EntryIndex]);
                        SetEntries();
                    }
                }
                else
                {
                    EditorGUILayout.LabelField($"{LS_CurrentDisplay}: {CurrentType.FullName}");

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

                    if (GUILayout.Button(LS_Reset))
                    {
                        CTS_Preferences_EnumEntries.Apply();
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
