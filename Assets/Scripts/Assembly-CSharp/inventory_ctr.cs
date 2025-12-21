using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inventory_ctr : MonoBehaviour
{
	[Serializable]
	public struct chest_loot
	{
		public string item;

		public rarity_t rarity;
	}

	public enum rarity_t
	{
		common = 0,
		super_rare = 1
	}

	[Serializable]
	public struct new_inv_item
	{
		public string name;

		public string overwrite_name;

		public Sprite inventory_sprite;

		public string crafting_desc;

		public string crafting_ingredientA;

		public int crafting_ingredientA_cnt;

		public string crafting_ingredientB;

		public int crafting_ingredientB_cnt;

		public inv_type_t type;

		public GameObject world_obj;

		public string crafting_IAP_key_required;

		public stacksize max_stack;

		public string equip_required_stat;

		public int equip_required_stat_lvl;

		public int market_cost;
	}

	public enum stacksize
	{
		one = 0,
		one_thousand = 1
	}

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

	public enum chest_type
	{
		NONE = 0,
		basket = 1,
		personal_chest = 2,
		gold_chest = 3,
		titanium_chest = 4
	}

	public enum button_state
	{
		DONE_USING_TOOL = 0,
		CANCEL_PLACE_OBJ = 1,
		DONE_SITTING_CHAIR = 2,
		CAST_PROJECTILE = 3
	}

	public enum ptype
	{
		firstPage = 0,
		secondPage = 1,
		eitherPage = 2
	}

	public enum tab
	{
		left = 0,
		right = 1
	}

	public static inventory_ctr Instance;

	public static int total_INV_spaces = 35;

	public static int n_cols = 5;

	public static int n_rows = 3;

	private int n_slots_per_page = 15;

	[HideInInspector]
	public string[] inventory_objType;

	[HideInInspector]
	public int[] inventory_count;

	[HideInInspector]
	public string[] inventory_special;

	[HideInInspector]
	public string[] container_objType;

	[HideInInspector]
	public int[] container_count;

	[HideInInspector]
	public string[] container_special;

	public new_inv_item[] new_inv_items;

	public Dictionary<string, new_inv_item> new_inv_items_by_name = new Dictionary<string, new_inv_item>();

	public string[] default_crafting;

	public string[] anvil_crafting;

	public string[] cauldron_crafting;

	public string[] craftingTable_crafting;

	public string[] NPC_seller_pineappleShark;

	public string[] NPC_seller_cupcakeCrab_ = new string[0];

	public string[] NPC_seller_octokitty;

	public string[] MysteryBoxRewards;

	public List<string> NPC_buyBack_pineappleShark;

	public List<string> NPC_buyBack_octoKitty;

	public chest_loot[] possible_gold_chest_loot;

	public chest_loot[] possible_titanium_chest_loot;

	public Text text_loot_respawn_in;

	private looted_chest curr_looted_chest;

	[HideInInspector]
	public Dictionary<int, looted_chest> already_looted_gold_chests = new Dictionary<int, looted_chest>();

	private int hand_index = 15;

	private int hat_index = 16;

	private int body_index = 17;

	private int trash_index = -2;

	private int page_1_index = -10;

	private int page_2_index = -11;

	public GameObject equip_hand_slot;

	public GameObject equip_hat_slot;

	public GameObject equip_body_slot;

	public Sprite blank_hand_sprite;

	public Sprite blank_hat_sprite;

	public Sprite blank_body_sprite;

	public Sprite empty_sprite;

	public chest_type CHEST_TYPE;

	private Vector2 INV_start_slots = new Vector2(-377f, 191f);

	private float INV_slot_spacing = 190f;

	private Vector2 CRAFT_start_slots = new Vector2(-307f, -55f);

	private float CRAFT_spacing = 290f;

	private int drag_threshold = 15;

	public Transform slot_parent;

	public Transform craft_slot_parent;

	private List<crafting_slot> instantiated_crafting_slots = new List<crafting_slot>();

	[HideInInspector]
	public List<GameObject> instantiated_inv_slots = new List<GameObject>();

	public GameObject craft_slot_type;

	public GameObject inv_slot_type;

	private List<string> buybacks = new List<string>();

	public GameObject crafting_page_left;

	public GameObject crafting_page_right;

	private string trying_to_craft;

	private bool allowClose = true;

	public GameObject type_particle_mixer;

	private List<int> potential_mixer_indices = new List<int>();

	public GameObject inventory_lock;

	private GameObject mouse_down_slot;

	private Sprite mouse_down_sprite;

	[HideInInspector]
	public int unique_id_iterator;

	[HideInInspector]
	public button_state done_button_context;

	public GameObject DONE_placing_button;

	public GameObject invPAGE1;

	public GameObject invPAGE2;

	[HideInInspector]
	public List<int> NPC_chests = new List<int>();

	private Vector2 initial_press;

	[HideInInspector]
	public bool is_dragging;

	private bool test_for_dragging;

	private int refresh_loot_counter_i;

	public Camera shop_cam;

	public GameObject drag_image;

	public Transform for_slots;

	private Vector2 standard_slots_position;

	public GameObject inventory_crafting_screen;

	public GameObject gui_loot_respawn_time;

	private bool is_open;

	[HideInInspector]
	public bool INTERACT_W_CONTAINER;

	private int current_inv_pageNo = 1;

	public GameObject pageSwitchers;

	public Image invp1;

	public Image invp2;

	public Sprite[] coin_sprites;

	private bool one_sellable;

	public Color inv_page_sel;

	public Color inv_page_desel;

	public GameObject transfer_to;

	public GameObject transfer_from;

	[HideInInspector]
	public int open_container_id;

	public Text crafting_tab_text;

	public Text inventory_tab_text;

	private int craft_PAGE;

	public Text text_take_from_basket;

	public Text text_put_in_basket;

	private bool is_sale;

	private string[] craftList;

	public GameObject double_tap_text;

	public GameObject trash_bin;

	private GameObject prev_mousedown_slot;

	private bool PLACING_OBJECT_OR_USING_TOOL;

	private string ITEM_USING = "";

	public GameObject angular;

	public GameObject click_to_place;

	public Text click_to_place_txt;

	public Text click_to_place_BUTTON_text;

	private int sell_index;

	private List<string> curr_buyback_list;

	private bool check_double_click;

	private IEnumerator W_DOUBLE_CLICK;

	public Color tab_selected_color;

	public Color tab_deselected_color;

	public GameObject inventory_Tab;

	public GameObject crafting_Tab;

	public Image inv_tab_button;

	public Image craft_tab_button;

	private tab CURRENT_TAB;

	private IEnumerator CASCADE_CRAFT_BUTS;

	public void open_gold_chest(int container_id, bool is_titanium, Vector3 position)
	{
		text_loot_respawn_in.text = "???";
		if (!already_looted_gold_chests.ContainsKey(container_id))
		{
			int num = 3;
			string[] array = new string[num];
			int[] array2 = new int[num];
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			chest_loot[] array3 = ((!is_titanium) ? possible_gold_chest_loot : possible_titanium_chest_loot);
			chest_loot[] array4 = array3;
			for (int i = 0; i < array4.Length; i++)
			{
				chest_loot chest_loot = array4[i];
				if (chest_loot.rarity == rarity_t.common)
				{
					list.Insert(UnityEngine.Random.Range(0, list.Count), chest_loot.item);
				}
				else if (chest_loot.rarity == rarity_t.super_rare)
				{
					list2.Insert(UnityEngine.Random.Range(0, list2.Count), chest_loot.item);
				}
			}
			if (UnityEngine.Random.value < 0.066f && is_titanium)
			{
				array[0] = list2[0];
				array2[0] = 1;
				array[1] = list[0];
				array2[1] = 1;
				array[2] = "";
				array2[2] = 0;
			}
			else
			{
				array[0] = list[0];
				array2[0] = 1;
				array[1] = list[1];
				array2[1] = 1;
				array[2] = "";
				array2[2] = 0;
			}
			if (UnityEngine.Random.value < 0.5f)
			{
				int num2 = UnityEngine.Random.Range(0, 1);
				array[num2] = "Coins";
				if (is_titanium)
				{
					array2[num2] = UnityEngine.Random.Range(60, 200);
				}
				else
				{
					array2[num2] = UnityEngine.Random.Range(20, 35);
				}
			}
			string grouping = "basket" + container_id;
			for (int j = 0; j < num; j++)
			{
				PlayerData.Instance.SetSlotString("stored-" + j, array[j], grouping);
				PlayerData.Instance.SetSlotInt("count-" + j, array2[j], grouping);
				PlayerData.Instance.SetSlotString("special-" + j, "", grouping);
			}
			for (int k = num; k < n_slots_per_page; k++)
			{
				PlayerData.Instance.SetSlotString("stored-" + k, "", grouping);
				PlayerData.Instance.SetSlotInt("count-" + k, 0, grouping);
				PlayerData.Instance.SetSlotString("special-" + k, "", grouping);
			}
			already_looted_gold_chests.Add(container_id, new looted_chest(container_id, true));
			save_looted_chests();
			NewBiomeControl.Instance.RebuildChunkAt(position);
		}
		curr_looted_chest = already_looted_gold_chests[container_id];
		UpdateLootedChestList();
	}

	private void load_looted_chests()
	{
		int slotInt = PlayerData.Instance.GetSlotInt("n_looted_chests", PlayerData.grouping_t.loot);
		for (int i = 0; i < slotInt; i++)
		{
			int slotInt2 = PlayerData.Instance.GetSlotInt("looted-" + i + "-container_id", PlayerData.grouping_t.loot);
			int slotInt3 = PlayerData.Instance.GetSlotInt("looted-" + i + "-year", PlayerData.grouping_t.loot);
			int slotInt4 = PlayerData.Instance.GetSlotInt("looted-" + i + "-month", PlayerData.grouping_t.loot);
			int slotInt5 = PlayerData.Instance.GetSlotInt("looted-" + i + "-day", PlayerData.grouping_t.loot);
			int slotInt6 = PlayerData.Instance.GetSlotInt("looted-" + i + "-hour", PlayerData.grouping_t.loot);
			int slotInt7 = PlayerData.Instance.GetSlotInt("looted-" + i + "-minute", PlayerData.grouping_t.loot);
			int slotInt8 = PlayerData.Instance.GetSlotInt("looted-" + i + "-second", PlayerData.grouping_t.loot);
			looted_chest looted_chest2 = new looted_chest(slotInt2, false);
			looted_chest2.respawns_at = new DateTime(slotInt3, slotInt4, slotInt5, slotInt6, slotInt7, slotInt8);
			already_looted_gold_chests.Add(slotInt2, looted_chest2);
		}
	}

	private void save_looted_chests()
	{
		PlayerData.Instance.SetSlotInt("n_looted_chests", already_looted_gold_chests.Count, PlayerData.grouping_t.loot);
		int num = 0;
		foreach (KeyValuePair<int, looted_chest> already_looted_gold_chest in already_looted_gold_chests)
		{
			looted_chest value = already_looted_gold_chest.Value;
			PlayerData.Instance.SetSlotInt("looted-" + num + "-container_id", value.container_id, PlayerData.grouping_t.loot);
			DateTime respawns_at = value.respawns_at;
			PlayerData.Instance.SetSlotInt("looted-" + num + "-year", respawns_at.Year, PlayerData.grouping_t.loot);
			PlayerData.Instance.SetSlotInt("looted-" + num + "-month", respawns_at.Month, PlayerData.grouping_t.loot);
			PlayerData.Instance.SetSlotInt("looted-" + num + "-day", respawns_at.Day, PlayerData.grouping_t.loot);
			PlayerData.Instance.SetSlotInt("looted-" + num + "-hour", respawns_at.Hour, PlayerData.grouping_t.loot);
			PlayerData.Instance.SetSlotInt("looted-" + num + "-minute", respawns_at.Minute, PlayerData.grouping_t.loot);
			PlayerData.Instance.SetSlotInt("looted-" + num + "-second", respawns_at.Second, PlayerData.grouping_t.loot);
			num++;
		}
	}

	private void UpdateLootedChestList()
	{
		if (!MultiplayerControl.Instance.last_known_real_time_set)
		{
			return;
		}
		DateTime dateTime = MultiplayerControl.Instance.last_known_real_time.AddSeconds((int)((float)MultiplayerControl.Instance.last_known_real_time_stopwatch.ElapsedMilliseconds / 1000f));
		if (is_open && (CHEST_TYPE == chest_type.gold_chest || CHEST_TYPE == chest_type.titanium_chest))
		{
			int num = (int)(curr_looted_chest.respawns_at - dateTime).TotalHours;
			int num2 = (int)(curr_looted_chest.respawns_at - dateTime).TotalMinutes - 60 * num;
			int num3 = (int)(curr_looted_chest.respawns_at - dateTime).TotalSeconds - 3600 * num - 60 * num2;
			text_loot_respawn_in.text = num + "h " + num2 + "m " + num3 + "s";
		}
		bool flag = false;
		List<looted_chest> list = new List<looted_chest>();
		foreach (KeyValuePair<int, looted_chest> already_looted_gold_chest in already_looted_gold_chests)
		{
			looted_chest value = already_looted_gold_chest.Value;
			if ((int)(value.respawns_at - dateTime).TotalSeconds <= 0)
			{
				list.Add(value);
				flag = true;
			}
		}
		foreach (looted_chest item in list)
		{
			already_looted_gold_chests.Remove(item.container_id);
		}
		if (flag)
		{
			save_looted_chests();
		}
	}

	public void reset_inv()
	{
		inventory_objType = new string[total_INV_spaces];
		inventory_count = new int[total_INV_spaces];
		inventory_special = new string[total_INV_spaces];
		for (int i = 0; i < total_INV_spaces; i++)
		{
			inventory_objType[i] = "";
			inventory_count[i] = 0;
			inventory_special[i] = "";
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		standard_slots_position = slot_parent.transform.localPosition;
		reset_inv();
		create_buttons_and_slots();
		invp1.color = inv_page_sel;
		invp2.color = inv_page_desel;
		for (int i = 0; i < new_inv_items.Length; i++)
		{
			new_inv_items_by_name.Add(new_inv_items[i].name, new_inv_items[i]);
		}
	}

	public void crafting_inc_page(int dir)
	{
		switch (dir)
		{
		case 1:
			crafting_page_right.GetComponent<Animation>().Stop();
			crafting_page_right.GetComponent<Animation>().Play();
			break;
		case -1:
			crafting_page_left.GetComponent<Animation>().Stop();
			crafting_page_left.GetComponent<Animation>().Play();
			break;
		}
		if (page_exists(dir))
		{
			craft_PAGE += dir;
			refresh_craft_slots();
		}
		crafting_page_left.SetActive(page_exists(-1));
		crafting_page_right.SetActive(page_exists(1));
		NewAudioControl.Instance.play_generic_click();
		do_cascade_craft_buts(dir);
	}

	public void show_DONE_button(string str, button_state context)
	{
		DONE_placing_button.SetActive(true);
		click_to_place_BUTTON_text.text = str;
		done_button_context = context;
	}

	public void load_inventory()
	{
		for (int i = 0; i < total_INV_spaces; i++)
		{
			string slotString = PlayerData.Instance.GetSlotString("inventory" + i, PlayerData.grouping_t.the_inventory);
			inventory_objType[i] = slotString;
			string slotString2 = PlayerData.Instance.GetSlotString("inventorySpecial" + i, PlayerData.grouping_t.the_inventory);
			inventory_special[i] = slotString2;
			int num = PlayerData.Instance.GetSlotInt("inventoryCount" + i, PlayerData.grouping_t.the_inventory);
			if (slotString != "" && num == 0)
			{
				num = 1;
			}
			inventory_count[i] = num;
		}
		equip_hand();
		equip_hat();
		equip_armor();
		load_looted_chests();
	}

	public void equip_hand()
	{
		string text = inventory_objType[hand_index];
		if (text != "")
		{
			GameController.Instance.player.GetComponent<creatureScript>().myCreatureModel.GetComponent<creatureModel>().equip_hold_item(new_inv_items_by_name[text].world_obj);
		}
	}

	public void equip_hat()
	{
		string text = inventory_objType[hat_index];
		if (text != "")
		{
			NewGameControl.Instance.ApplyHat(GameController.Instance.player.GetComponent<creatureScript>().myCreatureModel, new_inv_items_by_name[text].world_obj);
		}
	}

	public void equip_armor()
	{
		string text = inventory_objType[body_index];
		if (text != "")
		{
			NewGameControl.Instance.ApplyArmor(GameController.Instance.player.GetComponent<creatureScript>().myCreatureModel, new_inv_items_by_name[text].world_obj);
		}
	}

	public void ACCEPT_craft()
	{
		if (is_sale)
		{
			deduct_coins(new_inv_items_by_name[trying_to_craft].market_cost);
			try_give_item(trying_to_craft, 1, GameController.Instance.player.transform.position, "", false);
			SaveAllInventory();
			StartCoroutine(delayed_succeed_buy());
		}
		else
		{
			press_inventory_tab();
			allowClose = false;
			StartCoroutine(mix_items());
		}
	}

	private IEnumerator delayed_succeed_buy()
	{
		yield return new WaitForSeconds(0.13f);
		Shop_positioner.Instance.activate_popup(Shop_positioner.config.OKAY_button, "<color=#eeee55>" + trying_to_craft + "</color>\nRecieved " + trying_to_craft + "!", new Color(0.45f, 0.92f, 1f), new_inv_items_by_name[trying_to_craft].inventory_sprite, new Color(0.26f, 0.6f, 0.79f));
		Shop_positioner.Instance.suppress_models = true;
		Shop_positioner.Instance.temp_hide_close = true;
		Shop_positioner.Instance.do_blobble_gem_count = false;
	}

	private void deduct_coins(int amount)
	{
		int num = amount;
		if (PlayerData.Instance.GetGlobalInt("big_inv") == 1)
		{
			for (int num2 = 20 + n_slots_per_page - 1; num2 >= 20; num2--)
			{
				if (inventory_objType[num2] == "Coins")
				{
					if (num <= inventory_count[num2])
					{
						inventory_count[num2] -= num;
						return;
					}
					num -= inventory_count[num2];
					inventory_count[num2] = 0;
					inventory_objType[num2] = "";
				}
			}
		}
		for (int num3 = n_slots_per_page - 1; num3 >= 0; num3--)
		{
			if (inventory_objType[num3] == "Coins")
			{
				if (num <= inventory_count[num3])
				{
					inventory_count[num3] -= num;
					break;
				}
				num -= inventory_count[num3];
				inventory_count[num3] = 0;
				inventory_objType[num3] = "";
			}
		}
	}

	private IEnumerator mix_items()
	{
		yield return new WaitForSeconds(0.1f);
		int result_goto_index = potential_mixer_indices[0];
		if (current_inv_pageNo == 1 && result_goto_index >= 20 && result_goto_index <= 34)
		{
			inv_page_switch(2);
		}
		else if (current_inv_pageNo == 2 && result_goto_index >= 0 && result_goto_index <= n_slots_per_page)
		{
			inv_page_switch(1);
		}
		int slot_instance_index = -1;
		if (result_goto_index >= 15 && result_goto_index <= 19)
		{
			slot_instance_index = result_goto_index;
		}
		else if (current_inv_pageNo == 1)
		{
			slot_instance_index = result_goto_index;
		}
		else if (current_inv_pageNo == 2)
		{
			slot_instance_index = result_goto_index - 20;
		}
		List<GameObject> mixer_parts = new List<GameObject>();
		for (int i = 0; i < potential_mixer_indices.Count; i++)
		{
			if (potential_mixer_indices[i] == result_goto_index)
			{
				continue;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(type_particle_mixer);
			gameObject.transform.parent = slot_parent;
			gameObject.transform.localRotation = Quaternion.identity;
			if (current_inv_pageNo == 1)
			{
				if (potential_mixer_indices[i] >= 20)
				{
					gameObject.transform.localPosition = invPAGE2.transform.localPosition;
				}
				else
				{
					gameObject.transform.localPosition = instantiated_inv_slots[potential_mixer_indices[i]].transform.localPosition;
				}
			}
			else if (current_inv_pageNo == 2)
			{
				if (potential_mixer_indices[i] <= n_slots_per_page)
				{
					gameObject.transform.localPosition = invPAGE1.transform.localPosition;
				}
				else
				{
					gameObject.transform.localPosition = instantiated_inv_slots[potential_mixer_indices[i] - 20].transform.localPosition;
				}
			}
			gameObject.transform.localScale = Vector3.one;
			mixer_parts.Add(gameObject);
			gameObject.GetComponent<Image>().sprite = new_inv_items_by_name[inventory_objType[potential_mixer_indices[i]]].inventory_sprite;
			if (potential_mixer_indices[i] == hand_index)
			{
				instantiated_inv_slots[hand_index].GetComponent<Image>().sprite = blank_hand_sprite;
			}
			else if (potential_mixer_indices[i] == hat_index)
			{
				instantiated_inv_slots[hat_index].GetComponent<Image>().sprite = blank_hat_sprite;
			}
			else if (potential_mixer_indices[i] == body_index)
			{
				instantiated_inv_slots[body_index].GetComponent<Image>().sprite = blank_body_sprite;
			}
			else if (current_inv_pageNo == 1)
			{
				if (potential_mixer_indices[i] <= n_slots_per_page)
				{
					instantiated_inv_slots[potential_mixer_indices[i]].GetComponent<Image>().sprite = empty_sprite;
				}
			}
			else if (current_inv_pageNo == 2 && potential_mixer_indices[i] >= 20)
			{
				instantiated_inv_slots[potential_mixer_indices[i] - 20].GetComponent<Image>().sprite = empty_sprite;
			}
			inventory_objType[potential_mixer_indices[i]] = "";
			inventory_special[potential_mixer_indices[i]] = "";
			inventory_count[potential_mixer_indices[i]] = 0;
			PlayerData.Instance.SetSlotString("inventory" + potential_mixer_indices[i], "", PlayerData.grouping_t.the_inventory);
			TryUnequip3DObjects(potential_mixer_indices[i]);
		}
		Vector3 target_position = instantiated_inv_slots[slot_instance_index].transform.localPosition;
		for (float i2 = 0f; i2 < 10f; i2 += 1f)
		{
			foreach (GameObject item in mixer_parts)
			{
				item.transform.localPosition = Vector3.Lerp(item.transform.localPosition, target_position, i2 / 8f);
			}
			yield return new WaitForEndOfFrame();
		}
		foreach (GameObject item2 in mixer_parts)
		{
			UnityEngine.Object.Destroy(item2);
		}
		instantiated_inv_slots[slot_instance_index].GetComponent<Image>().sprite = new_inv_items_by_name[trying_to_craft].inventory_sprite;
		instantiated_inv_slots[slot_instance_index].GetComponent<Animation>().Play();
		inventory_objType[result_goto_index] = trying_to_craft;
		inventory_special[result_goto_index] = "";
		inventory_count[result_goto_index] = 1;
		PlayerData.Instance.SetSlotString("inventory" + result_goto_index, trying_to_craft, PlayerData.grouping_t.the_inventory);
		show_angular(target_position);
		yield return new WaitForSeconds(0.5f);
		allowClose = true;
	}

	private bool test_has_enough_materials(int index)
	{
		new_inv_item new_inv_item = new_inv_items_by_name[craftList[craft_PAGE * 3 + index]];
		string crafting_ingredientA = new_inv_item.crafting_ingredientA;
		int num = new_inv_item.crafting_ingredientA_cnt;
		string crafting_ingredientB = new_inv_item.crafting_ingredientB;
		int num2 = new_inv_item.crafting_ingredientB_cnt;
		potential_mixer_indices.Clear();
		for (int i = 0; i < total_INV_spaces; i++)
		{
			if (inventory_objType[i] == crafting_ingredientA && num != 0)
			{
				num--;
				potential_mixer_indices.Add(i);
			}
			else if (crafting_ingredientB != "" && inventory_objType[i] == crafting_ingredientB && num2 != 0)
			{
				num2--;
				potential_mixer_indices.Add(i);
			}
			if (crafting_ingredientB == "")
			{
				if (num <= 0)
				{
					return true;
				}
			}
			else if (num <= 0 && num2 <= 0)
			{
				return true;
			}
		}
		return false;
	}

	public void show_angular(Vector3 localPos)
	{
		angular.SetActive(true);
		angular.transform.localPosition = localPos;
		angular.GetComponent<Animation>().Play();
		GameController.Instance.sound_craft();
		StartCoroutine(hide_angular());
	}

	private IEnumerator hide_angular()
	{
		yield return new WaitForSeconds(0.5f);
		angular.SetActive(false);
	}

	public void container_saveContents()
	{
		string grouping = "basket" + open_container_id;
		for (int i = 0; i < n_slots_per_page; i++)
		{
			PlayerData.Instance.SetSlotString("stored-" + i, container_objType[i], grouping);
			PlayerData.Instance.SetSlotInt("count-" + i, container_count[i], grouping);
			PlayerData.Instance.SetSlotString("special-" + i, container_special[i], grouping);
		}
		PlayerData.Instance.SaveModified();
		PlayerData.Instance.DeLoadGrouping(grouping);
	}

	private int get_coin_count()
	{
		int num = 0;
		for (int i = 0; i < n_slots_per_page; i++)
		{
			if (inventory_objType[i] == "Coins")
			{
				num += inventory_count[i];
			}
		}
		for (int j = 20; j < 20 + n_slots_per_page; j++)
		{
			if (inventory_objType[j] == "Coins")
			{
				num += inventory_count[j];
			}
		}
		return num;
	}

	public void pressed_crafting_button(int index, bool allowedToBuild)
	{
		new_inv_item new_inv_item = new_inv_items_by_name[craftList[craft_PAGE * 3 + index]];
		if (is_sale)
		{
			int coin_count = get_coin_count();
			int market_cost = new_inv_item.market_cost;
			if (coin_count >= market_cost)
			{
				string text = ((!(new_inv_item.overwrite_name == "")) ? new_inv_item.overwrite_name : new_inv_item.name);
				Shop_positioner.Instance.activate_popup(Shop_positioner.config.YES_NO_buttons, "Buy <color=#eeee55>" + text + "</color>?", Color.white, new_inv_item.inventory_sprite, new Color(0.29803923f, 26f / 51f, 0.5294118f));
				Shop_positioner.Instance.suppress_models = true;
				Shop_positioner.Instance.on_craft = true;
				trying_to_craft = new_inv_item.name;
			}
			else
			{
				Shop_positioner.Instance.activate_popup(Shop_positioner.config.OKAY_button, "<color=#eeee55>" + new_inv_item.name + "</color>\nNot enough coins!", new Color(0.9411765f, 0.3019608f, 0.3019608f), new_inv_item.inventory_sprite, new Color(20f / 51f, 20f / 51f, 20f / 51f));
				Shop_positioner.Instance.suppress_models = true;
				Shop_positioner.Instance.temp_hide_close = true;
				Shop_positioner.Instance.do_blobble_gem_count = false;
			}
		}
		else if (!allowedToBuild)
		{
			Shop_positioner.Instance.activate_popup(Shop_positioner.config.OKAY_button, "<color=#eeee55>" + new_inv_item.name + "</color>\n<size=58>Unlock the FURNITURE PACK to build this! :)</size>", new Color(38f / 85f, 1f, 0f), new_inv_item.inventory_sprite, new Color(20f / 51f, 20f / 51f, 20f / 51f));
			Shop_positioner.Instance.suppress_models = true;
			Shop_positioner.Instance.do_blobble_gem_count = false;
		}
		else if (test_has_enough_materials(index))
		{
			Shop_positioner.Instance.activate_popup(Shop_positioner.config.YES_NO_buttons, "Build a <color=#eeee55>" + new_inv_item.name + "</color>?", Color.white, new_inv_item.inventory_sprite, new Color(0.29803923f, 26f / 51f, 0.5294118f));
			Shop_positioner.Instance.suppress_models = true;
			Shop_positioner.Instance.on_craft = true;
			trying_to_craft = craftList[craft_PAGE * 3 + index];
		}
		else
		{
			Shop_positioner.Instance.activate_popup(Shop_positioner.config.OKAY_button, "<color=#eeee55>" + new_inv_item.name + "</color>\nYou do not have enough materials!", new Color(0.9411765f, 0.3019608f, 0.3019608f), new_inv_item.inventory_sprite, new Color(20f / 51f, 20f / 51f, 20f / 51f));
			Shop_positioner.Instance.suppress_models = true;
			Shop_positioner.Instance.temp_hide_close = true;
			Shop_positioner.Instance.do_blobble_gem_count = false;
		}
	}

	public void use_tool_click(Vector3 clickedAt)
	{
		string chunkString_ = NewBiomeControl.Instance.GetChunkString_(clickedAt);
		Vector3 inner = NewBiomeControl.Instance.GetInner(clickedAt);
		int num = (int)inner.x;
		int num2 = (int)inner.z;
		Vector3 vector = new Vector3((float)NewBiomeControl.Round(clickedAt.x) + 0.5f, GameController.Instance.circ_h, (float)NewBiomeControl.Round(clickedAt.z) + 0.5f);
		bool flag = false;
		switch (ITEM_USING)
		{
		case "Wrench":
			if (PlayerData.Instance.GetSlotInt("fill(" + num + "," + num2 + ")", chunkString_) == 4)
			{
				int slotInt4 = PlayerData.Instance.GetSlotInt("rot(" + num + "," + num2 + ")", chunkString_);
				slotInt4++;
				if (slotInt4 == 4)
				{
					slotInt4 = 0;
				}
				PlayerData.Instance.SetSlotInt("rot(" + num + "," + num2 + ")", slotInt4, chunkString_);
				NewBiomeControl.Instance.RebuildChunkAt(clickedAt);
				flag = true;
			}
			break;
		case "SledgeHammer":
		{
			if (PlayerData.Instance.GetSlotInt("fill(" + num + "," + num2 + ")", chunkString_) != 4)
			{
				break;
			}
			string slotString = PlayerData.Instance.GetSlotString("built(" + num + "," + num2 + ")", chunkString_);
			bool flag2 = true;
			switch (slotString)
			{
			case "Chest":
			case "Shack":
			case "Mansion":
			case "Basket":
			case "Red Shack":
			{
				int slotInt3 = PlayerData.Instance.GetSlotInt("special(" + num + "," + num2 + ")", chunkString_);
				switch (slotString)
				{
				case "Chest":
				{
					for (int j = 0; j < n_slots_per_page; j++)
					{
						string slotString2 = PlayerData.Instance.GetSlotString("stored-" + j, "basket" + slotInt3);
						if (slotString2 != "")
						{
							PopupControl.Instance.ShowMessage("Chest must be empty before you can remove it. [" + slotString2 + "]");
							flag2 = false;
							break;
						}
					}
					break;
				}
				case "Basket":
				{
					List<int> list = new List<int>();
					list.Add(0);
					list.Add(1);
					list.Add(2);
					list.Add(5);
					list.Add(6);
					list.Add(7);
					list.Add(10);
					list.Add(11);
					list.Add(12);
					for (int i = 0; i < list.Count; i++)
					{
						if (PlayerData.Instance.GetSlotString("stored-" + list[i], "basket" + slotInt3) != "")
						{
							PopupControl.Instance.ShowMessage("Basket must be empty before you can remove it.");
							flag2 = false;
							break;
						}
					}
					break;
				}
				}
				break;
			}
			}
			if (flag2)
			{
				if (try_give_item(slotString, 1, vector, ""))
				{
					PlayerData.Instance.SetSlotInt("fill(" + num + "," + num2 + ")", 0, chunkString_);
					NewBiomeControl.Instance.RebuildChunkAt(clickedAt);
				}
				flag = true;
			}
			break;
		}
		case "Shovel":
		{
			if (PlayerData.Instance.GetSlotInt("fill(" + num + "," + num2 + ")", chunkString_) != 1)
			{
				break;
			}
			int slotInt = PlayerData.Instance.GetSlotInt("biome", chunkString_);
			int slotInt2 = PlayerData.Instance.GetSlotInt("obj(" + num + "," + num2 + ")", chunkString_);
			string text = "";
			if (slotInt == 0 && slotInt2 == 1)
			{
				text = "Berry Bush";
			}
			else if (slotInt == 1 && slotInt2 == 3)
			{
				text = "MoonBerry Bush";
			}
			if (text != "")
			{
				if (try_give_item(text, 1, vector, ""))
				{
					PlayerData.Instance.SetSlotInt("fill(" + num + "," + num2 + ")", 0, chunkString_);
					NewBiomeControl.Instance.RebuildChunkAt(clickedAt);
				}
				flag = true;
			}
			break;
		}
		}
		if (flag)
		{
			GameController.Instance.targeted_circle_graphic.transform.Find("graphic").GetComponent<Renderer>().material.mainTexture = GameController.Instance.target_circle_yellow;
			GameController.Instance.targeted_circle_graphic.transform.Find("graphic").transform.localScale = Vector3.one * 1.1f;
			GameController.Instance.targeted_circle_graphic.transform.position = vector;
			GameController.Instance.targeted_circle_graphic.GetComponent<Animation>().Stop();
			GameController.Instance.targeted_circle_graphic.GetComponent<Animation>().PlayQueued("targetGraphic");
		}
	}

	public int get_best_pick()
	{
		switch (inventory_objType[hand_index])
		{
		case "Wood Pick":
			return 1;
		case "Stone Pick":
			return 2;
		case "Metal Pick":
			return 3;
		default:
			return 0;
		}
	}

	public void do_swap(int drag_onto_index, GameObject drag_onto_obj)
	{
		int index = mouse_down_slot.GetComponent<inv_slot_scr>().index;
		switch (drag_onto_index)
		{
		case -4:
		{
			int num = container_count[index];
			int num2 = ToInventory(new_inv_items_by_name[container_objType[index]], container_count[index], ptype.eitherPage, container_special[index]);
			if (num2 == 0)
			{
				container_objType[index] = "";
				container_count[index] = 0;
				container_special[index] = "";
				SaveAllInventory();
			}
			else if (num != num2)
			{
				container_count[index] = num2;
				SaveAllInventory();
			}
			redraw_container();
			return;
		}
		case -5:
		{
			int num3 = index + pgmod();
			int num4 = inventory_count[num3];
			int num5 = ToContainer(new_inv_items_by_name[inventory_objType[num3]], inventory_count[num3], inventory_special[num3]);
			if (num5 == 0)
			{
				inventory_objType[num3] = "";
				inventory_special[num3] = "";
				inventory_count[num3] = 0;
				SaveAllInventory();
			}
			else if (num4 != num5)
			{
				inventory_count[num3] = num5;
				SaveAllInventory();
			}
			redraw_inventory();
			return;
		}
		}
		if (drag_onto_index == trash_index)
		{
			int num6 = index + ((index <= 14) ? pgmod() : 0);
			inventory_objType[num6] = "";
			inventory_special[num6] = "";
			inventory_count[num6] = 0;
			PlayerData.Instance.SetSlotString("inventory" + num6, "", PlayerData.grouping_t.the_inventory);
			redraw_inventory();
			TryUnequip3DObjects(index);
			return;
		}
		if (drag_onto_index == page_1_index || drag_onto_index == page_2_index)
		{
			int num7 = index + ((index <= 14) ? pgmod() : 0);
			int num8 = -1;
			int num9 = inventory_count[num7];
			num8 = ((drag_onto_index != page_1_index) ? ToInventory(new_inv_items_by_name[inventory_objType[num7]], inventory_count[num7], ptype.secondPage, inventory_special[num7]) : ToInventory(new_inv_items_by_name[inventory_objType[num7]], inventory_count[num7], ptype.firstPage, inventory_special[num7]));
			if (num8 == 0)
			{
				inventory_objType[num7] = "";
				inventory_special[num7] = "";
				inventory_count[num7] = 0;
				SaveAllInventory();
				TryUnequip3DObjects(index);
			}
			else if (num8 != num9)
			{
				inventory_count[num7] = num8;
				SaveAllInventory();
			}
			redraw_inventory();
			return;
		}
		int num10 = 0;
		string text;
		string text2;
		if (INTERACT_W_CONTAINER && CURRENT_TAB == tab.right)
		{
			text = container_objType[drag_onto_index];
			num10 = container_count[drag_onto_index];
			text2 = container_special[drag_onto_index];
		}
		else if (drag_onto_index == hand_index)
		{
			text = inventory_objType[hand_index];
			num10 = inventory_count[hand_index];
			text2 = inventory_special[hand_index];
		}
		else if (drag_onto_index == hat_index)
		{
			text = inventory_objType[hat_index];
			num10 = inventory_count[hat_index];
			text2 = inventory_special[hat_index];
		}
		else if (drag_onto_index == body_index)
		{
			text = inventory_objType[body_index];
			num10 = inventory_count[body_index];
			text2 = inventory_special[body_index];
		}
		else
		{
			text = inventory_objType[drag_onto_index + pgmod()];
			num10 = inventory_count[drag_onto_index + pgmod()];
			text2 = inventory_special[drag_onto_index + pgmod()];
		}
		if (INTERACT_W_CONTAINER && CURRENT_TAB == tab.right)
		{
			container_objType[drag_onto_index] = container_objType[index];
			container_count[drag_onto_index] = container_count[index];
			container_special[drag_onto_index] = container_special[index];
		}
		else
		{
			int num11 = ((drag_onto_index > 14) ? drag_onto_index : (drag_onto_index + pgmod()));
			int num12 = ((index > 14) ? index : (index + pgmod()));
			inventory_objType[num11] = inventory_objType[num12];
			inventory_count[num11] = inventory_count[num12];
			inventory_special[num11] = inventory_special[num12];
			PlayerData.Instance.SetSlotString("inventory" + num11, inventory_objType[num12], PlayerData.grouping_t.the_inventory);
			PlayerData.Instance.SetSlotInt("inventoryCount" + num11, inventory_count[num12], PlayerData.grouping_t.the_inventory);
			PlayerData.Instance.SetSlotString("inventorySpecial" + num11, inventory_special[num12], PlayerData.grouping_t.the_inventory);
		}
		if (INTERACT_W_CONTAINER && CURRENT_TAB == tab.right)
		{
			container_objType[index] = text;
			container_count[index] = num10;
			container_special[index] = text2;
		}
		else
		{
			int num13 = ((index > 14) ? index : (index + pgmod()));
			inventory_objType[num13] = text;
			inventory_count[num13] = num10;
			inventory_special[num13] = text2;
			PlayerData.Instance.SetSlotString("inventory" + num13, text, PlayerData.grouping_t.the_inventory);
			PlayerData.Instance.SetSlotInt("inventoryCount" + num13, num10, PlayerData.grouping_t.the_inventory);
			PlayerData.Instance.SetSlotString("inventorySpecial" + num13, text2, PlayerData.grouping_t.the_inventory);
		}
		if (drag_onto_index == hand_index)
		{
			show_angular(drag_onto_obj.transform.localPosition);
			equip_hand();
		}
		else if (drag_onto_index == hat_index)
		{
			show_angular(drag_onto_obj.transform.localPosition);
			equip_hat();
		}
		else if (drag_onto_index == body_index)
		{
			show_angular(drag_onto_obj.transform.localPosition);
			equip_armor();
		}
		if (text == "")
		{
			TryUnequip3DObjects(index);
		}
		if (INTERACT_W_CONTAINER && CURRENT_TAB == tab.right)
		{
			redraw_container();
		}
		else
		{
			redraw_inventory();
		}
		if (text != "")
		{
			mouse_down_slot.GetComponent<Animation>().Play();
		}
		drag_onto_obj.GetComponent<Animation>().Play();
	}

	private void SaveAllInventory()
	{
		for (int i = 0; i < n_slots_per_page + 5; i++)
		{
			PlayerData.Instance.SetSlotString("inventory" + i, inventory_objType[i], PlayerData.grouping_t.the_inventory);
			PlayerData.Instance.SetSlotInt("inventoryCount" + i, inventory_count[i], PlayerData.grouping_t.the_inventory);
			PlayerData.Instance.SetSlotString("inventorySpecial" + i, inventory_special[i], PlayerData.grouping_t.the_inventory);
		}
		if (PlayerData.Instance.GetGlobalInt("big_inv") == 1)
		{
			for (int j = 20; j < 20 + n_slots_per_page; j++)
			{
				PlayerData.Instance.SetSlotString("inventory" + j, inventory_objType[j], PlayerData.grouping_t.the_inventory);
				PlayerData.Instance.SetSlotInt("inventoryCount" + j, inventory_count[j], PlayerData.grouping_t.the_inventory);
				PlayerData.Instance.SetSlotString("inventorySpecial" + j, inventory_special[j], PlayerData.grouping_t.the_inventory);
			}
		}
	}

	private void TryUnequip3DObjects(int inv_index)
	{
		if (inv_index == hand_index)
		{
			if (GameController.Instance.player.GetComponent<creatureScript>().myCreatureModel.GetComponent<creatureModel>().obj_holding != null)
			{
				UnityEngine.Object.Destroy(GameController.Instance.player.GetComponent<creatureScript>().myCreatureModel.GetComponent<creatureModel>().obj_holding);
			}
		}
		else if (inv_index == hat_index)
		{
			if (GameController.Instance.player.GetComponent<creatureScript>().myCreatureModel.hat != null)
			{
				UnityEngine.Object.Destroy(GameController.Instance.player.GetComponent<creatureScript>().myCreatureModel.hat);
			}
		}
		else if (inv_index == body_index && GameController.Instance.player.GetComponent<creatureScript>().myCreatureModel.armor_body != null)
		{
			UnityEngine.Object.Destroy(GameController.Instance.player.GetComponent<creatureScript>().myCreatureModel.armor_body);
		}
	}

	private IEnumerator delayed_unpause()
	{
		yield return new WaitForSeconds(0.05f);
		Shop_positioner.Instance.resume_gameplay(false);
	}

	public void click_DONE_placing()
	{
		DONE_placing_button.SetActive(false);
		StartCoroutine(delayed_unpause());
		switch (done_button_context)
		{
		case button_state.CANCEL_PLACE_OBJ:
			PLACING_OBJECT_OR_USING_TOOL = false;
			click_to_place.SetActive(false);
			unlock();
			try_give_item(ITEM_USING, 1, GameController.Instance.player.transform.position, "", false);
			break;
		case button_state.DONE_USING_TOOL:
			PLACING_OBJECT_OR_USING_TOOL = false;
			click_to_place.SetActive(false);
			break;
		case button_state.DONE_SITTING_CHAIR:
			GameController.Instance.done_with_chair();
			break;
		case button_state.CAST_PROJECTILE:
			click_to_place.SetActive(false);
			GameController.Instance.button_clicked = true;
			GameController.Instance.is_casting_spell = false;
			break;
		}
	}

	public void unlock()
	{
		StartCoroutine(delayed_unpause());
	}

	public void inc_then_set_iterator()
	{
		do
		{
			unique_id_iterator++;
		}
		while (NPC_chests.Contains(unique_id_iterator));
		PlayerData.Instance.SetSlotInt("unique_id_iterator", unique_id_iterator, PlayerData.grouping_t.general);
	}

	public void BuildObject(string item_name, Vector3 position, int rot, bool write, int set_special_id = -1)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(new_inv_items_by_name[item_name].world_obj);
		gameObject.transform.position = position;
		gameObject.transform.rotation = Quaternion.identity;
		gameObject.transform.Rotate(Vector3.up, (float)rot * 90f);
		gameObject.transform.parent = NewBiomeControl.Instance.GetChunkParent(position);
		if (item_name == "Berry Bush" || item_name == "MoonBerry Bush")
		{
			gameObject.GetComponent<NewCollectible>().CustomStart();
		}
		string chunkString_ = NewBiomeControl.Instance.GetChunkString_(position);
		Vector3 inner = NewBiomeControl.Instance.GetInner(position);
		string text = "(" + (int)inner.x + "," + (int)inner.z + ")";
		if (write)
		{
			PlayerData.Instance.SetSlotInt("fill" + text, 4, chunkString_);
			PlayerData.Instance.SetSlotInt("rot" + text, 0, chunkString_);
			PlayerData.Instance.SetSlotString("built" + text, item_name, chunkString_);
		}
		switch (item_name)
		{
		case "Basket":
		case "Chest":
		case "Shack":
		case "Mansion":
		case "Red Shack":
		{
			int num;
			if (!write)
			{
				num = ((set_special_id != -1) ? set_special_id : PlayerData.Instance.GetSlotInt("special" + text, chunkString_));
			}
			else
			{
				num = unique_id_iterator;
				PlayerData.Instance.SetSlotInt("special" + text, num, chunkString_);
				inc_then_set_iterator();
				if (item_name == "Shack")
				{
					MultiplayerControl.Instance.unlock_achievement("CgkI7aPb66UWEAIQAQ");
				}
				else if (item_name == "Mansion")
				{
					MultiplayerControl.Instance.unlock_achievement("CgkI7aPb66UWEAIQAg");
				}
			}
			NewInteractable newInteractable = ((!(gameObject.GetComponent<NewInteractable>() == null)) ? gameObject.GetComponent<NewInteractable>() : gameObject.transform.Find("Interactable").GetComponent<NewInteractable>());
			newInteractable.unique_id = num;
			newInteractable.temp_rot = rot;
			break;
		}
		}
	}

	private bool check_required_skill(string skill_key, int min_skill_lvl)
	{
		if (Application.isEditor)
		{
			return true;
		}
		switch (skill_key)
		{
		case "Attack":
			if (GameController.Instance.player_stats[3] >= min_skill_lvl)
			{
				return true;
			}
			return false;
		case "Health":
			if (GameController.Instance.player_stats[2] >= min_skill_lvl)
			{
				return true;
			}
			return false;
		case "Mining":
			if (GameController.Instance.player_stats[0] >= min_skill_lvl)
			{
				return true;
			}
			return false;
		default:
			return true;
		}
	}

	private void Update()
	{
		if (PLACING_OBJECT_OR_USING_TOOL && Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float enter;
			if (GameController.Instance.plane.Raycast(ray, out enter))
			{
				Vector3 point = ray.GetPoint(enter);
				Vector3 vector = new Vector3((float)NewBiomeControl.Round(point.x) + 0.5f, 0f, (float)NewBiomeControl.Round(point.z) + 0.5f);
				if (new_inv_items_by_name[ITEM_USING].type == inv_type_t.tool)
				{
					use_tool_click(vector);
				}
				else if (new_inv_items_by_name[ITEM_USING].type == inv_type_t.place_in_world)
				{
					bool flag = true;
					string chunkString_ = NewBiomeControl.Instance.GetChunkString_(vector);
					Vector3 inner = NewBiomeControl.Instance.GetInner(vector);
					int num = (int)inner.x;
					int num2 = (int)inner.z;
					switch (PlayerData.Instance.GetSlotInt("fill(" + num + "," + num2 + ")", chunkString_))
					{
					case 1:
					case 4:
						flag = false;
						break;
					}
					switch (ITEM_USING)
					{
					case "Berry Bush":
					case "MoonBerry Bush":
					case "Shack":
					case "Red Shack":
					case "Mansion":
						if (NewBiomeControl.Instance.player_zone != "overworld")
						{
							flag = false;
						}
						break;
					}
					if (NewBiomeControl.Instance.chunks_with_specials.ContainsKey(chunkString_))
					{
						foreach (place_special item2 in NewBiomeControl.Instance.chunks_with_specials[chunkString_])
						{
							if (item2.innerX == num && item2.innerZ == num2)
							{
								flag = false;
								break;
							}
						}
					}
					if (NewBiomeControl.Instance.player_zone != "overworld")
					{
						Vector3 vector2 = new Vector3(NewBiomeControl.Instance.zone_origin_chunkX * 10 + NewBiomeControl.Instance.zone_origin_innerX, 0f, NewBiomeControl.Instance.zone_origin_chunkZ * 10 + NewBiomeControl.Instance.zone_origin_innerZ) + new Vector3(0.5f, 0f, 0.5f);
						List<Vector3> list = new List<Vector3>();
						Vector3 item = vector - vector2;
						if (NewBiomeControl.Instance.player_zone_type == "shack")
						{
							for (int i = -5; i <= 0; i++)
							{
								for (int j = 1; j <= 4; j++)
								{
									list.Add(new Vector3(i, 0f, j));
								}
							}
							for (int k = -4; k <= -1; k++)
							{
								list.Add(new Vector3(k, 0f, 0f));
								list.Add(new Vector3(k, 0f, 5f));
							}
							if (!list.Contains(item))
							{
								flag = false;
							}
						}
						else if (NewBiomeControl.Instance.player_zone_type == "mansion")
						{
							for (int l = -4; l <= -1; l++)
							{
								list.Add(new Vector3(l, 0f, 8f));
								list.Add(new Vector3(l, 0f, 9f));
								list.Add(new Vector3(l, 0f, 13f));
							}
							for (int m = -5; m <= 0; m++)
							{
								list.Add(new Vector3(m, 0f, 0f));
								list.Add(new Vector3(m, 0f, 7f));
							}
							for (int n = -6; n <= 1; n++)
							{
								for (int num3 = 1; num3 <= 6; num3++)
								{
									list.Add(new Vector3(n, 0f, num3));
								}
							}
							for (int num4 = -5; num4 <= 0; num4++)
							{
								for (int num5 = 10; num5 <= 12; num5++)
								{
									list.Add(new Vector3(num4, 0f, num5));
								}
							}
							if (!list.Contains(item))
							{
								flag = false;
							}
						}
					}
					if (NewBiomeControl.Instance.player_zone == "shack9999")
					{
						flag = false;
					}
					if (flag && Vector3.Distance(GameController.Instance.player.transform.position, point) > 1f)
					{
						PLACING_OBJECT_OR_USING_TOOL = false;
						click_to_place.SetActive(false);
						DONE_placing_button.SetActive(false);
						BuildObject(ITEM_USING, vector, 0, true);
						unlock();
					}
				}
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			if (test_for_dragging)
			{
				test_for_dragging = false;
			}
			else if (is_dragging)
			{
				float num6 = float.MaxValue;
				int num7 = -1;
				GameObject gameObject = null;
				bool flag2 = false;
				for (int num8 = 0; num8 < instantiated_inv_slots.Count; num8++)
				{
					if (instantiated_inv_slots[num8].gameObject.activeInHierarchy)
					{
						float num9 = Vector2.Distance(drag_image.transform.position, instantiated_inv_slots[num8].transform.position);
						if (num9 < num6)
						{
							num6 = num9;
							num7 = num8;
							gameObject = instantiated_inv_slots[num8];
						}
					}
				}
				if (transfer_from.activeInHierarchy)
				{
					float num10 = Vector2.Distance(drag_image.transform.position, transfer_from.transform.position);
					if (num10 < num6)
					{
						num6 = num10;
						num7 = -5;
						gameObject = transfer_from;
						flag2 = true;
					}
				}
				if (transfer_to.activeInHierarchy)
				{
					float num11 = Vector2.Distance(drag_image.transform.position, transfer_to.transform.position);
					if (num11 < num6)
					{
						num6 = num11;
						num7 = -4;
						gameObject = transfer_to;
						flag2 = true;
					}
				}
				if (trash_bin.activeInHierarchy)
				{
					float num12 = Vector2.Distance(drag_image.transform.position, trash_bin.transform.position);
					if (num12 < num6)
					{
						num6 = num12;
						num7 = trash_index;
						gameObject = trash_bin;
						flag2 = true;
					}
				}
				if (current_inv_pageNo == 1 && pageSwitchers.activeInHierarchy)
				{
					float num13 = Vector2.Distance(drag_image.transform.position, invPAGE2.transform.position);
					if (num13 < num6)
					{
						num6 = num13;
						num7 = page_2_index;
						gameObject = invPAGE2;
						flag2 = true;
					}
				}
				if (current_inv_pageNo == 2 && pageSwitchers.activeInHierarchy)
				{
					float num14 = Vector2.Distance(drag_image.transform.position, invPAGE1.transform.position);
					if (num14 < num6)
					{
						num6 = num14;
						num7 = page_1_index;
						gameObject = invPAGE1;
						flag2 = true;
					}
				}
				bool flag3 = true;
				if (!flag2)
				{
					int index = mouse_down_slot.GetComponent<inv_slot_scr>().index;
					int num15 = index + ((index <= 14) ? pgmod() : 0);
					int num16 = num7 + ((num7 <= 14) ? pgmod() : 0);
					new_inv_item new_inv_item = new_inv_items_by_name[inventory_objType[num15]];
					new_inv_item new_inv_item2 = new_inv_items_by_name[inventory_objType[num16]];
					if (num7 == hand_index)
					{
						if (new_inv_item.type != inv_type_t.holdable)
						{
							flag3 = false;
						}
						else if (!check_required_skill(new_inv_item.equip_required_stat, new_inv_item.equip_required_stat_lvl))
						{
							PopupControl.Instance.ShowMessage("Your <color=#66d8ff>" + new_inv_item.equip_required_stat + "</color> skill must be <color=#f44242>" + new_inv_item.equip_required_stat_lvl + "</color> or greater to equip this.");
							flag3 = false;
						}
					}
					else if (index == hand_index)
					{
						if (num7 == trash_index || num7 == page_1_index || num7 == page_2_index)
						{
							flag3 = true;
						}
						else if (new_inv_item2.type != inv_type_t.holdable && inventory_objType[num16] != "")
						{
							flag3 = false;
						}
					}
					else if (num7 == hat_index)
					{
						if (new_inv_item.type != inv_type_t.helmet)
						{
							flag3 = false;
						}
						else if (!check_required_skill(new_inv_item.equip_required_stat, new_inv_item.equip_required_stat_lvl))
						{
							PopupControl.Instance.ShowMessage("Your <color=#66d8ff>" + new_inv_item.equip_required_stat + "</color> skill must be <color=#f44242>" + new_inv_item.equip_required_stat_lvl + "</color> or greater to equip this.");
							flag3 = false;
						}
					}
					else if (index == hat_index)
					{
						if (num7 == trash_index || num7 == page_1_index || num7 == page_2_index)
						{
							flag3 = true;
						}
						else if (new_inv_item2.type != inv_type_t.helmet && inventory_objType[num16] != "")
						{
							flag3 = false;
						}
					}
					else if (num7 == body_index)
					{
						if (new_inv_item.type != inv_type_t.armor)
						{
							flag3 = false;
						}
						else if (!check_required_skill(new_inv_item.equip_required_stat, new_inv_item.equip_required_stat_lvl))
						{
							PopupControl.Instance.ShowMessage("Your <color=#66d8ff>" + new_inv_item.equip_required_stat + "</color> skill must be <color=#f44242>" + new_inv_item.equip_required_stat_lvl + "</color> or greater to equip this.");
							flag3 = false;
						}
					}
					else if (index == body_index)
					{
						if (num7 == trash_index || num7 == page_1_index || num7 == page_2_index)
						{
							flag3 = true;
						}
						else if (new_inv_item2.type != inv_type_t.armor && inventory_objType[num16] != "")
						{
							flag3 = false;
						}
					}
				}
				if (mouse_down_slot != gameObject && flag3)
				{
					Instance.do_swap(num7, gameObject);
				}
				else
				{
					mouse_down_slot.GetComponent<Image>().sprite = mouse_down_sprite;
				}
				is_dragging = false;
				drag_image.SetActive(false);
			}
		}
		if (test_for_dragging && Vector3.Distance(Input.mousePosition, initial_press) > (float)drag_threshold)
		{
			mouse_down_sprite = mouse_down_slot.GetComponent<Image>().sprite;
			int index2 = mouse_down_slot.GetComponent<inv_slot_scr>().index;
			if (index2 == hand_index)
			{
				mouse_down_slot.GetComponent<Image>().sprite = blank_hand_sprite;
			}
			else if (index2 == hat_index)
			{
				mouse_down_slot.GetComponent<Image>().sprite = blank_hat_sprite;
			}
			else if (index2 == body_index)
			{
				mouse_down_slot.GetComponent<Image>().sprite = blank_body_sprite;
			}
			else
			{
				mouse_down_slot.GetComponent<Image>().sprite = empty_sprite;
			}
			position_drag_at_mouse();
			drag_image.SetActive(true);
			drag_image.GetComponent<Image>().sprite = mouse_down_sprite;
			is_dragging = true;
			test_for_dragging = false;
		}
	}

	private void FixedUpdate()
	{
		if (refresh_loot_counter_i <= 0)
		{
			UpdateLootedChestList();
			refresh_loot_counter_i = 50;
		}
		refresh_loot_counter_i--;
		if (is_dragging)
		{
			position_drag_at_mouse();
		}
	}

	private void position_drag_at_mouse()
	{
		float num = Input.mousePosition.x / (float)Screen.width;
		float num2 = Input.mousePosition.y / (float)Screen.height;
		float x = (2f * num - 1f) * Shop_positioner.SCREEN_MAX_X;
		float y = (2f * num2 - 1f) * 400f;
		drag_image.transform.localPosition = new Vector2(x, y) * 1.15f;
	}

	public bool try_give_item(string item_type, int count, Vector3 player_position, string special, bool visual = true)
	{
		int num = ToInventory(new_inv_items_by_name[item_type], count, ptype.eitherPage, special);
		SaveAllInventory();
		if (num == count)
		{
			if (visual)
			{
				GameController.Instance.showOverheadNotif("Inventory Full!", GameController.Instance.player.transform.position, false, true);
			}
			return false;
		}
		if (visual)
		{
			GameController.Instance.showOverheadNotif("+" + (count - num) + " " + item_type, GameController.Instance.player.transform.position, true, true);
		}
		return true;
	}

	private void create_buttons_and_slots()
	{
		for (int i = 0; i < n_rows; i++)
		{
			for (int j = 0; j < n_cols; j++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(inv_slot_type);
				gameObject.transform.parent = for_slots;
				gameObject.transform.localPosition = INV_start_slots + new Vector2((float)j * INV_slot_spacing, (float)i * (0f - INV_slot_spacing));
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.localRotation = Quaternion.identity;
				instantiated_inv_slots.Add(gameObject);
				gameObject.GetComponent<inv_slot_scr>().index = j % n_cols + i * n_cols;
			}
		}
		instantiated_inv_slots.Add(equip_hand_slot);
		instantiated_inv_slots.Add(equip_hat_slot);
		instantiated_inv_slots.Add(equip_body_slot);
		for (int k = 0; k < 3; k++)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(craft_slot_type);
			gameObject2.transform.parent = craft_slot_parent;
			gameObject2.transform.localRotation = Quaternion.identity;
			gameObject2.transform.localScale = Vector3.one;
			gameObject2.transform.localPosition = CRAFT_start_slots + Vector2.right * CRAFT_spacing * k;
			gameObject2.GetComponent<crafting_slot>().index = k;
			instantiated_crafting_slots.Add(gameObject2.GetComponent<crafting_slot>());
		}
		drag_image.transform.SetAsLastSibling();
		angular.transform.SetAsLastSibling();
	}

	public void close_inv()
	{
		if (allowClose)
		{
			NewAudioControl.Instance.play_generic_click();
			close_inv_window();
			close_generic_window();
			if (INTERACT_W_CONTAINER)
			{
				container_saveContents();
			}
		}
	}

	public void close_generic_window()
	{
		GameController.Instance.give_all_display();
		StartCoroutine(delayed_unpause());
	}

	public void OPEN(tab defaultTab, string TAB_B_NAME, string TAB_A_NAME = "INVENTORY")
	{
		if (Shop_positioner.Instance.try_open_generic_window())
		{
			inventory_crafting_screen.SetActive(true);
			is_open = true;
			NewAudioControl.Instance.play_generic_click();
			CURRENT_TAB = defaultTab;
			inventory_tab_text.text = TAB_A_NAME;
			crafting_tab_text.text = TAB_B_NAME;
			if (TAB_B_NAME == "CRAFTING")
			{
				double_tap_text.SetActive(true);
				double_tap_text.GetComponent<Text>().text = TranslationControl.Instance.Translate("Double-tap to use/place objects");
			}
			else if (TAB_B_NAME == "BUY ITEMS")
			{
				double_tap_text.SetActive(true);
				double_tap_text.GetComponent<Text>().text = TranslationControl.Instance.Translate("Double-tap to sell items");
			}
			else
			{
				double_tap_text.SetActive(false);
			}
			bool active = TAB_B_NAME == "CRAFTING" || TAB_B_NAME == "WORKBENCH" || TAB_B_NAME == "ANVIL";
			equip_hand_slot.SetActive(active);
			equip_hat_slot.SetActive(active);
			equip_body_slot.SetActive(active);
			switch (defaultTab)
			{
			case tab.left:
				press_inventory_tab();
				break;
			case tab.right:
				press_crafting_tab();
				break;
			}
		}
	}

	public void press_crafting_tab()
	{
		if (allowClose)
		{
			if (!INTERACT_W_CONTAINER)
			{
				craft_PAGE = 0;
				crafting_page_left.SetActive(false);
				crafting_page_right.SetActive(page_exists(1));
			}
			CURRENT_TAB = tab.right;
			open_crafting_tab();
		}
	}

	public void press_inventory_tab()
	{
		if (allowClose)
		{
			CURRENT_TAB = tab.left;
			open_inventory_tab();
		}
	}

	private void set_vendor(int seller_id)
	{
		switch (seller_id)
		{
		case 2:
			craftList = NPC_seller_pineappleShark;
			curr_buyback_list = NPC_buyBack_pineappleShark;
			break;
		case 3:
			craftList = NPC_seller_cupcakeCrab_;
			break;
		case 4:
			craftList = NPC_seller_octokitty;
			curr_buyback_list = NPC_buyBack_octoKitty;
			break;
		}
	}

	public void open_buy(int seller_id)
	{
		set_vendor(seller_id);
		is_sale = true;
		INTERACT_W_CONTAINER = false;
		OPEN(tab.right, "BUY ITEMS", "SELL ITEMS");
	}

	public void open_sell(int seller_id)
	{
		set_vendor(seller_id);
		is_sale = true;
		INTERACT_W_CONTAINER = false;
		OPEN(tab.left, "BUY ITEMS", "SELL ITEMS");
	}

	public void open_inventory_crafting_window()
	{
		GameController.Instance.button_clicked = true;
		craftList = default_crafting;
		is_sale = false;
		INTERACT_W_CONTAINER = false;
		OPEN(tab.left, "CRAFTING");
	}

	public void open_craftingTable()
	{
		craftList = craftingTable_crafting;
		is_sale = false;
		INTERACT_W_CONTAINER = false;
		OPEN(tab.right, "WORKBENCH");
	}

	public void open_anvil()
	{
		craftList = anvil_crafting;
		is_sale = false;
		INTERACT_W_CONTAINER = false;
		OPEN(tab.right, "ANVIL");
	}

	public void open_cauldron()
	{
		craftList = cauldron_crafting;
		is_sale = false;
		INTERACT_W_CONTAINER = false;
		OPEN(tab.right, "CAULDRON");
	}

	public void open_container()
	{
		is_sale = false;
		INTERACT_W_CONTAINER = true;
		inventory_lock.SetActive(true);
		container_objType = new string[n_slots_per_page];
		container_count = new int[n_slots_per_page];
		container_special = new string[n_slots_per_page];
		string grouping = "basket" + open_container_id;
		inventory_lock.SetActive(false);
		for (int i = 0; i < n_slots_per_page; i++)
		{
			string slotString = PlayerData.Instance.GetSlotString("stored-" + i, grouping);
			container_objType[i] = slotString;
			int num = PlayerData.Instance.GetSlotInt("count-" + i, grouping);
			if (slotString != "" && num == 0)
			{
				num = 1;
			}
			container_count[i] = num;
			string slotString2 = PlayerData.Instance.GetSlotString("special-" + i, grouping);
			container_special[i] = slotString2;
		}
		redraw_container();
		string tAB_B_NAME = "???";
		switch (CHEST_TYPE)
		{
		case chest_type.basket:
			tAB_B_NAME = "BASKET";
			text_put_in_basket.text = "PUT IN\nBASKET";
			text_take_from_basket.text = "TAKE FROM\nBASKET";
			break;
		case chest_type.personal_chest:
			tAB_B_NAME = "CHEST";
			text_put_in_basket.text = "PUT IN\nCHEST";
			text_take_from_basket.text = "TAKE FROM\nCHEST";
			break;
		case chest_type.gold_chest:
			tAB_B_NAME = "GOLD CHEST";
			text_put_in_basket.text = "PUT IN\nCHEST";
			text_take_from_basket.text = "TAKE FROM\nCHEST";
			break;
		case chest_type.titanium_chest:
			tAB_B_NAME = "TITANIUM CHEST";
			text_put_in_basket.text = "PUT IN\nCHEST";
			text_take_from_basket.text = "TAKE FROM\nCHEST";
			break;
		}
		OPEN(tab.right, tAB_B_NAME);
	}

	private int ToContainer(new_inv_item item, int curr_count, string special)
	{
		int num = get_largest_stack(item.max_stack);
		int num2 = curr_count;
		List<int> list = new List<int>();
		if (CHEST_TYPE != chest_type.basket)
		{
			for (int i = 0; i < n_slots_per_page; i++)
			{
				list.Add(i);
			}
		}
		else
		{
			list.Add(0);
			list.Add(1);
			list.Add(2);
			list.Add(5);
			list.Add(6);
			list.Add(7);
			list.Add(10);
			list.Add(11);
			list.Add(12);
		}
		if (num > 1)
		{
			for (int j = 0; j < list.Count; j++)
			{
				int num3 = list[j];
				if (container_objType[num3] == item.name && container_count[num3] != num)
				{
					int num4 = num - container_count[num3];
					if (num2 <= num4)
					{
						container_count[num3] += num2;
						num2 = 0;
						break;
					}
					num2 -= num4;
					container_count[num3] = num;
				}
			}
		}
		if (num2 != 0)
		{
			for (int k = 0; k < list.Count; k++)
			{
				int num5 = list[k];
				if (container_objType[num5] == "")
				{
					container_objType[num5] = item.name;
					container_special[num5] = special;
					if (num2 <= num)
					{
						container_count[num5] = num2;
						num2 = 0;
						break;
					}
					container_count[num5] = num;
					num2 -= num;
				}
			}
		}
		return num2;
	}

	private int ToInventory(new_inv_item item, int curr_count, ptype page_type, string special)
	{
		int num = curr_count;
		if (page_type == ptype.eitherPage || page_type == ptype.firstPage)
		{
			num = add_to_preexisting_stacks(item, num, 0, n_slots_per_page);
			if (num != 0)
			{
				num = add_to_empty_stacks(item, num, 0, n_slots_per_page, special);
			}
		}
		if (page_type == ptype.eitherPage || page_type == ptype.secondPage)
		{
			if (num != 0 && PlayerData.Instance.GetGlobalInt("big_inv") == 1)
			{
				num = add_to_preexisting_stacks(item, num, 20, 20 + n_slots_per_page);
			}
			if (num != 0 && PlayerData.Instance.GetGlobalInt("big_inv") == 1)
			{
				num = add_to_empty_stacks(item, num, 20, 20 + n_slots_per_page, special);
			}
		}
		return num;
	}

	private int add_to_preexisting_stacks(new_inv_item item, int curr_count, int startIndex, int endIndex)
	{
		int num = get_largest_stack(item.max_stack);
		int num2 = curr_count;
		if (num > 1)
		{
			for (int i = startIndex; i < endIndex; i++)
			{
				if (inventory_objType[i] == item.name && inventory_count[i] != num)
				{
					int num3 = num - inventory_count[i];
					if (num2 <= num3)
					{
						inventory_count[i] += num2;
						num2 = 0;
						break;
					}
					num2 -= num3;
					inventory_count[i] = num;
				}
			}
		}
		return num2;
	}

	private int add_to_empty_stacks(new_inv_item item, int curr_count, int startIndex, int endIndex, string special)
	{
		int num = get_largest_stack(item.max_stack);
		int num2 = curr_count;
		for (int i = startIndex; i < endIndex; i++)
		{
			if (inventory_objType[i] == "")
			{
				inventory_objType[i] = item.name;
				inventory_special[i] = special;
				if (num2 <= num)
				{
					inventory_count[i] = num2;
					num2 = 0;
					break;
				}
				inventory_count[i] = num;
				num2 -= num;
			}
		}
		return num2;
	}

	private int get_largest_stack(stacksize size)
	{
		switch (size)
		{
		case stacksize.one:
			return 1;
		case stacksize.one_thousand:
			return 1000;
		default:
			return 1;
		}
	}

	public void inv_page_switch(int ID)
	{
		NewAudioControl.Instance.play_generic_click();
		current_inv_pageNo = ID;
		redraw_inventory();
		switch (ID)
		{
		case 1:
			invp1.color = inv_page_sel;
			invp2.color = inv_page_desel;
			break;
		case 2:
			invp1.color = inv_page_desel;
			invp2.color = inv_page_sel;
			break;
		}
	}

	private int pgmod()
	{
		if (current_inv_pageNo != 2)
		{
			return 0;
		}
		return 20;
	}

	public Sprite get_coin_sprite(int count)
	{
		if (count == 1)
		{
			return coin_sprites[0];
		}
		if (count < 13)
		{
			return coin_sprites[1];
		}
		if (count < 100)
		{
			return coin_sprites[2];
		}
		if (count < 400)
		{
			return coin_sprites[3];
		}
		if (count < 1000)
		{
			return coin_sprites[4];
		}
		return coin_sprites[5];
	}

	public void redraw_container()
	{
		for (int i = 0; i < n_slots_per_page; i++)
		{
			instantiated_inv_slots[i].GetComponent<Image>().color = Color.white;
			instantiated_inv_slots[i].GetComponent<Image>().DisableSpriteOptimizations();
			instantiated_inv_slots[i].transform.Find("Req").gameObject.SetActive(false);
			if (container_objType[i] == "")
			{
				instantiated_inv_slots[i].GetComponent<Image>().sprite = empty_sprite;
				instantiated_inv_slots[i].transform.Find("Count").gameObject.SetActive(false);
				instantiated_inv_slots[i].transform.Find("special").gameObject.SetActive(false);
				instantiated_inv_slots[i].transform.Find("special2").gameObject.SetActive(false);
				continue;
			}
			new_inv_item new_inv_item = new_inv_items_by_name[container_objType[i]];
			if (new_inv_item.name == "Coins")
			{
				instantiated_inv_slots[i].GetComponent<Image>().sprite = get_coin_sprite(container_count[i]);
			}
			else
			{
				instantiated_inv_slots[i].GetComponent<Image>().sprite = new_inv_item.inventory_sprite;
			}
			if (container_count[i] == 1 && new_inv_item.name != "Coins")
			{
				instantiated_inv_slots[i].transform.Find("Count").gameObject.SetActive(false);
			}
			else
			{
				instantiated_inv_slots[i].transform.Find("Count").gameObject.SetActive(true);
				instantiated_inv_slots[i].transform.Find("Count").Find("Text").gameObject.GetComponent<Text>().text = string.Concat(container_count[i]);
			}
			if (new_inv_item.name == "Egg")
			{
				instantiated_inv_slots[i].transform.Find("special").gameObject.SetActive(true);
				instantiated_inv_slots[i].transform.Find("special").GetComponent<Image>().color = NewMobControl.Instance.string_to_color(container_special[i + pgmod()] + container_special[i + pgmod()]);
				instantiated_inv_slots[i].transform.Find("special").GetComponent<Image>().DisableSpriteOptimizations();
				instantiated_inv_slots[i].transform.Find("special2").gameObject.SetActive(true);
				int num = Loader.Instance.temp_static_names_list.IndexOf(container_special[i + pgmod()]);
				if (num != -1)
				{
					instantiated_inv_slots[i].transform.Find("special2").GetComponent<Image>().sprite = Loader.Instance.creatureSprites[num];
				}
			}
			else
			{
				instantiated_inv_slots[i].transform.Find("special").gameObject.SetActive(false);
				instantiated_inv_slots[i].transform.Find("special2").gameObject.SetActive(false);
			}
		}
	}

	private void redraw_inventory()
	{
		for (int i = 0; i < n_slots_per_page; i++)
		{
			if (inventory_objType[i + pgmod()] == "")
			{
				instantiated_inv_slots[i].GetComponent<Image>().color = Color.white;
				instantiated_inv_slots[i].GetComponent<Image>().sprite = empty_sprite;
				instantiated_inv_slots[i].transform.Find("Count").gameObject.SetActive(false);
				instantiated_inv_slots[i].transform.Find("Req").gameObject.SetActive(false);
				instantiated_inv_slots[i].transform.Find("special").gameObject.SetActive(false);
				instantiated_inv_slots[i].transform.Find("special2").gameObject.SetActive(false);
			}
			else
			{
				new_inv_item new_inv_item = new_inv_items_by_name[inventory_objType[i + pgmod()]];
				if (new_inv_item.name == "Coins")
				{
					instantiated_inv_slots[i].GetComponent<Image>().sprite = get_coin_sprite(inventory_count[i + pgmod()]);
				}
				else
				{
					instantiated_inv_slots[i].GetComponent<Image>().sprite = new_inv_item.inventory_sprite;
				}
				if (inventory_count[i + pgmod()] == 1 && new_inv_item.name != "Coins")
				{
					instantiated_inv_slots[i].transform.Find("Count").gameObject.SetActive(false);
				}
				else
				{
					instantiated_inv_slots[i].transform.Find("Count").gameObject.SetActive(true);
					instantiated_inv_slots[i].transform.Find("Count").Find("Text").gameObject.GetComponent<Text>().text = string.Concat(inventory_count[i + pgmod()]);
					instantiated_inv_slots[i].transform.Find("Count").Find("Text").gameObject.GetComponent<Text>().color = new Color(1f, 0.936f, 0.1839f);
				}
				if (new_inv_item.equip_required_stat != "" && !is_sale)
				{
					if (!check_required_skill(new_inv_item.equip_required_stat, new_inv_item.equip_required_stat_lvl))
					{
						instantiated_inv_slots[i].transform.Find("Req").gameObject.SetActive(true);
						instantiated_inv_slots[i].transform.Find("Req").Find("Text").GetComponent<Text>()
							.text = new_inv_item.equip_required_stat + " " + new_inv_item.equip_required_stat_lvl + "+";
					}
					else
					{
						instantiated_inv_slots[i].transform.Find("Req").gameObject.SetActive(false);
					}
				}
				else
				{
					instantiated_inv_slots[i].transform.Find("Req").gameObject.SetActive(false);
				}
				if (is_sale)
				{
					if (new_inv_item.name == "Coins")
					{
						instantiated_inv_slots[i].GetComponent<Image>().color = Color.white;
					}
					else
					{
						bool flag = false;
						if (DialogueControl.Instance.curr_NPC == 3)
						{
							if (new_inv_item.market_cost != 0)
							{
								flag = true;
							}
						}
						else if (curr_buyback_list.Contains(new_inv_item.name))
						{
							flag = true;
						}
						if (flag)
						{
							instantiated_inv_slots[i].transform.Find("Count").gameObject.SetActive(true);
							instantiated_inv_slots[i].transform.Find("Count").Find("Text").gameObject.GetComponent<Text>().text = "$" + (int)((float)new_inv_item.market_cost * 0.6f);
							instantiated_inv_slots[i].GetComponent<Image>().color = Color.white;
							instantiated_inv_slots[i].transform.Find("Count").Find("Text").gameObject.GetComponent<Text>().color = new Color(0.66f, 1f, 0.38f);
						}
						else
						{
							instantiated_inv_slots[i].GetComponent<Image>().color = new Color(0.16f, 0.19f, 0.29f);
							if (one_sellable)
							{
								instantiated_inv_slots[i].transform.Find("Count").gameObject.SetActive(false);
							}
							else
							{
								instantiated_inv_slots[i].transform.Find("Count").gameObject.SetActive(true);
								instantiated_inv_slots[i].transform.Find("Count").Find("Text").gameObject.GetComponent<Text>().text = "$0";
								instantiated_inv_slots[i].transform.Find("Count").Find("Text").gameObject.GetComponent<Text>().color = new Color(0.5f, 0.5f, 0.5f);
							}
						}
					}
				}
				else
				{
					instantiated_inv_slots[i].GetComponent<Image>().color = Color.white;
				}
				if (new_inv_item.name == "Egg")
				{
					instantiated_inv_slots[i].transform.Find("special").gameObject.SetActive(true);
					instantiated_inv_slots[i].transform.Find("special").GetComponent<Image>().color = NewMobControl.Instance.string_to_color(inventory_special[i + pgmod()] + inventory_special[i + pgmod()]);
					instantiated_inv_slots[i].transform.Find("special").GetComponent<Image>().DisableSpriteOptimizations();
					instantiated_inv_slots[i].transform.Find("special2").gameObject.SetActive(true);
					int num = Loader.Instance.temp_static_names_list.IndexOf(inventory_special[i + pgmod()]);
					if (num != -1)
					{
						instantiated_inv_slots[i].transform.Find("special2").GetComponent<Image>().sprite = Loader.Instance.creatureSprites[num];
					}
				}
				else
				{
					instantiated_inv_slots[i].transform.Find("special").gameObject.SetActive(false);
					instantiated_inv_slots[i].transform.Find("special2").gameObject.SetActive(false);
				}
			}
			instantiated_inv_slots[i].GetComponent<Image>().DisableSpriteOptimizations();
		}
		if (inventory_objType[hand_index] == "")
		{
			instantiated_inv_slots[hand_index].GetComponent<Image>().sprite = blank_hand_sprite;
		}
		else
		{
			new_inv_item new_inv_item2 = new_inv_items_by_name[inventory_objType[hand_index]];
			instantiated_inv_slots[hand_index].GetComponent<Image>().sprite = new_inv_item2.inventory_sprite;
		}
		if (inventory_objType[hat_index] == "")
		{
			instantiated_inv_slots[hat_index].GetComponent<Image>().sprite = blank_hat_sprite;
		}
		else
		{
			new_inv_item new_inv_item3 = new_inv_items_by_name[inventory_objType[hat_index]];
			instantiated_inv_slots[hat_index].GetComponent<Image>().sprite = new_inv_item3.inventory_sprite;
		}
		if (inventory_objType[body_index] == "")
		{
			instantiated_inv_slots[body_index].GetComponent<Image>().sprite = blank_body_sprite;
			return;
		}
		new_inv_item new_inv_item4 = new_inv_items_by_name[inventory_objType[body_index]];
		instantiated_inv_slots[body_index].GetComponent<Image>().sprite = new_inv_item4.inventory_sprite;
	}

	private void open_inventory_tab()
	{
		trash_bin.SetActive(true);
		inv_tab_button.color = tab_selected_color;
		craft_tab_button.color = tab_deselected_color;
		inventory_Tab.SetActive(true);
		crafting_Tab.SetActive(false);
		gui_loot_respawn_time.SetActive(false);
		extra_slots_enabled(true);
		slot_parent.transform.localPosition = standard_slots_position;
		for_slots.transform.localPosition = Vector2.zero;
		((RectTransform)slot_parent.transform).sizeDelta = new Vector2(981f, 604f);
		one_sellable = false;
		if (is_sale)
		{
			for (int i = 0; i < n_slots_per_page; i++)
			{
				string text = inventory_objType[i + pgmod()];
				if (text == "")
				{
					continue;
				}
				new_inv_item new_inv_item = new_inv_items_by_name[text];
				if (DialogueControl.Instance.curr_NPC == 3)
				{
					if (new_inv_item.market_cost != 0)
					{
						one_sellable = true;
						break;
					}
				}
				else if (curr_buyback_list.Contains(new_inv_item.name))
				{
					one_sellable = true;
					break;
				}
			}
		}
		redraw_inventory();
		if (PlayerData.Instance.GetGlobalInt("big_inv") == 1)
		{
			pageSwitchers.SetActive(true);
			double_tap_text.SetActive(false);
		}
		if (INTERACT_W_CONTAINER)
		{
			transfer_to.SetActive(false);
			transfer_from.SetActive(true);
		}
		else
		{
			transfer_to.SetActive(false);
			transfer_from.SetActive(false);
		}
	}

	private void extra_slots_enabled(bool state)
	{
		instantiated_inv_slots[3].SetActive(state);
		instantiated_inv_slots[4].SetActive(state);
		instantiated_inv_slots[8].SetActive(state);
		instantiated_inv_slots[9].SetActive(state);
		instantiated_inv_slots[13].SetActive(state);
		instantiated_inv_slots[14].SetActive(state);
	}

	private void open_crafting_tab()
	{
		inv_tab_button.color = tab_deselected_color;
		craft_tab_button.color = tab_selected_color;
		pageSwitchers.SetActive(false);
		if (INTERACT_W_CONTAINER)
		{
			trash_bin.SetActive(false);
			inventory_Tab.SetActive(true);
			crafting_Tab.SetActive(false);
			transfer_to.SetActive(true);
			transfer_from.SetActive(false);
			if (CHEST_TYPE == chest_type.gold_chest || CHEST_TYPE == chest_type.titanium_chest)
			{
				gui_loot_respawn_time.SetActive(true);
			}
			else
			{
				gui_loot_respawn_time.SetActive(false);
			}
			extra_slots_enabled(CHEST_TYPE == chest_type.personal_chest);
			if (CHEST_TYPE == chest_type.personal_chest)
			{
				slot_parent.transform.localPosition = new Vector2(70f, -16f);
				transfer_to.transform.localPosition = new Vector2(-582f, 92f);
				for_slots.transform.localPosition = Vector2.zero;
				((RectTransform)slot_parent.transform).sizeDelta = new Vector2(981f, 604f);
			}
			else
			{
				slot_parent.transform.localPosition = new Vector2(0f, -16f);
				transfer_to.transform.localPosition = new Vector2(-398f, 92f);
				for_slots.transform.localPosition = new Vector2(187f, 0f);
				((RectTransform)slot_parent.transform).sizeDelta = new Vector2(614f, 604f);
			}
			redraw_container();
		}
		else
		{
			inventory_Tab.SetActive(false);
			crafting_Tab.SetActive(true);
			transfer_to.SetActive(false);
			transfer_from.SetActive(false);
			refresh_craft_slots();
		}
	}

	private void refresh_craft_slots()
	{
		for (int i = 0; i < 3; i++)
		{
			int num = craft_PAGE * 3 + i;
			if (num >= craftList.Length)
			{
				instantiated_crafting_slots[i].gameObject.SetActive(false);
				continue;
			}
			instantiated_crafting_slots[i].GetComponent<crafting_slot>().set_graphic(new_inv_items_by_name[craftList[num]], is_sale);
			instantiated_crafting_slots[i].gameObject.SetActive(true);
		}
	}

	public void slot_mouse_down(int index, GameObject mouse_down_slot)
	{
		if (!allowClose)
		{
			return;
		}
		if (CURRENT_TAB == tab.left)
		{
			int num = -1;
			num = ((index == hand_index) ? hand_index : ((index == hat_index) ? hat_index : ((index != body_index) ? (index + pgmod()) : body_index)));
			if (inventory_objType[num] == "")
			{
				return;
			}
		}
		else if (INTERACT_W_CONTAINER && container_objType[index] == "")
		{
			return;
		}
		if (!INTERACT_W_CONTAINER)
		{
			if (mouse_down_slot != prev_mousedown_slot)
			{
				check_double_click = false;
			}
			prev_mousedown_slot = mouse_down_slot;
			if (check_double_click)
			{
				StartCoroutine(ON_DOUBLE_CLICK(index, mouse_down_slot));
				check_double_click = false;
				StopCoroutine(W_DOUBLE_CLICK);
				return;
			}
			if (W_DOUBLE_CLICK != null)
			{
				StopCoroutine(W_DOUBLE_CLICK);
			}
			W_DOUBLE_CLICK = w_double_click();
			StartCoroutine(W_DOUBLE_CLICK);
		}
		this.mouse_down_slot = mouse_down_slot;
		test_for_dragging = true;
		initial_press = Input.mousePosition;
	}

	public void Complete_Sell()
	{
		string key = inventory_objType[sell_index];
		inventory_objType[sell_index] = "";
		inventory_special[sell_index] = "";
		inventory_count[sell_index] = 0;
		try_give_item("Coins", (int)((float)new_inv_items_by_name[key].market_cost * 0.6f), GameController.Instance.player.transform.position, "", false);
		redraw_inventory();
		mouse_down_slot.GetComponent<Animation>().Play();
		show_angular(mouse_down_slot.transform.localPosition);
		GameController.Instance.sound_craft();
	}

	public void perk_item(string perk_key, float effectA_amount)
	{
		perk_controller.Instance.apply_perk_effects(perk_key, effectA_amount, 0f, null, GameController.Instance.player.GetComponent<creatureScript>(), true);
		unlock();
	}

	private IEnumerator ON_DOUBLE_CLICK(int inventory_slot_id, GameObject mouse_down_slot)
	{
		int inv_index = inventory_slot_id + pgmod();
		new_inv_item item_clicked = new_inv_items_by_name[inventory_objType[inv_index]];
		if (is_sale)
		{
			if (item_clicked.name == "Coins")
			{
				yield break;
			}
			if (DialogueControl.Instance.curr_NPC == 3)
			{
				if (item_clicked.market_cost == 0)
				{
					PopupControl.Instance.ShowMessage(DialogueControl.Instance.NPC_names[DialogueControl.Instance.curr_NPC] + " doesn't want that.");
					yield break;
				}
			}
			else if (!curr_buyback_list.Contains(item_clicked.name))
			{
				PopupControl.Instance.ShowMessage(DialogueControl.Instance.NPC_names[DialogueControl.Instance.curr_NPC] + " doesn't want that.");
				yield break;
			}
			PopupControl.Instance.ShowYesNo("Sell your <color=#5b9aff>" + item_clicked.name + "</color> for <color=#ffff5b>" + (int)((float)item_clicked.market_cost * 0.6f) + " gold?</color>", "Yes", "No", PopupControl.context.sell);
			sell_index = inv_index;
			yield break;
		}
		bool remove_from_inventory_after_use;
		if (item_clicked.type == inv_type_t.place_in_world)
		{
			remove_from_inventory_after_use = true;
			click_to_place_txt.text = "Click to PLACE " + item_clicked.name;
		}
		else if (item_clicked.type == inv_type_t.tool)
		{
			remove_from_inventory_after_use = false;
			switch (item_clicked.name)
			{
			case "SledgeHammer":
				click_to_place_txt.text = "Click to PICK UP furniture";
				break;
			case "Wrench":
				click_to_place_txt.text = "Click to ROTATE furniture";
				break;
			case "Shovel":
				click_to_place_txt.text = "Click to PICK UP bushes.";
				break;
			}
		}
		else
		{
			if (item_clicked.type != inv_type_t.consumable)
			{
				yield break;
			}
			remove_from_inventory_after_use = true;
		}
		allowClose = false;
		mouse_down_slot.GetComponent<Animation>().Play();
		show_angular(mouse_down_slot.transform.localPosition);
		GameController.Instance.sound_craft();
		yield return new WaitForSeconds(0.5f);
		if (item_clicked.type == inv_type_t.consumable)
		{
			int num = 0;
			switch (item_clicked.name)
			{
			case "Health Potion":
				num = 30;
				break;
			case "Toasted Nuts":
				num = 5;
				break;
			case "Raisin Bread":
				close_inv_window();
				perk_item("perk_giant", 40f);
				break;
			case "Rushers":
				close_inv_window();
				perk_item("perk_sprint", 30f);
				break;
			}
			if (num != 0)
			{
				GameController.Instance.player.GetComponent<NewCombatant>().HP = Mathf.Min(GameController.Instance.player.GetComponent<NewCombatant>().HP + (float)num, GameController.Instance.player.GetComponent<NewCombatant>().HP_start);
				GameController.Instance.player.GetComponent<NewCombatant>().set_heath_visual(0);
			}
		}
		else if (item_clicked.type == inv_type_t.tool)
		{
			ITEM_USING = inventory_objType[inv_index];
			PLACING_OBJECT_OR_USING_TOOL = true;
			click_to_place.SetActive(true);
			show_DONE_button("DONE", button_state.DONE_USING_TOOL);
			close_inv_window();
		}
		else if (item_clicked.type == inv_type_t.place_in_world)
		{
			ITEM_USING = inventory_objType[inv_index];
			PLACING_OBJECT_OR_USING_TOOL = true;
			click_to_place.SetActive(true);
			show_DONE_button("CANCEL", button_state.CANCEL_PLACE_OBJ);
			close_inv_window();
		}
		if (remove_from_inventory_after_use)
		{
			inventory_count[inv_index]--;
			PlayerData.Instance.SetSlotInt("inventoryCount" + inv_index, inventory_count[inv_index], PlayerData.grouping_t.the_inventory);
			if (inventory_count[inv_index] == 0)
			{
				inventory_objType[inv_index] = "";
				inventory_special[inv_index] = "";
				PlayerData.Instance.SetSlotString("inventorySpecial" + inv_index, "", PlayerData.grouping_t.the_inventory);
				PlayerData.Instance.SetSlotString("inventory" + inv_index, "", PlayerData.grouping_t.the_inventory);
			}
			redraw_inventory();
		}
		allowClose = true;
	}

	private void close_inv_window()
	{
		inventory_crafting_screen.SetActive(false);
		is_open = false;
	}

	private IEnumerator w_double_click()
	{
		check_double_click = true;
		yield return new WaitForSeconds(0.7f);
		check_double_click = false;
		W_DOUBLE_CLICK = null;
	}

	private bool page_exists(int mod)
	{
		switch (mod)
		{
		case 1:
			return (craft_PAGE + 1) * 3 < craftList.Length;
		case -1:
			return craft_PAGE * 3 > 0;
		default:
			return false;
		}
	}

	private void do_cascade_craft_buts(int dir)
	{
		if (CASCADE_CRAFT_BUTS != null)
		{
			StopCoroutine(CASCADE_CRAFT_BUTS);
		}
		CASCADE_CRAFT_BUTS = cascade_craft_buts(dir);
		StartCoroutine(CASCADE_CRAFT_BUTS);
	}

	private IEnumerator cascade_craft_buts(int dir)
	{
		for (int i = 0; i < 3; i++)
		{
			int index = ((dir != -1) ? (2 - i) : i);
			instantiated_crafting_slots[index].GetComponent<Animation>().Stop();
			instantiated_crafting_slots[index].GetComponent<Animation>().Play();
			yield return new WaitForSeconds(0.01f);
		}
		CASCADE_CRAFT_BUTS = null;
	}
}
