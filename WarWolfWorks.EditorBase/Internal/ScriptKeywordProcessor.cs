using UnityEngine;
using UnityEditor;
using System.IO;
using WarWolfWorks.Utility;
using System.Text.RegularExpressions;
using static WarWolfWorks.EditorBase.Constants;

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

            string nameCut = originalPath.Substring(originalPath.LastIndexOf('/') + 1); // Retrieves the name of the file.

            bool isInterface = Hooks.Text.Is_InterfaceFile_Name.IsMatch(nameCut); //Checks if the file is an iterface
            fileContent = fileContent.Replace("#SCRIPTTYPE#", isInterface ? "interface" : "class");

            string @namespace = Application.productName.Replace(' ', '_');

            fileContent = fileContent.Replace("using static #NAMESPACE#", "using static " + @namespace);
            string NAMESPACEReplace = isInterface ? @namespace + ".Interfaces" : @namespace;
            fileContent = fileContent.Replace("#NAMESPACE#", NAMESPACEReplace);

            int fullNSCutoutIndexStart = originalPath.LastIndexOf(VN_SCRIPTKEYWORDPROCESSOR_FULLNAMESPACE_SCRIPTFOLDER)
                + VN_SCRIPTKEYWORDPROCESSOR_FULLNAMESPACE_SCRIPTFOLDER.Length;
            int fullNSCutoutIndexLength = originalPath.LastIndexOf('/');
            string fullNSPath = originalPath.Substring(fullNSCutoutIndexStart, fullNSCutoutIndexLength - fullNSCutoutIndexStart);
            string fullNSReplacer = @namespace + '.' + fullNSPath.Replace('/', '.');
            fileContent = fileContent.Replace("#FULLNAMESPACE#", fullNSReplacer);

            if (isInterface)
                fileContent = fileContent.Replace(" : MonoBehaviour", string.Empty);

            File.WriteAllText(path, fileContent);
            AssetDatabase.Refresh();
        }
    }
}