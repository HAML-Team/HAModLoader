using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	public enum slider_type
	{
		attack = 0,
		health = 1,
		accuracy = 2,
		dodge = 3
	}

	private enum background_type_t
	{
		breeder = 0,
		NPC_background = 1,
		explore_background = 2
	}

	public enum creatureStates
	{
		neutral = 0,
		aggressive = 1,
		fearful = 2,
		friendly = 3,
		unknown = 4
	}

	public enum shack_type
	{
		unknown = 0,
		shack = 1,
		mansion = 2
	}

	public static GameController Instance;

	public Texture[] grass_textures;

	public Texture[] snow_textures;

	private IEnumerator track_distance;

	private IEnumerator track_gametime;

	private IEnumerator repeat_footstep_sound;

	public static float segmentRadius = 5f;

	private bool FIRST_TIME_PLAYING;

	private int ALL_TIME_HIGHSCORE;

	public int debug_exp;

	public Sprite[] overhead_logos;

	public AudioClip[] sfx_flesh_impact;

	public AudioClip[] sfx_creatures;

	public AudioClip[] sfx_player;

	public AudioClip[] sfx_stepGrass;

	public AudioClip[] sfx_stepGiant;

	public AudioClip sfx_got_level;

	public AudioClip sfx_exp_get;

	public AudioClip sfx_got_level_b;

	public AudioClip sfx_levelButton;

	public AudioClip sfx_death;

	public AudioClip sfx_gamestart;

	public AudioClip[] sfx_breeder_hits;

	public AudioClip sfx_buttonsAppear;

	public AudioClip sfx_mixMutants;

	public AudioClip sfx_craft;

	public AudioClip sfx_relax;

	public AudioClip sfx_crackShell;

	public GameObject type_healthbar;

	public GameObject type_creature;

	public GameObject type_expParticle;

	public GameObject type_creatureLevelDisplay;

	public GameObject type_levelMeter_nib;

	public GameObject type_friendly_levelup_particles;

	public GameObject type_NPC_overhead;

	public Sprite[] hit_sprite_frames;

	public Texture2D target_circle_blue;

	public Texture2D target_circle_red;

	public Texture2D target_circle_yellow;

	public Sprite sprite_splatMiss;

	public Sprite sprite_splatHit;

	public Sprite[] sprites_creatureStates;

	public Color[] nibColors;

	public Text txt_GUI_player_combo;

	public Image expBarYellow;

	public Text coordinateDisplay;

	public Text coordinateDisplay2;

	public Text deathDisplay;

	public Text text_playerLevel;

	public Text text_skillPoints;

	[HideInInspector]
	public Plane plane = new Plane(Vector3.up, Vector3.zero);

	public GameObject[] stat_meters;

	public Text text_highScore;

	public AudioSource sfx;

	public GameObject breeder_floor_plane;

	public GameObject scenic_elevator;

	public GameObject targeted_circle_graphic;

	public GameObject mainCamera;

	public GameObject player;

	public bool targetShowing;

	public int currentEXP;

	public int temp_nextLevelExp_;

	public int playerLevel;

	public int skillPointsSpendable;

	public bool pause;

	public int[] player_stats;

	private Vector3 startPos;

	private bool level_up_screen_open;

	private bool lock_level_visual;

	[HideInInspector]
	public int stats_kills;

	[HideInInspector]
	public int stats_greenFruits;

	[HideInInspector]
	public int stats_redFruits;

	[HideInInspector]
	public int stats_blueFruits;

	[HideInInspector]
	public int stats_seconds;

	[HideInInspector]
	public int stats_minutes;

	public float SX;

	public float SZ;

	private float stats_maxDist;

	private float visualEXP;

	public float cam_offset;

	public float eagle_offset;

	public float giant_offset;

	public string killed_by = "Shark-Bear";

	[HideInInspector]
	public int n_hatchlings_;

	public GameObject temp_trailer;

	public Gradient day_night_colors;

	public Gradient day_night_SKY;

	public Gradient day_night_exploreBG;

	private IEnumerator reward_ad_timer_t;

	public Light directional_light;

	private float time_of_day;

	private IEnumerator daynight_cycle_t;

	private background_type_t background_type;

	private bool nibs_lists_defined;

	public Text respawn_button_text;

	private List<GameObject>[] lvlNibs;

	[HideInInspector]
	public bool player_auto_dead;

	public GameObject start_bonus_par;

	public GameObject curr_pickup_overhead;

	private int last_footstep_sound;

	private Vector3 prevPos = Vector3.zero;

	public Animation death_screen;

	public Animation new_death_screen;

	public bool LOCK_LEVEL_SCREEN;

	private bool can_show_ad = true;

	private int levelups;

	private int rapid_click_count;

	private bool shown_rapid_click_display;

	public GameObject rapid_click_notification;

	public bool is_casting_spell;

	public GameObject interaction_target;

	[HideInInspector]
	public bool player_interacting;

	private bool player_in_chair;

	[HideInInspector]
	public float circ_h = 0.05f;

	public GameObject splash_enter_house;

	[HideInInspector]
	public bool ENTER_SHACK_TRANSITION_PLAYING;

	[HideInInspector]
	public shack_type curr_shack_type;

	[HideInInspector]
	public bool on_exit_house;

	private int curr_shack_id;

	[HideInInspector]
	public bool on_teleport;

	public List<GameObject> possible_destroy = new List<GameObject>();

	private int sound_creature_hit_iterator;

	private int sound_player_hit_iterator;

	public RectTransform canv;

	public static int level_EXPONENT = 1;

	public List<GameObject> trail_nodes__ = new List<GameObject>();

	public static int max_trail_nodes = 2;

	public bool previous_step_click_detected;

	public bool button_clicked;

	private Vector3 previous_step_clicked_at;

	private int no_spam_ads_counter;

	[HideInInspector]
	public GameObject focus_npc;

	public GameObject no_fall_thru_floor;

	private Vector3 prev_no_fall_thru_floor_pos;

	public Animation levelbar;

	public GameObject prefab_teleport;

	// Optional: assign these in the Inspector to avoid string-based GameObject.Find
	[Header("In-Game Overlay (optional)")]
	[Tooltip("Assign the overlay canvas if you have it in the scene. If left empty the code will attempt GameObject.Find by name.")]
	[SerializeField]
	private GameObject overlayCanvas;

	[Tooltip("Assign the overlay message object if you have it in the scene.")]
	[SerializeField]
	private GameObject overlayMessage;

	[Tooltip("Assign the overlay Okay button if you have it in the scene.")]
	[SerializeField]
	private GameObject overlayOkayButton;

	// Runtime scanner to find overlay objects that may live in DontDestroyOnLoad
	private Coroutine overlayScannerCoroutine;
	private bool pendingOverlayShow = false;

	private void Start()
	{
		PopupControl.Instance.GetComponent<Canvas>().worldCamera = Shop_positioner.Instance.GetComponent<Camera>();
		PopupControl.Instance.black_background.GetComponent<Canvas>().worldCamera = Shop_positioner.Instance.GetComponent<Camera>();
		CreateNibsLists();
		if (PlayerData.Instance.GetGlobalInt("High Score") != 0)
		{
			Got_Highscore(false, PlayerData.Instance.GetGlobalInt("High Score"));
		}
		else
		{
			Got_Highscore(true, 1);
		}
		cam_offset = calculate_cam_offset((float)Screen.width / (float)Screen.height);

		// start background scanner to detect overlay objects that may come from DontDestroyOnLoad
		if (overlayScannerCoroutine == null)
		{
			overlayScannerCoroutine = StartCoroutine(OverlayScanner());
		}
	}

	public float calc_slider(slider_type type)
	{
		float num = 0f;
		num += (float)player_stats[2];
		num += (float)player_stats[3];
		num += (float)player_stats[4];
		num += (float)player_stats[7];
		if (num == 0f)
		{
			return 0f;
		}
		switch (type)
		{
		case slider_type.health:
			return (float)player_stats[2] / num;
		case slider_type.attack:
			return (float)player_stats[3] / num;
		case slider_type.accuracy:
			return (float)player_stats[4] / num;
		case slider_type.dodge:
			return (float)player_stats[7] / num;
		default:
			return 0f;
		}
	}

	private float calculate_cam_offset(float ratio)
	{
		float t = Mathf.InverseLerp(1.33f, 2f, ratio);
		return Mathf.Lerp(0.59f, 0.41f, t);
	}

	public void start_reward_ad_timer()
	{
		if (PlayerData.Instance.GetGlobalInt("no_ads") != 1 && reward_ad_timer_t == null)
		{
			reward_ad_timer_t = reward_ad_timer();
			StartCoroutine(reward_ad_timer_t);
		}
	}

	public void stop_reward_ad_timer()
	{
		if (PlayerData.Instance.GetGlobalInt("no_ads") != 1 && reward_ad_timer_t != null)
		{
			StopCoroutine(reward_ad_timer_t);
			reward_ad_timer_t = null;
		}
	}

	public void reset_reward_ad_timer()
	{
		if (PlayerData.Instance.GetGlobalInt("no_ads") != 1)
		{
			stop_reward_ad_timer();
			start_reward_ad_timer();
		}
	}

	private IEnumerator reward_ad_timer()
	{
		int wait_interval = 3;
		int seconds_remaining = 540;
		while (true)
		{
			yield return new WaitForSeconds(wait_interval);
			if (!(player != null) || (player.GetComponent<creatureScript>().targetCombatant != null && player.GetComponent<creatureScript>().targetCombatant.GetComponent<NewCombatant>().mob_type == NewCombatant.TYPE_T.creature))
			{
				continue;
			}
			bool flag = false;
			foreach (GameObject active_combatant in NewMobControl.Instance.active_combatants)
			{
				if (active_combatant.GetComponent<NewCombatant>().mob_type == NewCombatant.TYPE_T.creature && active_combatant.GetComponent<creatureScript>().targetCombatant == player)
				{
					flag = true;
					break;
				}
			}
			if (!flag && !pause)
			{
				seconds_remaining -= wait_interval;
				if (seconds_remaining <= 0)
				{
					break;
				}
			}
		}
		PopupControl.Instance.ShowRewardAskPopup();
	}

	public void start_daynight_cycle()
	{
		if (daynight_cycle_t == null)
		{
			daynight_cycle_t = daynight_cycle();
			StartCoroutine(daynight_cycle_t);
		}
	}

	public void stop_daynight_cycle()
	{
		if (daynight_cycle_t != null)
		{
			StopCoroutine(daynight_cycle_t);
			daynight_cycle_t = null;
		}
		time_of_day = 0.35f;
		eval_current_daynight();
	}

	private IEnumerator daynight_cycle()
	{
		time_of_day = 0.35f;
		while (true)
		{
			time_of_day += 0.002f;
			if (time_of_day > 1f)
			{
				time_of_day = 0f;
			}
			eval_current_daynight();
			yield return new WaitForSeconds(0.9f);
		}
	}

	private void eval_current_daynight()
	{
		float time = Mathf.Clamp(time_of_day, 0f, 1f);
		Color color = (RenderSettings.ambientLight = day_night_colors.Evaluate(time));
		directional_light.color = color;
		if (NewBiomeControl.Instance.player_zone != "overworld")
		{
			Camera.main.backgroundColor = new Color(0.11f, 0.11f, 0.11f);
		}
		else if (background_type == background_type_t.NPC_background)
		{
			Camera.main.backgroundColor = day_night_SKY.Evaluate(time);
		}
		else if (background_type == background_type_t.explore_background)
		{
			Camera.main.backgroundColor = day_night_exploreBG.Evaluate(time);
		}
	}

	public void SetBackground_Breeder()
	{
		background_type = background_type_t.breeder;
		Camera.main.backgroundColor = Color.white;
	}

	public void SetBackground_NPC()
	{
		background_type = background_type_t.NPC_background;
		eval_current_daynight();
	}

	public void SetBackground_Explore()
	{
		background_type = background_type_t.explore_background;
		eval_current_daynight();
	}

	public void CancelPlayerMovement()
	{
		player.GetComponent<creatureScript>().generic_moveTo = Instance.player.transform.position;
	}

	private void Awake()
	{
		Instance = this;
	}

	public void CreateNibsLists()
	{
		if (!nibs_lists_defined)
		{
			lvlNibs = new List<GameObject>[Loader.n_stats];
			for (int i = 0; i < Loader.n_stats; i++)
			{
				lvlNibs[i] = new List<GameObject>();
			}
			nibs_lists_defined = true;
		}
	}

	private void ResetVariables()
	{
		temp_nextLevelExp_ = NextLevelExp(1);
		playerLevel = 1;
		player_stats = new int[Loader.n_stats];
		level_up_screen_open = (targetShowing = (lock_level_visual = (pause = false)));
		NewBiomeControl.Instance.player_chunk_X = (NewBiomeControl.Instance.player_chunk_Z = (stats_kills = (stats_greenFruits = (stats_redFruits = (stats_blueFruits = (stats_seconds = (stats_minutes = (skillPointsSpendable = (currentEXP = 0)))))))));
		SX = (SZ = (stats_maxDist = (visualEXP = 0f)));
		text_playerLevel.text = "Level 1";
	}

	public void Got_Highscore(bool FIRST_TIME_PLAYING, int ALL_TIME_HIGHSCORE)
	{
		this.FIRST_TIME_PLAYING = FIRST_TIME_PLAYING;
		this.ALL_TIME_HIGHSCORE = ALL_TIME_HIGHSCORE;
	}

	public void press_option_revive()
	{
		new_death_screen.gameObject.SetActive(false);
		int globalInt = PlayerData.Instance.GetGlobalInt("GEMS");
		if (globalInt >= 2)
		{
			PlayerData.Instance.SetGlobalInt("GEMS", globalInt - 2);
			doRevive();
			return;
		}
		NewAudioControl.Instance.play_generic_click();
		Shop_positioner.Instance.HideGameplayGUI();
		Shop_positioner.Instance.revive = true;
		Shop_positioner.Instance.activate_popup(Shop_positioner.config.YES_NO_buttons, "<color=#00aaff>OOPS!</color> You do not have enough Gems.\nGet more gems?", Color.white, Shop_positioner.Instance.purchase_sad, new Color(0.29803923f, 26f / 51f, 0.5294118f));
	}

	public void press_option_respawn()
	{
		PopupControl.Instance.ShowYesNo("Are you sure?", "Yes", "No", PopupControl.context.respawn);
	}

	public void press_option_restart()
	{
		PopupControl.Instance.ShowYesNo("Are you sure?\nYou will start over from Level 1.\n\n(Note: you'll keep all your Gems and Purchases)", "Yes", "No", PopupControl.context.restart_game);
	}

	public void revive_gems_accept()
	{
		NewAudioControl.Instance.play_generic_click();
		Shop_positioner.Instance.Gems_window.SetActive(true);
		WindowControl.Instance.ShowClose(WindowControl.window_type_t.buy_gems_revive);
		Shop_positioner.Instance.Gems_models.SetActive(true);
		Shop_positioner.Instance.purchase_from_revive = true;
	}

	public void revive_cancel()
	{
		new_death_screen.gameObject.SetActive(true);
	}

	public void doRevive()
	{
		RestartGame(true);
		Loader.Instance.skipBreedScreen();
		player_auto_dead = false;
		PlayerData.Instance.SetSlotInt("PLAYER_ALIVE", 1, PlayerData.grouping_t.general);
	}

	public void Restart_Accepted()
	{
		RestartGame(false);
		player_auto_dead = false;
		PlayerData.Instance.SetSlotInt("PLAYER_ALIVE", 0, PlayerData.grouping_t.general);
	}

	public void Respawn_Accepted()
	{
		RestartGame(true);
		int num = levels_to_lose();
		int slotInt = PlayerData.Instance.GetSlotInt("playerLevel", PlayerData.grouping_t.general);
		int value = Mathf.Max(1, slotInt - num);
		PlayerData.Instance.SetSlotInt("playerLevel", value, PlayerData.grouping_t.general);
		int slotInt2 = PlayerData.Instance.GetSlotInt("skillPointsSpendable", PlayerData.grouping_t.general);
		if (slotInt2 >= num)
		{
			slotInt2 -= num;
			PlayerData.Instance.SetSlotInt("skillPointsSpendable", slotInt2, PlayerData.grouping_t.general);
		}
		else
		{
			num -= slotInt2;
			PlayerData.Instance.SetSlotInt("skillPointsSpendable", 0, PlayerData.grouping_t.general);
			while (num > 0)
			{
				int num2 = Random.Range(0, Loader.n_stats);
				int slotInt3 = PlayerData.Instance.GetSlotInt("stat" + num2, PlayerData.grouping_t.general);
				PlayerData.Instance.SetSlotInt("stat" + num2, Mathf.Max(0, slotInt3 - 1), PlayerData.grouping_t.general);
				num--;
			}
		}
		for (int i = 0; i < inventory_ctr.total_INV_spaces; i++)
		{
			PlayerData.Instance.SetSlotString("inventory" + i, "", PlayerData.grouping_t.the_inventory);
			PlayerData.Instance.SetSlotInt("inventoryCount" + i, 0, PlayerData.grouping_t.the_inventory);
		}
		PlayerData.Instance.SetSlotInt("player_chunk_x", 999, PlayerData.grouping_t.playerpos);
		PlayerData.Instance.SetSlotInt("player_chunk_z", 999, PlayerData.grouping_t.playerpos);
		PlayerData.Instance.SetSlotInt("player_inner_x", 999, PlayerData.grouping_t.playerpos);
		PlayerData.Instance.SetSlotInt("player_inner_z", 999, PlayerData.grouping_t.playerpos);
		Loader.Instance.skipBreedScreen();
		player_auto_dead = false;
		PlayerData.Instance.SetSlotInt("PLAYER_ALIVE", 1, PlayerData.grouping_t.general);
	}

	public void RestartGame(bool onrevive)
	{
		new_death_screen.gameObject.SetActive(false);
		NewAudioControl.Instance.play_generic_click();
		reset_ad_numbers();
		for (int i = 0; i < Loader.n_stats; i++)
		{
			set_lvl_name(i, 0);
		}
		Shop_positioner.Instance.disable_top_left_buttons = false;
		Shop_positioner.Instance.hatched_companions.Clear();
		NewBreedControl.Instance.num_rebreeds = 0;
		n_hatchlings_ = 0;
		foreach (GameObject item in trail_nodes__)
		{
			Object.Destroy(item);
		}
		trail_nodes__.Clear();
		NewBiomeControl.Instance.DestroyAllTerrain();
		ClearAllCreatures(false);
		NewMobControl.Instance.curr_respawn_list().Clear();
		inventory_ctr.Instance.already_looted_gold_chests.Clear();
		for (int j = 0; j < Loader.n_stats; j++)
		{
			foreach (GameObject item2 in lvlNibs[j])
			{
				Object.Destroy(item2);
			}
			lvlNibs[j].Clear();
		}
		clear_all_canvas();
		Instance.scenic_elevator.SetActive(true);
		Shop_positioner.Instance.breeder_base.SetActive(true);
		NewBreedControl.Instance.gameObject.SetActive(true);
		if (!onrevive)
		{
			NewAudioControl.Instance.play_menu_and_breed_music();
			NewBreedControl.Instance.transition_back_to_breeder(NewBreedControl.breeder_transition.on_death);
			Instance.stop_daynight_cycle();
		}
	}

	public void ClearAllCreatures(bool keep_companions_and_player)
	{
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject active_combatant in NewMobControl.Instance.active_combatants)
		{
			if (keep_companions_and_player)
			{
				if (player != null && active_combatant == player)
				{
					list.Add(active_combatant);
					continue;
				}
				if (Shop_positioner.Instance.hatched_companions.Contains(active_combatant))
				{
					list.Add(active_combatant);
					continue;
				}
			}
			if (active_combatant.GetComponent<creatureScript>() != null)
			{
				active_combatant.GetComponent<creatureScript>().DeLoad();
			}
		}
		NewMobControl.Instance.active_combatants.Clear();
		NewMobControl.Instance.loaded_creatures.Clear();
		foreach (GameObject item in list)
		{
			NewMobControl.Instance.active_combatants.Add(item);
		}
	}

	private IEnumerator TRACK_GAMETIME()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			stats_seconds++;
			if (stats_seconds == 60)
			{
				stats_seconds = 0;
				stats_minutes++;
			}
		}
	}

	public Chunk_f loadChunk(int x, int z)
	{
		return null;
	}

	public void showOverheadNotif(string str, Vector3 position, bool sound, bool delete_on_many)
	{
		if (sound)
		{
			sfx.PlayOneShot(sfx_exp_get);
		}
		GameObject gameObject = Object.Instantiate(type_expParticle);
		gameObject.transform.parent = NewMobControl.Instance.gameObject.transform;
		gameObject.transform.SetAsFirstSibling();
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one * 0.61f;
		gameObject.transform.localPosition = Vector2.zero;
		gameObject.GetComponent<exp_gain_particle>().Init(position, Camera.main, str);
		possible_destroy.Add(gameObject);
		gameObject.GetComponent<Animation>().Stop();
		gameObject.GetComponent<Animation>().PlayQueued("expText");
		if (delete_on_many)
		{
			if (curr_pickup_overhead != null)
			{
				Object.Destroy(curr_pickup_overhead);
			}
			curr_pickup_overhead = gameObject;
		}
	}

	public void set_player(GameObject creatureObj, Vector3 startPosition, bool bump_down)
	{
		ResetVariables();
		player = Object.Instantiate(type_creature);
		player.name = "PlayerObject";
		player.transform.position = startPosition + Vector3.up * creatureScript.H;
		player.GetComponent<creatureScript>().Init(creatureObj, true, creatureStates.friendly, playerLevel, "", player_stats, Color.white, bump_down);
		string text = "";
		if (creatureObj.GetComponent<creatureModel>().creatures_that_made_me.Count == 2)
		{
			text = first_upper(creatureObj.GetComponent<creatureModel>().creatures_that_made_me[0]) + " + " + first_upper(creatureObj.GetComponent<creatureModel>().creatures_that_made_me[1]);
		}
		txt_GUI_player_combo.text = text;
		NewMobControl.Instance.active_combatants.Add(player);
		apply_player_effects();
		SX = player.transform.position.x;
		SZ = player.transform.position.z;
		track_distance = TRACK_DISTANCE();
		StartCoroutine(track_distance);
		track_gametime = TRACK_GAMETIME();
		StartCoroutine(track_gametime);
		repeat_footstep_sound = REPEAT_FOOTSTEP_SOUND();
		StartCoroutine(repeat_footstep_sound);
	}

	public static string first_upper(string input)
	{
		return (input[0].ToString() ?? "").ToUpper() + input.Substring(1, input.Length - 1);
	}

	private void sound_footstep(bool giant)
	{
		switch (last_footstep_sound)
		{
		case 0:
			if (Random.value < 0.5f)
			{
				last_footstep_sound = 1;
			}
			else
			{
				last_footstep_sound = 2;
			}
			break;
		case 1:
			if (Random.value < 0.5f)
			{
				last_footstep_sound = 0;
			}
			else
			{
				last_footstep_sound = 2;
			}
			break;
		case 2:
			if (Random.value < 0.5f)
			{
				last_footstep_sound = 0;
			}
			else
			{
				last_footstep_sound = 1;
			}
			break;
		}
		if (giant)
		{
			mainCamera.GetComponent<Animation>().Stop();
			mainCamera.GetComponent<Animation>().Play();
			sfx.PlayOneShot(sfx_stepGiant[last_footstep_sound], 0.4f);
		}
		else
		{
			sfx.PlayOneShot(sfx_stepGrass[last_footstep_sound], 0.4f);
		}
	}

	private IEnumerator REPEAT_FOOTSTEP_SOUND()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.05f);
			if (player != null && Vector3.Distance(prevPos, player.transform.position) > 0.9f)
			{
				sound_footstep(player.GetComponent<creatureScript>().isGiant);
				prevPos = player.transform.position;
			}
		}
	}

	public void PlayerDied()
	{
		PlayerData.Instance.SetSlotInt("PLAYER_ALIVE", 2, PlayerData.grouping_t.general);
		stop_reward_ad_timer();
		if (track_distance != null)
		{
			StopCoroutine(track_distance);
		}
		if (track_gametime != null)
		{
			StopCoroutine(track_gametime);
		}
		if (repeat_footstep_sound != null)
		{
			StopCoroutine(repeat_footstep_sound);
		}
		player = null;
		if (targetShowing)
		{
			targeted_circle_graphic.GetComponent<Animation>().PlayQueued("targetDisappear");
		}
		show_death_screen();
		new_death_screen.Stop();
		new_death_screen.Play("new death anm");
	}

	public void show_death_screen()
	{
		new_death_screen.gameObject.SetActive(true);
		if (levels_to_lose() == 0)
		{
			respawn_button_text.text = TranslationControl.Instance.Translate("You lose the items in your inventory.");
			return;
		}
		string text = TranslationControl.Instance.Translate("You lose the items in your inventory, and lose 999 levels.");
		respawn_button_text.text = text.Replace("999", string.Concat(levels_to_lose()));
	}

	public int levels_to_lose()
	{
		return (int)((float)PlayerData.Instance.GetSlotInt("playerLevel", PlayerData.grouping_t.general) * 0.13f);
	}

	public float depth_at(Vector3 position)
	{
		if (player != null)
		{
			return Vector3.Distance(position, startPos);
		}
		return 0f;
	}

	private IEnumerator TRACK_DISTANCE()
	{
		startPos = player.transform.position;
		while (true)
		{
			yield return new WaitForSeconds(0.25f);
			if (player != null)
			{
				float num = depth_at(player.transform.position);
				coordinateDisplay.text = num.ToString("F0") + "<size=21> M</size>";
				if (num > stats_maxDist)
				{
					stats_maxDist = num;
				}
			}
		}
	}

	public void sound_crackshell()
	{
		sfx.PlayOneShot(sfx_crackShell);
	}

	public void sound_mixmutants()
	{
		sfx.PlayOneShot(sfx_mixMutants);
	}

	public void sound_death()
	{
		sfx.PlayOneShot(sfx_death);
	}

	public void sound_craft()
	{
		sfx.PlayOneShot(sfx_craft);
	}

	public void sound_levelButton()
	{
		sfx.PlayOneShot(sfx_levelButton);
		sfx.PlayOneShot(NewAudioControl.Instance.sfx_genericClick);
	}

	public void sound_relax()
	{
		sfx.PlayOneShot(sfx_relax, 0.3f);
	}

	public void level_up_meter_pressed(int index)
	{
		if (!LOCK_LEVEL_SCREEN && skillPointsSpendable > 0)
		{
			skillPointsSpendable--;
			PlayerData.Instance.SetSlotInt("skillPointsSpendable", skillPointsSpendable, PlayerData.grouping_t.general);
			if (skillPointsSpendable == 0)
			{
				text_skillPoints.text = "<color=#cccccc>0</color> <size=33><color=#cccccc>POINTS</color></size>";
			}
			else
			{
				text_skillPoints.text = skillPointsSpendable + " <size=33><color=#cccccc>POINTS</color></size>";
			}
			sound_levelButton();
			stat_meters[index].GetComponent<Animation>().Play();
			create_nib(index);
			player_stats[index]++;
			PlayerData.Instance.SetSlotInt("stat" + index, player_stats[index], PlayerData.grouping_t.general);
			set_lvl_name(index, player_stats[index]);
			apply_player_effects();
			if (player_stats[index] % 6 == 0 && player_stats[index] != 0)
			{
				LOCK_LEVEL_SCREEN = true;
				show_get_perk(index);
			}
		}
	}

	public void apply_player_effects()
	{
		player.GetComponent<creatureScript>().setStats(player_stats, playerLevel);
		float num = player_stats[6];
		bool num2 = perk_controller.mana_available == perk_controller.max_mana;
		perk_controller.max_mana = 0.5f * num + 8f;
		if (num2)
		{
			perk_controller.mana_available = perk_controller.max_mana;
		}
		perk_controller.Instance.update_mana_visual();
	}

	public void create_nib(int index)
	{
		GameObject gameObject = Object.Instantiate(type_levelMeter_nib);
		lvlNibs[index].Add(gameObject);
		gameObject.transform.parent = stat_meters[index].transform;
		gameObject.transform.rotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localPosition = new Vector2(61.5f + (float)((lvlNibs[index].Count - 1) * 40), -11.3f);
		gameObject.GetComponent<Image>().color = nibColors[index];
	}

	public void set_lvl_name(int index, int TO)
	{
		string text = "?";
		switch (index)
		{
		case 0:
			text = "Mining";
			break;
		case 1:
			text = "Health Regen";
			break;
		case 2:
			text = "Health";
			break;
		case 3:
			text = "Attack";
			break;
		case 4:
			text = "Accuracy";
			break;
		case 5:
			text = "Crafting";
			break;
		case 6:
			text = "Power Energy";
			break;
		case 7:
			text = "Dodge";
			break;
		}
		string text2 = "</color>";
		string text3 = " ~</color> " + TO;
		stat_meters[index].transform.Find("skill_title").gameObject.GetComponent<Text>().text = "<color=#ffffff>" + text + ((TO == 0) ? text2 : text3);
	}

	private void show_get_perk(int index)
	{
		StartCoroutine(perk_get_display(index));
	}

	private IEnumerator perk_get_display(int index)
	{
		perk_controller.Instance.sound_maxout();
		for (int j = 0; j < 2; j++)
		{
			for (int i = 0; i < 6; i++)
			{
				lvlNibs[index][i].GetComponent<Animation>().Stop();
				lvlNibs[index][i].GetComponent<Animation>().Play();
				yield return new WaitForSeconds(0.03f);
			}
			yield return new WaitForSeconds(0.05f);
		}
		for (float i2 = 0f; i2 < 10f; i2 += 1f)
		{
			foreach (GameObject item in lvlNibs[index])
			{
				item.transform.localPosition = Vector3.Lerp(item.transform.localPosition, Vector2.zero, i2 / 8f);
			}
			yield return new WaitForEndOfFrame();
		}
		foreach (GameObject item2 in lvlNibs[index])
		{
			Object.Destroy(item2);
		}
		lvlNibs[index].Clear();
		stat_meters[index].GetComponent<Animation>().Play();
		GetComponent<perk_controller>().show_perk_get();
	}

	public void PressViewAchievements()
	{
		// defensive: Instance might be null if this was invoked before Awake()
		if (Instance != null)
		{
			Instance.button_clicked = true;
		}
		else
		{
			Debug.LogWarning("GameController.Instance is null in PressViewAchievements().");
		}

		// If achievements are disabled (e.g. you removed Google Play Games), skip showing platform UI
		if (MultiplayerControl.Instance == null || !MultiplayerControl.Instance.achievementsEnabled)
		{
			Debug.Log("Achievements disabled or MultiplayerControl missing. Skipping ShowAchievementsUI().");
			// still show local info overlay as fallback
			ShowInGameOverlay();
			return;
		}

		// Ensure there's an active social provider and user is authenticated
		if (Social.Active == null || Social.localUser == null || !Social.localUser.authenticated)
		{
			Debug.LogWarning("Cannot show Achievements UI: no active social provider or user not authenticated.");
			ShowInGameOverlay();
			return;
		}

		try
		{
			Social.ShowAchievementsUI();
		}
		catch (System.Exception e)
		{
			Debug.LogError("Exception while showing achievements UI: " + e);
		}

		// original behaviour: show in-game overlay (if objects are not ready, mark pending so scanner will show when found)
		// Prefer inspector-assigned references; if any are missing, start the scanner which will look in DontDestroyOnLoad roots.
		if (overlayCanvas == null || overlayMessage == null || overlayOkayButton == null)
		{
			pendingOverlayShow = true;
			if (overlayScannerCoroutine == null)
			{
				overlayScannerCoroutine = StartCoroutine(OverlayScanner());
			}
			Debug.Log("PressViewAchievements: overlay objects not all present yet, will show when available.");
		}
		else
		{
			ShowInGameOverlay();
		}
	}

	private void ShowInGameOverlay()
	{
		// Prefer inspector-assigned references. If missing, search the DontDestroyOnLoad roots
		// (returned by GetDontDestroyOnLoadObjects) and look through their hierarchies.
		GameObject canvasGO = overlayCanvas;
		GameObject messageGO = overlayMessage;
		GameObject buttonGO = overlayOkayButton;

		if (canvasGO == null || messageGO == null || buttonGO == null)
		{
			GameObject[] roots = GetDontDestroyOnLoadObjects();
			foreach (GameObject root in roots)
			{
				if (root == null) continue;
				Transform[] all = root.GetComponentsInChildren<Transform>(true);
				foreach (Transform t in all)
				{
					if (t == null || string.IsNullOrEmpty(t.name)) continue;
					string n = t.name.ToLower();
					if (canvasGO == null && (n == "canvas - black overlay" || n.Contains("canvas")))
					{
						canvasGO = t.gameObject;
						Debug.Log("ShowInGameOverlay: Found overlay canvas in DontDestroyOnLoad: " + t.name);
					}
					if (messageGO == null && (n == "(center) generic message" || n.Contains("message")))
					{
						messageGO = t.gameObject;
						Debug.Log("ShowInGameOverlay: Found overlay message in DontDestroyOnLoad: " + t.name);
					}
					if (buttonGO == null && (n == "(bottom) okay button" || n.Contains("ok") || n.Contains("button")))
					{
						buttonGO = t.gameObject;
						Debug.Log("ShowInGameOverlay: Found overlay button in DontDestroyOnLoad: " + t.name);
					}
					if (canvasGO != null && messageGO != null && buttonGO != null) break;
				}
				if (canvasGO != null && messageGO != null && buttonGO != null) break;
			}

			if (canvasGO == null) Debug.LogWarning("ShowInGameOverlay: Overlay canvas not found.");
			if (messageGO == null) Debug.LogWarning("ShowInGameOverlay: Overlay message not found.");
			if (buttonGO == null) Debug.LogWarning("ShowInGameOverlay: Overlay button not found.");
		}

		// Prepare and show (if found)
		ShowGameObjectAndParents(canvasGO);
		ShowGameObjectAndParents(messageGO);
		ShowGameObjectAndParents(buttonGO);
	}

	public static GameObject[] GetDontDestroyOnLoadObjects()
	{
    	GameObject temp = null;
    	try
    	{
    	    temp = new GameObject();
    	    Object.DontDestroyOnLoad( temp );
    	    UnityEngine.SceneManagement.Scene dontDestroyOnLoad = temp.scene;
    	    Object.DestroyImmediate( temp );
    	    temp = null;
        	return dontDestroyOnLoad.GetRootGameObjects();
    	}
    	finally
    	{
    	    if( temp != null )
    	        Object.DestroyImmediate( temp );
    	}
	}

	private void ShowGameObjectAndParents(GameObject go)
	{
		if (go == null)
		{
			Debug.LogWarning("ShowGameObjectAndParents: provided GameObject is null.");
			return;
		}
		if (go.name == "(center) generic message" || go.name == "(Center) Generic Message")
		{
			go.transform.GetChild(0).GetComponent<Text>().text = "This feature is currently unavaible. An implementation may be added in a future release.";
		}
		// Activate any disabled parents (so the object can be visible)
		Transform t = go.transform.parent;
		while (t != null)
		{
			if (!t.gameObject.activeSelf)
			{
				t.gameObject.SetActive(true);
			}
			t = t.parent;
		}
		// Enable the object itself
		if (!go.activeSelf) go.SetActive(true);
		// If it has a Canvas, make sure it's enabled and has a camera set (but do not force sorting order here).
		Canvas c = go.GetComponent<Canvas>();
		if (c != null)
		{
			c.enabled = true;
			if (c.worldCamera == null && Shop_positioner.Instance != null)
			{
				c.worldCamera = Shop_positioner.Instance.GetComponent<Camera>();
			}
		}
		// If there's a CanvasGroup, ensure visibility and interaction
		CanvasGroup cg = go.GetComponent<CanvasGroup>();
		if (cg != null)
		{
			cg.alpha = 1f;
			cg.blocksRaycasts = true;
			cg.interactable = true;
		}
		// Bring to front in the hierarchy
		try
		{
			go.transform.SetAsLastSibling();
		}
		catch (System.Exception e)
		{
			Debug.LogWarning("Failed to set sibling for " + go.name + ": " + e.Message);
		}
	}

	/// <summary>
	/// Prepare overlay canvas, message and button so that the canvas stays behind
	/// and the message/button render above it.
	/// </summary>
	private void PrepareAndShowOverlay(GameObject canvasGO, GameObject messageGO, GameObject buttonGO)
	{
		// Activate parents and objects first
		if (canvasGO != null) ShowGameObjectAndParents(canvasGO);
		if (messageGO != null) ShowGameObjectAndParents(messageGO);
		if (buttonGO != null) ShowGameObjectAndParents(buttonGO);

		// If there's a canvas for the overlay, set a base sorting order
		int baseOrder = 1000;
		Canvas overlayCanvasComp = null;
		if (canvasGO != null)
		{
			overlayCanvasComp = canvasGO.GetComponent<Canvas>();
			if (overlayCanvasComp == null)
			{
				// maybe the canvas is on a parent
				overlayCanvasComp = canvasGO.GetComponentInParent<Canvas>();
			}
			if (overlayCanvasComp != null)
			{
				overlayCanvasComp.overrideSorting = true;
				overlayCanvasComp.sortingOrder = baseOrder;
			}
		}

		// Ensure message and button render above the base canvas
		int messageOrder = baseOrder + 1;
		int buttonOrder = baseOrder + 2;

		if (messageGO != null)
		{
			Canvas msgCanvas = messageGO.GetComponent<Canvas>() ?? messageGO.GetComponentInParent<Canvas>(true);
			if (msgCanvas != null)
			{
				msgCanvas.overrideSorting = true;
				msgCanvas.sortingOrder = messageOrder;
			}
			// bring message to top of its canvas hierarchy
			try { messageGO.transform.SetAsLastSibling(); } catch { }
		}

		if (buttonGO != null)
		{
			Canvas btnCanvas = buttonGO.GetComponent<Canvas>() ?? buttonGO.GetComponentInParent<Canvas>(true);
			if (btnCanvas != null)
			{
				btnCanvas.overrideSorting = true;
				btnCanvas.sortingOrder = buttonOrder;
			}
			try { buttonGO.transform.SetAsLastSibling(); } catch { }
		}
	}

	private IEnumerator OverlayScanner()
	{
		int attempts = 0;
		while (attempts < 60)
		{
			bool foundAny = false;
			// Search DontDestroyOnLoad roots and their hierarchies for missing overlay refs
			if (overlayCanvas == null || overlayMessage == null || overlayOkayButton == null)
			{
				GameObject[] roots = GetDontDestroyOnLoadObjects();
				foreach (GameObject root in roots)
				{
					if (root == null) continue;
					Transform[] all = root.GetComponentsInChildren<Transform>(true);
					foreach (Transform t in all)
					{
						if (t == null || string.IsNullOrEmpty(t.name)) continue;
						string n = t.name.ToLower();
						if (overlayCanvas == null && (n == "canvas - black overlay" || n.Contains("canvas")))
						{
							overlayCanvas = t.gameObject;
							Debug.Log("OverlayScanner: found canvas in DontDestroyOnLoad: " + t.name);
							foundAny = true;
						}
						if (overlayMessage == null && (n == "(center) generic message" || n.Contains("message")))
						{
							overlayMessage = t.gameObject;
							Debug.Log("OverlayScanner: found message in DontDestroyOnLoad: " + t.name);
							foundAny = true;
						}
						if (overlayOkayButton == null && (n == "(bottom) okay button" || n.Contains("ok") || n.Contains("button")))
						{
							overlayOkayButton = t.gameObject;
							Debug.Log("OverlayScanner: found button in DontDestroyOnLoad: " + t.name);
							foundAny = true;
						}
						if (overlayCanvas != null && overlayMessage != null && overlayOkayButton != null) break;
					}
					if (overlayCanvas != null && overlayMessage != null && overlayOkayButton != null) break;
				}
			}

			if (overlayCanvas != null || overlayMessage != null || overlayOkayButton != null)
			{
				// If we found any, cache references but do not activate them unless a show was requested.
				// This prevents the overlay from appearing automatically on scene load.
				Debug.Log("OverlayScanner: cached overlay references (not activating).\n  canvas=" + (overlayCanvas?overlayCanvas.name:"null") + ", message=" + (overlayMessage?overlayMessage.name:"null") + ", button=" + (overlayOkayButton?overlayOkayButton.name:"null"));
				if (pendingOverlayShow)
				{
					// If a show is pending (user requested), then prepare+show now.
					PrepareAndShowOverlay(overlayCanvas, overlayMessage, overlayOkayButton);
				}
			}

			if (overlayCanvas != null && overlayMessage != null && overlayOkayButton != null)
			{
				if (pendingOverlayShow)
				{
					Debug.Log("OverlayScanner: all overlay objects found, showing overlay now.");
					pendingOverlayShow = false;
					// arrange canvas/message/button ordering and show
					PrepareAndShowOverlay(overlayCanvas, overlayMessage, overlayOkayButton);
				}
				overlayScannerCoroutine = null;
				yield break;
			}

			// If nothing found this iteration, wait and retry
			if (!foundAny)
			{
				yield return new WaitForSeconds(0.5f);
			}
			else
			{
				// small delay to allow object initialization
				yield return new WaitForSeconds(0.1f);
			}
			attempts++;
		}
		Debug.LogWarning("OverlayScanner: timed out without finding all overlay objects.");
		overlayScannerCoroutine = null;
	}

	/// <summary>
	/// Register overlay object references at runtime (alternative to assigning in Inspector).
	/// Call this from other setup code if the objects are created dynamically.
	/// </summary>
	public void RegisterOverlayObjects(GameObject canvas, GameObject message, GameObject button)
	{
		overlayCanvas = canvas;
		overlayMessage = message;
		overlayOkayButton = button;
		Debug.Log("RegisterOverlayObjects: overlay references registered: " + (canvas?canvas.name:"null") + ", " + (message?message.name:"null") + ", " + (button?button.name:"null"));
	}
	public void reset_ad_numbers()
	{
		levelups = 0;
	}

	public void ATTEMPT_AD()
	{
		levelups++;
		int num = ((playerLevel >= 12) ? 2 : 3);
		if (levelups >= num && can_show_ad)
		{
			levelups = 0;
			stop_reward_ad_timer();
			LOCK_LEVEL_SCREEN = false;
			can_show_ad = false;
			Loader.Instance.SHOW_AD(Loader.ad_context.after_few_levelups);
		}
		else
		{
			LOCK_LEVEL_SCREEN = false;
		}
	}

	public void level_up_screen_close()
	{
		if (player == null)
		{
			return;
		}
		if (LOCK_LEVEL_SCREEN)
		{
			Debug.Log("LOCKED");
			return;
		}
		button_clicked = true;
		if (playerLevel >= 3)
		{
			MultiplayerControl.Instance.unlock_achievement("CgkI7aPb66UWEAIQCA");
		}
		if (playerLevel >= 15)
		{
			MultiplayerControl.Instance.unlock_achievement("CgkI7aPb66UWEAIQAw");
		}
		if (playerLevel >= 25)
		{
			MultiplayerControl.Instance.unlock_achievement("CgkI7aPb66UWEAIQBQ");
		}
		if (playerLevel >= 50)
		{
			MultiplayerControl.Instance.unlock_achievement("CgkI7aPb66UWEAIQBw");
		}
		if (playerLevel >= 100)
		{
			MultiplayerControl.Instance.unlock_achievement("CgkI7aPb66UWEAIQBg");
		}
		Shop_positioner.Instance.disable_top_left_buttons = false;
		WindowControl.Instance.GetComponent<Animation>().Stop();
		WindowControl.Instance.GetComponent<Animation>().Play("levelup hide");
		level_up_screen_open = false;
		pause = false;
		NewAudioControl.Instance.play_generic_click();
		start_reward_ad_timer();
	}

	public void done_with_chair()
	{
		player_in_chair = false;
		player.GetComponent<SphereCollider>().enabled = true;
		player.GetComponent<Rigidbody>().isKinematic = false;
	}

	public void player_interact()
	{
		if (interaction_target == null)
		{
			return;
		}
		if (interaction_target.GetComponent<NewCollectible>() != null)
		{
			NewCollectible component = interaction_target.GetComponent<NewCollectible>();
			int count = 1;
			string special = "";
			if (component.corresponding_inventory_object == "Coins Few")
			{
				count = Random.Range(2, 5);
				component.corresponding_inventory_object = "Coins";
			}
			else if (component.corresponding_inventory_object == "Egg")
			{
				special = component.flag_special;
			}
			if (inventory_ctr.Instance.try_give_item(component.corresponding_inventory_object, count, player.transform.position, special))
			{
				component.VisuallyHarvest();
				if (NewBiomeControl.Instance.active_interactibles.Contains(interaction_target))
				{
					NewBiomeControl.Instance.active_interactibles.Remove(interaction_target);
				}
				if (component.respawn_time != -1)
				{
					Dictionary<Vector3, respawn> dictionary = NewMobControl.Instance.curr_respawn_list();
					if (!dictionary.ContainsKey(interaction_target.transform.position))
					{
						dictionary.Add(interaction_target.transform.position, new respawn(component.respawn_time));
					}
				}
			}
		}
		else if (interaction_target.GetComponent<NewInteractable>() != null)
		{
			NewInteractable component2 = interaction_target.GetComponent<NewInteractable>();
			switch (component2.interaction_type)
			{
			case NewInteractable.interaction.crafting_table:
				inventory_ctr.Instance.open_craftingTable();
				break;
			case NewInteractable.interaction.anvil:
				inventory_ctr.Instance.open_anvil();
				break;
			case NewInteractable.interaction.cauldron:
				inventory_ctr.Instance.open_cauldron();
				break;
			case NewInteractable.interaction.gold_chest:
			case NewInteractable.interaction.titanium_chest:
				if (!MultiplayerControl.Instance.auto_time_enabled())
				{
					PopupControl.Instance.ShowMessage("You must have auto-time enabled in your Phone Settings to open Loot Chests.");
					break;
				}
				inventory_ctr.Instance.open_gold_chest(component2.unique_id, component2.interaction_type == NewInteractable.interaction.titanium_chest, interaction_target.transform.position);
				if (component2.interaction_type == NewInteractable.interaction.gold_chest)
				{
					inventory_ctr.Instance.CHEST_TYPE = inventory_ctr.chest_type.gold_chest;
				}
				else
				{
					inventory_ctr.Instance.CHEST_TYPE = inventory_ctr.chest_type.titanium_chest;
				}
				inventory_ctr.Instance.open_container_id = component2.unique_id;
				inventory_ctr.Instance.open_container();
				break;
			case NewInteractable.interaction.basket:
				inventory_ctr.Instance.CHEST_TYPE = inventory_ctr.chest_type.basket;
				inventory_ctr.Instance.open_container_id = component2.unique_id;
				inventory_ctr.Instance.open_container();
				break;
			case NewInteractable.interaction.chest:
				inventory_ctr.Instance.CHEST_TYPE = inventory_ctr.chest_type.personal_chest;
				inventory_ctr.Instance.open_container_id = component2.unique_id;
				inventory_ctr.Instance.open_container();
				break;
			case NewInteractable.interaction.chair_normal:
			case NewInteractable.interaction.chair_w_sound:
			{
				if (component2.interaction_type != NewInteractable.interaction.chair_normal)
				{
					sound_relax();
				}
				player_in_chair = true;
				inventory_ctr.Instance.show_DONE_button("DONE", inventory_ctr.button_state.DONE_SITTING_CHAIR);
				Shop_positioner.Instance.HideGameplayGUI();
				player.GetComponent<SphereCollider>().enabled = false;
				player.GetComponent<Rigidbody>().isKinematic = true;
				player.GetComponent<creatureScript>().velocity = Vector3.zero;
				Transform transform = interaction_target.transform.Find("target");
				player.transform.position = new Vector3(transform.position.x, creatureScript.H + 0.5f, transform.position.z);
				player.GetComponent<creatureScript>().look_rotation.rotation = transform.transform.rotation;
				break;
			}
			case NewInteractable.interaction.NPC:
				if (!Shop_positioner.Instance.try_open_generic_window())
				{
					return;
				}
				focus_npc = interaction_target.transform.GetChild(0).GetComponent<creatureModel>().head_obj;
				Instance.player.GetComponent<creatureScript>().myCreatureModel.gameObject.SetActive(false);
				DialogueControl.Instance.EnterDialogue(component2.unique_id);
				WindowControl.Instance.ShowClose(WindowControl.window_type_t.dialogue);
				break;
			case NewInteractable.interaction.enter_shack:
			case NewInteractable.interaction.enter_mansion:
			case NewInteractable.interaction.exit_house:
			{
				bool flag = false;
				if (component2.interaction_type == NewInteractable.interaction.enter_shack)
				{
					flag = true;
					NewAudioControl.Instance.Play(NewAudioControl.Instance.sfx_opendoor, 0.5f);
					Instance.begin_enter_house_transition(component2.unique_id, shack_type.shack);
				}
				else if (component2.interaction_type == NewInteractable.interaction.enter_mansion)
				{
					flag = true;
					NewAudioControl.Instance.Play(NewAudioControl.Instance.sfx_opendoor, 0.5f);
					Instance.begin_enter_house_transition(component2.unique_id, shack_type.mansion);
				}
				else if (component2.interaction_type == NewInteractable.interaction.exit_house)
				{
					Instance.begin_exit_house_transition();
				}
				if (flag)
				{
					Vector3 chunkCoords = NewBiomeControl.Instance.GetChunkCoords(interaction_target.transform.parent.gameObject.transform.position);
					NewBiomeControl.Instance.zone_origin_chunkX = (int)chunkCoords.x;
					NewBiomeControl.Instance.zone_origin_chunkZ = (int)chunkCoords.z;
					Vector3 inner = NewBiomeControl.Instance.GetInner(interaction_target.transform.parent.gameObject.transform.position);
					NewBiomeControl.Instance.zone_origin_innerX = (int)inner.x;
					NewBiomeControl.Instance.zone_origin_innerZ = (int)inner.z;
					NewBiomeControl.Instance.zone_rotation = component2.temp_rot;
					Vector3 chunkCoords2 = NewBiomeControl.Instance.GetChunkCoords(player.transform.position);
					NewBiomeControl.Instance.shack_entrance_chunkX = (int)chunkCoords2.x;
					NewBiomeControl.Instance.shack_entrance_chunkZ = (int)chunkCoords2.z;
					Vector3 inner2 = NewBiomeControl.Instance.GetInner(player.transform.position);
					NewBiomeControl.Instance.shack_entrance_innerX = (int)inner2.x;
					NewBiomeControl.Instance.shack_entrance_innerZ = (int)inner2.z;
				}
				break;
			}
			}
		}
		interaction_target = null;
		player_interacting = false;
	}

	private void on_click_floor()
	{
		if (player == null)
		{
			return;
		}
		bool flag = true;
		player_interacting = false;
		GameObject targetCombatant = player.GetComponent<creatureScript>().targetCombatant;
		player.GetComponent<creatureScript>().targetCombatant = null;
		foreach (GameObject active_combatant in NewMobControl.Instance.active_combatants)
		{
			if (active_combatant.GetComponent<NewCombatant>().HP <= 0f || active_combatant == player || Shop_positioner.Instance.hatched_companions.Contains(active_combatant) || !active_combatant.activeInHierarchy || !(Vector3.Distance(active_combatant.transform.position, previous_step_clicked_at) < active_combatant.GetComponent<NewCombatant>().scale))
			{
				continue;
			}
			player.GetComponent<creatureScript>().targetCombatant = active_combatant;
			player.GetComponent<creatureScript>().generic_moveTo = Vector3.zero;
			if (active_combatant.GetComponent<NewCombatant>().mob_type == NewCombatant.TYPE_T.mineral)
			{
				targeted_circle_graphic.transform.Find("graphic").GetComponent<Renderer>().material.mainTexture = target_circle_yellow;
			}
			else
			{
				targeted_circle_graphic.transform.Find("graphic").GetComponent<Renderer>().material.mainTexture = target_circle_red;
			}
			targeted_circle_graphic.transform.Find("graphic").transform.localScale = Vector3.one * active_combatant.GetComponent<NewCombatant>().scale;
			if (active_combatant == targetCombatant)
			{
				flag = false;
				rapid_click_count++;
				if (rapid_click_count == 6 && !shown_rapid_click_display)
				{
					PAUSE_GAME();
					PopupControl.Instance.ShowMessage("Note:\n\nYou only need to click an enemy\n<color=#00ff00>once</color> to target it.\n\nYour creature will attack it\nrepeatedly on it's own.", PopupControl.context.rapidclicknotif);
					shown_rapid_click_display = true;
				}
			}
			else
			{
				rapid_click_count = 0;
			}
			break;
		}
		if (player.GetComponent<creatureScript>().targetCombatant == null)
		{
			bool flag2 = false;
			foreach (GameObject active_interactible in NewBiomeControl.Instance.active_interactibles)
			{
				if (Vector3.Distance(active_interactible.transform.position, previous_step_clicked_at) < 0.85f)
				{
					targeted_circle_graphic.transform.Find("graphic").GetComponent<Renderer>().material.mainTexture = target_circle_yellow;
					targeted_circle_graphic.transform.Find("graphic").transform.localScale = Vector3.one * 1.1f;
					targeted_circle_graphic.transform.position = new Vector3(active_interactible.transform.position.x, circ_h, active_interactible.transform.position.z);
					player.GetComponent<creatureScript>().generic_moveTo = new Vector3(active_interactible.transform.position.x, creatureScript.H, active_interactible.transform.position.z);
					flag2 = true;
					if (active_interactible.GetComponent<NewInteractable>() != null)
					{
						player.GetComponent<creatureScript>().extra_interact_dist = active_interactible.GetComponent<NewInteractable>().extra_interact_dist;
					}
					else
					{
						player.GetComponent<creatureScript>().extra_interact_dist = active_interactible.GetComponent<NewCollectible>().extra_interact_dist;
					}
					player_interacting = true;
					interaction_target = active_interactible;
					break;
				}
			}
			if (!flag2)
			{
				targeted_circle_graphic.transform.Find("graphic").GetComponent<Renderer>().material.mainTexture = target_circle_blue;
				targeted_circle_graphic.transform.Find("graphic").transform.localScale = Vector3.one * player.GetComponent<NewCombatant>().scale;
				targeted_circle_graphic.transform.position = new Vector3(previous_step_clicked_at.x, circ_h, previous_step_clicked_at.z);
				player.GetComponent<creatureScript>().generic_moveTo = previous_step_clicked_at;
				player.GetComponent<creatureScript>().extra_interact_dist = 0f;
			}
		}
		if (flag)
		{
			targeted_circle_graphic.GetComponent<Animation>().Stop();
			targeted_circle_graphic.GetComponent<Animation>().PlayQueued("targetGraphic");
			targetShowing = true;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.D) && Application.isEditor)
		{
			player.GetComponent<NewCombatant>().die(player.GetComponent<creatureScript>(), 0.5f, 0f);
		}
		if (Input.GetKeyDown(KeyCode.I))
		{
			bool isEditor = Application.isEditor;
		}
		if (!previous_step_click_detected && Input.GetMouseButtonDown(0) && !pause && !player_in_chair && !player_auto_dead)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float enter;
			if (plane.Raycast(ray, out enter))
			{
				previous_step_click_detected = true;
				previous_step_clicked_at = ray.GetPoint(enter) + Vector3.up * creatureScript.H;
			}
		}
	}

	public void begin_exit_house_transition()
	{
		if (!ENTER_SHACK_TRANSITION_PLAYING)
		{
			PAUSE_GAME();
			ENTER_SHACK_TRANSITION_PLAYING = true;
			splash_enter_house.SetActive(true);
			splash_enter_house.GetComponent<Animation>().Play();
			on_exit_house = true;
		}
	}

	public void begin_enter_house_transition(int shack_id, shack_type type)
	{
		if (!ENTER_SHACK_TRANSITION_PLAYING)
		{
			PAUSE_GAME();
			ENTER_SHACK_TRANSITION_PLAYING = true;
			curr_shack_id = shack_id;
			curr_shack_type = type;
			splash_enter_house.SetActive(true);
			splash_enter_house.GetComponent<Animation>().Play();
			on_exit_house = false;
		}
	}

	public IEnumerator delayed_teleport()
	{
		yield return new WaitForSeconds(2.5f);
		on_teleport = true;
		splash_enter_house.SetActive(true);
		splash_enter_house.GetComponent<Animation>().Play();
	}

	public void DoTeleport()
	{
		player.transform.parent = null;
		player.transform.position = NewBreedControl.Instance.campos_result.transform.position + Vector3.up * 2f;
		player.transform.localScale = Vector3.one;
		player.GetComponent<Rigidbody>().isKinematic = false;
		exit_shack_extra();
		Shop_positioner.Instance.ShowGameplayGUI();
		pause = false;
		on_teleport = false;
	}

	public void do_exit_shack()
	{
		give_all_display();
		NewAudioControl.Instance.Play(NewAudioControl.Instance.sfx_closedoor, 0.6f);
		ENTER_SHACK_TRANSITION_PLAYING = false;
		circ_h = 0.1f;
		exit_shack_extra();
		bool flag = true;
		if (NewBiomeControl.Instance.shack_entrance_chunkX == 0 && NewBiomeControl.Instance.shack_entrance_chunkZ == 0 && NewBiomeControl.Instance.shack_entrance_innerX == 0 && NewBiomeControl.Instance.shack_entrance_innerZ == 0)
		{
			flag = false;
		}
		if (flag)
		{
			player.transform.position = new Vector3((float)NewBiomeControl.Instance.shack_entrance_chunkX * 10f + (float)NewBiomeControl.Instance.shack_entrance_innerX, 0.5f, (float)NewBiomeControl.Instance.shack_entrance_chunkZ * 10f + (float)NewBiomeControl.Instance.shack_entrance_innerZ);
		}
		pause = false;
	}

	private void exit_shack_extra()
	{
		Shop_positioner.Instance.enable_objects();
		NewBiomeControl.Instance.player_zone = "overworld";
		NewBiomeControl.Instance.player_zone_type = "";
		NewBiomeControl.Instance.TerrainChanged(true);
		Loader.Instance.snap_cam();
		breeder_floor_plane.SetActive(false);
		if (curr_shack_type == shack_type.shack)
		{
			NewBiomeControl.Instance.shack_interior.SetActive(false);
		}
		else if (curr_shack_type == shack_type.mansion)
		{
			NewBiomeControl.Instance.mansion_interior.SetActive(false);
		}
	}

	public void ClickBackToMenu()
	{
		PAUSE_GAME();
		MultiplayerControl.Instance.was_on_press_back_to_menu = true;
		PopupControl.Instance.HideAll();
		PopupControl.Instance.ShowConnecting("Loading Game");
		StartCoroutine(async_return_to_menu());
	}

	private IEnumerator async_return_to_menu()
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Menu");
		while (!asyncLoad.isDone)
		{
			yield return null;
		}
	}

	public void load_shack()
	{
		clear_all_canvas();
		ENTER_SHACK_TRANSITION_PLAYING = false;
		if (curr_shack_type == shack_type.shack)
		{
			NewBiomeControl.Instance.player_zone_type = "shack";
		}
		else if (curr_shack_type == shack_type.mansion)
		{
			NewBiomeControl.Instance.player_zone_type = "mansion";
		}
		GameObject gameObject = NewBiomeControl.Instance.set_zone_models();
		NewBiomeControl.Instance.player_zone = "shack" + curr_shack_id;
		Debug.Log(Time.time + " ... " + NewBiomeControl.Instance.player_zone);
		NewBiomeControl.Instance.TerrainChanged(true);
		player.transform.position = find_in_children("exit", gameObject.transform).transform.position + Vector3.up * 0.5f;
		Loader.Instance.snap_cam();
		pause = false;
	}

	public GameObject find_in_children(string t_name, Transform start)
	{
		if (start.gameObject.name == t_name)
		{
			return start.gameObject;
		}
		for (int i = 0; i < start.transform.childCount; i++)
		{
			GameObject gameObject = find_in_children(t_name, start.transform.GetChild(i));
			if (gameObject != null)
			{
				return gameObject;
			}
		}
		return null;
	}

	public void clear_all_canvas()
	{
		foreach (GameObject item in possible_destroy)
		{
			if (item != null)
			{
				Object.Destroy(item);
			}
		}
		possible_destroy.Clear();
	}

	public void give_all_display()
	{
		foreach (GameObject active_combatant in NewMobControl.Instance.active_combatants)
		{
			if (active_combatant.GetComponent<NewCombatant>().mob_type == NewCombatant.TYPE_T.creature && active_combatant != player && active_combatant.GetComponent<creatureScript>().levelDisplay == null)
			{
				active_combatant.GetComponent<creatureScript>().assign_level_display();
			}
		}
		foreach (GameObject active_interactible in NewBiomeControl.Instance.active_interactibles)
		{
			if (active_interactible.GetComponent<NewInteractable>() != null && active_interactible.GetComponent<NewInteractable>().interaction_type == NewInteractable.interaction.NPC)
			{
				active_interactible.GetComponent<NewInteractable>().assign_overhead_icon();
			}
		}
	}

	public void PAUSE_GAME()
	{
		pause = true;
	}

	public void animation_set_levelup_text()
	{
		text_playerLevel.text = "Level Up";
		sfx.PlayOneShot(sfx_got_level_b);
	}

	public void animation_sound_levelScreenAppear()
	{
		sfx.PlayOneShot(sfx_got_level);
	}

	public void sound_gameStart(float intensity)
	{
		sfx.PlayOneShot(sfx_gamestart);
	}

	public void sound_creature_hit(float intensity)
	{
		sfx.PlayOneShot(sfx_creatures[sound_creature_hit_iterator++ % 3]);
	}

	public void sound_player_hit()
	{
		sfx.PlayOneShot(sfx_player[sound_player_hit_iterator++ % 2]);
	}

	public void animation_unlock_levelup_text()
	{
		text_playerLevel.text = "Level " + playerLevel;
		lock_level_visual = false;
	}

	public void DebugPressRed()
	{
		button_clicked = true;
		currentEXP += 10;
	}

	private void on_cast_spell()
	{
		GameObject gameObject = null;
		foreach (GameObject active_combatant in NewMobControl.Instance.active_combatants)
		{
			if (!(active_combatant == null) && !(active_combatant == player) && active_combatant.GetComponent<NewCombatant>().mob_type != NewCombatant.TYPE_T.mineral && (active_combatant.GetComponent<NewCombatant>().mob_type != NewCombatant.TYPE_T.creature || active_combatant.GetComponent<creatureScript>().STATE != creatureStates.friendly) && Vector3.Distance(active_combatant.transform.position, previous_step_clicked_at) < 1.5f)
			{
				gameObject = active_combatant;
				break;
			}
		}
		if (player == null)
		{
			inventory_ctr.Instance.click_DONE_placing();
		}
		else if (gameObject != null)
		{
			perk_controller.Instance.CAST(player, perk_controller.Instance.about_to_cast_str, perk_controller.Instance.about_to_cast_lvl, gameObject);
			if (perk_controller.Instance.about_to_cast_str == "perk_ram")
			{
				targeted_circle_graphic.transform.Find("graphic").GetComponent<Renderer>().material.mainTexture = target_circle_red;
				targeted_circle_graphic.GetComponent<Animation>().Stop();
				targeted_circle_graphic.GetComponent<Animation>().PlayQueued("targetGraphic");
				targetShowing = true;
			}
			inventory_ctr.Instance.click_DONE_placing();
			perk_controller.Instance.spendMana(perk_controller.Instance.get_cost(perk_controller.Instance.about_to_cast_str));
		}
	}

	private void FixedUpdate()
	{
		if (player != null && Vector3.Distance(player.transform.position, prev_no_fall_thru_floor_pos) > 3f)
		{
			no_fall_thru_floor.transform.position = new Vector3(player.transform.position.x, 0f, player.transform.position.z);
			prev_no_fall_thru_floor_pos = no_fall_thru_floor.transform.position;
		}
		no_spam_ads_counter--;
		if (no_spam_ads_counter <= 0)
		{
			can_show_ad = true;
			no_spam_ads_counter = 1200;
		}
		if (previous_step_click_detected && !button_clicked)
		{
			if (is_casting_spell)
			{
				on_cast_spell();
			}
			else
			{
				on_click_floor();
			}
		}
		previous_step_click_detected = (button_clicked = false);
		if (player != null)
		{
			GameObject targetCombatant = player.GetComponent<creatureScript>().targetCombatant;
			if (targetCombatant != null)
			{
				targeted_circle_graphic.transform.position = new Vector3(targetCombatant.transform.position.x, circ_h, targetCombatant.transform.position.z);
			}
			Quaternion b;
			Vector3 vector;
			if (focus_npc == null)
			{
				vector = player.transform.position + new Vector3(5f, 15f, -5f) * (cam_offset + eagle_offset + giant_offset) * NewBiomeControl.Instance.zone_view_scale;
				b = Quaternion.LookRotation((player.transform.position - vector).normalized);
			}
			else
			{
				vector = focus_npc.transform.position + new Vector3(2f, 0f, -1.3f) * 0.74f;
				vector = new Vector3(vector.x, 0.7f, vector.z);
				Vector3 vector2 = 0.12f * Vector3.down;
				b = Quaternion.LookRotation((focus_npc.transform.position + vector2 - vector).normalized);
			}
			mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, vector, Time.deltaTime * 4f);
			mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, b, Time.deltaTime * 4f);
			if (!is_casting_spell)
			{
				visualEXP = Mathf.Lerp(visualEXP, currentEXP, Time.deltaTime * 5f);
				float num = visualEXP / (float)temp_nextLevelExp_;
				if (num >= 1f)
				{
					visualEXP = 0f;
					num = 0f;
					currentEXP -= temp_nextLevelExp_;
					PlayerData.Instance.SetSlotInt("currentEXP", currentEXP, PlayerData.grouping_t.general);
					playerLevel++;
					PlayerData.Instance.SetSlotInt("playerLevel", playerLevel, PlayerData.grouping_t.general);
					skillPointsSpendable++;
					PlayerData.Instance.SetSlotInt("skillPointsSpendable", skillPointsSpendable, PlayerData.grouping_t.general);
					temp_nextLevelExp_ = NextLevelExp(playerLevel);
					PlayerData.Instance.SetSlotInt("temp_nextLevelExp", temp_nextLevelExp_, PlayerData.grouping_t.general);
					text_skillPoints.text = skillPointsSpendable + " <size=33><color=#cccccc>POINTS</color></size>";
					player.GetComponent<creatureScript>().setStats(player_stats, playerLevel);
					Object.Instantiate(Instance.type_friendly_levelup_particles).transform.position = new Vector3(player.transform.position.x, 0f, player.transform.position.z);
					if (playerLevel > ALL_TIME_HIGHSCORE)
					{
						ALL_TIME_HIGHSCORE = playerLevel;
						PlayerData.Instance.SetGlobalInt("High Score", playerLevel);
					}
					if (!lock_level_visual)
					{
						text_playerLevel.text = "Level " + playerLevel;
					}
					if (!level_up_screen_open && player != null)
					{
						Shop_positioner.Instance.disable_top_left_buttons = true;
						levelbar.Play("on level up");
						lock_level_visual = true;
						level_up_screen_open = true;
						LOCK_LEVEL_SCREEN = true;
					}
				}
				if (temp_nextLevelExp_ != 0)
				{
					expBarYellow.rectTransform.sizeDelta = new Vector2(358f * num, 18f);
					expBarYellow.transform.localPosition = new Vector2((1f - num) * -178f, 0f);
				}
			}
		}
		if (!(player != null))
		{
			return;
		}
		if (trail_nodes__.Count == 0)
		{
			GameObject gameObject = Object.Instantiate(temp_trailer);
			gameObject.transform.position = player.transform.position;
			trail_nodes__.Add(gameObject);
		}
		else if (Vector3.Distance(player.transform.position, trail_nodes__[0].transform.position) > 1.3f)
		{
			GameObject gameObject2 = Object.Instantiate(temp_trailer);
			gameObject2.transform.position = player.transform.position;
			trail_nodes__.Insert(0, gameObject2);
			if (trail_nodes__.Count > n_hatchlings_ * 2 + 2)
			{
				GameObject gameObject3 = trail_nodes__[trail_nodes__.Count - 1];
				trail_nodes__.Remove(gameObject3);
				Object.Destroy(gameObject3);
			}
		}
	}

	public void press_teleport()
	{
		button_clicked = true;
		PopupControl.Instance.ShowYesNo("Teleport to start area?", "Yes", "No", PopupControl.context.teleport);
	}

	public int NextLevelExp(int curr_level)
	{
		if (curr_level <= 26)
		{
			return (int)(9f + 2f * (float)(curr_level - 1));
		}
		return 60;
	}

	public void start_creature_animation(GameObject[] g, int animationIndex)
	{
		float speed_ = 0.7f;
		float spaghetti_ = 18f;
		bool inversion = true;
		switch (animationIndex)
		{
		case 1:
			speed_ = 0.15f;
			inversion = false;
			break;
		case 2:
			speed_ = 0.15f;
			inversion = false;
			break;
		case 3:
			speed_ = 0.16f;
			spaghetti_ = 90f;
			inversion = false;
			break;
		}
		for (int i = 0; i < g.Length; i++)
		{
			g[i].GetComponent<limb_scr>().createandplayAnimation(g[i].GetComponent<limb_scr>().frames_snapPositions[animationIndex], g[i].GetComponent<limb_scr>().frames_rotations[animationIndex], speed_, inversion, spaghetti_);
		}
	}

	public void AssignHealthbar(GameObject obj)
	{
		sfx.PlayOneShot(sfx_flesh_impact[Random.Range(0, 5)]);
		if (obj.GetComponent<NewCombatant>().healthbar == null)
		{
			creatureScript component = obj.GetComponent<creatureScript>();
			if (component != null && component.levelDisplay != null)
			{
				possible_destroy.Remove(component.levelDisplay);
				Object.Destroy(component.levelDisplay);
			}
			obj.GetComponent<NewCombatant>().RecieveHealthbar(Object.Instantiate(type_healthbar), Camera.main);
		}
		else
		{
			obj.GetComponent<NewCombatant>().RefreshHealthbarDisappearTimer(5f);
		}
	}
}
