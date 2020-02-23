using UnityEngine;
using UnityEditor;
using System.IO;

namespace WarWolfWorks.EditorBase.Internal
{
    internal sealed class ScriptKeywordProcessor : UnityEditor.AssetModificationProcessor
    {
        public static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", "");
            int index = path.LastIndexOf(".");
            if (index < 0)
                return;

            string file = path.Substring(index);
            if (file != ".cs")
                return;

            index = Application.dataPath.LastIndexOf("Assets");
            string originalPath = path;
            path = Application.dataPath.Substring(0, index) + path;
            if (!File.Exists(path))
                return;

            string fileContent = File.ReadAllText(path);
            fileContent = fileContent.Replace("#NAMESPACE#", Application.productName);

            string nameCut = originalPath.Substring(originalPath.LastIndexOf('/'), 3);
            bool isInterface = nameCut[1] == 'I' && nameCut[2] == char.ToUpper(nameCut[2]);
            fileContent = fileContent.Replace("#SCRIPTTYPE#", isInterface ? "interface" : "class");

            if (isInterface)
                fileContent = fileContent.Replace(" : MonoBehaviour", string.Empty);

            File.WriteAllText(path, fileContent);
            AssetDatabase.Refresh();
        }
    }
}