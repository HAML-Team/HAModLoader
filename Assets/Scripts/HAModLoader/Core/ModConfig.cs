using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public static class ModConfig
{
    [System.Serializable]
    private class ConfigData
    {
        public List<ModEntry> mods = new List<ModEntry>();
    }

    [System.Serializable]
    private class ModEntry
    {
        public string name;
        public bool enabled;
    }

    private static ConfigData data;
    private static string ConfigPath => Path.Combine(ModManager.ModsDir, "config.js");

    private static void EnsureLoaded()
    {
        if (data != null) return;
        data = new ConfigData();
        
        if (File.Exists(ConfigPath))
        {
            try
            {
                string json = File.ReadAllText(ConfigPath);
                data = JsonUtility.FromJson<ConfigData>(json) ?? new ConfigData();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[ModConfig] Failed to load config: {e}");
                data = new ConfigData();
            }
        }
    }

    public static bool IsModEnabled(string modName)
    {
        EnsureLoaded();
        var entry = data.mods.FirstOrDefault(m => m.name == modName);
        // Default to true if not found in config
        return entry == null || entry.enabled;
    }

    public static void SetModEnabled(string modName, bool enabled)
    {
        EnsureLoaded();
        var entry = data.mods.FirstOrDefault(m => m.name == modName);
        if (entry != null)
        {
            entry.enabled = enabled;
        }
        else
        {
            data.mods.Add(new ModEntry { name = modName, enabled = enabled });
        }
        
        File.WriteAllText(ConfigPath, JsonUtility.ToJson(data, true));
    }
}