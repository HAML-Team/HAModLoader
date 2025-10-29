using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBreedControl : MonoBehaviour
{
	public enum breeder_transition
	{
		on_death = 0,
		on_mutation = 1,
		on_companion = 2,
		on_play = 3
	}

	private enum buttons_pos
	{
		left = 0,
		right = 1
	}

	public enum banner_type
	{
		breed = 0,
		mutate = 1,
		companion = 2
	}

	private enum state
	{
		select_father = 0,
		select_mother = 1,
		select_mutant = 2,
		view_regular_result = 3
	}

	public static NewBreedControl Instance;

	public GameObject prefab_breed_button;

	public Transform button_parent;

	public Animation header_fade;

	public Animation result_emerge;

	public RectTransform[] elements_to_reverse;

	private List<GameObject> buttons = new List<GameObject>();

	private Vector2 buttons_parent_start_pos;

	private Vector3 prev_mouse;

	private float buttons_velocity;

	public Text select_Father_text;

	public Color col_select_father_Text;

	public Color col_select_mother_Text;

	public Color col_select_mutant_text;

	public Sprite spr_select_father_button;

	public Sprite spr_select_mother_button;

	public Sprite spr_premium_button;

	public Sprite spr_mutant_button;

	private state state_t;

	[HideInInspector]
	public GameObject prev_button_pressed;

	public Transform spawn_father;

	public Transform spawn_mother;

	[HideInInspector]
	public Transform spawn_mutant;

	public Transform campos_fathr_16_by_9;

	public Transform campos_mothr_16_by_9;

	public Transform campos_fathr_4_by_3;

	public Transform campos_mothr_4_by_3;

	public Transform campos_result;

	public Transform campos_first_animal_short;

	public Transform campos_first_animal_tall;

	public Transform campos_second_animal_short;

	public Transform campos_second_animal_tall;

	private Transform campos_fathr_gen;

	private Transform campos_mothr_gen;

	private Transform campos_first_gen;

	private Transform campos_second_gen;

	private Transform campos_mutant_gen;

	[HideInInspector]
	public Transform cam_curr_dest;

	public GameObject accept_button;

	private GameObject father;

	private GameObject mother;

	[HideInInspector]
	public GameObject result;

	public Text animal_selected_name;

	public Text animal_one_name;

	public Text animal_two_name;

	private float cam_lerp_speed;

	public Transform cam_result_rotate_around;

	public float result_rot_angle;

	public float result_cam_h;

	public float result_cam_view_h;

	public float result_view_dist;

	private GameObject rot_point;

	private bool viewing_result;

	[HideInInspector]
	public bool view_result_rotate;

	private Vector3 elevator_start_pos;

	private bool can_interact_with_buttons;

	public Text text_result_name;

	public GameObject text_THE;

	public Text text_perk;

	public Image img_perk;

	private int father_index;

	private int mother_index;

	private float max_buttons_y;

	private GameObject attempted_to_click_button;

	private float hold_button_timer;

	private string startbonus_key;

	public Animation results_anm;

	private bool cam_lerp;

	public GameObject elevator;

	public GameObject pos_elevator_finished;

	public GameObject breeder_fine_details;

	public int num_rebreeds;

	private List<int> added;

	private buttons_pos curr_buttons_pos = buttons_pos.right;

	private Transform rotate_around;

	private bool lerp_mutants;

	public Canvas canvas;

	private GameObject mutant;

	private int mutant_index;

	public GameObject type_mutation_bubbles;

	private GameObject bubbles;

	private Transform mid_mutant;

	public GameObject perk_display;

	public GameObject leveup_button;

	public GameObject banner_button_A;

	public GameObject banner_button_B;

	public bool on_hatched_companion;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		CreateCameraPoints();
		Shop_positioner.Instance.define_purchaseables();
		PopupControl.Instance.HideAll();
		int slotInt = PlayerData.Instance.GetSlotInt("PLAYER_ALIVE", PlayerData.grouping_t.general);
		if (slotInt == 0)
		{
			SetUpBreeder();
			GameController.Instance.SetBackground_Breeder();
		}
		else
		{
			Loader.Instance.skipBreedScreen();
			if (slotInt == 2)
			{
				GameController.Instance.player_auto_dead = true;
				GameController.Instance.show_death_screen();
			}
		}
		spawn_mutant = new GameObject().transform;
		spawn_mutant.name = "(Gen) spawn mutant";
	}

	public void SetElevatorAtFinishedPos()
	{
		elevator.transform.localPosition = pos_elevator_finished.transform.localPosition;
	}

	private void HideGUI()
	{
		GameController.Instance.enabled = false;
		Shop_positioner.Instance.HideGameplayGUI();
	}

	public void transition_back_to_breeder(breeder_transition transition)
	{
		HideGUI();
		switch (transition)
		{
		case breeder_transition.on_mutation:
			SetUpBreeder(state.select_mutant);
			GameController.Instance.SetBackground_NPC();
			break;
		case breeder_transition.on_companion:
			cam_lerp = true;
			cam_lerp_speed = 5f;
			view_result(Shop_positioner.Instance.EGG.transform, -1.5f);
			GameController.Instance.SetBackground_NPC();
			break;
		case breeder_transition.on_death:
		{
			breeder_fine_details.SetActive(true);
			SetUpBreeder();
			GameController.Instance.SetBackground_Breeder();
			GameController.Instance.breeder_floor_plane.SetActive(true);
			GameController.Instance.breeder_floor_plane.transform.position = Vector3.zero;
			Texture2D texture2D = new Texture2D(2, 2);
			Color[] array = new Color[4];
			for (int i = 0; i < 4; i++)
			{
				array[i] = Color.white;
			}
			texture2D.SetPixels(0, 0, 2, 2, array);
			texture2D.Apply();
			GameController.Instance.breeder_floor_plane.GetComponent<Renderer>().material.mainTexture = texture2D;
			break;
		}
		}
	}

	private void DestroyButtons()
	{
		foreach (GameObject button in buttons)
		{
			UnityEngine.Object.Destroy(button);
		}
		buttons.Clear();
	}

	private void SetUpBreeder(state state_t = state.select_father)
	{
		added = new List<int>();
		CreateButtons(0, 57, false);
		bool flag = false;
		if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)
		{
			if (MultiplayerControl.Instance.auto_time_enabled())
			{
				flag = true;
			}
			else
			{
				PopupControl.Instance.ShowMessage("You must have auto-time enabled in your Phone Settings to use free premium creatures on Wednesday.");
			}
		}
		if (PlayerData.Instance.GetGlobalInt("creature_pack_1") == 1 || flag)
		{
			CreateButtons(57, 5, true);
		}
		if (PlayerData.Instance.GetGlobalInt("creature_pack_2") == 1 || flag)
		{
			CreateButtons(62, 5, true);
		}
		if (PlayerData.Instance.GetGlobalInt("creature_pack_3") == 1 || flag)
		{
			CreateButtons(67, 5, true);
		}
		if (PlayerData.Instance.GetGlobalInt("creature_pack_4") == 1 || flag)
		{
			CreateButtons(72, 5, true);
		}
		if (PlayerData.Instance.GetGlobalInt("creature_pack_5") == 1 || flag)
		{
			CreateButtons(77, 5, true);
		}
		if (PlayerData.Instance.GetGlobalInt("creature_pack_6") == 1 || flag)
		{
			CreateButtons(82, 5, true);
		}
		if (PlayerData.Instance.GetGlobalInt("creature_pack_7") == 1 || flag)
		{
			CreateButtons(87, 5, true);
		}
		for (int i = 0; i < PlayerData.Instance.GetGlobalInt("n_free_creatures"); i++)
		{
			int globalInt = PlayerData.Instance.GetGlobalInt("free_creature_" + i);
			if (!added.Contains(globalInt))
			{
				CreateButtons(globalInt, 1, true);
			}
		}
		cam_lerp = true;
		Set_State(state_t);
	}

	private void CreateCameraPoints()
	{
		rot_point = new GameObject();
		rot_point.name = "(Gen) Result Rotation Point";
		float t = screen_value();
		campos_fathr_gen = new GameObject().transform;
		campos_fathr_gen.transform.position = Vector3.Lerp(campos_fathr_4_by_3.position, campos_fathr_16_by_9.position, t);
		campos_fathr_gen.transform.rotation = Quaternion.Lerp(campos_fathr_4_by_3.rotation, campos_fathr_16_by_9.rotation, t);
		campos_fathr_gen.gameObject.name = "(Gen) Father Camera Point";
		campos_mothr_gen = new GameObject().transform;
		campos_mothr_gen.transform.position = Vector3.Lerp(campos_mothr_4_by_3.position, campos_mothr_16_by_9.position, t);
		campos_mothr_gen.transform.rotation = Quaternion.Lerp(campos_mothr_4_by_3.rotation, campos_mothr_16_by_9.rotation, t);
		campos_mothr_gen.gameObject.name = "(Gen) Mother Camera Point";
		campos_first_gen = new GameObject().transform;
		campos_first_gen.name = "(Gen) First Animal Camera Point";
		campos_second_gen = new GameObject().transform;
		campos_second_gen.name = "(Gen) Second Animal Camera Point";
		campos_mutant_gen = new GameObject().transform;
		campos_mutant_gen.name = "(Gen) Mutant Camera Point";
		mid_mutant = new GameObject().transform;
		mid_mutant.name = "(Gen) Mid mutant point";
		buttons_parent_start_pos = button_parent.transform.localPosition;
		elevator_start_pos = campos_result.parent.transform.localPosition;
	}

	private void EndBreeder()
	{
		hold_button_timer = 0f;
		attempted_to_click_button = null;
		prev_button_pressed = null;
		DestroyButtons();
		stop_view_result();
		cam_lerp = false;
		HideBanners();
	}

	public void press_play()
	{
		EndBreeder();
		UnityEngine.Object.Destroy(mother);
		UnityEngine.Object.Destroy(father);
		NewAudioControl.Instance.play_generic_click();
		NewAudioControl.Instance.play_gameplay_music();
		perk_controller.Instance.slotB_main.SetActive(false);
		for (int i = 0; i < perk_controller.Instance.perk_unlocked_slots.Length; i++)
		{
			perk_controller.Instance.perk_unlocked_slots[i].GetComponent<Image>().sprite = perk_controller.Instance.not_unlocked;
			perk_controller.Instance.perk_unlocked_slots[i].transform.Find("Image").gameObject.SetActive(false);
		}
		perk_controller.Instance.slotA.transform.Find("Image").gameObject.SetActive(false);
		perk_controller.Instance.slotB.transform.Find("Image").gameObject.SetActive(false);
		perk_controller.Instance.slotB.GetComponent<Image>().sprite = perk_controller.Instance.not_unlocked;
		perk_controller.Instance.slotB.GetComponent<Image>().color = new Color(0f, 0f, 1f, 0.4f);
		Loader.Instance.ADS_num_rebreeds = 0;
		UnityEngine.Random.Range(0, 1000);
		PlayerData.Instance.SetSlotInt("PLAYER_ALIVE", 1, PlayerData.grouping_t.general);
		PlayerData.Instance.SetSlotInt("n_morphed_creatures", 2, PlayerData.grouping_t.general);
		PlayerData.Instance.SetSlotInt("morph0", father_index, PlayerData.grouping_t.general);
		PlayerData.Instance.SetSlotInt("morph1", mother_index, PlayerData.grouping_t.general);
		PlayerData.Instance.SetSlotString("creatureName", text_result_name.text, PlayerData.grouping_t.general);
		PlayerData.Instance.SetSlotInt("playerLevel", 1, PlayerData.grouping_t.general);
		PlayerData.Instance.SetSlotInt("currentEXP", 0, PlayerData.grouping_t.general);
		PlayerData.Instance.SetSlotInt("temp_nextLevelExp", 4, PlayerData.grouping_t.general);
		for (int j = 0; j < Loader.n_stats; j++)
		{
			PlayerData.Instance.SetSlotInt("stat" + j, 0, PlayerData.grouping_t.general);
		}
		for (int k = 0; k < inventory_ctr.total_INV_spaces; k++)
		{
			PlayerData.Instance.SetSlotString("inventory" + k, "", PlayerData.grouping_t.the_inventory);
			PlayerData.Instance.SetSlotInt("inventoryCount" + k, 0, PlayerData.grouping_t.the_inventory);
		}
		inventory_ctr.Instance.reset_inv();
		PlayerData.Instance.SetSlotInt("num_hatchlings", 0, PlayerData.grouping_t.general);
		PlayerData.Instance.SetSlotInt("numRebreeds", 0, PlayerData.grouping_t.general);
		PlayerData.Instance.SetSlotInt("skillPointsSpendable", 0, PlayerData.grouping_t.general);
		PlayerData.Instance.SetSlotString("perk_slot_A", "", PlayerData.grouping_t.perks);
		PlayerData.Instance.SetSlotString("perk_slot_B", "", PlayerData.grouping_t.perks);
		foreach (string key in perk_controller.Instance.perk_defs.Keys)
		{
			PlayerData.Instance.SetSlotInt(key, 0, PlayerData.grouping_t.perks);
		}
		perk_controller.Instance.increment_perk(startbonus_key, img_perk.sprite);
		StartCoroutine(transition_to_gameplay(2f, false, false));
	}

	public IEnumerator transition_to_gameplay(float secs, bool on_mutate_or_breed, bool bump_down)
	{
		if (!on_mutate_or_breed)
		{
			NewAudioControl.Instance.Play(NewAudioControl.Instance.sfx_game_Start, 0.8f);
			GameController.Instance.set_player(result, campos_result.transform.position, bump_down);
			Loader.Instance.load_companions();
		}
		Shop_positioner.Instance.ShowGameplayGUI();
		GameController.Instance.enabled = true;
		GameController.Instance.SetBackground_Explore();
		GameController.Instance.start_daynight_cycle();
		GameController.Instance.start_reward_ad_timer();
		breeder_fine_details.SetActive(false);
		if (on_mutate_or_breed)
		{
			if (NewBiomeControl.Instance.player_zone == "overworld")
			{
				Shop_positioner.Instance.enable_objects();
			}
			GameController.Instance.txt_GUI_player_combo.text = "";
		}
		else
		{
			NewBiomeControl.Instance.player_zone = "overworld";
			NewBiomeControl.Instance.TerrainChanged(true);
			GameController.Instance.breeder_floor_plane.SetActive(false);
		}
		yield return new WaitForSeconds(secs);
		base.gameObject.SetActive(false);
	}

	private void Randomize_Buttons()
	{
		int num = 30;
		for (int i = 0; i < num; i++)
		{
			int index = UnityEngine.Random.Range(0, buttons.Count);
			int index2 = UnityEngine.Random.Range(0, buttons.Count);
			int index3 = buttons[index].GetComponent<NewBreedButton>().index;
			Sprite sprite = buttons[index].transform.Find("Image").GetComponent<Image>().sprite;
			Sprite sprite2 = buttons[index].GetComponent<Image>().sprite;
			buttons[index].GetComponent<NewBreedButton>().index = buttons[index2].GetComponent<NewBreedButton>().index;
			buttons[index].transform.Find("Image").GetComponent<Image>().sprite = buttons[index2].transform.Find("Image").GetComponent<Image>().sprite;
			buttons[index].GetComponent<Image>().sprite = buttons[index2].GetComponent<Image>().sprite;
			buttons[index2].GetComponent<NewBreedButton>().index = index3;
			buttons[index2].transform.Find("Image").GetComponent<Image>().sprite = sprite;
			buttons[index2].GetComponent<Image>().sprite = sprite2;
		}
	}

	public void press_breed_again()
	{
		GetComponent<Animation>().Play("hide-banners");
		UnityEngine.Object.Destroy(mother);
		stop_view_result();
		Set_State(state.select_father);
		Loader.Instance.SHOW_AD(Loader.ad_context.on_breeder);
	}

	private IEnumerator Switch_To_Mother()
	{
		cam_curr_dest = campos_mothr_gen;
		cam_lerp_speed = 1f;
		yield return new WaitForSeconds(0.5f);
		Set_State(state.select_mother);
	}

	public void PressAccept()
	{
		if (!can_interact_with_buttons)
		{
			return;
		}
		can_interact_with_buttons = false;
		HideButtons();
		if (state_t == state.select_mutant)
		{
			Shop_positioner.Instance.mutation_scroll_white.GetComponent<Animation>().Play();
			results_anm.Play();
		}
		else if (state_t == state.select_father)
		{
			StartCoroutine(Switch_To_Mother());
		}
		else if (state_t == state.select_mother)
		{
			if (result != null)
			{
				UnityEngine.Object.Destroy(result);
			}
			campos_result.parent.transform.localPosition = elevator_start_pos;
			new List<int> { father_index, mother_index };
			List<string> list = new List<string>();
			list.Add(Loader.Instance.temp_static_names[father_index]);
			list.Add(Loader.Instance.temp_static_names[mother_index]);
			result = Loader.Instance.GetHybrid(list);
			result.transform.parent = campos_result;
			result.transform.localPosition = Vector3.zero;
			int bonus_stat = result.GetComponent<creatureModel>().bonus_stat;
			List<string> list2 = new List<string>(perk_controller.Instance.perk_defs.Keys);
			startbonus_key = list2[bonus_stat];
			Dictionary<string, object> dictionary = (Dictionary<string, object>)perk_controller.Instance.perk_defs[startbonus_key];
			text_perk.text = (string)dictionary["NAME"];
			img_perk.sprite = (Sprite)dictionary["Spr"];
			set_banner_text((result.GetComponent<creatureModel>().namePrefix + " " + result.GetComponent<creatureModel>().nameSuffix).ToUpper());
			results_anm.Play();
		}
	}

	private void Set_State(state state_t)
	{
		can_interact_with_buttons = true;
		header_fade.Play();
		Randomize_Buttons();
		StartCoroutine(blobble_buttons());
		this.state_t = state_t;
		accept_button.GetComponent<CanvasGroup>().alpha = 1f;
		accept_button.SetActive(false);
		animal_selected_name.transform.parent.GetComponent<CanvasGroup>().alpha = 1f;
		animal_selected_name.text = "";
		button_parent.transform.localPosition = buttons_parent_start_pos;
		buttons_pos buttons_pos = buttons_pos.left;
		switch (state_t)
		{
		case state.select_father:
			buttons_pos = buttons_pos.left;
			cam_lerp_speed = 3f;
			cam_curr_dest = campos_fathr_gen;
			select_Father_text.text = "Pick a Dad";
			select_Father_text.color = col_select_father_Text;
			foreach (GameObject button in buttons)
			{
				if (button.GetComponent<Image>().sprite != spr_premium_button)
				{
					button.GetComponent<Image>().sprite = spr_select_father_button;
				}
			}
			break;
		case state.select_mother:
			buttons_pos = buttons_pos.right;
			select_Father_text.text = "Pick a Mom";
			select_Father_text.color = col_select_mother_Text;
			foreach (GameObject button2 in buttons)
			{
				if (button2.GetComponent<Image>().sprite != spr_premium_button)
				{
					button2.GetComponent<Image>().sprite = spr_select_mother_button;
				}
			}
			break;
		case state.select_mutant:
		{
			buttons_pos = buttons_pos.right;
			Vector3 vector = spawn_mother.position - spawn_father.position;
			spawn_mutant.transform.position = GameController.Instance.player.transform.position + vector;
			spawn_mutant.transform.position = new Vector3(spawn_mutant.transform.position.x, 0f, spawn_mutant.transform.position.z);
			spawn_mutant.transform.rotation = spawn_mother.transform.rotation;
			Vector3 vector2 = campos_mothr_gen.transform.position - spawn_father.transform.position;
			campos_mutant_gen.transform.position = GameController.Instance.player.transform.position + vector2;
			campos_mutant_gen.transform.position = new Vector3(campos_mutant_gen.transform.position.x, campos_mothr_gen.position.y, campos_mutant_gen.transform.position.z);
			campos_mutant_gen.transform.rotation = campos_mothr_gen.rotation;
			Vector3 vector3 = campos_mothr_gen.transform.position - spawn_mother.transform.position;
			GameController.Instance.CancelPlayerMovement();
			Trans look_rotation = GameController.Instance.player.GetComponent<creatureScript>().look_rotation;
			Quaternion rotation = (GameController.Instance.player.transform.rotation = spawn_father.transform.rotation);
			look_rotation.rotation = rotation;
			cam_lerp_speed = 3f;
			cam_curr_dest = campos_mutant_gen;
			select_Father_text.text = "Pick a Mate";
			select_Father_text.color = col_select_mutant_text;
			foreach (GameObject button3 in buttons)
			{
				if (button3.GetComponent<Image>().sprite != spr_premium_button)
				{
					button3.GetComponent<Image>().sprite = spr_mutant_button;
				}
			}
			break;
		}
		}
		if (curr_buttons_pos != buttons_pos)
		{
			RectTransform[] array = elements_to_reverse;
			foreach (RectTransform r in array)
			{
				ReverseElement(r);
			}
			curr_buttons_pos = buttons_pos;
			if (curr_buttons_pos == buttons_pos.left)
			{
				animal_selected_name.alignment = TextAnchor.MiddleRight;
			}
			else
			{
				animal_selected_name.alignment = TextAnchor.MiddleLeft;
			}
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			prev_mouse = Input.mousePosition;
		}
		if (hold_button_timer > 0f && Input.GetMouseButtonUp(0))
		{
			SucceedClickButton(attempted_to_click_button);
			attempted_to_click_button = null;
			hold_button_timer = 0f;
		}
		if (can_interact_with_buttons && Input.GetMouseButton(0))
		{
			float y = (Input.mousePosition - prev_mouse).y / canvas.scaleFactor;
			button_parent.transform.localPosition += new Vector3(0f, y, 0f);
			ClampButtonParent();
			buttons_velocity = y;
		}
		prev_mouse = Input.mousePosition;
	}

	private void FixedUpdate()
	{
		if (state_t == state.select_mutant && lerp_mutants)
		{
			mutant.transform.position = Vector3.Lerp(mutant.transform.position, mid_mutant.position, Time.fixedDeltaTime * 3f);
			mutant.transform.localScale = Vector3.Lerp(mutant.transform.localScale, Vector3.zero, Time.fixedDeltaTime * 1f);
			GameController.Instance.player.transform.position = Vector3.Lerp(GameController.Instance.player.transform.position, mid_mutant.position, Time.fixedDeltaTime * 5f);
			GameController.Instance.player.GetComponent<creatureScript>().myCreatureModel.gameObject.transform.localScale = Vector3.Lerp(GameController.Instance.player.GetComponent<creatureScript>().myCreatureModel.gameObject.transform.localScale, Vector3.zero, Time.fixedDeltaTime * 2.5f);
		}
		if (hold_button_timer > 0f)
		{
			hold_button_timer -= 1f;
		}
		if (viewing_result)
		{
			rot_point.transform.position = rotate_around.position + new Vector3(Mathf.Sin(result_rot_angle) * result_view_dist, result_cam_h, Mathf.Cos(result_rot_angle) * result_view_dist);
			rot_point.transform.LookAt(rotate_around.position + result_cam_view_h * Vector3.up);
		}
		if (view_result_rotate)
		{
			result_rot_angle -= 0.003f;
		}
		if (cam_lerp)
		{
			GameController.Instance.mainCamera.transform.position = Vector3.Lerp(GameController.Instance.mainCamera.transform.position, cam_curr_dest.position, Time.fixedDeltaTime * cam_lerp_speed);
			GameController.Instance.mainCamera.transform.rotation = Quaternion.Lerp(GameController.Instance.mainCamera.transform.rotation, cam_curr_dest.rotation, Time.fixedDeltaTime * cam_lerp_speed);
		}
		if (!Input.GetMouseButton(0))
		{
			button_parent.transform.localPosition += new Vector3(0f, buttons_velocity);
			buttons_velocity *= 0.94f;
			ClampButtonParent();
		}
	}

	public void ClickBreedButton(GameObject clicked)
	{
		hold_button_timer = 13f;
		attempted_to_click_button = clicked;
	}

	private void SucceedClickButton(GameObject clicked)
	{
		NewAudioControl.Instance.play_generic_click();
		if (prev_button_pressed != null)
		{
			prev_button_pressed.GetComponent<Animation>().Stop();
			prev_button_pressed.transform.localScale = Vector3.one;
		}
		prev_button_pressed = clicked;
		clicked.GetComponent<Animation>().Play("new-breedbutton-click");
		int index = clicked.GetComponent<NewBreedButton>().index;
		if (state_t == state.select_father)
		{
			father_index = index;
		}
		else if (state_t == state.select_mother)
		{
			mother_index = index;
		}
		else if (state_t == state.select_mutant)
		{
			mutant_index = index;
		}
		accept_button.SetActive(true);
		animal_selected_name.text = first_upper(Loader.Instance.temp_static_names[index]);
		if (state_t == state.select_father)
		{
			CreateParent(ref father, spawn_father, index);
		}
		else if (state_t == state.select_mother)
		{
			CreateParent(ref mother, spawn_mother, index);
		}
		else if (state_t == state.select_mutant)
		{
			CreateParent(ref mutant, spawn_mutant, index);
		}
	}

	private void ClampButtonParent()
	{
		if (button_parent.transform.localPosition.y < buttons_parent_start_pos.y)
		{
			button_parent.transform.localPosition = buttons_parent_start_pos;
		}
		else if (button_parent.transform.localPosition.y > max_buttons_y)
		{
			button_parent.transform.localPosition = new Vector3(buttons_parent_start_pos.x, max_buttons_y);
		}
	}

	public void anm_lerp_first()
	{
		cam_lerp_speed = 3f;
		if (state_t == state.select_mutant)
		{
			animal_one_name.text = PlayerData.Instance.GetSlotString("creatureName", PlayerData.grouping_t.general);
			float t = height_value(GameController.Instance.player.GetComponent<creatureScript>().myCreatureModel.height);
			Vector3 vector = new Vector3(GameController.Instance.player.transform.position.x, 0f, GameController.Instance.player.transform.position.z);
			Vector3 a = vector + (campos_first_animal_short.position - spawn_father.position);
			Vector3 b = vector + (campos_first_animal_tall.position - spawn_father.position);
			campos_first_gen.position = Vector3.Lerp(a, b, t);
			campos_first_gen.rotation = Quaternion.Lerp(campos_first_animal_short.rotation, campos_first_animal_tall.rotation, t);
		}
		else
		{
			animal_one_name.text = father.GetComponent<creatureModel>().myName.ToUpper();
			float t2 = height_value(father.GetComponent<creatureModel>().height);
			campos_first_gen.position = Vector3.Lerp(campos_first_animal_short.position, campos_first_animal_tall.position, t2);
			campos_first_gen.rotation = Quaternion.Lerp(campos_first_animal_short.rotation, campos_first_animal_tall.rotation, t2);
		}
		cam_curr_dest = campos_first_gen;
	}

	public void anm_lerp_second()
	{
		if (state_t == state.select_mutant)
		{
			animal_two_name.text = mutant.GetComponent<creatureModel>().myName.ToUpper();
			float t = height_value(mutant.GetComponent<creatureModel>().height);
			Vector3 a = spawn_mutant.position + (campos_second_animal_short.position - spawn_mother.position);
			Vector3 b = spawn_mutant.position + (campos_second_animal_tall.position - spawn_mother.position);
			campos_second_gen.position = Vector3.Lerp(a, b, t);
			campos_second_gen.rotation = Quaternion.Lerp(campos_second_animal_short.rotation, campos_second_animal_tall.rotation, t);
		}
		else
		{
			animal_two_name.text = mother.GetComponent<creatureModel>().myName.ToUpper();
			float t2 = height_value(mother.GetComponent<creatureModel>().height);
			campos_second_gen.position = Vector3.Lerp(campos_second_animal_short.position, campos_second_animal_tall.position, t2);
			campos_second_gen.rotation = Quaternion.Lerp(campos_second_animal_short.rotation, campos_second_animal_tall.rotation, t2);
		}
		cam_curr_dest = campos_second_gen;
	}

	public void anm_lerp_result()
	{
		if (state_t == state.select_mutant)
		{
			mid_mutant.position = Vector3.Lerp(mutant.transform.position, GameController.Instance.player.transform.position, 0.5f);
			mid_mutant.position = new Vector3(mid_mutant.position.x, 0.3f, mid_mutant.position.z);
			view_result(mid_mutant, -1.5f);
		}
		else
		{
			result.transform.localRotation = Quaternion.identity;
			GameController.Instance.start_creature_animation(result.GetComponent<creatureModel>().children_, 0);
			view_result(cam_result_rotate_around, 3f);
		}
	}

	public void mutants_merge()
	{
		if (state_t == state.select_mutant)
		{
			lerp_mutants = true;
			bubbles = UnityEngine.Object.Instantiate(type_mutation_bubbles);
			bubbles.transform.position = new Vector3(mid_mutant.position.x, 0f, mid_mutant.position.z);
			NewAudioControl.Instance.Play(NewAudioControl.Instance.sfx_mutate);
		}
	}

	public void adjust_cam_height_to_creature_height(GameObject target_creature, bool creature_on_floor = false)
	{
		float num = 0.2f;
		float t = height_value(target_creature.GetComponent<creatureModel>().height);
		result_cam_h = Mathf.Lerp(0.9f, 0.7f, t) - (creature_on_floor ? num : 0f);
		result_cam_view_h = Mathf.Lerp(0.2f, 0.7f, t) - (creature_on_floor ? num : 0f);
		result_view_dist = Mathf.Lerp(2.25f, 2.6f, t);
	}

	public void anm_result_emerge()
	{
		if (state_t != state.select_mutant)
		{
			cam_lerp_speed = 1.5f;
			view_result_rotate = true;
			adjust_cam_height_to_creature_height(result);
			ShowBanners(banner_type.breed);
			result_emerge.Play();
		}
	}

	public void set_banner_text(string text, bool show_THE = true)
	{
		text_result_name.text = text;
		if (show_THE)
		{
			text_THE.SetActive(true);
			text_THE.transform.localPosition = new Vector3(-((int)(text_result_name.preferredWidth / 2f) + 32), text_THE.transform.localPosition.y);
		}
		else
		{
			text_THE.SetActive(false);
		}
	}

	public void ShowBanners(banner_type banner_type_t)
	{
		GetComponent<Animation>().Play("show-banners");
		if (banner_type_t == banner_type.breed)
		{
			banner_button_A.SetActive(true);
			banner_button_B.SetActive(true);
			leveup_button.SetActive(false);
			perk_display.SetActive(true);
			return;
		}
		banner_button_A.SetActive(false);
		banner_button_B.SetActive(false);
		leveup_button.SetActive(true);
		perk_display.SetActive(false);
		if (banner_type_t == banner_type.mutate)
		{
			leveup_button.transform.Find("Text (1)").GetComponent<Text>().text = "LEVEL UP";
		}
		else
		{
			leveup_button.transform.Find("Text (1)").GetComponent<Text>().text = "OKAY";
		}
	}

	public void HideBanners()
	{
		GetComponent<Animation>().Play("hide-banners");
	}

	private void view_result(Transform rotate_around, float start_rot)
	{
		viewing_result = true;
		this.rotate_around = rotate_around;
		cam_curr_dest = rot_point.transform;
		result_rot_angle = start_rot;
		result_cam_h = 0.6f;
		result_cam_view_h = 0f;
		result_view_dist = 2.4f;
	}

	private void stop_view_result()
	{
		viewing_result = false;
		view_result_rotate = false;
	}

	public void mutant_complete()
	{
		if (state_t == state.select_mutant)
		{
			lerp_mutants = false;
			UnityEngine.Object.Destroy(mutant);
			UnityEngine.Object.Destroy(bubbles);
			ShowBanners(banner_type.mutate);
			view_result_rotate = true;
			adjust_cam_height_to_creature_height(result, true);
			List<GameObject> list = new List<GameObject>();
			list.Add(GameController.Instance.player.GetComponent<creatureScript>().myCreatureModel.gameObject);
			list.Add(mutant);
			result = morpher.Instance.morphCreatures_(list, false, 1);
			result.transform.position = new Vector3(mid_mutant.position.x, 0f, mid_mutant.position.z);
			result.transform.rotation = Quaternion.Euler(0f, 220f, 0f);
			GameController.Instance.start_creature_animation(result.GetComponent<creatureModel>().children_, 0);
			num_rebreeds++;
			string text;
			switch (num_rebreeds)
			{
			case 1:
				text = "Mutant";
				break;
			case 2:
				text = "Legendary";
				break;
			case 3:
				text = "God";
				break;
			case 4:
				text = "Mythical";
				break;
			case 5:
				text = "Unholy";
				break;
			default:
				text = "Supreme";
				break;
			}
			string text2 = char.ToUpper(result.GetComponent<creatureModel>().nameSuffix[0]) + result.GetComponent<creatureModel>().nameSuffix.Substring(1);
			set_banner_text(text + " " + text2);
			UnityEngine.Object.Destroy(GameController.Instance.player.GetComponent<creatureScript>().myCreatureModel.gameObject);
			GameController.Instance.player.GetComponent<creatureScript>().myCreatureModel = result.GetComponent<creatureModel>();
			inventory_ctr.Instance.equip_hat();
			inventory_ctr.Instance.equip_armor();
			NewGameControl.Instance.create_mutant_particle(num_rebreeds, GameController.Instance.player.GetComponent<creatureScript>());
		}
	}

	public void accept_mutant()
	{
		GameController.Instance.pause = false;
		if (on_hatched_companion)
		{
			cam_lerp = false;
			stop_view_result();
			cam_lerp = false;
			HideBanners();
		}
		else
		{
			EndBreeder();
			GameController.Instance.player.transform.position = new Vector3(mid_mutant.position.x, 0.3f, mid_mutant.position.z);
			GameController.Instance.player.name = "zaz";
			result.transform.parent = GameController.Instance.player.transform;
			result.transform.localRotation = Quaternion.identity;
			float num = (float)GameController.Instance.currentEXP / (float)GameController.Instance.temp_nextLevelExp_;
			int num2 = (int)((1f - num) * (float)GameController.Instance.temp_nextLevelExp_) + (GameController.Instance.temp_nextLevelExp_ + GameController.level_EXPONENT) + 2;
			GameController.Instance.currentEXP += num2;
			PlayerData.Instance.SetSlotInt("currentEXP", GameController.Instance.currentEXP, PlayerData.grouping_t.general);
			GameController.Instance.showOverheadNotif("+" + num2 + " Exp!", GameController.Instance.player.transform.position, true, true);
			PlayerData.Instance.SetSlotString("creatureName", text_result_name.text, PlayerData.grouping_t.general);
			int slotInt = PlayerData.Instance.GetSlotInt("n_morphed_creatures", PlayerData.grouping_t.general);
			PlayerData.Instance.SetSlotInt("n_morphed_creatures", slotInt + 1, PlayerData.grouping_t.general);
			PlayerData.Instance.SetSlotInt("morph" + slotInt, mutant_index, PlayerData.grouping_t.general);
			PlayerData.Instance.SetSlotInt("numRebreeds", num_rebreeds, PlayerData.grouping_t.general);
		}
		on_hatched_companion = false;
		StartCoroutine(transition_to_gameplay(2f, true, false));
	}

	private void CreateParent(ref GameObject parent, Transform spawn, int index)
	{
		if (parent != null)
		{
			UnityEngine.Object.Destroy(parent);
		}
		parent = Loader.Instance.GetSingleAnimal(Loader.Instance.temp_static_names[index]);
		float height = parent.GetComponent<creatureModel>().height;
		parent.transform.position = spawn.transform.position + height * Vector3.up;
		parent.transform.rotation = spawn.transform.rotation;
		GameController.Instance.start_creature_animation(parent.GetComponent<creatureModel>().children_, 0);
	}

	private void HideButtons()
	{
		header_fade.Play("fade_out_2");
		animal_selected_name.transform.parent.GetComponent<Animation>().Play("fade_out_2");
		accept_button.GetComponent<Animation>().Play("fade_out_2");
		foreach (GameObject button in buttons)
		{
			button.GetComponent<Animation>().Play("new-breedbutton-disappear");
		}
	}

	private IEnumerator blobble_buttons()
	{
		foreach (GameObject button in buttons)
		{
			button.GetComponent<Animation>().Stop();
			button.transform.localScale = Vector3.zero;
		}
		for (int i = 0; i < 13; i++)
		{
			buttons[i].GetComponent<Animation>().Play("new-breedbutton-appear");
			NewAudioControl.Instance.PlayRand(NewAudioControl.Instance.sfx_bubble, 0.85f, 1.05f);
			yield return new WaitForSeconds(0.05f);
		}
		for (int j = 13; j < buttons.Count; j++)
		{
			buttons[j].transform.localScale = Vector3.one;
		}
	}

	private void CreateButtons(int start_index, int n_create, bool premium)
	{
		float num = 110f;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int count = buttons.Count;
		while (true)
		{
			for (int i = 0; i < 3; i++)
			{
				if (num2 >= count)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(prefab_breed_button);
					gameObject.transform.parent = button_parent;
					gameObject.transform.localRotation = Quaternion.identity;
					gameObject.transform.localScale = Vector3.zero;
					gameObject.transform.localPosition = new Vector2(0f - num + (float)i * num, (float)(-num3) * num);
					gameObject.transform.Find("Image").GetComponent<Image>().sprite = Loader.Instance.creatureSprites[start_index + num4];
					gameObject.GetComponent<NewBreedButton>().index = start_index + num4;
					added.Add(start_index + num4);
					if (premium)
					{
						gameObject.GetComponent<Image>().sprite = spr_premium_button;
					}
					buttons.Add(gameObject);
					num4++;
				}
				num2++;
				if (num4 == n_create)
				{
					return;
				}
			}
			num3++;
			max_buttons_y = (float)num3 * num;
		}
	}

	public static string first_upper(string input)
	{
		return char.ToUpper(input[0]) + input.Substring(1);
	}

	private float height_value(float height)
	{
		return 1.9084f * height - 0.3416f;
	}

	private float screen_value()
	{
		float num = (float)Screen.width / (float)Screen.height;
		return 2.25225f * num - 3.00225f;
	}

	public void ReverseElement(RectTransform R)
	{
		Vector2 anchorMax = (R.anchorMin = new Vector2(1f - R.anchorMin.x, R.anchorMax.y));
		R.anchorMax = anchorMax;
		R.anchoredPosition = Vector2.zero;
	}
}
