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
        Assembly asm = types.FirstOrDefault()?.Assembly;
        foreach (var t in types)
        {
            if (t == null) continue;
            try
            {
                if (!typeof(HAItem).IsAssignableFrom(t) || t.IsAbstract) continue;

                var haObj = Activator.CreateInstance(t) as HAItem;
                if (haObj == null || string.IsNullOrEmpty(haObj.name)) continue;
                if (haObj.inventory_sprite == null)
                {
                    var autoSprite = new HASprite(haObj.name + ".png");
                    var tex = autoSprite.ToUnity();
                    if (tex == null)
                    {
                        autoSprite = new HASprite(haObj.name + ".jpg");
                    }
                    haObj.inventory_sprite = autoSprite;
                }
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
                s_cached[entry.name] = entry;
            }
            catch (Exception ex)
            {
                Debug.LogError($"LoadItems: failed to instantiate HAItem type '{t.FullName}': {ex}");
            }
        }
    }

    public static void ClearStaticCache()
    {
        s_cached.Clear();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryMergeIntoInventoryController();
    }

    private static void TryMergeIntoInventoryController()
    {
        if (inventory_ctr.Instance == null) return;

        var inv = inventory_ctr.Instance;

        // 1. Ensure dictionary exists
        if (inv.new_inv_items_by_name == null)
            inv.new_inv_items_by_name = new Dictionary<string, inventory_ctr.new_inv_item>(StringComparer.OrdinalIgnoreCase);

        // 2. Ensure array exists
        if (inv.new_inv_items == null)
            inv.new_inv_items = new inventory_ctr.new_inv_item[0];

        var list = inv.new_inv_items.ToList();

        foreach (var kv in s_cached)
        {
            var name = kv.Key;
            var item = kv.Value;

            // ---- DICTIONARY SAFE INSERT ----
            inv.new_inv_items_by_name[name] = item;

            // ---- ARRAY SAFE INSERT ----
            bool existsInArray = list.Any(x => x.name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (!existsInArray)
            {
                list.Add(item);
            }
        }

        inv.new_inv_items = list.ToArray();
        // Clear cache after merging
        s_cached.Clear();
    }
}
