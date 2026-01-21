using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using HAModLoaderAPI;

public static class LoadCreatures
{
    // Cache discovered creatures until Loader exists
    private static readonly Dictionary<string, Sprite> s_cached =
        new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);

    private static bool s_registeredSceneCallback = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void RegisterHACreatures()
    {
        try
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in assemblies)
                SafeProcessAssembly(a);

            TryMergeIntoLoader();

            if (!s_registeredSceneCallback)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                s_registeredSceneCallback = true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"LoadCreatures.RegisterHACreatures() failed: {ex}");
        }
    }

    // Called by ModManager when a mod assembly is loaded
    public static void ScanAssembly(Assembly asm)
    {
        SafeProcessAssembly(asm);
        TryMergeIntoLoader();
    }

    private static void SafeProcessAssembly(Assembly asm)
    {
        if (asm == null) return;

        try
        {
            ProcessTypes(asm.GetTypes());
        }
        catch (ReflectionTypeLoadException ex)
        {
            ProcessTypes(ex.Types.Where(t => t != null).ToArray());
        }
        catch (Exception ex)
        {
            Debug.LogError($"LoadCreatures: error processing assembly '{asm.FullName}': {ex}");
        }
    }

    private static void ProcessTypes(Type[] types)
    {
        if (types == null) return;
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "Creatures");
        bool dirChecked = false;

        foreach (var t in types)
        {
            if (t == null) continue;
            try
            {
                if (!typeof(HACreature).IsAssignableFrom(t) || t.IsAbstract)
                    continue;
                var creature = Activator.CreateInstance(t) as HACreature;
                if (creature == null || string.IsNullOrEmpty(creature.name)) continue;

                if (!dirChecked)
                {
                    if (!Directory.Exists(streamingPath)) Directory.CreateDirectory(streamingPath);
                    dirChecked = true;
                }
                // Attempt to extract .tbc or .tbcx
                string[] extensions = { ".tbc", ".tbcx" };
                foreach (var ext in extensions)
                {
                    // ExtractResourceToCache is modified to accept a custom target path
                    LoadAssets.ExtractResourceToSpecificPath(t.Assembly, creature.name + ext, streamingPath);
                }
                s_cached[creature.name] = creature.sprite;
            }
            catch (Exception ex)
            {
                Debug.LogError($"LoadCreatures: failed to instantiate HACreature '{t.FullName}': {ex}");
            }
        }
    }

    public static void ClearStaticCache()
    {
        s_cached.Clear();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryMergeIntoLoader();
    }

    private static void TryMergeIntoLoader()
    {
        if (s_cached.Count == 0) return;

        var loader = UnityEngine.Object.FindObjectOfType<Loader>();
        if (loader == null) return;

        var names = loader.temp_static_names != null
            ? loader.temp_static_names.ToList()
            : new List<string>();

        var sprites = loader.creatureSprites != null
            ? loader.creatureSprites.ToList()
            : new List<Sprite>();

        var existingNames = new HashSet<string>(names, StringComparer.OrdinalIgnoreCase);
        bool modified = false;

        foreach (var kv in s_cached)
        {
            if (existingNames.Contains(kv.Key))
                continue;

            names.Add(kv.Key);
            sprites.Add(kv.Value);
            existingNames.Add(kv.Key);
            modified = true;
        }

        if (modified)
        {
            loader.temp_static_names = names.ToArray();
            loader.creatureSprites = sprites.ToArray();
        }

        s_cached.Clear();
    }
}
