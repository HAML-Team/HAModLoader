using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using HAModLoaderAPI;

public class ModManager : MonoBehaviour
{
    private HAModHandler Handler;
    private HAModLoaderAPI.HAModLoaderAPI API;
    static string logFile;
    public static string LogFilePath => logFile;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        string logDir;
#if UNITY_ANDROID
        logDir = Path.Combine(Application.persistentDataPath, "Logs");
#elif UNITY_STANDALONE_WIN
        logDir = Path.Combine(Application.dataPath, "Logs");
#else
        logDir = Path.Combine(Application.streamingAssetsPath, "Logs");
#endif
#if UNITY_ANDROID
    logDir = $"/storage/emulated/0/Android/obb/{Application.identifier}/Logs";
#elif UNITY_STANDALONE_WIN
    logDir = Path.Combine(Application.dataPath, "../Logs");
#elif UNITY_STANDALONE_LINUX
    logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "../.local/share/HAModLoader/Logs");
#elif UNITY_STANDALONE_OSX
    logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "../Library/Application Support/HAModLoader/Logs");
#else
        logDir = Path.Combine(Application.streamingAssetsPath, "Logs");
#endif
        if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);
        logFile = Path.Combine(logDir, $"ModLoader_{DateTime.Now:yyyyMMdd_HHmmss}.log");
        File.WriteAllText(logFile, $"[ModManager] Log started at {DateTime.Now}\n");
        LoadAssets.ClearCache();
        HAModLoaderAPI.Log.Info("[ModManager] Initializing before scene load...");
        GameObject go = new GameObject("ModManager");
        UnityEngine.Object.DontDestroyOnLoad(go);
        go.AddComponent<ModManager>().LoadMods();
        Log.LogFile = logFile;
        Log.GetLoadedMods = () => ModRegistry.LoadedMods;
        LoadAssets.InitializeAPI();
        TranslationControl.ApiTranslationHook = HAModLoaderAPI.ModLocalization.TryGet;
        HAModLoaderAPI.Log.Info("[ModManager] Mod loader initialized successfully.");
    }

    void LoadMods()
    {
        Handler = new HAModHandler();
        API = new HAModLoaderAPI.HAModLoaderAPI();
        ModRegistry.APIInstance = API;
        Handler.Initialize(API);
        HAModLoaderAPI.Log.Info("[ModManager] Loading mods...");
        string[] modPaths = GetPlatformModPaths();
        HAModLoaderAPI.Log.Info($"[ModManager] Found {modPaths.Length} mod files.");
        if (modPaths.Length == 0)
            HAModLoaderAPI.Log.Warning("[ModManager] No mod files found.");
        foreach (string path in modPaths)
        {
            try
            {
                HAModLoaderAPI.Log.Info($"[ModManager] Found DLL: {path}");
                Assembly asm = Assembly.Load(File.ReadAllBytes(path));
                try
                {
                    LoadItems.ScanAssembly(asm);
                    HAModLoaderAPI.Log.Info($"[ModManager] ScanAssembly called for {Path.GetFileName(path)}");
                }
                catch (Exception exScan)
                {
                    HAModLoaderAPI.Log.Warning($"[ModManager] LoadItems.ScanAssembly failed for {path}: {exScan}");
                }
                var modType = asm.GetTypes().FirstOrDefault(t =>typeof(HAMod).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
                if (modType == null)
                {
                    HAModLoaderAPI.Log.Warning($"[ModManager] No HAMod found in {asm.FullName}");
                    continue;
                }
                var mod = (HAMod)Activator.CreateInstance(modType);
                ModRegistry.RegisterMod(mod);
                if (ModConfig.IsModEnabled(mod.ModName))
                {
                    Handler.SafeInvoke(mod, "OnModLoad");
                    HAModLoaderAPI.Log.Info($"[ModManager] Loaded mod: {mod.ModName}");
                }
                else
                {
                    HAModLoaderAPI.Log.Info($"[ModManager] Mod disabled in config: {mod.ModName}");
                }
            }
            catch (Exception ex)
            {
                HAModLoaderAPI.Log.Error($"[ModManager] Failed to load {path}\n{ex}");
            }
        }
        HAModLoaderAPI.Log.Info("[ModManager] All mods preloaded.");
    }

    private void UnloadMods()
    {
        HAModLoaderAPI.Log.Info("[ModManager] Unloading mods...");
        if (Handler != null && API != null)
        {
            foreach (var mod in API.loadedMods.Reverse())
            {
                Handler.SafeInvoke(mod, "OnModUnload");
                HAModLoaderAPI.Log.Info($"[ModManager] Unloaded mod: {mod.ModName}");
            }
        }
        if (Handler != null)
        {
            SceneManager.sceneLoaded -= Handler.OnSceneLoaded;
        }
        if (ModRegistry.APIInstance != null)
        {
            ModRegistry.APIInstance = null;
        }
        LoadItems.ClearStaticCache();
        LoadCreatures.ClearStaticCache();
        LoadAssets.ClearCache();
        Handler = null;
        API = null;
        HAModLoaderAPI.Log.Info("[ModManager] Mods unloaded.");
    }

    private void OnApplicationQuit()
    {
        UnloadMods();
    }

    public void Reload()
    {
        HAModLoaderAPI.Log.Info("[ModManager] Reloading mods...");
        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        UnloadMods();
        CleanDontDestroyOnLoad();

        try
        {
            LoadMods();
        }
        catch (Exception ex)
        {
            HAModLoaderAPI.Log.Error($"[ModManager] LoadMods failed during reload: {ex}");
        }

        HAModLoaderAPI.Log.Info("[ModManager] Loading scene 'FAST START'...");
        AsyncOperation op = SceneManager.LoadSceneAsync("FAST START", LoadSceneMode.Single);
        if (op == null)
        {
            HAModLoaderAPI.Log.Error("[ModManager] Failed to load scene 'FAST START'");
            yield break;
        }

        while (!op.isDone)
            yield return null;

        HAModLoaderAPI.Log.Info("[ModManager] Scene 'FAST START' loaded.");
    }

    private void CleanDontDestroyOnLoad()
    {
        GameObject temp = new GameObject("DDOLCleaner");
        DontDestroyOnLoad(temp);
        foreach (GameObject root in temp.scene.GetRootGameObjects())
        {
            if (root == temp) continue;
            if (root == this.gameObject) continue;
            if (root.name == "ModManager") continue;
            HAModLoaderAPI.Log.Info($"[ModManager] Cleaning DDOL: Destroying {root.name}");
            Destroy(root);
        }
        Destroy(temp);
    }

    public static string ModsDir
    {
        get
        {
            string dir;
#if UNITY_ANDROID
            dir = $"/storage/emulated/0/Android/obb/{Application.identifier}/Mods";
#elif UNITY_STANDALONE_WIN
            dir = Path.Combine(Application.dataPath, "../Mods");
#elif UNITY_STANDALONE_LINUX
            dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "../.local/share/HAModLoader/Mods");
#elif UNITY_STANDALONE_OSX
            dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "../Library/Application Support/HAModLoader/Mods");
#else
            dir = Path.Combine(Application.streamingAssetsPath, "Mods");
#endif
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return dir;
        }
    }

    static string[] GetPlatformModPaths()
    {
        return Directory.GetFiles(ModsDir, "*.dll");
    }
}
