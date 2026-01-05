using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Reflection;
using System.IO;

namespace HAModLoaderAPI
{
    public class HAModLoaderAPI
    {
        internal readonly List<HAMod> _loadedMods = new List<HAMod>();
        public ReadOnlyCollection<HAMod> loadedMods => _loadedMods.AsReadOnly();

        public GameObject SpawnPrefab(GameObject prefab, Vector3 pos)
        {
            var obj = Object.Instantiate(prefab, pos, Quaternion.identity);
            NotifyCreate(obj);
            return obj;
        }

        public void RegisterMod(HAMod mod)
        {
            _loadedMods.Add(mod);
        }

        internal void NotifyCreate(GameObject obj)
        {
            foreach (var mod in loadedMods)
            {
                MethodInfo m = mod.GetType().GetMethod("OnCreate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (m != null)
                {
                    try { m.Invoke(mod, new object[] { obj }); }
                    catch (System.Exception e) { Debug.LogError($"[HAModLoader] Error in {mod.GetType().Name}.OnCreate: {e}"); }
                }
            }
        }

        public static string GetModDataPath(Assembly assembly, string fileName)
        {
            string dllPath = assembly.Location;
            // If Location is empty (happens on some Android setups), fallback to persistentDataPath
            if (string.IsNullOrEmpty(dllPath))
                dllPath = Path.Combine(Application.persistentDataPath, "Mods", assembly.GetName().Name + ".dll");

            string modDir = Path.GetDirectoryName(dllPath);
            string modName = Path.GetFileNameWithoutExtension(dllPath);

            return Path.Combine(modDir, modName, fileName);
        }
    }
}