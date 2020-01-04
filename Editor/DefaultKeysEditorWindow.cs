using System;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.Utility;

namespace WarWolfWorks.EditorBase
{
    public class KeyCodeEditorWindow : EditorWindow
    {
        public const string WindowName = "Default Keys Customizer";

        private Vector3 ScrollPosition;

        private bool pop;

        private bool pop2;

        private bool pop3;

        private bool pop4;

        private bool pop5;

        private string keySave;

        private string keycodeSave;

        private (string, KeyCode)[] Keys;

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

        private void UpdateKeys()
        {
            try { Keys = DefaultKeys.GetAllKeys(); }
            catch { goto DefaultSetter; }
            if (Keys != null)
                return;

            DefaultSetter:
            Keys = new (string, KeyCode)[]
            {
                ("ConsoleKey", KeyCode.F1)
            };
        }

        private void OnGUI()
        {
            if (!DefaultKeys.IsOptimized)
            {
                EditorGUILayout.BeginVertical();
                ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);

                for (int num = Keys.Length - 1; num >= 0; num--)
                {
                    EditorGUILayout.LabelField($"{Keys[num].Item1}: {Keys[num].Item2}");
                }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();

                pop = GUILayout.Button("Save Keys");
                if (pop && Keys != null)
                {
                    for (int i = 0; i < Keys.Length; i++)
                    {
                        DefaultKeys.ChangeKey(Keys[i]);
                    }
                }
                pop2 = GUILayout.Button("Update Keys");
                if (pop2)
                {
                    UpdateKeys();
                }
                keySave = EditorGUILayout.TextField("Custom Key", keySave, Array.Empty<GUILayoutOption>());
                keycodeSave = EditorGUILayout.TextField("Parsed Value", keycodeSave, Array.Empty<GUILayoutOption>());
                KeyCode item = KeyCode.None;
                if (keycodeSave != string.Empty)
                {
                    try
                    {
                        item = Hooks.Parse<KeyCode>(keycodeSave);
                    }
                    catch
                    {
                        EditorGUILayout.HelpBox("The parsed value given is invalid! Make sure there are no spaces in your input.", MessageType.Warning);
                    }
                }
                pop3 = GUILayout.Button("Save Custom Key");
                if (pop3)
                {
                    DefaultKeys.AddKey((keySave, item));
                    UpdateKeys();
                }
                pop4 = GUILayout.Button("Remove Custom Key");
                if (pop4)
                {
                    DefaultKeys.RemoveKey(keySave);
                    UpdateKeys();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Cannot modify keys while Optimized mode is active!", Array.Empty<GUILayoutOption>());
                if (!Application.isPlaying)
                {
                    pop5 = GUILayout.Button("Force Disable Optimization");
                    if (pop5)
                    {
                        DefaultKeys.Unoptimize();
                    }
                }
            }
            for (int j = 0; j < 4; j++)
            {
                EditorGUILayout.Space();
            }
            EditorGUILayout.LabelField("Current Optimization State: " + (DefaultKeys.IsOptimized ? "Active" : "Inactive"));
        }
    }
}
