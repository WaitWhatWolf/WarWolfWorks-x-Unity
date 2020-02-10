using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using WarWolfWorks.Debugging;
using WarWolfWorks.EditorBase.Utility;
using WarWolfWorks.Utility;

namespace WarWolfWorks.EditorBase.Custom
{
    public class KeyCodeEditorWindow : EditorWindow
    {
        public const string WINDOW_NAME = "Default Keys Customizer";

        private const float WINDOW_KR_OFFSET_Y = 10;
        private const float WINDOW_KR_CELL_SIZE_Y = 20;
        private const float WINDOW_KR_CELL_PADDING_X = 5;

        private const int WINDOW_KR_CELL_DIVISIONS = 3;

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

        [MenuItem("WarWolfWorks/Default Keys Customizer")]
        public static void ShowWindow()
        {
            KeyCodeEditorWindow window = EditorWindow.GetWindow<KeyCodeEditorWindow>("Default Keys Customizer");
            window.Show();
            window.minSize = new Vector2(250f, 350f);
            window.maxSize = new Vector2(350f, 900f);
            window.position = new Rect(1200f, 300f, 350f, 500f);
        }

        private void OnEnable()
        {
            UpdateKeys();
        }

        private float DivWidth => position.DividedWidth(WINDOW_KR_CELL_DIVISIONS);

        private Rect GetKeyRect(float posX, float sizeX, float posY)
        {
            return new Rect(position.xMin + DivWidth,
                    posY,
                    DivWidth - WINDOW_KR_CELL_PADDING_X,
                    WINDOW_KR_CELL_SIZE_Y);
        }

        private void UpdateKeys()
        {
            try
            {
                Keys = DefaultKeys.GetAllKeys();
                list = new ReorderableList(Keys, typeof(DefaultKeys.WKey), true, false, false, false);

                list.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    string txt = EditorGUI.TextField(
                        new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), Keys[index].Name);

                    KeyCode key = Keys[index].Key;

                    if (ParsesValues)
                    {
                        key = (KeyCode)EditorGUI.EnumPopup(
                            new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
                            Keys[index].Key);
                    }
                    else
                    {
                        try
                        {
                            key = Hooks.Parse<KeyCode>(EditorGUI.TextField(
                                new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
                                Keys[index].Key.ToString()));
                        }
                        catch
                        {
                            key = Keys[index].Key;
                        }
                    }

                    Keys[index] = new DefaultKeys.WKey(txt, key);

                    rect.y += 2;
                };

                list.onReorderCallback = (ReorderableList rl)
                =>
                {
                    try
                    {
                        List<DefaultKeys.WKey> tmpKeys = GetSortedList();
                        Hooks.Streaming.Reorder(DefaultKeys.KeysLoader(string.Empty), 
                            t => tmpKeys.FindIndex(key => key.Name == t.Split(Hooks.Streaming.STREAM_VALUE_POINTER, StringSplitOptions.None)[0]));
                    }
                    catch(Exception e)
                    {
                        AdvancedDebug.LogException(e);
                    }
                };
            }
            catch { goto DefaultSetter; }
            if (Keys != null)
                return;

            DefaultSetter:
            Keys = new List<DefaultKeys.WKey>
            {
                WWWConsole.DefaultKey
            };
        }

        private List<DefaultKeys.WKey> GetSortedList()
            => Hooks.Enumeration.ToGenericList<DefaultKeys.WKey>(list.list, false);

        private void OnGUI()
        {
            if (list != null && list.list.Count > 0)
            {
                EditorGUILayout.BeginVertical();
                ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);

                list.DoLayoutList();

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }

            pops[0] = GUILayout.Button(!ParsesValues ? LS_SwitchViewEnum : LS_SwitchViewParse);
            if (pops[0]) ParsesValues = !ParsesValues;

            EditorHooks.SlickSeparator();

            pops[4] = GUILayout.Button(LS_Add);
            if (pops[4])
            {
                DefaultKeys.AddKey(new DefaultKeys.WKey("New Key", KeyCode.None));
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
            if (pops[1] && Keys != null)
            {
                List<DefaultKeys.WKey> keyz = GetSortedList();
                for (int i = 0; i < Keys.Count; i++)
                {
                    DefaultKeys.AddKey(keyz[i]);
                }
            }

            EditorHooks.MultiSpace(4);
            EditorGUILayout.LabelField("Current Optimization State: " + (DefaultKeys.IsOptimized ? "Active" : "Inactive"));
        }
    }
}
