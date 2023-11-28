//Diadrasis ©2023 - Stathis Georgiou
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Diadrasis.Rethymno 
{
	[CreateAssetMenu(fileName = "Terms_Database", menuName = "Data/New Terms Database")]
	[Serializable]
	public class JsonTermsDatabase : ScriptableObject
	{
        [Header("[---Give a unique name for the runtime created enum variable---]")]
        public string EnumFieldName;
        [Header("--------------------------")]

		[SerializeField]
		public TextAsset[] jsonData;

		public List<cTerm> terms = new List<cTerm>();

        public void ReadJsonFilesOverridde()
        {
            terms.Clear();
            if (IsJsonMissing()) return;
            string _result = string.Empty;
            //INFOS
            foreach (TextAsset json in jsonData)
            {
                _result = JsonHelper.FixJson(json.text);
                terms.AddRange(JsonHelper.FromJson<cTerm>(_result).ToList());
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
                CreateNewEnum();
#endif
        }

#if UNITY_EDITOR
        //a new enum variable is created to help us autocomplete term keys in code
        void CreateNewEnum()
        {
            List<string> keys = new List<string>();
            terms.ForEach(b => keys.Add(b.info_key));
            string _namespace = typeof(JsonTermsDatabase).Namespace;
            EditorUtils.CreateEnumsFile(EnumFieldName, _namespace, keys);
        }

        [ContextMenu("CreateCSV")]
        public void CreateCSV()
        {
            string path = Path.Combine(Application.persistentDataPath, "Database/");
            if(!Directory.Exists(path))Directory.CreateDirectory(path);
            path = Path.Combine(path, "newFile.csv");
            Debug.Log(path);
            TextWriter tw = new StreamWriter(path, false, System.Text.Encoding.UTF8);
            tw.WriteLine("KEY-GREEK-ENGLISH-FRANCE-GERMAN-RUSSIAN");
            //tw = new StreamWriter(path, true);

            for(int i = 0; i < terms.Count; i++)
            {
                cTerm term = terms[i];
                List<cLocalizedText> texts = term.info_text.ToList();
                string line = term.info_key;
                if (texts.Count > 0)
                {
                    cLocalizedText lt = texts.Find(b => b.key == "gr");
                    if (lt != null) { line += "-" + lt.text.Trim().Replace("\n","").Replace("\r", ""); } else { line += "," + ""; }
                    lt = texts.Find(b => b.key == "en");
                    if (lt != null) { line += "-" + lt.text.Trim().Replace("\n", "").Replace("\r", ""); } else { line += "," + ""; }
                    lt = texts.Find(b => b.key == "fr");
                    if (lt != null) { line += "-" + lt.text.Trim().Replace("\n", "").Replace("\r", ""); } else { line += "," + ""; }
                    lt = texts.Find(b => b.key == "de");
                    if (lt != null) { line += "-" + lt.text.Trim().Replace("\n", "").Replace("\r", ""); } else { line += "," + ""; }
                    lt = texts.Find(b => b.key == "ru");
                    if (lt != null) { line += "-" + lt.text.Trim().Replace("\n", "").Replace("\r", ""); } else { line += "," + ""; }

                    tw.WriteLine(line);
                }

            }

            tw.Close();

            bool openInsidesOfFolder = false;
            string p = Path.Combine(Application.persistentDataPath, "Database/");

            string folderPath = p.Replace(@"/", @"\");
            if (Directory.Exists(folderPath)) { openInsidesOfFolder = true; }
            try { System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + folderPath); }
            catch (System.ComponentModel.Win32Exception e) { e.HelpLink = ""; }

        }
#endif

        /// <summary>
        /// returns the text for current language
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="localizedTexts"></param>
        /// <returns></returns>
        public cLocalizedText[] GetText(string _key)
		{
			cTerm term = terms.Find(b => b.info_key == _key);
			if (term == null) return null;
			return term.info_text;
		}

		public bool IsJsonMissing()
		{
			return jsonData != null && jsonData.Length <= 0;
		}

	}

}
