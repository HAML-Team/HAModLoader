using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using HAModLoaderAPI;

public static class LoadItems
{
    // Cache discovered items until inventory_ctr.Instance exists
    private static readonly Dictionary<string, inventory_ctr.new_inv_item> s_cached = new Dictionary<string, inventory_ctr.new_inv_item>(StringComparer.OrdinalIgnoreCase);
    private static bool s_registeredSceneCallback = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void RegisterHAItems()
    {
        try
        {
            // Full-scan fallback: scan all currently loaded assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in assemblies)
            {
                SafeProcessAssembly(a);
            }

            // If inventory controller already exists, merge immediately
            TryMergeIntoInventoryController();

            // Ensure we merge when a scene loads (inventory_ctr.Instance is set during Awake)
            if (!s_registeredSceneCallback)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                s_registeredSceneCallback = true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"LoadItems.RegisterHAItems() failed: {ex}");
        }
    }

    // Called by ModManager after it loads a mod assembly so we only process that assembly's types
    public static void ScanAssembly(Assembly asm)
    {
        SafeProcessAssembly(asm);
        TryMergeIntoInventoryController();
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
            Debug.LogError($"LoadItems: error processing assembly '{asm.FullName}': {ex}");
        }
    }

    private static void ProcessTypes(Type[] types)
    {
        if (types == null) return;
        foreach (var t in types)
        {
            if (t == null) continue;
            try
            {
                if (!typeof(HAItem).IsAssignableFrom(t) || t.IsAbstract) continue;

                var haObj = Activator.CreateInstance(t) as HAItem;
                if (haObj == null || string.IsNullOrEmpty(haObj.name)) continue;

                var entry = new inventory_ctr.new_inv_item
                {
                    name = haObj.name,
                    overwrite_name = haObj.overwrite_name ?? "",
                    inventory_sprite = haObj.inventory_sprite,
                    crafting_desc = haObj.crafting_desc ?? "",
                    crafting_ingredientA = haObj.crafting_ingredientA ?? "",
                    crafting_ingredientA_cnt = haObj.crafting_ingredientA_cnt,
                    crafting_ingredientB = haObj.crafting_ingredientB ?? "",
                    crafting_ingredientB_cnt = haObj.crafting_ingredientB_cnt,
                    // Map enum values by underlying int (the enums exist in both places)
                    type = (inventory_ctr.inv_type_t)(int)haObj.type,
                    world_obj = haObj.world_obj,
                    crafting_IAP_key_required = haObj.crafting_IAP_key_required ?? "",
                    max_stack = (inventory_ctr.stacksize)(int)haObj.max_stack,
                    equip_required_stat = haObj.equip_required_stat ?? "",
                    equip_required_stat_lvl = haObj.equip_required_stat_lvl,
                    market_cost = haObj.market_cost
                };

                // Cache by name (overwrites duplicates)
                s_cached[entry.name] = entry;
            }
            catch (Exception ex)
            {
                Debug.LogError($"LoadItems: failed to instantiate HAItem type '{t.FullName}': {ex}");
            }
        }
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryMergeIntoInventoryController();
    }

    private static void TryMergeIntoInventoryController()
    {
        if (inventory_ctr.Instance == null) return;

        foreach (var kv in s_cached)
        {
            try
            {
                // Ensure the runtime dictionary exists and add/replace entries
                inventory_ctr.Instance.new_inv_items_by_name[kv.Key] = kv.Value;
            }
            catch (Exception ex)
            {
                Debug.LogError($"LoadItems: failed to register item '{kv.Key}' into inventory_ctr: {ex}");
            }
        }

        // Optionally: you can also extend inventory_ctr.Instance.new_inv_items array
        // if you need items to appear in the serialized array (not required for lookups).
    }
}
