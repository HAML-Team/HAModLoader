using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HAModLoaderAPI
{
    public enum inv_type_t
    {
        none = 0,
        place_in_world = 1,
        armor = 2,
        helmet = 3,
        holdable = 4,
        tool = 5,
        consumable = 6
    }

    public enum stacksize
    {
        one = 0,
        one_thousand = 1
    }

    public abstract class HAItem
    {
        public string name;
        public string overwrite_name;
        public Sprite inventory_sprite;
        public string crafting_desc = "";
        public string crafting_ingredientA = "";
        public int crafting_ingredientA_cnt = 0;
        public string crafting_ingredientB = "";
        public int crafting_ingredientB_cnt = 0;
        public inv_type_t type;
        public GameObject world_obj;
        public string crafting_IAP_key_required;
        public stacksize max_stack;
        public string equip_required_stat;
        public int equip_required_stat_lvl;
        public int market_cost;
    }
}
