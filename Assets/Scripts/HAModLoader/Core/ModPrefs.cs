using UnityEngine;

namespace HAModLoaderAPI
{
    public static class ModPrefs
    {
        // Helper to ensure keys are unique per mod
        private static string GetKey(HAMod mod, string key) => $"{mod.ModName}_{key}";

        public static void SetInt(HAMod mod, string key, int value) => PlayerPrefs.SetInt(GetKey(mod, key), value);
        public static int GetInt(HAMod mod, string key, int defaultValue = 0) => PlayerPrefs.GetInt(GetKey(mod, key), defaultValue);
        
        public static void SetString(HAMod mod, string key, string value) => PlayerPrefs.SetString(GetKey(mod, key), value);
        public static string GetString(HAMod mod, string key, string defaultValue = "") => PlayerPrefs.GetString(GetKey(mod, key), defaultValue);

        public static void SetFloat(HAMod mod, string key, float value) => PlayerPrefs.SetFloat(GetKey(mod, key), value);
        public static float GetFloat(HAMod mod, string key, float defaultValue = 0.0f) => PlayerPrefs.GetFloat(GetKey(mod, key), defaultValue);

        public static bool HasKey(HAMod mod, string key) => PlayerPrefs.HasKey(GetKey(mod, key));
        public static void DeleteKey(HAMod mod, string key) => PlayerPrefs.DeleteKey(GetKey(mod, key));
        
        public static void Save() => PlayerPrefs.Save();
    }
}