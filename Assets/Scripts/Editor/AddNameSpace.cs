using UnityEditor;
using UnityEngine;

//Unity's default templates can be found under your Unity installation's directory
//in Editor\Data\Resources\ScriptTemplates for Windows and /Contents/Resources/ScriptTemplates for OSX.
//And open the file 81-C# Script-NewBehaviourScript.cs.txt
//And make the following change:
//namespace #NAMESPACE# {

//hub/editor/[unityversion]/editor/resources/scripttemplates/81-C# Script-NewBehaviourScript.cs.txt

public class AddNameSpace : UnityEditor.AssetModificationProcessor
{

    public static void OnWillCreateAsset(string path)
    {
        path = path.Replace(".meta", "");
        int index = path.LastIndexOf(".");
        if (index < 0)
            return;

        string file = path.Substring(index);
        if (file != ".cs" && file != ".js" && file != ".boo")
            return;

        index = Application.dataPath.LastIndexOf("Assets");
        path = Application.dataPath.Substring(0, index) + path;
        if (!System.IO.File.Exists(path))
            return;

        string fileContent = System.IO.File.ReadAllText(path);
        fileContent = fileContent.Replace("#AUTHOR#", "Diadrasis ©2023 - Stathis Georgiou");
        fileContent = fileContent.Replace("#NAMESPACE#", "Diadrasis.Rethymno");

        System.IO.File.WriteAllText(path, fileContent);
        AssetDatabase.Refresh();
    }

}

