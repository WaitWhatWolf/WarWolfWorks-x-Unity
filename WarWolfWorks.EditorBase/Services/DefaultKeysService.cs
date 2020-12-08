using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using WarWolfWorks.Debugging;
using WarWolfWorks.EditorBase.Interfaces;
using WarWolfWorks.EditorBase.Utility;
using WarWolfWorks.Utility;
using static WarWolfWorks.EditorBase.Constants;
using static WarWolfWorks.Constants;
using WarWolfWorks.IO.CTS;

namespace WarWolfWorks.EditorBase.Services
{
    /// <summary>
    /// Draws <see cref="DefaultKeys"/> on the <see cref="ServicesWindow"/>.
    /// </summary>
    internal sealed class DefaultKeysService : IService
    {
        private readonly LanguageString LS_Remove = new LanguageString("Remove Selected", ("Usuń", SystemLanguage.Polish), ("削除", SystemLanguage.Japanese));
        private readonly LanguageString LS_Add = new LanguageString("New Key", ("Nowy Przycisk", SystemLanguage.Polish), ("新しいキー", SystemLanguage.Japanese));
        private readonly LanguageString LS_Save = new LanguageString("Save Selected", ("Zapisz", SystemLanguage.Polish), ("保存", SystemLanguage.Japanese));
        private readonly LanguageString LS_SaveAll = new LanguageString("Save All Keys", ("Zapisz wszystkie przyciski", SystemLanguage.Polish), ("全てキー保存", SystemLanguage.Japanese));
        private readonly LanguageString LS_SwitchViewEnum = new LanguageString("Switch to Enum View", ("Zmień na widok 'Enum'", SystemLanguage.Polish), ("'Enum' 見る", SystemLanguage.Japanese));
        private readonly LanguageString LS_SwitchViewParse = new LanguageString("Switch to Parse View", ("Zmień na widok 'Parse'", SystemLanguage.Polish), ("'Parse' 見る", SystemLanguage.Japanese));

        private ReorderableList list;

        private Vector2 ScrollPosition;

        private bool[] pops = new bool[6];
        private bool ParsesValues;

        private List<DefaultKeys.WKey> Keys;

        public string Name => ELS_DefaultKeys;

        private void UpdateKeys()
        {
            try
            {
                Keys = DefaultKeys.GetAllKeys();
                list = new ReorderableList(Keys, typeof(DefaultKeys.WKey), true, false, false, false);

                list.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    Rect nameRect = new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight);
                    Rect keyRect = new Rect(nameRect);
                    keyRect.x = keyRect.xMax;

                    if (DefaultKeys.IsOptimized)
                    {
                        Color usedColor = new Color(.3f, .3f, .3f, .4f);
                        EditorGUI.LabelField(nameRect, Keys[index].Name);
                        nameRect.width /= 1.1f;
                        EditorHooks.DrawColoredSquare(nameRect, usedColor);
                        EditorGUI.LabelField(keyRect, Keys[index].Key.ToString());
                        keyRect.width /= 1.1f;
                        EditorHooks.DrawColoredSquare(keyRect, usedColor);
                    }
                    else
                    {
                        string txt = EditorGUI.TextField(nameRect, Keys[index].Name);

                        KeyCode key = Keys[index].Key;

                        if (ParsesValues)
                        {
                            key = (KeyCode)EditorGUI.EnumPopup(keyRect, Keys[index].Key);
                        }
                        else
                        {
                            try
                            {
                                key = Hooks.Parse<KeyCode>(EditorGUI.TextField(keyRect, Keys[index].Key.ToString()));
                            }
                            catch
                            {
                                key = Keys[index].Key;
                            }
                        }

                        Keys[index] = new DefaultKeys.WKey(txt, key);
                    }

                    rect.y += EV_DEFKEYS_KEY_CELL_PADDING_Y;
                };

                list.onReorderCallback = (ReorderableList rl)
                =>
                {
                    try
                    {
                        List<DefaultKeys.WKey> tmpKeys = GetSortedList();
                        CTS_DefaultKeys.OrderBy(v => tmpKeys.FindIndex(key => key.Name == v.Name));
                    }
                    catch (Exception e)
                    {
                        AdvancedDebug.LogException(e);
                    }
                };
            }
            catch { goto DefaultSetter; }
            if (Keys != null)
                return;

            DefaultSetter:
            Keys = new();
        }

        private List<DefaultKeys.WKey> GetSortedList()
            => Hooks.Enumeration.ToGenericList<DefaultKeys.WKey>(list.list, false);

        public void Draw()
        {
            if (list != null && list.list.Count > 0)
            {
                EditorGUILayout.BeginVertical();
                ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);

                list.DoLayoutList();

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }

            if (!DefaultKeys.IsOptimized)
            {
                pops[0] = GUILayout.Button(!ParsesValues ? LS_SwitchViewEnum : LS_SwitchViewParse);
                if (pops[0]) ParsesValues = !ParsesValues;

                EditorHooks.SlickSeparator();

                pops[4] = GUILayout.Button(LS_Add);
                if (pops[4])
                {
                    int count = 0;
                    while (DefaultKeys.KeyExists(EV_DEFKEYS_KEY_NEW + count.ToString()))
                        count++;
                    DefaultKeys.AddKey(new DefaultKeys.WKey("New Key" + count.ToString(), KeyCode.None));
                    UpdateKeys();
                }

                if (list != null && list.index >= 0 && list.index < Keys.Count)
                {
                    pops[2] = GUILayout.Button(LS_Save);
                    if (pops[2])
                    {
                        DefaultKeys.AddKey(GetSortedList()[list.index]);
                    }

                    pops[3] = GUILayout.Button(LS_Remove);
                    if (pops[3])
                    {
                        DefaultKeys.WKey removed = GetSortedList()[list.index];
                        DefaultKeys.RemoveKey(removed.Name);
                        UpdateKeys();
                    }
                }

                EditorHooks.SlickSeparator();

                pops[1] = GUILayout.Button(LS_SaveAll);

                if (pops[1])
                {
                    if (Keys != null)
                    {
                        List<DefaultKeys.WKey> keyz = GetSortedList();
                        List<Variable> variables = new List<Variable>(Keys.Count);
                        for (int i = 0; i < Keys.Count; i++)
                        {
                            variables.Add(new Variable(keyz[i].Name, keyz[i].Key.ToString()));
                            DefaultKeys.DebugKeyAddition(keyz[i].Name, keyz[i].Key);
                        }
                        CTS_DefaultKeys.Override(variables);
                        CTS_DefaultKeys.Apply();
                    }
                }
            }

            EditorHooks.MultiSpace(4);
            EditorGUILayout.LabelField("Current Optimization State: " + (DefaultKeys.IsOptimized ? "Active" : "Inactive"));
        }

        public void OnEnable()
        {
            UpdateKeys();
        }

        public void OnDisable()
        {
            DefaultKeys.Apply();
        }
    }
}
