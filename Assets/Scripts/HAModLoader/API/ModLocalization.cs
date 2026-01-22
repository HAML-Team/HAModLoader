using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HAModLoaderAPI
{
    public static class ModLocalization
    {
        private static Dictionary<string, string> _translations = new Dictionary<string, string>();

        public static void AddTranslation(string key, string value)
        {
            if (_translations.ContainsKey(key)) _translations[key] = value;
            else _translations.Add(key, value);
        }

        public static void LoadFromFile(string path)
        {
            if (!File.Exists(path)) return;
            string[] lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                // Assuming format: Original Text~Translated Text
                var parts = line.Split('~');
                if (parts.Length >= 2) AddTranslation(parts[0], parts[1]);
            }
        }

        public static string Get(string key) => _translations.TryGetValue(key, out string val) ? val : key;

        public static bool TryGet(string key, out string value)
        {
            return _translations.TryGetValue(key, out value);
        }
    }
}