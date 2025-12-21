using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class perk_controller : MonoBehaviour
{
	public enum rarity
	{
		common = 0,
		uncommon = 1
	}

	public enum cast
	{
		PROJECTILE = 0,
		SELF = 1
	}

	public enum effect
	{
		A = 0,
		B = 1
	}

	public Color unavailable_for_casting;

	public Color available_for_casting;

	public AudioSource sfx_source;

	public AudioClip maxout;

	public AudioClip resolve;

	public static perk_controller Instance;

	public static float mana_available = 1f;

	public static float max_mana = 1f;

	public static float mana_recharge_rate = 0.8f;

	public GameObject prefab_darksword_kill;

	public Text managePage_infoDisplay_title;

	public Text managePage_infoDisplay_desc;

	public Text managePage_infoDisplay_level;

	public Image managePage_infoDisplay_img;

	public AudioClip sound_fire;

	public AudioClip sound_lightning;

	public AudioClip sound_web;

	public AudioClip sound_screech;

	public AudioClip sound_force;

	public AudioClip sound_heal;

	public AudioClip sound_giant;

	public AudioClip sound_ram;

	public AudioClip sound_sting;

	public AudioClip sound_eagle;

	public AudioClip sound_sprint;

	public GameObject type_fireball;

	public GameObject type_lightning;

	public GameObject type_web;

	public GameObject type_forcePush;

	public GameObject type_Screech;

	public GameObject type_weaken;

	public GameObject type_sting;

	public GameObject type_heal;

	public GameObject type_sprint;

	public GameObject glob_web;

	public GameObject glob_screech;

	public GameObject glob_fire;

	public GameObject perk_GET_screen;

	public Sprite s_fireball;

	public Sprite s_lightning;

	public Sprite s_forcePush;

	public Sprite s_bind;

	public Sprite s_confuse;

	public Sprite s_weaken;

	public Sprite s_heal;

	public Sprite s_ram;

	public Sprite s_giant;

	public Sprite s_sting;

	public Sprite s_eagleEye;

	public Sprite s_sprint;

	public Dictionary<string, object> perk_defs = new Dictionary<string, object>();

	private string[] perk_layout = new string[17]
	{
		"perk_forcePsh", "perk_heal", "perk_giant", "perk_sprint", "perk_fireball", "perk_lightning", "perk_sting", "perk_ram", "", "",
		"perk_confuse", "perk_weaken", "perk_bind", "", "", "", "perk_eagleEye"
	};

	public GameObject perk_manage_screen;

	public GameObject perk_info;

	private GameObject recently_instantiated_selfCast_particle;

	public int about_to_cast_lvl;

	public string about_to_cast_str;

	public float costA;

	public float costB;

	public GameObject slotA;

	public GameObject slotB;

	public GameObject slotA_main;

	public GameObject slotB_main;

	public bool autoequip;

	public GameObject[] perk_unlocked_slots;

	private Vector2 managePage_infoDisplay_title_original_pos;

	public GameObject curr_sel_perk;

	public Sprite not_unlocked;

	public string curr_sel_perk_key;

	public Text mana_text;

	public Image mana_foreground;

	public Image mana_mask;

	private Vector2 mana_mask_start_pos;

	private Vector2 mana_overlay_start_pos;

	private float prevMana;

	public static float sample_max_perk_level = 15f;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		mana_mask_start_pos = mana_mask.transform.localPosition;
		mana_overlay_start_pos = mana_foreground.transform.localPosition;
		StartCoroutine(regen_mana());
		do_define_perk("Eagle Eye", rarity.common, s_eagleEye, "perk_eagleEye", cast.SELF, null, sound_eagle, "See a lot further!", 4, 10, "Seconds ", 10, 20);
		do_define_perk("Giant", rarity.uncommon, s_giant, "perk_giant", cast.SELF, null, sound_giant, "Squish other animals", 5, 22, "Seconds ", 7, 14);
		do_define_perk("Ram", rarity.common, s_ram, "perk_ram", cast.PROJECTILE, null, sound_ram, "Ram into your enemy!", 5, 16, "Damage ", 2, 9);
		do_define_perk("Heal", rarity.common, s_heal, "perk_heal", cast.SELF, type_heal, sound_heal, "Heals your wounds", 4, 15, "Health +", 3, 15);
		do_define_perk("Poison Sting", rarity.common, s_sting, "perk_sting", cast.PROJECTILE, type_sting, sound_sting, "Poisons your enemy", 5, 15, "Damage/Second ", 1, 4, "Seconds ", 3, 5);
		do_define_perk("Fireball", rarity.common, s_fireball, "perk_fireball", cast.PROJECTILE, type_fireball, sound_fire, "Shoot a fireball at your enemy", 5, 11, "Damage ", 3, 20);
		do_define_perk("Lightning", rarity.common, s_lightning, "perk_lightning", cast.PROJECTILE, type_lightning, sound_lightning, "Shoot a thunderbolt at your enemy", 5, 11, "Damage ", 3, 20);
		do_define_perk("Force Push", rarity.common, s_forcePush, "perk_forcePsh", cast.SELF, type_forcePush, sound_force, "Push all nearby animals away.", 3, 7, "Size ", 3, 5, "Extra Damage ", 1, 10);
		do_define_perk("Spider Web", rarity.common, s_bind, "perk_bind", cast.PROJECTILE, type_web, sound_web, "Stops your enemy from moving", 5, 15, "Seconds ", 3, 10);
		do_define_perk("Screech", rarity.common, s_confuse, "perk_confuse", cast.PROJECTILE, type_Screech, sound_screech, "Confuses your enemy ", 3, 7, "Seconds ", 6, 20, "Enemy Accuracy -", 3, 17);
		do_define_perk("Weaken", rarity.common, s_weaken, "perk_weaken", cast.PROJECTILE, type_weaken, sound_screech, "Makes your enemy less strong", 3, 7, "Seconds ", 6, 20, "Enemy Attack -", 3, 17);
		do_define_perk("Sprint", rarity.common, s_sprint, "perk_sprint", cast.SELF, type_sprint, sound_sprint, "Run quickly", 5, 22, "Seconds ", 7, 14);
		for (int i = 0; i < perk_unlocked_slots.Length; i++)
		{
			perk_unlocked_slots[i].GetComponent<perk_slot>().INDEX = i;
		}
		managePage_infoDisplay_title_original_pos = managePage_infoDisplay_title.transform.localPosition;
	}

	public void apply_perk_effects(string PERK_KEY, float effectA_amount, float effectB_amount, GameObject target, creatureScript shooter, bool sound = false)
	{
		Dictionary<string, object> dictionary = (Dictionary<string, object>)perk_defs[PERK_KEY];
		if (sound)
		{
			GameController.Instance.sfx.PlayOneShot((AudioClip)dictionary["SFX"]);
		}
		if ((cast)dictionary["CAST"] == cast.SELF)
		{
			GameObject gameObject = (GameObject)dictionary["projectile"];
			if (gameObject != null)
			{
				recently_instantiated_selfCast_particle = Object.Instantiate(gameObject);
				recently_instantiated_selfCast_particle.transform.position = new Vector3(GameController.Instance.player.transform.position.x, 0.6f, GameController.Instance.player.transform.position.z);
			}
		}
		switch (PERK_KEY)
		{
		case "perk_sprint":
			shooter.GetComponent<NewCombatant>().add_effect((int)effectA_amount, NewCombatant.effect.sprint, 1f);
			break;
		case "perk_eagleEye":
			shooter.GetComponent<NewCombatant>().add_effect((int)effectA_amount, NewCombatant.effect.eagleEye, 1f);
			break;
		case "perk_giant":
			shooter.GetComponent<NewCombatant>().add_effect((int)effectA_amount, NewCombatant.effect.giant, 1f);
			break;
		case "perk_heal":
			shooter.GetComponent<NewCombatant>().HP = Mathf.Min(shooter.GetComponent<NewCombatant>().HP + (float)(int)effectA_amount, shooter.GetComponent<NewCombatant>().HP_start);
			shooter.GetComponent<NewCombatant>().set_heath_visual(0);
			break;
		case "perk_confuse":
			target.GetComponent<NewCombatant>().add_effect((int)effectA_amount, NewCombatant.effect.accuracy, effectB_amount);
			break;
		case "perk_weaken":
			target.GetComponent<NewCombatant>().add_effect((int)effectA_amount, NewCombatant.effect.out_dmg, effectB_amount);
			break;
		case "perk_bind":
			target.GetComponent<NewCombatant>().add_effect((int)effectA_amount, NewCombatant.effect.bind, 1f);
			break;
		case "perk_sting":
			target.GetComponent<NewCombatant>().who_poisoned_me = shooter;
			target.GetComponent<NewCombatant>().add_effect((int)effectB_amount, NewCombatant.effect.poison, effectA_amount);
			break;
		case "perk_fireball":
		case "perk_lightning":
			target.GetComponent<NewCombatant>().wasHit((int)effectA_amount, shooter, false, NewCombatant.hit_col.color_red);
			break;
		case "perk_forcePsh":
			recently_instantiated_selfCast_particle.transform.localScale = effectA_amount * Vector3.one * 0.75f;
			{
				foreach (GameObject active_combatant in NewMobControl.Instance.active_combatants)
				{
					if (!(active_combatant == shooter.gameObject) && Vector3.Distance(shooter.transform.position, active_combatant.transform.position) < effectA_amount)
					{
						if (active_combatant.GetComponent<Rigidbody>() != null)
						{
							Vector3 normalized = (shooter.transform.position - active_combatant.transform.position).normalized;
							active_combatant.GetComponent<Rigidbody>().velocity -= normalized * 13f;
						}
						active_combatant.GetComponent<NewCombatant>().wasHit((int)effectB_amount, shooter, false, NewCombatant.hit_col.color_red);
						if (active_combatant.GetComponent<NewCombatant>().mob_type == NewCombatant.TYPE_T.creature && active_combatant.GetComponent<creatureScript>().STATE != GameController.creatureStates.friendly && active_combatant.GetComponent<creatureScript>().STATE != GameController.creatureStates.fearful)
						{
							active_combatant.GetComponent<creatureScript>().targetCombatant = shooter.gameObject;
						}
					}
				}
				break;
			}
		default:
			Debug.Log("no effects specified.");
			break;
		}
	}

	public void NATURAL_PERK(GameObject target, creatureScript shooter, string PERK_KEY, int PERK_LEVEL, GameObject caller)
	{
		float effectA_amount = Instance.calc_effect_amount(effect.A, PERK_KEY, PERK_LEVEL);
		float effectB_amount = 0f;
		if (((Dictionary<string, object>)perk_defs[PERK_KEY]).ContainsKey("effectB"))
		{
			effectB_amount = Instance.calc_effect_amount(effect.B, PERK_KEY, PERK_LEVEL);
		}
		apply_perk_effects(PERK_KEY, effectA_amount, effectB_amount, target, shooter);
	}

	private void desel_curr_perk()
	{
		curr_sel_perk.GetComponent<Animation>().Stop();
		curr_sel_perk.GetComponent<Image>().raycastTarget = true;
		curr_sel_perk.transform.localScale = Vector3.one;
	}

	public void open_perk_manage_screen()
	{
		if (Shop_positioner.Instance.try_open_generic_window())
		{
			if (curr_sel_perk != null)
			{
				desel_curr_perk();
			}
			curr_sel_perk = null;
			perk_info.SetActive(false);
			perk_manage_screen.SetActive(true);
			WindowControl.Instance.ShowClose(WindowControl.window_type_t.perkmanage);
		}
	}

	public void CAST(GameObject caster, string key, int level, GameObject target)
	{
		Dictionary<string, object> dictionary = (Dictionary<string, object>)perk_defs[key];
		GameController.Instance.sfx.PlayOneShot((AudioClip)dictionary["SFX"]);
		if ((cast)dictionary["CAST"] == cast.SELF)
		{
			NATURAL_PERK(null, GameController.Instance.player.GetComponent<creatureScript>(), key, level, base.gameObject);
		}
		else if (key == "perk_ram")
		{
			caster.GetComponent<creatureScript>().generic_moveTo = Vector3.zero;
			caster.GetComponent<creatureScript>().targetCombatant = target;
			caster.GetComponent<creatureScript>().isRamming = true;
			caster.GetComponent<creatureScript>().ramDMG = Instance.calc_effect_amount(effect.A, key, level);
		}
		else
		{
			GameObject obj = Object.Instantiate((GameObject)dictionary["projectile"]);
			obj.transform.position = new Vector3(caster.transform.position.x, 0.6f, caster.transform.position.z);
			obj.GetComponent<projectile>().SHOOT_AT(key, level, target, 0.22f, caster.GetComponent<creatureScript>());
		}
	}

	public void cast_perk(int index)
	{
		GameController.Instance.button_clicked = true;
		if (Shop_positioner.Instance.disable_top_left_buttons)
		{
			return;
		}
		string slotString = PlayerData.Instance.GetSlotString((index == 0) ? "perk_slot_A" : "perk_slot_B", PlayerData.grouping_t.perks);
		int slotInt = PlayerData.Instance.GetSlotInt(slotString, PlayerData.grouping_t.perks);
		if (!perk_defs.ContainsKey(slotString))
		{
			return;
		}
		Dictionary<string, object> dictionary = (Dictionary<string, object>)perk_defs[slotString];
		float num = get_cost(slotString);
		if (mana_available - num >= 0f)
		{
			if ((cast)dictionary["CAST"] == cast.SELF)
			{
				CAST(GameController.Instance.player, slotString, slotInt, null);
				spendMana(num);
				return;
			}
			GameController.Instance.is_casting_spell = true;
			Shop_positioner.Instance.HideGameplayGUI();
			inventory_ctr.Instance.show_DONE_button("CANCEL", inventory_ctr.button_state.CAST_PROJECTILE);
			inventory_ctr.Instance.click_to_place.SetActive(true);
			inventory_ctr.Instance.click_to_place_txt.text = "Click an enemy!";
			about_to_cast_str = slotString;
			about_to_cast_lvl = slotInt;
		}
	}

	public float get_cost(string key)
	{
		Dictionary<string, object> dictionary = (Dictionary<string, object>)perk_defs[key];
		float t = (float)PlayerData.Instance.GetSlotInt(key, PlayerData.grouping_t.perks) / sample_max_perk_level;
		return Mathf.Lerp((int)dictionary["cost_base"], (int)dictionary["cost_max"], t);
	}

	public void spendMana(float cost)
	{
		mana_available -= cost;
		update_mana_visual();
		update_main_slots_color();
	}

	public void update_main_slots_color()
	{
		slotA_main.GetComponent<Image>().color = ((mana_available < costA) ? unavailable_for_casting : available_for_casting);
		slotB_main.GetComponent<Image>().color = ((mana_available < costB) ? unavailable_for_casting : available_for_casting);
		slotA_main.GetComponent<Image>().DisableSpriteOptimizations();
		slotB_main.GetComponent<Image>().DisableSpriteOptimizations();
	}

	public void on_equip_perk_pressed(int index)
	{
		string key = "";
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		switch (index)
		{
		case 0:
			gameObject = slotA;
			gameObject2 = slotA_main;
			costA = get_cost(curr_sel_perk_key);
			key = "perk_slot_A";
			break;
		case 1:
			gameObject = slotB;
			gameObject2 = slotB_main;
			costB = get_cost(curr_sel_perk_key);
			key = "perk_slot_B";
			slotB_main.SetActive(true);
			break;
		}
		PlayerData.Instance.SetSlotString(key, curr_sel_perk_key, PlayerData.grouping_t.perks);
		int slotInt = PlayerData.Instance.GetSlotInt(curr_sel_perk_key, PlayerData.grouping_t.perks);
		Image component = gameObject2.GetComponent<Image>();
		Sprite sprite = (gameObject.GetComponent<Image>().sprite = (Sprite)((Dictionary<string, object>)perk_defs[curr_sel_perk_key])["Spr"]);
		component.sprite = sprite;
		gameObject.GetComponent<Image>().color = Color.white;
		if (!autoequip)
		{
			gameObject.GetComponent<Animation>().Stop();
			gameObject.GetComponent<Animation>().Play();
		}
		if (slotInt == 1)
		{
			gameObject.transform.Find("Image").gameObject.SetActive(false);
			gameObject2.transform.Find("Image").gameObject.SetActive(false);
		}
		else
		{
			gameObject.transform.Find("Image").gameObject.SetActive(true);
			gameObject2.transform.Find("Image").gameObject.SetActive(true);
			gameObject.transform.Find("Image").Find("Text").gameObject.GetComponent<Text>().text = string.Concat(slotInt);
			gameObject2.transform.Find("Image").Find("Text").gameObject.GetComponent<Text>().text = string.Concat(slotInt);
		}
		autoequip = false;
	}

	public void press_close()
	{
		NewAudioControl.Instance.play_generic_click();
		perk_manage_screen.SetActive(false);
		inventory_ctr.Instance.close_generic_window();
	}

	private void convert_old_perks()
	{
	}

	public void load_saved_perks()
	{
		string slotString = PlayerData.Instance.GetSlotString("perk_slot_A", PlayerData.grouping_t.perks);
		string slotString2 = PlayerData.Instance.GetSlotString("perk_slot_B", PlayerData.grouping_t.perks);
		foreach (string key in perk_defs.Keys)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)perk_defs[key];
			int slotInt = PlayerData.Instance.GetSlotInt(key, PlayerData.grouping_t.perks);
			if (slotString != "" && key == slotString)
			{
				slotA.GetComponent<Image>().color = Color.white;
				Image component = slotA_main.GetComponent<Image>();
				Sprite sprite = (slotA.GetComponent<Image>().sprite = (Sprite)dictionary["Spr"]);
				component.sprite = sprite;
				if (slotInt != 1)
				{
					slotA.transform.Find("Image").gameObject.SetActive(true);
					slotA_main.transform.Find("Image").gameObject.SetActive(true);
					slotA.transform.Find("Image").Find("Text").gameObject.GetComponent<Text>().text = string.Concat(slotInt);
					slotA_main.transform.Find("Image").Find("Text").gameObject.GetComponent<Text>().text = string.Concat(slotInt);
				}
				costA = get_cost(key);
			}
			if (slotString2 != "" && key == slotString2)
			{
				slotB_main.SetActive(true);
				slotB.GetComponent<Image>().color = Color.white;
				Image component2 = slotB_main.GetComponent<Image>();
				Sprite sprite = (slotB.GetComponent<Image>().sprite = (Sprite)dictionary["Spr"]);
				component2.sprite = sprite;
				if (slotInt != 1)
				{
					slotB.transform.Find("Image").gameObject.SetActive(true);
					slotB_main.transform.Find("Image").gameObject.SetActive(true);
					slotB.transform.Find("Image").Find("Text").gameObject.GetComponent<Text>().text = string.Concat(slotInt);
					slotB_main.transform.Find("Image").Find("Text").gameObject.GetComponent<Text>().text = string.Concat(slotInt);
				}
				costB = get_cost(key);
			}
			try_unlock_on_table(key, slotInt, (Sprite)dictionary["Spr"]);
		}
		update_main_slots_color();
	}

	public void try_unlock_on_table(string perk_name, int LVL, Sprite spr)
	{
		for (int i = 0; i < perk_layout.Length; i++)
		{
			if (perk_layout[i] == "" || !(perk_name == perk_layout[i]))
			{
				continue;
			}
			if (LVL > 0)
			{
				perk_unlocked_slots[i].GetComponent<Image>().sprite = spr;
				if (LVL > 1)
				{
					perk_unlocked_slots[i].transform.Find("Image").gameObject.SetActive(true);
					perk_unlocked_slots[i].transform.Find("Image").Find("Text").gameObject.GetComponent<Text>().text = string.Concat(LVL);
				}
			}
			break;
		}
	}

	public void perk_clicked(int i)
	{
		if (curr_sel_perk != null)
		{
			desel_curr_perk();
		}
		curr_sel_perk = perk_unlocked_slots[i];
		if (perk_layout[i] != "")
		{
			curr_sel_perk_key = perk_layout[i];
			int slotInt = PlayerData.Instance.GetSlotInt(perk_layout[i], PlayerData.grouping_t.perks);
			if (slotInt > 0)
			{
				perk_info.SetActive(true);
				perk_info.GetComponent<Animation>().Stop();
				perk_info.GetComponent<Animation>().Play();
				Dictionary<string, object> dictionary = (Dictionary<string, object>)perk_defs[perk_layout[i]];
				managePage_infoDisplay_title.text = (string)dictionary["NAME"];
				managePage_infoDisplay_img.sprite = (Sprite)dictionary["Spr"];
				managePage_infoDisplay_desc.text = (string)dictionary["desc"];
				Text text = managePage_infoDisplay_desc;
				text.text = text.text + "\n" + get_perk_string(perk_layout[i], slotInt, false);
				if (slotInt > 1)
				{
					managePage_infoDisplay_level.gameObject.SetActive(true);
					managePage_infoDisplay_level.text = "Level " + slotInt;
					managePage_infoDisplay_title.transform.localPosition = managePage_infoDisplay_title_original_pos + Vector2.up * 23f;
				}
				else
				{
					managePage_infoDisplay_level.gameObject.SetActive(false);
					managePage_infoDisplay_title.transform.localPosition = managePage_infoDisplay_title_original_pos;
				}
			}
			else
			{
				perk_info.SetActive(false);
			}
		}
		else
		{
			perk_info.SetActive(false);
		}
	}

	public void update_mana_visual()
	{
		float num = mana_available / max_mana;
		mana_text.text = mana_available.ToString("F0") + " / " + max_mana.ToString("F0");
		mana_mask.rectTransform.sizeDelta = new Vector3(100f, num * 100f);
		mana_mask.transform.localPosition = mana_mask_start_pos + Vector2.down * (1f - num) * 50f;
		mana_foreground.transform.localPosition = mana_overlay_start_pos + Vector2.up * (1f - num) * 50f;
		update_main_slots_color();
	}

	private IEnumerator regen_mana()
	{
		while (true)
		{
			yield return new WaitForSeconds(3f);
			mana_available = Mathf.Min(max_mana, mana_available + mana_recharge_rate);
			update_mana_visual();
		}
	}

	public void do_define_perk(string perkname, rarity RARITY, Sprite Spr, string key, cast CAST, GameObject the_projectile, AudioClip SFX, string desc, int cost_base, int cost_max, string effectA, int effectA_base, int effectA_max, string effectB = "", int effectB_base = -1, int effectB_max = -1)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("NAME", perkname);
		dictionary.Add("RARITY", RARITY);
		dictionary.Add("Spr", Spr);
		dictionary.Add("desc", desc);
		dictionary.Add("CAST", CAST);
		dictionary.Add("projectile", the_projectile);
		dictionary.Add("SFX", SFX);
		dictionary.Add("cost_base", cost_base);
		dictionary.Add("cost_max", cost_max);
		dictionary.Add("effectA", effectA);
		dictionary.Add("effectA_base", effectA_base);
		dictionary.Add("effectA_max", effectA_max);
		if (effectB != "")
		{
			dictionary.Add("effectB", effectB);
			dictionary.Add("effectB_base", effectB_base);
			dictionary.Add("effectB_max", effectB_max);
		}
		perk_defs.Add(key, dictionary);
	}

	public void sound_maxout()
	{
		sfx_source.PlayOneShot(maxout);
	}

	public void show_perk_get()
	{
		perk_GET_screen.SetActive(true);
		perk_GET_screen.GetComponent<Animation>().Play();
	}

	public float calc_effect_amount(effect EFFECT, string perkKey, int lvl_of)
	{
		if (perkKey == "")
		{
			Debug.Log("ERROR!!! attempting to locate non-existent perkKey");
			return 0f;
		}
		Dictionary<string, object> dictionary = (Dictionary<string, object>)perk_defs[perkKey];
		string key = "";
		string key2 = "";
		switch (EFFECT)
		{
		case effect.A:
			key = "effectA_base";
			key2 = "effectA_max";
			break;
		case effect.B:
			key = "effectB_base";
			key2 = "effectB_max";
			break;
		}
		float a = (int)dictionary[key];
		float b = (int)dictionary[key2];
		return Mathf.Lerp(a, b, (float)lvl_of / sample_max_perk_level);
	}

	public string get_perk_string(string perkKey, int currentLevel, bool showDifferences)
	{
		string text = "";
		Dictionary<string, object> dictionary = (Dictionary<string, object>)perk_defs[perkKey];
		switch ((rarity)dictionary["RARITY"])
		{
		case rarity.common:
			text += "<color=#ffff00>Common</color>\n";
			break;
		case rarity.uncommon:
			text += "<color=#00ffff>Uncommon</color>\n";
			break;
		}
		float num = calc_effect_amount(effect.A, perkKey, currentLevel - 1);
		float num2 = calc_effect_amount(effect.A, perkKey, currentLevel);
		string text2 = "";
		if (showDifferences)
		{
			text2 = ((currentLevel == 1) ? "" : (" <color=#00ff00>(+" + Mathf.Abs(num2 - num).ToString("F1") + ")</color>"));
		}
		text = text + (string)dictionary["effectA"] + num2.ToString("F1") + text2 + "\n";
		if (dictionary.ContainsKey("effectB"))
		{
			float num3 = calc_effect_amount(effect.B, perkKey, currentLevel - 1);
			float num4 = calc_effect_amount(effect.B, perkKey, currentLevel);
			text2 = "";
			if (showDifferences)
			{
				text2 = ((currentLevel == 1) ? "" : (" <color=#00ff00>(+" + Mathf.Abs(num4 - num3).ToString("F1") + ")</color>"));
			}
			text = text + (string)dictionary["effectB"] + num4.ToString("F1") + text2 + "\n";
		}
		return text + "<color=#00aaff>uses " + get_cost(perkKey).ToString("F0") + " energy</color>";
	}

	public void try_achievement()
	{
		int num = 0;
		foreach (string key in perk_defs.Keys)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)perk_defs[key];
			if (PlayerData.Instance.GetSlotInt(key, PlayerData.grouping_t.perks) > 0)
			{
				num++;
			}
		}
		if (num >= perk_defs.Keys.Count / 2)
		{
			MultiplayerControl.Instance.unlock_achievement("CgkI7aPb66UWEAIQDQ");
		}
		if (num == perk_defs.Keys.Count - 1)
		{
			MultiplayerControl.Instance.unlock_achievement("CgkI7aPb66UWEAIQCw");
		}
	}

	public void increment_perk(string key, Sprite quickSprie)
	{
		int num = PlayerData.Instance.GetSlotInt(key, PlayerData.grouping_t.perks) + 1;
		PlayerData.Instance.SetSlotInt(key, num, PlayerData.grouping_t.perks);
		Instance.try_unlock_on_table(key, num, quickSprie);
		string slotString = PlayerData.Instance.GetSlotString("perk_slot_A", PlayerData.grouping_t.perks);
		string slotString2 = PlayerData.Instance.GetSlotString("perk_slot_B", PlayerData.grouping_t.perks);
		if (key == slotString)
		{
			if (num > 1)
			{
				slotA.transform.Find("Image").gameObject.SetActive(true);
				slotA_main.transform.Find("Image").gameObject.SetActive(true);
				slotA.transform.Find("Image").Find("Text").gameObject.GetComponent<Text>().text = string.Concat(num);
				slotA_main.transform.Find("Image").Find("Text").gameObject.GetComponent<Text>().text = string.Concat(num);
			}
			costA = get_cost(key);
		}
		if (key == slotString2)
		{
			if (num > 1)
			{
				slotB.transform.Find("Image").gameObject.SetActive(true);
				slotB_main.transform.Find("Image").gameObject.SetActive(true);
				slotB.transform.Find("Image").Find("Text").gameObject.GetComponent<Text>().text = string.Concat(num);
				slotB_main.transform.Find("Image").Find("Text").gameObject.GetComponent<Text>().text = string.Concat(num);
			}
			Instance.costB = get_cost(key);
		}
		if (PlayerData.Instance.GetSlotString("perk_slot_A", PlayerData.grouping_t.perks) == "")
		{
			autoequip = true;
			curr_sel_perk_key = key;
			on_equip_perk_pressed(0);
		}
		else if (PlayerData.Instance.GetSlotString("perk_slot_B", PlayerData.grouping_t.perks) == "" && key != PlayerData.Instance.GetSlotString("perk_slot_A", PlayerData.grouping_t.perks))
		{
			autoequip = true;
			curr_sel_perk_key = key;
			on_equip_perk_pressed(1);
			slotB_main.SetActive(true);
		}
	}
}
