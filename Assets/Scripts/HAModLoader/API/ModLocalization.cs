using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HAModLoaderAPI
{
    public static class ModLocalization
    {
        private static Dictionary<string, Dictionary<string, string>> _translations = new Dictionary<string, Dictionary<string, string>>();

        public static string CurrentLanguage { get; set; } = "English";

        public static void AddTranslation(string key, string value, string language = "English")
        {
            if (!_translations.ContainsKey(language))
                _translations[language] = new Dictionary<string, string>();

            if (_translations[language].ContainsKey(key)) _translations[language][key] = value;
            else _translations[language].Add(key, value);
        }

        public static void LoadFromFile(string path, string language = "English")
        {
            if (!File.Exists(path)) return;
            string[] lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                // Assuming format: Original Text~Translated Text
                var parts = line.Split(new char[] { '~' }, 2);
                if (parts.Length >= 2) AddTranslation(parts[0], parts[1], language);
            }
        }

        public static string Get(string key)
        {
            if (_translations.TryGetValue(CurrentLanguage, out var langDict))
                return langDict.TryGetValue(key, out string val) ? val : key;
            return key;
        }

        public static bool TryGet(string key, out string value)
        {
            value = null;
            if (_translations.TryGetValue(CurrentLanguage, out var langDict))
                return langDict.TryGetValue(key, out value);
            return false;
        }
    }
}