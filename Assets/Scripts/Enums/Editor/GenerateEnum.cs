//Diadrasis ©2023 - Stathis Georgiou
using UnityEditor;
using System.IO;

namespace Diadrasis.Rethymno 
{

#if UNITY_EDITOR
    

    public class GenerateEnum
    {
        [MenuItem("Tools/GenerateEnum")]
        public static void Go()
        {
            string _namespace = typeof(GenerateEnum).Namespace;
            string enumName = "MyEnum";
            string[] enumEntries = { "Foo", "Goo", "Hoo" };
            string filePathAndName = "Assets/Scripts/Enums/" + enumName + ".cs"; //The folder Scripts/Enums/ is expected to exist

            using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
            {
                streamWriter.WriteLine("namespace " + _namespace);
                streamWriter.WriteLine("{");
                streamWriter.WriteLine("public enum " + enumName);
                streamWriter.WriteLine("{");
                for (int i = 0; i < enumEntries.Length; i++)
                {
                    streamWriter.WriteLine("\t" + enumEntries[i] + ",");
                }
                streamWriter.WriteLine("}");
                streamWriter.WriteLine("}");
            }
            AssetDatabase.Refresh();
        }
    }
#endif

}
