using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Utilities;
using Game_Stuff;
using UnityEngine;
using Logger = Core.Logging.Logger;

namespace Core.UI
{
    [CreateAssetMenu(fileName = "LocalizationManager", menuName = "Managers/Localization Manager")]
    public class Localization : SingletonManager<Localization>
    {
        [SerializeField] private string defaultTranslationsFilePath;
        [SerializeField] private string customTranslationsDirPath;

        private static Dictionary<string, string> _translations;

        private static string _csvResourcePath;
        private static string _translationsDir;
        
        public static IEnumerable<string> Languages
        {
            get
            {
                var defaultCsvAsset = Resources.Load<TextAsset>(_csvResourcePath);
                // Debug.Log(defaultCsvAsset);
                var csvLanguages = ParseCsv(defaultCsvAsset.text)[0];
                return csvLanguages.Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
            }
        }
        
        //Available landuages

        protected override void OnInit()
        {
            _translations = new Dictionary<string, string>();
            _csvResourcePath = defaultTranslationsFilePath;
            _translationsDir = customTranslationsDirPath;
        }

#if UNITY_EDITOR
        private void OnValidate() => Tools.ValidateFieldsForNull(this);
#endif


        /// <summary>
        /// Get a translation into the previously selected language by the name of the key
        /// </summary>
        /// <param name="key">Translation key literal</param>
        /// <returns>Text for the selected key in the current language</returns>
        public static string For(string key)
        {
            // var textKey = key.Split('_').Last();
            return _translations.ContainsKey(key) ? _translations[key] : "```Nofio```";
        }


        /// <summary>
        /// Load language asset from resources
        /// </summary>
        /// <param name="language">String language literal</param>
        public static void SetLanguage(string language)
        {
            if (string.IsNullOrEmpty(language))
                throw new Exception("Language is not selected!");

            var defaultCsvAsset = Resources.Load<TextAsset>(_csvResourcePath);
            if (defaultCsvAsset is null)
            {
                Logger.Error("Default localization asset cannot be found: " + _csvResourcePath);
                return;
            }

            _translations.Clear();

            string defaultCsv = defaultCsvAsset.text;
            var defaultCsvData = ParseCsv(defaultCsv);
            int langIndex = Array.IndexOf(defaultCsvData[0], language);
            
            if (langIndex == -1) // is custom language asset
            {
                string customCsvFilePath = Path.Combine(_translationsDir, language);
                // Debug.Log(customCsvFilePath + " path");
                var customCsvAsset = Resources.Load<TextAsset>(customCsvFilePath);
            
                if (customCsvAsset is null)
                {
                    Logger.Error("Localization asset cannot be found: /Resources/" + customCsvFilePath);
                    return;
                }
            
                string customCsv = customCsvAsset.text;
                var customCsvData = ParseCsv(defaultCsv);
            
                // language asset not exist -> use default
                if (customCsv is null)
                {
                    language = defaultCsvData[0][1];
                    SetLanguage(language); // recursion
                    Logger.Warn($"Language asset [{customCsvFilePath}] not found!");
                    return;
                }
            
                // select all translations that are not defined in the custom
                var combinedCsvData = defaultCsvData.Where(item =>
                    !customCsvData[0].Contains(item[0])).ToArray();
            
                AddTranslations(customCsvData, 1);
                AddTranslations(combinedCsvData, 1);
            }
            // is one of default languages 
            else AddTranslations(defaultCsvData, langIndex);
        }
        
        private static void AddTranslations(string[][] csvData, int langIndex)
        {
            // Add translations in the dictionary (and if not exist - add default language translation)
            for (int i = 1; i < csvData.GetLength(0) - 1; i++)
            {
                var line = csvData[i];
                string value = string.IsNullOrEmpty(line[langIndex]) ? line[1] : line[langIndex];
                _translations.Add(line[0], value);
            }
        }
        
        private static string[][] ParseCsv(string text) => text.Split('\n')
            .Select(line => line.Split(';').Select(item => item.Trim()).ToArray()).ToArray();
    }
}