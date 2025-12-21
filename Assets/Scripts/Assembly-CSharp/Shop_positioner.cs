using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop_positioner : MonoBehaviour
{
	private enum store_type
	{
		consumable = 0,
		permanent = 1
	}

	public enum shop_state
	{
		LOADING = 0,
		load_GOOD = 1,
		load_FAILED = 2
	}

	public enum config
	{
		OKAY_button = 0,
		YES_NO_buttons = 1
	}

	public Color[] sampler_shop_button_colors;

	public static Shop_positioner Instance;

	private List<Dictionary<string, object>> buyable_stuff = new List<Dictionary<string, object>>();

	public GameObject model_noAds;

	public GameObject model_creaturePack;

	public GameObject model_egg;

	public GameObject model_mutate;

	public GameObject model_furniture;

	public GameObject model_creaturePack2;

	public GameObject model_creaturePack3;

	public GameObject model_extended_inv;

	public GameObject model_creaturePack4;

	public GameObject model_creaturePack5;

	public GameObject model_creaturePack6;

	public GameObject model_creaturePack7;

	private bool is_window_open;

	public bool disable_top_left_buttons;

	private bool window_elements_positioned;

	private shop_state SHOP_STATE;

	public int attempt_buy_index = -1;

	public List<GameObject> models_for_buttons = new List<GameObject>();

	public GameObject model_on_popup;

	private string loading_string = "err";

	public Sprite button_BG_generic;

	public Text text_loading;

	public Text text_popup;

	public Text text_GEM_count;

	public GameObject loading_splash;

	public GameObject loading_bubbles;

	public GameObject loading_cancelButton;

	public GameObject retry_button;

	public GameObject Shop_screen;

	public GameObject Gems_window;

	public GameObject Gems_models;

	public GameObject popup;

	public GameObject popup_okay;

	public GameObject popup_yesno;

	public Image popup_head;

	public Sprite purchase_happy;

	public Sprite purchase_sad;

	public Sprite purchase_v_sad;

	public Outline popup_outline;

	public GameObject top_left_buttons;

	public GameObject levelbar;

	public GameObject distancedisplay;

	public GameObject perkbuttons;

	public Color sky_color;

	public GameObject game_canvas;

	public GameObject breeder_base;

	public GameObject type_animatedEgg;

	public string[] companion_prefixes;

	public string[] companion_suffixes;

	public List<GameObject> hatched_companions = new List<GameObject>();

	public GameObject EGG;

	public GameObject mutation_scroll_white;

	public GameObject button_nextpage;

	public GameObject button_prevpage;

	public bool on_craft;

	[HideInInspector]
	public List<GameObject> temporarily_disabled = new List<GameObject>();

	public bool purchase_from_revive;

	public bool revive;

	public bool gems_purchase_something_went_wrong;

	public bool do_blobble_gem_count;

	public bool suppress_models;

	private bool has_no_gems;

	private bool purchases_defined;

	public GameObject popup_ring;

	public static float SCREEN_MAX_X;

	private int numDots;

	private IEnumerator show_loading_dots;

	private int gems_before;

	[HideInInspector]
	public bool temp_hide_close;

	public GameObject[] shop_buttons;

	public Image[] button_backdrops;

	public Text[] shop_costs;

	public Text[] shop_descriptions;

	public Text[] shop_titles;

	private int shopPage;

	public GameObject spawn_companion(Vector3 pos, int creatureA, int creatureB, int LEVEL, int hatchIndex)
	{
		GameObject gameObject = Object.Instantiate(GameController.Instance.type_creature);
		gameObject.name = "Companion";
		gameObject.transform.position = pos;
		gameObject.transform.rotation = Quaternion.Euler(0f, 220f, 0f);
		List<string> list = new List<string>();
		list.Add(Loader.Instance.temp_static_names[creatureA]);
		list.Add(Loader.Instance.temp_static_names[creatureB]);
		gameObject.GetComponent<creatureScript>().Init(Loader.Instance.GetHybrid(list), false, GameController.creatureStates.friendly, LEVEL, "", new int[9], Color.white, true);
		gameObject.GetComponent<creatureScript>().packMates_ = new List<GameObject>();
		NewMobControl.Instance.active_combatants.Add(gameObject);
		gameObject.GetComponent<creatureScript>().hatch_index_ = hatchIndex;
		gameObject.GetComponent<creatureScript>().friendly_creatureA = creatureA;
		gameObject.GetComponent<creatureScript>().friendly_creatureB = creatureB;
		hatched_companions.Add(gameObject);
		return gameObject;
	}

	public void next_page()
	{
		if (!PopupControl.Instance.popup_open)
		{
			shopPage++;
			destroy_shop_models();
			LayoutButtons();
			create_button_models();
			set_page_buttons();
		}
	}

	public void prev_page()
	{
		if (!PopupControl.Instance.popup_open)
		{
			shopPage--;
			destroy_shop_models();
			LayoutButtons();
			create_button_models();
			set_page_buttons();
		}
	}

	public void set_page_buttons()
	{
		button_prevpage.SetActive(shopPage != 0);
		button_nextpage.SetActive(shopPage * 3 + 3 < buyable_stuff.Count);
	}

	private IEnumerator show_animated_egg(GameObject EGG)
	{
		NewBreedControl.Instance.view_result_rotate = true;
		yield return new WaitForSeconds(1f);
		EGG.GetComponent<Animation>().Play();
		yield return new WaitForSeconds(2.5f);
		EGG.GetComponent<animated_egg>().crack();
		GameController.Instance.GetComponent<GameController>().sound_crackshell();
		GameController.Instance.GetComponent<GameController>().animation_sound_levelScreenAppear();
		NewBreedControl.Instance.set_banner_text(companion_prefixes[Random.Range(0, companion_prefixes.Length)] + " " + companion_suffixes[Random.Range(0, companion_suffixes.Length)], false);
		NewBreedControl.Instance.ShowBanners(NewBreedControl.banner_type.companion);
		int lEVEL = 4;
		int num = (int)(Random.value * 56f);
		int num2 = (int)(Random.value * 56f);
		PlayerData.Instance.SetSlotInt("friendly_lvl" + GameController.Instance.n_hatchlings_, 4, PlayerData.grouping_t.general);
		PlayerData.Instance.SetSlotInt("friendly_nextLevelExp" + GameController.Instance.n_hatchlings_, 5, PlayerData.grouping_t.general);
		PlayerData.Instance.SetSlotInt("friendly_exp" + GameController.Instance.n_hatchlings_, 0, PlayerData.grouping_t.general);
		PlayerData.Instance.SetSlotInt("friendly_creatureA" + GameController.Instance.n_hatchlings_, num, PlayerData.grouping_t.general);
		PlayerData.Instance.SetSlotInt("friendly_creatureB" + GameController.Instance.n_hatchlings_, num2, PlayerData.grouping_t.general);
		GameObject critter = spawn_companion(EGG.transform.position, num, num2, lEVEL, GameController.Instance.n_hatchlings_);
		GameController.Instance.n_hatchlings_++;
		PlayerData.Instance.SetSlotInt("num_hatchlings", GameController.Instance.n_hatchlings_, PlayerData.grouping_t.general);
		yield return new WaitForSeconds(0.02f);
		NewBreedControl.Instance.adjust_cam_height_to_creature_height(critter.GetComponent<creatureScript>().myCreatureModel.gameObject);
		GameController.Instance.start_creature_animation(critter.GetComponent<creatureScript>().myCreatureModel.children_, 0);
		yield return new WaitForSeconds(2.5f);
	}

	public void DISABLE_OBJECTS(Vector3 origin, float range, bool on_enter_shack)
	{
		if (Vector3.Distance(origin, breeder_base.transform.position) < range)
		{
			temporarily_disabled.Add(breeder_base);
			temporarily_disabled.Add(GameController.Instance.scenic_elevator);
		}
		if (!on_enter_shack)
		{
			foreach (GameObject active_combatant in NewMobControl.Instance.active_combatants)
			{
				if (active_combatant != GameController.Instance.player)
				{
					temporarily_disabled.Add(active_combatant);
				}
			}
			foreach (KeyValuePair<string, Chunk_f> item in NewBiomeControl.Instance.chunks_loaded)
			{
				int childCount = item.Value.parent_obj.transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					GameObject gameObject = item.Value.parent_obj.transform.GetChild(i).gameObject;
					if (!(gameObject.name == "floor-plane") && Vector3.Distance(origin, gameObject.transform.position) < range)
					{
						temporarily_disabled.Add(gameObject);
					}
				}
			}
		}
		else
		{
			GameController.Instance.ClearAllCreatures(true);
			foreach (GameObject hatched_companion in hatched_companions)
			{
				temporarily_disabled.Add(hatched_companion);
			}
		}
		foreach (GameObject item2 in temporarily_disabled)
		{
			item2.SetActive(false);
		}
	}

	public void enable_objects()
	{
		foreach (GameObject item in temporarily_disabled)
		{
			if (!(item == null))
			{
				item.SetActive(true);
				if (item.GetComponent<creatureScript>() != null)
				{
					item.GetComponent<creatureScript>().re_enable();
				}
			}
		}
		temporarily_disabled.Clear();
	}

	public void HideGameplayGUI()
	{
		top_left_buttons.SetActive(false);
		levelbar.SetActive(false);
		distancedisplay.SetActive(false);
		perkbuttons.SetActive(false);
	}

	public void ShowGameplayGUI()
	{
		top_left_buttons.SetActive(true);
		levelbar.SetActive(true);
		distancedisplay.SetActive(true);
		perkbuttons.SetActive(true);
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		Shop_Control.Instance.TransferStateInfo();
	}

	public void create_button_models()
	{
		for (int i = 0; i < 3; i++)
		{
			int num = shopPage * 3 + i;
			if (num < buyable_stuff.Count)
			{
				GameObject gameObject = Object.Instantiate((GameObject)buyable_stuff[num]["MODEL"]);
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = new Vector3(-9.57f + 8.97f * (float)i, 5.38f, 22.48f);
				models_for_buttons.Add(gameObject);
			}
		}
	}

	public void define_purchaseables()
	{
		if (!purchases_defined)
		{
			purchases_defined = true;
			StartCoroutine(position_shop_button());
			define_purchaseable("no_ads", store_type.permanent, 15, "Remove Ads", "Permanently removes ads!", 19, sampler_shop_button_colors[0], model_noAds, "REMOVE ADS PERMANENTLY?", "Advertisements are now disabled!");
			define_purchaseable("creature_pack_7", store_type.permanent, 12, "More Animals", "+ COW`+ RAPTOR`+ KOMODO DRAGON`+ SKUNK`+ PANDA", 16, sampler_shop_button_colors[7], model_creaturePack7, "<size=55>GET 5 NEW CREATURES PERMANENTLY?</size>", "Enjoy your 5 new creatures!");
			define_purchaseable("doMUTATE", store_type.consumable, 5, "Mutation", "Mix your current hybrid with ANOTHER animal! Also, gain 2 levels!", 19, sampler_shop_button_colors[3], model_mutate, "MUTATE now?", "---");
			define_purchaseable("big_inv", store_type.permanent, 12, "Huge Inventory", "Permanently adds a second page to your inventory!", 19, sampler_shop_button_colors[6], model_extended_inv, "<size=55>PERMANENTLY UNLOCK BIG INVENTORY?</size>", "Enjoy your huge inventory!");
			define_purchaseable("creature_pack_1", store_type.permanent, 12, "More Animals", "+ KITTEN`+ ASTRONAUT`+ DEVIL`+ TREE`+ PLASTIC SLIDE", 16, sampler_shop_button_colors[1], model_creaturePack, "<size=55>GET 5 NEW CREATURES PERMANENTLY?</size>", "Enjoy your 5 new creatures!");
			define_purchaseable("doCOMPANION", store_type.consumable, 6, "Companion Egg", "Hatches into a friendly monster that follows you and fights with you!", 19, sampler_shop_button_colors[2], model_egg, "HATCH a friend?", "---");
			define_purchaseable("furniture_pack", store_type.permanent, 12, "Furniture Pack", "Unlocks 8 new furniture recipes!", 19, sampler_shop_button_colors[4], model_furniture, "<size=55>GET 8 NEW FURNITURE RECIPES?</size>", "Enjoy your new crafting recipes!");
			define_purchaseable("creature_pack_2", store_type.permanent, 12, "More Animals", "+ DRAGON`+ FLAMINGO`+ GRASSHOPPER`+ FLY`+ EAGLE", 16, sampler_shop_button_colors[5], model_creaturePack2, "<size=55>GET 5 NEW CREATURES PERMANENTLY?</size>", "Enjoy your 5 new creatures!");
			define_purchaseable("creature_pack_3", store_type.permanent, 12, "More Animals", "+ HOTDOG`+ FRENCH FRIES`+ DOUGHNUT`+ PIZZA SLICE`+ CHICKEN WING", 16, sampler_shop_button_colors[7], model_creaturePack3, "<size=55>GET 5 NEW CREATURES PERMANENTLY?</size>", "Enjoy your 5 new creatures!");
			define_purchaseable("creature_pack_5", store_type.permanent, 12, "More Animals", "+ BICYCLE`+ GARBAGE TRUCK`+ HELICOPTER`+ AIRPLANE`+ CAR", 16, sampler_shop_button_colors[1], model_creaturePack5, "<size=55>GET 5 NEW CREATURES PERMANENTLY?</size>", "Enjoy your 5 new creatures!");
			define_purchaseable("creature_pack_4", store_type.permanent, 12, "More Animals", "+ AVOCADO`+ PUMPKIN`+ CORN`+ CARROT`+ ASPARAGUS", 16, sampler_shop_button_colors[8], model_creaturePack4, "<size=55>GET 5 NEW CREATURES PERMANENTLY?</size>", "Enjoy your 5 new creatures!");
			define_purchaseable("creature_pack_6", store_type.permanent, 12, "More Animals", "+ BANANA`+ PINEAPPLE`+ COOKIE`+ CUPCAKE`+ WATERMELON", 16, sampler_shop_button_colors[7], model_creaturePack6, "<size=55>GET 5 NEW CREATURES PERMANENTLY?</size>", "Enjoy your 5 new creatures!");
		}
	}

	private void define_purchaseable(string KEY, store_type TYPE, int COST, string FULL_NAME, string FULL_desc, int descFontSize, Color COLOR, GameObject MODEL, string DESC, string ON_PURCHASE_DESC)
	{
		if (TYPE == store_type.permanent)
		{
			NewGameControl.Instance.list_of_permanent_purchases.Add(KEY);
		}
		if (TYPE != store_type.permanent || PlayerData.Instance.GetGlobalInt(KEY) != 1)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("KEY", KEY);
			dictionary.Add("TYPE", TYPE);
			dictionary.Add("COST", COST);
			dictionary.Add("FULL_NAME", FULL_NAME);
			dictionary.Add("FULL_DESC", FULL_desc);
			dictionary.Add("descFontSize", descFontSize);
			dictionary.Add("DESC", DESC);
			dictionary.Add("DESC_COL", COLOR);
			dictionary.Add("MODEL", MODEL);
			dictionary.Add("ON_PURCHASE_DESC", ON_PURCHASE_DESC);
			buyable_stuff.Add(dictionary);
		}
	}

	public void click_purchaseble_item(int buttonIndex)
	{
		if (!PopupControl.Instance.popup_open)
		{
			buttonIndex += shopPage * 3;
			NewAudioControl.Instance.play_generic_click();
			destroy_shop_models();
			create_popup_model((GameObject)buyable_stuff[buttonIndex]["MODEL"]);
			attempt_buy_index = buttonIndex;
			if (PlayerData.Instance.GetGlobalInt("GEMS") < (int)buyable_stuff[buttonIndex]["COST"])
			{
				activate_popup(config.OKAY_button, (string)buyable_stuff[buttonIndex]["FULL_NAME"] + "\n<color=#ff4444>You don't have enough gems!</color>", (Color)buyable_stuff[buttonIndex]["DESC_COL"], button_BG_generic, new Color(20f / 51f, 20f / 51f, 20f / 51f));
				popup_ring.SetActive(true);
				popup_head.color = (Color)buyable_stuff[buttonIndex]["DESC_COL"];
				return;
			}
			activate_popup(config.YES_NO_buttons, "<color=#ffffff>Spend </color><color=#00aaff>" + (int)buyable_stuff[buttonIndex]["COST"] + "</color> <color=ffffff>gems and</color>\n" + (string)buyable_stuff[buttonIndex]["DESC"], (Color)buyable_stuff[buttonIndex]["DESC_COL"], button_BG_generic, (Color)buyable_stuff[buttonIndex]["DESC_COL"]);
			popup_ring.SetActive(true);
			popup_head.color = (Color)buyable_stuff[buttonIndex]["DESC_COL"];
		}
	}

	public void destroy_shop_models()
	{
		foreach (GameObject models_for_button in models_for_buttons)
		{
			Object.Destroy(models_for_button);
		}
		models_for_buttons.Clear();
	}

	public void create_popup_model(GameObject modelType)
	{
		model_on_popup = Object.Instantiate(modelType);
		model_on_popup.transform.parent = base.transform;
		model_on_popup.transform.localPosition = new Vector3(0.5f, 4.49f, 22.48f);
	}

	public bool try_open_generic_window()
	{
		if (disable_top_left_buttons || GameController.Instance.player == null)
		{
			return false;
		}
		GameController.Instance.clear_all_canvas();
		GameController.Instance.PAUSE_GAME();
		GameController.Instance.player.GetComponent<creatureScript>().targetCombatant = null;
		GameController.Instance.player.GetComponent<creatureScript>().generic_moveTo = Vector3.zero;
		GameController.Instance.targeted_circle_graphic.GetComponent<Animation>().PlayQueued("targetDisappear");
		GameController.Instance.targetShowing = false;
		Instance.HideGameplayGUI();
		return true;
	}

	public void resume_gameplay(bool pause_state)
	{
		Instance.ShowGameplayGUI();
		GameController.Instance.pause = pause_state;
	}

	public void PRESS_SHOP_BUTTON()
	{
		GameController.Instance.button_clicked = true;
		if (try_open_generic_window())
		{
			open_shop();
			WindowControl.Instance.ShowClose(WindowControl.window_type_t.mutant_market);
		}
	}

	public void activate_popup(config b_config, string TEXT, Color textColor, Sprite headerSprite, Color outlineColor)
	{
		WindowControl.Instance.close_button.SetActive(false);
		popup.SetActive(true);
		popup_ring.SetActive(false);
		popup_okay.SetActive(b_config == config.OKAY_button);
		popup_yesno.SetActive(b_config == config.YES_NO_buttons);
		text_popup.text = TEXT;
		text_popup.color = textColor;
		popup_head.sprite = headerSprite;
		popup_head.color = Color.white;
		popup_outline.effectColor = outlineColor;
	}

	private IEnumerator position_shop_button()
	{
		yield return new WaitForSeconds(0f);
		float num = (float)Screen.width / (float)Screen.height;
		SCREEN_MAX_X = 397.01492f * num + 6.970149f;
	}

	private void position_shop_window_elements()
	{
		window_elements_positioned = true;
		loading_cancelButton.transform.localPosition = new Vector2(SCREEN_MAX_X, 400f);
	}

	public void open_gems_window()
	{
		if (!PopupControl.Instance.popup_open)
		{
			NewAudioControl.Instance.play_generic_click();
			Gems_models.SetActive(true);
			Gems_window.SetActive(true);
			WindowControl.Instance.ShowClose(WindowControl.window_type_t.buy_gems_store);
			destroy_shop_models();
		}
	}

	public void close_market()
	{
		NewAudioControl.Instance.play_generic_click();
		do_close_market(false);
	}

	public void press_reward_button()
	{
		if (!PopupControl.Instance.popup_open)
		{
			NewAudioControl.Instance.play_generic_click();
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				PopupControl.Instance.ShowMessage("You must be connected to the internet");
			}
			else
			{
				PopupControl.Instance.ShowRewardAskPopup();
			}
		}
	}

	public void succeded_purchase(int numGems)
	{
		back_to_market(numGems);
	}

	public void failed_purchase()
	{
		back_to_market(0);
	}

	public void shop_failed_loading()
	{
		SHOP_STATE = shop_state.load_FAILED;
		if (is_window_open)
		{
			if (show_loading_dots != null)
			{
				StopCoroutine(show_loading_dots);
			}
			loading_bubbles.SetActive(false);
			VIS_failed();
		}
	}

	public void close_loading_screen()
	{
		NewAudioControl.Instance.play_generic_click();
		resume_gameplay(false);
		is_window_open = false;
		VIS_hide_load();
	}

	public void shop_finished_loading()
	{
		SHOP_STATE = shop_state.load_GOOD;
		if (is_window_open)
		{
			VIS_show_market();
			VIS_hide_load();
		}
	}

	public void press_retry()
	{
		NewAudioControl.Instance.play_generic_click();
		Shop_Control.Instance.InitializePurchasing();
		SHOP_STATE = shop_state.LOADING;
		retry_button.SetActive(false);
		VIS_loading("Loading Mutant Market");
	}

	public void close_gems_window()
	{
		NewAudioControl.Instance.play_generic_click();
		Gems_models.SetActive(false);
		Gems_window.SetActive(false);
		if (purchase_from_revive)
		{
			GameController.Instance.revive_cancel();
			purchase_from_revive = false;
		}
		else
		{
			destroy_shop_models();
		}
	}

	private void VIS_hide_load()
	{
		loading_splash.SetActive(false);
		loading_bubbles.SetActive(false);
		if (show_loading_dots != null)
		{
			StopCoroutine(show_loading_dots);
		}
		retry_button.SetActive(false);
	}

	private void VIS_failed()
	{
		text_loading.text = "Failed to connect!";
		retry_button.SetActive(true);
	}

	private void VIS_loading(string str)
	{
		loading_bubbles.SetActive(true);
		loading_string = str;
		show_loading_dots = SHOW_LOADING_DOTS();
		StartCoroutine(show_loading_dots);
	}

	public void VIS_show_market()
	{
		Shop_screen.SetActive(true);
		text_GEM_count.text = string.Concat(PlayerData.Instance.GetGlobalInt("GEMS"));
		LayoutButtons();
		create_button_models();
	}

	private void do_close_market(bool pause_state)
	{
		resume_gameplay(pause_state);
		GameController.Instance.give_all_display();
		is_window_open = false;
		destroy_shop_models();
		Shop_screen.SetActive(false);
	}

	private IEnumerator SHOW_LOADING_DOTS()
	{
		while (true)
		{
			numDots++;
			if (numDots == 15)
			{
				numDots = 0;
			}
			string text = "";
			for (int i = 0; i < numDots; i++)
			{
				text += ".";
			}
			text_loading.text = text + loading_string + text;
			yield return new WaitForSeconds(0.05f);
		}
	}

	public void buy_gems(int amount)
	{
		if (amount == 30 || amount == 60)
		{
			loading_splash.SetActive(true);
			loading_cancelButton.SetActive(false);
			VIS_loading("Processing Transaction");
			Shop_screen.SetActive(false);
			Gems_window.SetActive(false);
			Gems_models.SetActive(false);
			gems_before = PlayerData.Instance.GetGlobalInt("GEMS");
			do_blobble_gem_count = true;
			if (purchase_from_revive)
			{
				WindowControl.Instance.close_button.SetActive(false);
			}
			else
			{
				WindowControl.Instance.curr_window = WindowControl.window_type_t.mutant_market;
			}
			if (amount == 60)
			{
				Shop_Control.Instance.Buy60gems();
			}
			else
			{
				Shop_Control.Instance.Buy30gems();
			}
		}
	}

	private void FixedUpdate()
	{
		if (is_window_open)
		{
			text_GEM_count.color = Color.Lerp(text_GEM_count.color, has_no_gems ? Color.red : Color.white, Time.fixedDeltaTime * 0.5f);
			text_GEM_count.transform.localScale = Vector3.Lerp(text_GEM_count.transform.localScale, Vector3.one * 1.2f, Time.fixedDeltaTime * 1f);
		}
	}

	public void back_to_market(int numGems)
	{
		VIS_hide_load();
		if (numGems == 0)
		{
			activate_popup(config.OKAY_button, "<color=#00aaff>OOPS!</color> Something went wrong.\nThe transaction did not complete.", Color.white, purchase_sad, new Color(0.29803923f, 26f / 51f, 0.5294118f));
			gems_purchase_something_went_wrong = true;
		}
		else
		{
			activate_popup(config.OKAY_button, "THANK YOU for supporting us!\nEnjoy your <color=#00aaff>Gems!</color>", new Color(13f / 15f, 0.9411765f, 0.16078432f), purchase_happy, new Color(0.5294118f, 26f / 51f, 0.29803923f));
			GameController.Instance.sound_levelButton();
			gems_purchase_something_went_wrong = false;
		}
		if (!purchase_from_revive)
		{
			Shop_screen.SetActive(true);
		}
	}

	public void open_shop()
	{
		NewAudioControl.Instance.play_generic_click();
		is_window_open = true;
		if (!window_elements_positioned)
		{
			position_shop_window_elements();
		}
		switch (SHOP_STATE)
		{
		case shop_state.load_GOOD:
			VIS_show_market();
			break;
		case shop_state.LOADING:
		case shop_state.load_FAILED:
			loading_splash.SetActive(true);
			loading_cancelButton.SetActive(true);
			if (SHOP_STATE == shop_state.load_FAILED)
			{
				VIS_failed();
			}
			else
			{
				VIS_loading("Loading Mutant Market");
			}
			break;
		}
	}

	public void purchased_upgrade_popup()
	{
		PlayerData.Instance.SetGlobalInt((string)buyable_stuff[attempt_buy_index]["KEY"], 1);
		activate_popup(config.OKAY_button, "THANK YOU!\n<color=#44bbff>" + (string)buyable_stuff[attempt_buy_index]["ON_PURCHASE_DESC"] + "</color>", new Color(13f / 15f, 0.9411765f, 0.16078432f), purchase_happy, new Color(0.5294118f, 26f / 51f, 0.29803923f));
		GameController.Instance.sound_levelButton();
		buyable_stuff.RemoveAt(attempt_buy_index);
		destroy_shop_models();
		LayoutButtons();
		set_page_buttons();
	}

	public void popup_YES_pressed()
	{
		popup.SetActive(false);
		if (revive)
		{
			GameController.Instance.revive_gems_accept();
			revive = false;
			return;
		}
		if (!suppress_models)
		{
			Object.Destroy(model_on_popup);
			create_button_models();
		}
		else
		{
			suppress_models = false;
		}
		if (on_craft)
		{
			inventory_ctr.Instance.ACCEPT_craft();
			on_craft = false;
			return;
		}
		int globalInt = PlayerData.Instance.GetGlobalInt("GEMS");
		int num = (int)buyable_stuff[attempt_buy_index]["COST"];
		globalInt -= num;
		PlayerData.Instance.SetGlobalInt("GEMS", globalInt);
		text_GEM_count.text = string.Concat(PlayerData.Instance.GetGlobalInt("GEMS"));
		if ((store_type)buyable_stuff[attempt_buy_index]["TYPE"] == store_type.permanent)
		{
			WindowControl.Instance.close_button.SetActive(true);
			StartCoroutine(wait_display_result());
		}
		else if ((store_type)buyable_stuff[attempt_buy_index]["TYPE"] == store_type.consumable)
		{
			WindowControl.Instance.close_button.SetActive(false);
			do_close_market(true);
			if (NewBiomeControl.Instance.player_zone == "overworld")
			{
				DISABLE_OBJECTS(NewBreedControl.Instance.spawn_mutant.transform.position, 7f, false);
			}
			string obj = (string)buyable_stuff[attempt_buy_index]["KEY"];
			if (obj == "doCOMPANION")
			{
				GameController.Instance.player.transform.rotation = (GameController.Instance.player.GetComponent<creatureScript>().look_rotation.rotation = Quaternion.Euler(0f, 268f, 0f));
				EGG = Object.Instantiate(type_animatedEgg);
				int num2 = GameController.Instance.n_hatchlings_ * 2 + 1;
				Vector3 vector = ((GameController.Instance.trail_nodes__.Count > num2) ? GameController.Instance.trail_nodes__[num2].transform.position : GameController.Instance.trail_nodes__[GameController.Instance.trail_nodes__.Count - 1].transform.position);
				vector = new Vector3(vector.x, 0.1f, vector.z);
				EGG.transform.position = vector;
				StartCoroutine(show_animated_egg(EGG));
				NewBreedControl.Instance.on_hatched_companion = true;
				NewBreedControl.Instance.gameObject.SetActive(true);
				NewBreedControl.Instance.transition_back_to_breeder(NewBreedControl.breeder_transition.on_companion);
			}
			if (obj == "doMUTATE")
			{
				NewBreedControl.Instance.gameObject.SetActive(true);
				NewBreedControl.Instance.transition_back_to_breeder(NewBreedControl.breeder_transition.on_mutation);
				mutation_scroll_white.SetActive(true);
				mutation_scroll_white.GetComponent<CanvasGroup>().alpha = 1f;
			}
		}
	}

	public void popup_cancel_pressed()
	{
		popup.SetActive(false);
		if (revive)
		{
			GameController.Instance.revive_cancel();
			revive = false;
			return;
		}
		if (!on_craft)
		{
			WindowControl.Instance.close_button.SetActive(true);
		}
		if (!suppress_models)
		{
			Object.Destroy(model_on_popup);
			create_button_models();
		}
		else
		{
			suppress_models = false;
		}
		on_craft = false;
	}

	public void popup_OKAY_pressed()
	{
		NewAudioControl.Instance.play_generic_click();
		popup.SetActive(false);
		if (purchase_from_revive)
		{
			if (gems_purchase_something_went_wrong)
			{
				GameController.Instance.revive_cancel();
			}
			else
			{
				GameController.Instance.doRevive();
			}
			purchase_from_revive = false;
			return;
		}
		if (!temp_hide_close)
		{
			WindowControl.Instance.close_button.SetActive(true);
		}
		else
		{
			temp_hide_close = false;
		}
		if (do_blobble_gem_count)
		{
			int globalInt = PlayerData.Instance.GetGlobalInt("GEMS");
			text_GEM_count.text = string.Concat(globalInt);
			if (globalInt != 0 && globalInt != gems_before)
			{
				text_GEM_count.color = Color.green;
				text_GEM_count.transform.localScale = Vector3.one * 4.5f;
				has_no_gems = false;
			}
			do_blobble_gem_count = false;
		}
		if (!suppress_models)
		{
			Object.Destroy(model_on_popup);
			create_button_models();
		}
		else
		{
			suppress_models = false;
		}
	}

	public void Blob_gem_count()
	{
		text_GEM_count.text = string.Concat(PlayerData.Instance.GetGlobalInt("GEMS"));
		text_GEM_count.color = Color.green;
		text_GEM_count.transform.localScale = Vector3.one * 4.5f;
		has_no_gems = false;
	}

	private IEnumerator wait_display_result()
	{
		yield return new WaitForSeconds(0.3f);
		purchased_upgrade_popup();
	}

	private void LayoutButtons()
	{
		for (int i = 0; i < 3; i++)
		{
			int num = shopPage * 3 + i;
			if (num < buyable_stuff.Count)
			{
				Color color = (Color)buyable_stuff[num]["DESC_COL"];
				shop_buttons[i].SetActive(true);
				button_backdrops[i].color = color;
				shop_costs[i].text = string.Concat((int)buyable_stuff[num]["COST"]);
				shop_descriptions[i].text = TranslationControl.Instance.Translate((string)buyable_stuff[num]["FULL_DESC"]);
				shop_descriptions[i].fontSize = (int)buyable_stuff[num]["descFontSize"];
				shop_titles[i].text = TranslationControl.Instance.Translate((string)buyable_stuff[num]["FULL_NAME"]);
				shop_titles[i].color = color;
			}
			else
			{
				if (i == 0)
				{
					shopPage--;
					LayoutButtons();
					return;
				}
				shop_buttons[i].SetActive(false);
			}
		}
		if (PlayerData.Instance.GetGlobalInt("GEMS") == 0)
		{
			has_no_gems = true;
		}
		else
		{
			text_GEM_count.text = string.Concat(PlayerData.Instance.GetGlobalInt("GEMS"));
		}
	}
}
