using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Reflection;

namespace HAModLoaderAPI
{
    public class HAModLoaderAPI
    {
        internal readonly List<HAMod> _loadedMods = new List<HAMod>();
        public ReadOnlyCollection<HAMod> loadedMods => _loadedMods.AsReadOnly();

        public void RegisterMod(HAMod mod)
        {
            _loadedMods.Add(mod);
            Debug.Log($"[HAModLoaderAPI] Registered mod: {mod.GetType().Name}");
        }

        public GameObject SpawnPrefab(GameObject prefab, Vector3 pos)
        {
            var obj = Object.Instantiate(prefab, pos, Quaternion.identity);
            NotifyCreate(obj);
            return obj;
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
    }
}