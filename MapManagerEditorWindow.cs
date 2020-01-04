using System;
using UnityEditor;
using UnityEngine;
using WarWolfWorks.Utility;

namespace WarWolfWorks.EditorBase
{
    public class MapManagerWindow : EditorWindow
    {
        private const string WindowName = "Map Manager";

        private static readonly string SavePath = Hooks.Streaming.GetStreamingAssetsFilePath("Maps.kfidk");

        private static readonly string CategoryName = "MapsConfig";

        private const string EncryptionKey = "MapsUnite!";

        private string path;

        private bool pop;

        private bool pop2;

        private RuntimeInitializeLoadType rilt;

        [MenuItem("WWWLibrary/Map Manager")]
        public static void ShowWindow()
        {
            MapManagerWindow window = EditorWindow.GetWindow<MapManagerWindow>("Map Manager");
            window.Show();
            window.minSize = new Vector2(250f, 150f);
            window.maxSize = new Vector2(350f, 250f);
            window.position = new Rect(1200f, 300f, 350f, 500f);
        }

        private void OnEnable()
        {
            pop = Convert.ToBoolean(Hooks.Streaming.Load(SavePath, CategoryName, "LoadsStartingMap", false.ToString()));
            rilt = Hooks.Parse<RuntimeInitializeLoadType>(Hooks.Streaming.Load(SavePath, CategoryName, "SSLT", RuntimeInitializeLoadType.AfterSceneLoad.ToString()));
            path = Hooks.Streaming.Load(SavePath, CategoryName, "StartingMapPath", string.Empty);
        }

        private void OnDisable()
        {
            Save();
        }

        private void OnGUI()
        {
            pop = EditorGUILayout.Toggle("Load Starting Map", pop, Array.Empty<GUILayoutOption>());
            if (pop)
            {
                path = EditorGUILayout.TextField("Starting Map Resources Path:", path);
                rilt = (RuntimeInitializeLoadType)EditorGUILayout.EnumPopup("Execution Order:", rilt);
                if (rilt != 0)
                {
                    EditorGUILayout.HelpBox("Execution that is not set as 'AfterSceneLoad' can cause the map manager not to load the default map inside the editor.", MessageType.Warning);
                }
            }
            pop2 = GUILayout.Button("Save");
            if (pop2)
            {
                Save();
            }
        }

        private void Save()
        {
            Hooks.Streaming.CreateFolder(SavePath);
            Hooks.Streaming.Save(SavePath, CategoryName, "StartingMapPath", path);
            Hooks.Streaming.Save(SavePath, CategoryName, "SSLT", rilt.ToString());
            Hooks.Streaming.Save(SavePath, CategoryName, "LoadsStartingMap", pop.ToString());
        }

    }
}
