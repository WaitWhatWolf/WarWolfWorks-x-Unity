using UnityEngine;
using UnityEditor;
using System.IO;

internal sealed class ScriptKeywordProcessor : UnityEditor.AssetModificationProcessor
{
    public static void OnWillCreateAsset(string path)
    {
        path = path.Replace(".meta", "");
        int index = path.LastIndexOf(".");
        if (index < 0)
            return;

        string file = path.Substring(index);
        if (file != ".cs" && file != ".js")
            return;
        
        index = Application.dataPath.LastIndexOf("Assets");
        path = Application.dataPath.Substring(0, index) + path;
        if (!File.Exists(path))
            return;

        string fileContent = File.ReadAllText(path);
        fileContent = fileContent.Replace("#NAMESPACE#", Application.productName);
        File.WriteAllText(path, fileContent);
        AssetDatabase.Refresh();
    }
}