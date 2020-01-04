using UnityEditor;
using WarWolfWorks.Utility;
using System.Linq;
using System.IO;

namespace WarWolfWorks.EditorBase
{
    public class CreateEmptyScriptableObject : Editor
    {
        //Name which will be applied to new objects.
        private const string MissingName = "MissingScriptableObject";

        //Content of the .asset file created.
        private static string[] StreamedLines = new string[]
        {
            "%YAML 1.1",
            "%TAG !u! tag:unity3d.com,2011:",
            "--- !u!114 &11400000",
            "MonoBehaviour:",
            "m_ObjectHideFlags: 0",
            "m_CorrespondingSourceObject: {fileID: 0}",
            "m_PrefabInstance: {fileID: 0}",
            "m_PrefabAsset: {fileID: 0}",
            "m_GameObject: {fileID: 0}",
            "m_Enabled: 1",
            "m_EditorHideFlags: 0",
            "m_Script:"
        };
        /// <summary>
        /// Creates the missing file to the selected file's folder.
        /// </summary>
        [MenuItem(itemName: "WWWLibrary/Create Missing ScriptableObject")]
        public static void Create()
        {
            //Gets the selected object inside Assets/Project window.
            UnityEngine.Object @object = Selection.activeObject;

            //Sets the path to empty if no object was selected, otherwise it takes the path of the selected object.
            string path = @object ? AssetDatabase.GetAssetPath(@object) : "";

            //If the selected file is nor a folder nor null, it will remove the last part of the path (to get only the folderpath)
            if (path.Length != 0)
            {
                if(Path.GetExtension(path) != "") path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(@object)), "");
                path = path.Replace("Assets/", "");
                path = path.Replace('/', '\\');
            }

            //Combines the Asset path with a windows file path to the asset folder.
            path = Path.Combine(Hooks.Streaming.AssetsPath, path);

            //Creates a string that will set the name of the asset created.
            string fileName = $"new {MissingName}.asset";

            //Gets all files inside the path folder to see if fileName should be changed.
            string[] files = Hooks.Streaming.GetAllFilesInFolder(path, fileName.Replace(".", "?."), false);

            //If a file or files of fileName were found, it will loop through each file inside the folder to see 
            //if any file is named the same as fileName.
            if (files != null)
            {
                for (int i = 1; i < files.Length + 1; i++)
                {
                    //Sets the new name of the file.
                    string TmpFileName = i == 0 ? fileName : fileName.Replace(".asset", i.ToString() + ".asset");
                    //If the name is does not exist inside path folder, it will be chosen.
                    if (!files.Contains(TmpFileName))
                    {
                        fileName = TmpFileName;
                        break;
                    }
                }
            }

            //Creates a path to a file for a StreamWriter to write into.
            string filePath = Path.Combine(path, fileName);

            //Creates the file.
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                for (int i = 0; i < StreamedLines.Length; i++)
                {
                    sw.WriteLine(StreamedLines[i]);
                }

                sw.Dispose();
            }

            //Replaces all backslashes f the path with forwardslashes for unity to read.
            string unityPath = filePath.Replace('\\', '/');

            //Refreshes the Project window at path starting from "Assets" for it to be displayed.
            AssetDatabase.ImportAsset(unityPath.Substring(unityPath.IndexOf("Assets")));
        }
    }
}
