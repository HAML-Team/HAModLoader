using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using HAModLoaderAPI;

public class LoadAssets : MonoBehaviour
{
    private static string CacheRoot => Path.Combine(Application.temporaryCachePath, "ModCache");

    public static void InitializeAPI()
    {
        HAModLoaderAPI.Image.ResourceExtractor = ExtractResourceToCache;
        HAModLoaderAPI.Log.Info("[ModManager] Asset loading started.");
        ClearCache();
    }

    public static void LoadModAssets(Assembly assembly, string[] fileNames)
    {
        if (fileNames == null) return;
        foreach (string name in fileNames)
        {
            string path = ExtractResourceToCache(assembly, name);
            if (string.IsNullOrEmpty(path))
            {
                HAModLoaderAPI.Log.Warning($"[LoadAssets] Asset '{name}' not found for {assembly.GetName().Name}");
            }
        }
    }

    public static string ExtractResourceToCache(Assembly assembly, string resourceName)
    {
        string dllPath = assembly.Location;
        if (!string.IsNullOrEmpty(dllPath))
        {
            string modDir = Path.GetDirectoryName(dllPath);
            string modNameFolder = Path.GetFileNameWithoutExtension(dllPath);
            string externalPath = Path.Combine(modDir, modNameFolder, resourceName);
            if (File.Exists(externalPath)) return externalPath;
        }
        string modName = assembly.GetName().Name;
        string cacheDir = Path.Combine(CacheRoot, modName);
        if (!Directory.Exists(cacheDir)) Directory.CreateDirectory(cacheDir);
        string filePath = Path.Combine(cacheDir, resourceName);
        if (!File.Exists(filePath))
        {
            string[] allResources = assembly.GetManifestResourceNames();
            string actualName = Array.Find(allResources, r => r.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase));
            if (actualName == null) return null;
            using (Stream stream = assembly.GetManifestResourceStream(actualName))
            {
                if (stream == null) return null;
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    stream.CopyTo(fs);
                }
            }
        }
        return filePath;
    }

    public static string ExtractResourceToSpecificPath(Assembly assembly, string resourceName, string targetDir)
    {
        // Search mod folder first
        string dllPath = assembly.Location;
        if (!string.IsNullOrEmpty(dllPath))
        {
            string modDir = Path.GetDirectoryName(dllPath);
            string externalPath = Path.Combine(modDir, Path.GetFileNameWithoutExtension(dllPath), resourceName);
            if (File.Exists(externalPath)) 
            {
                string finalPath = Path.Combine(targetDir, resourceName);
                if (!File.Exists(finalPath)) File.Copy(externalPath, finalPath);
                return finalPath;
            }
        }

        // Fallback: Extract from Assembly to the specific target directory
        string filePath = Path.Combine(targetDir, resourceName);
        if (!File.Exists(filePath))
        {
            string[] allResources = assembly.GetManifestResourceNames();
            string actualName = Array.Find(allResources, r => r.EndsWith(resourceName,  StringComparison.OrdinalIgnoreCase));
            if (actualName == null) return null;

            using (Stream stream = assembly.GetManifestResourceStream(actualName))
            {
                if (stream == null) return null;
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    stream.CopyTo(fs);
                }
            }
        }
        return filePath;
    }

    public static void ClearCache()
    {
        if (Directory.Exists(CacheRoot))
        {
            try { Directory.Delete(CacheRoot, true); }
            catch (Exception e) { Log.Error($"[LoadAssets] Cache cleanup failed: {e.Message}"); }
        }
    }

    private void OnApplicationQuit() => ClearCache();
}