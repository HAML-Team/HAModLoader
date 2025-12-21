using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewMenuController : MonoBehaviour
{
	public static NewMenuController Instance;

	public GameObject submenu;

	public GameObject singleplayer_elements;

	public GameObject multiplayer_elements;

	public RectTransform black;

	public RectTransform background;

	public Text submenu_header;

	public Color slot_active;

	public Color slot_header_active;

	public GameObject[] sp_slots;

	public GameObject[] sp_creatures;

	public GameObject delete_general_button;

	[HideInInspector]
	public bool in_sumenu;

	public Sprite spr_muted;

	public Sprite spr_not_muted;

	public Image muted_ico;

	private Color slot_header_inactive;

	private Color slot_inactive;

	public GameObject puke;

	public Text narwhalText;

	private GameObject spinningNarwhal;

	private bool dialog_changeable = true;

	private int DIALOG_STATE;

	public Animation narwhalTextAnm;

	public string[] menu_creatures;

	private float currentMouseX;

	private float currentMouseY;

	private bool spinning = true;

	private float tick = 0.4f;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		PopupControl.Instance.error_log = "";
		CreateNarwhal();
		sp_creatures = new GameObject[3];
		slot_header_inactive = sp_slots[0].transform.Find("slot num").gameObject.GetComponent<Text>().color;
		slot_inactive = sp_slots[0].GetComponent<Image>().color;
		PopupControl.Instance.black_background.GetComponent<Canvas>().worldCamera = Camera.main;
		CapPixelsB();
		if (MultiplayerControl.Instance.was_on_press_back_to_menu)
		{
			PopupControl.Instance.HideAll();
			NewAudioControl.Instance.play_menu_and_breed_music();
			MultiplayerControl.Instance.was_on_press_back_to_menu = false;
		}
		if (Application.isEditor)
		{
			delete_general_button.SetActive(true);
		}
		apply_mute();
	}

	public void press_delete_general()
	{
		if (Application.isEditor)
		{
			PlayerData.Instance.DELETE_GEMS_ETC();
		}
	}

	private void CapPixelsB()
	{
		if (!Loader.Instance.screen_resized)
		{
			float value = (float)Screen.width / Screen.dpi * ((float)Screen.height / Screen.dpi);
			float num = Mathf.InverseLerp(10.125f, 30.72f, value);
			if (num < 0.2f)
			{
				QualitySettings.SetQualityLevel(0);
			}
			else if (num < 0.4f)
			{
				QualitySettings.SetQualityLevel(1);
			}
			else if (num < 0.6f)
			{
				QualitySettings.SetQualityLevel(2);
			}
			else if (num < 0.8f)
			{
				QualitySettings.SetQualityLevel(3);
			}
			else
			{
				QualitySettings.SetQualityLevel(4);
			}
			Loader.Instance.screen_resized = true;
		}
	}

	public void press_slot(int index)
	{
		NewAudioControl.Instance.play_generic_click();
		enter_game(index);
	}

	public void press_mute_nu()
	{
		Loader.Instance.is_muted = !Loader.Instance.is_muted;
		apply_mute();
	}

	private void apply_mute()
	{
		if (Loader.Instance.is_muted)
		{
			AudioListener.volume = 0f;
			muted_ico.sprite = spr_muted;
		}
		else
		{
			AudioListener.volume = 1f;
			muted_ico.sprite = spr_not_muted;
		}
	}

	private void try_convert_old_save(string convert_str, string path)
	{
		if (PlayerData.Instance.GetGlobalInt(convert_str) != 0)
		{
			return;
		}
		if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + path))
		{
			Dictionary<string, object> dictionary = PlayerData.Instance.LoadOrCreateDictionary(path, false);
			if ((int)dictionary["PLAYER_ALIVE"] == 0)
			{
				PlayerData.Instance.SetGlobalInt(convert_str, 99);
				return;
			}
			int num = -1;
			for (int i = 0; i < 3; i++)
			{
				if (PlayerData.Instance.GetSlotInt("PLAYER_ALIVE", PlayerData.grouping_t.general, i) == 0)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				return;
			}
			PlayerData.Instance.SetSlotInt("PLAYER_ALIVE", 1, PlayerData.grouping_t.general, num);
			int num2 = 0;
			if (dictionary.ContainsKey("n_baskets"))
			{
				num2 += (int)dictionary["n_baskets"];
			}
			if (dictionary.ContainsKey("n_shacks"))
			{
				num2 = (int)dictionary["n_shacks"];
			}
			PlayerData.Instance.SetSlotInt("unique_id_iterator", num2, PlayerData.grouping_t.general, num);
			ConvertInt("n_morphed_creatures", PlayerData.grouping_t.general, dictionary, num);
			new List<int>();
			if (dictionary.ContainsKey("n_morphed_creatures"))
			{
				for (int j = 0; j < (int)dictionary["n_morphed_creatures"]; j++)
				{
					ConvertInt("morph" + j, PlayerData.grouping_t.general, dictionary, num);
				}
			}
			ConvertString("creatureName", PlayerData.grouping_t.general, dictionary, num);
			ConvertInt("numRebreeds", PlayerData.grouping_t.general, dictionary, num);
			ConvertInt("playerLevel", PlayerData.grouping_t.general, dictionary, num);
			ConvertInt("skillPointsSpendable", PlayerData.grouping_t.general, dictionary, num);
			ConvertInt("currentEXP", PlayerData.grouping_t.general, dictionary, num);
			ConvertInt("temp_nextLevelExp", PlayerData.grouping_t.general, dictionary, num);
			for (int k = 0; k < 8; k++)
			{
				ConvertInt("stat" + k, PlayerData.grouping_t.general, dictionary, num);
			}
			PlayerData.Instance.SetGlobalInt(convert_str, 22);
		}
		else
		{
			PlayerData.Instance.SetGlobalInt(convert_str, 99);
		}
	}

	private void ConvertInt(string key, PlayerData.grouping_t new_grouping, Dictionary<string, object> old_prefs, int slot)
	{
		if (old_prefs.ContainsKey(key))
		{
			int value = (int)old_prefs[key];
			PlayerData.Instance.SetSlotInt(key, value, new_grouping, slot);
		}
	}

	private void ConvertString(string key, PlayerData.grouping_t new_grouping, Dictionary<string, object> old_prefs, int slot)
	{
		if (old_prefs.ContainsKey(key))
		{
			string value = (string)old_prefs[key];
			PlayerData.Instance.SetSlotString(key, value, new_grouping, slot);
		}
	}

	public void Press_Single_Player()
	{
		if (in_sumenu || PopupControl.Instance.popup_open)
		{
			return;
		}
		in_sumenu = true;
		NewAudioControl.Instance.play_generic_click();
		try
		{
			try_convert_old_save("converted_old_slot0", "slot0_");
			try_convert_old_save("converted_old_slot1", "slot1_");
			try_convert_old_save("converted_old_slot2", "slot2_");
		}
		catch (Exception)
		{
		}
		bool flag = false;
		for (int i = 0; i < 3; i++)
		{
			int slotInt = PlayerData.Instance.GetSlotInt("PLAYER_ALIVE", PlayerData.grouping_t.general, i);
			if (slotInt == 1 || slotInt == 2)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			enter_game(0);
			return;
		}
		PopupControl.Instance.black_background.SetActive(true);
		submenu.SetActive(true);
		singleplayer_elements.SetActive(true);
		submenu_header.text = "SINGLE PLAYER";
		for (int j = 0; j < 3; j++)
		{
			int slotInt2 = PlayerData.Instance.GetSlotInt("PLAYER_ALIVE", PlayerData.grouping_t.general, j);
			if (slotInt2 == 1 || slotInt2 == 2)
			{
				set_slot(j, true);
			}
		}
	}

	public void press_delete_save(int index)
	{
		PlayerData.Instance.DeleteSlot(index);
		set_slot(index, false);
	}

	public void set_slot(int index, bool active)
	{
		if (active)
		{
			GameObject gameObject = Loader.Instance.load_player_creature("menu", index);
			gameObject.transform.parent = sp_slots[index].transform.Find("creature-point").transform;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			sp_creatures[index] = gameObject;
			GameObject[] children_ = gameObject.GetComponent<creatureModel>().children_;
			foreach (GameObject gameObject2 in children_)
			{
				gameObject2.GetComponent<limb_scr>().createandplayAnimation(gameObject2.GetComponent<limb_scr>().frames_snapPositions[0], gameObject2.GetComponent<limb_scr>().frames_rotations[0], 0.7f, true, 18f);
			}
			NewGameControl.Instance.create_mutant_particle_on_creature(PlayerData.Instance.GetSlotInt("numRebreeds", PlayerData.grouping_t.general, index), gameObject);
			sp_slots[index].GetComponent<Image>().color = slot_active;
			sp_slots[index].transform.Find("slot num").gameObject.GetComponent<Text>().color = slot_header_active;
			sp_slots[index].transform.Find("text_newgame").gameObject.SetActive(false);
			sp_slots[index].transform.Find("trash").gameObject.SetActive(true);
			sp_slots[index].transform.Find("text_creaturename").gameObject.SetActive(true);
			sp_slots[index].transform.Find("text_creaturename").GetComponent<Text>().text = CorrectedString(PlayerData.Instance.GetSlotString("creatureName", PlayerData.grouping_t.general, index));
			sp_slots[index].transform.Find("text_creatureLvl").gameObject.SetActive(true);
			sp_slots[index].transform.Find("text_creatureLvl").GetComponent<Text>().text = "Level " + PlayerData.Instance.GetSlotInt("playerLevel", PlayerData.grouping_t.general, index);
		}
		else
		{
			if (sp_creatures[index] != null)
			{
				UnityEngine.Object.Destroy(sp_creatures[index]);
			}
			sp_slots[index].GetComponent<Image>().color = slot_inactive;
			sp_slots[index].transform.Find("slot num").gameObject.GetComponent<Text>().color = slot_header_inactive;
			sp_slots[index].transform.Find("text_newgame").gameObject.SetActive(true);
			sp_slots[index].transform.Find("text_creaturename").gameObject.SetActive(false);
			sp_slots[index].transform.Find("text_creatureLvl").gameObject.SetActive(false);
			sp_slots[index].transform.Find("trash").gameObject.SetActive(false);
		}
	}

	private string CorrectedString(string input)
	{
		bool flag = false;
		List<char> list = new List<char>();
		for (int i = 0; i < input.Length; i++)
		{
			if (i == 0)
			{
				list.Add(input[i].ToString().ToUpper()[0]);
				continue;
			}
			if (flag)
			{
				list.Add(input[i].ToString().ToUpper()[0]);
				flag = false;
			}
			else
			{
				list.Add(input[i].ToString().ToLower()[0]);
			}
			if (list[i] == ' ')
			{
				flag = true;
			}
		}
		return new string(list.ToArray());
	}

	public void Press_Multi_Player()
	{
		if (!in_sumenu && !PopupControl.Instance.popup_open)
		{
			in_sumenu = true;
			NewAudioControl.Instance.play_generic_click();
			PopupControl.Instance.black_background.SetActive(true);
			submenu.SetActive(true);
			multiplayer_elements.SetActive(true);
			submenu_header.text = "MULTI PLAYER";
		}
	}

	public void Press_Back()
	{
		NewAudioControl.Instance.play_generic_click();
		Cancel();
	}

	public void Cancel()
	{
		in_sumenu = false;
		PopupControl.Instance.black_background.SetActive(false);
		submenu.SetActive(false);
		singleplayer_elements.SetActive(false);
		multiplayer_elements.SetActive(false);
		GameObject[] array = sp_creatures;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
	}

	private void enter_game(int index)
	{
		PlayerData.Instance.SLOT = index;
		Cancel();
		PopupControl.Instance.ShowConnecting("Loading Game");
		StartCoroutine(async_load_game());
	}

	private IEnumerator async_load_game()
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");
		while (!asyncLoad.isDone)
		{
			yield return null;
		}
	}

	private void CreateNarwhal()
	{
		if (Loader.Instance.session_animal == "")
		{
			if (PlayerData.Instance.GetGlobalInt("first_session") == 0)
			{
				Loader.Instance.session_animal = "narwhal";
				PlayerData.Instance.SetGlobalInt("first_session", 1);
			}
			else
			{
				string text = menu_creatures[UnityEngine.Random.Range(0, menu_creatures.Length)];
				if (Loader.Instance.temp_static_names_list.Contains(text))
				{
					Loader.Instance.session_animal = text;
				}
				else
				{
					Loader.Instance.session_animal = "narwhal";
				}
			}
		}
		spinningNarwhal = Loader.Instance.GetSingleAnimal(Loader.Instance.session_animal);
		spinningNarwhal.GetComponent<Rigidbody>().useGravity = false;
		spinningNarwhal.GetComponent<Rigidbody>().isKinematic = false;
		spinningNarwhal.GetComponent<Rigidbody>().angularVelocity = UnityEngine.Random.insideUnitSphere * 0.3f;
		spinningNarwhal.GetComponent<Rigidbody>().angularDrag = 0f;
		spinningNarwhal.transform.position = Vector3.zero;
		spinningNarwhal.transform.localScale = Vector3.one * 2.4f;
		spinningNarwhal.transform.localRotation = Quaternion.Euler(5f, 160f, 0f);
		spinningNarwhal.name = "Narwhal";
		puke.transform.parent = spinningNarwhal.GetComponent<creatureModel>().head_obj.transform;
		puke.transform.localPosition = new Vector3(0f, (0f - spinningNarwhal.GetComponent<creatureModel>().head_obj.GetComponent<limb_scr>().visual.transform.localScale.z) * 0.8f, spinningNarwhal.GetComponent<creatureModel>().head_obj.GetComponent<limb_scr>().visual.transform.localScale.z + 0.07f);
		puke.transform.localRotation = Quaternion.identity;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			currentMouseX = Input.mousePosition.x;
			currentMouseY = Input.mousePosition.y;
		}
		if (Input.GetMouseButton(0))
		{
			float num = Input.mousePosition.x - currentMouseX;
			currentMouseX = Input.mousePosition.x;
			float num2 = Input.mousePosition.y - currentMouseY;
			currentMouseY = Input.mousePosition.y;
			spinningNarwhal.GetComponent<Rigidbody>().angularVelocity += Vector3.right * num2 * 0.2f;
			spinningNarwhal.GetComponent<Rigidbody>().angularVelocity += Vector3.down * num * 0.2f;
		}
		if (spinningNarwhal == null)
		{
			return;
		}
		switch (DIALOG_STATE)
		{
		case -1:
			talk_narwhal("", 2f, -1);
			break;
		case 0:
			if (test_spinning())
			{
				talk_narwhal("AGGGHH!!", 3f, 1);
			}
			break;
		case 1:
			talk_narwhal("WHYY!?", 4f, 2);
			break;
		case 2:
			talk_narwhal("i'm going to\nbe sick", 4.5f, 3);
			break;
		case 3:
			talk_narwhal("*BLEGH*", 10f, 4);
			break;
		case 4:
			talk_narwhal("such is life", 8f, 5);
			break;
		case 5:
			talk_narwhal("wheee!", 2.5f, -1);
			break;
		case 9:
			talk_narwhal("I hate spinning.", 6f, 10);
			break;
		case 10:
			talk_narwhal("where am I?", 6f, 11);
			break;
		case 11:
			talk_narwhal("what is this\nvoid?", 6f, 12);
			break;
		case 12:
			talk_narwhal("who are you?", 6f, 13);
			break;
		case 13:
			talk_narwhal("Echo..\nEcho..", 6f, -1);
			break;
		case 6:
		case 7:
		case 8:
			break;
		}
	}

	private void talk_narwhal(string text, float waitTimer, int nextStage)
	{
		if (dialog_changeable)
		{
			if (DIALOG_STATE == 3)
			{
				StartCoroutine(do_puke());
			}
			DIALOG_STATE = nextStage;
			narwhalTextAnm.Stop();
			narwhalTextAnm.Play();
			narwhalText.text = text;
			StartCoroutine(dialog_wait(waitTimer));
			dialog_changeable = false;
		}
	}

	public bool test_spinning()
	{
		if (spinningNarwhal != null)
		{
			return spinningNarwhal.GetComponent<Rigidbody>().angularVelocity.magnitude > 1.7f;
		}
		return false;
	}

	private IEnumerator dialog_wait(float waitTimer)
	{
		float counter = waitTimer;
		while (true)
		{
			yield return new WaitForSeconds(tick);
			counter -= tick;
			if (counter < 0f)
			{
				break;
			}
			if (spinning)
			{
				if (!test_spinning())
				{
					stopped_spinning();
					yield break;
				}
			}
			else if (test_spinning())
			{
				resume_spinning();
				yield break;
			}
		}
		dialog_changeable = true;
	}

	private void stopped_spinning()
	{
		dialog_changeable = true;
		spinning = false;
		talk_narwhal("thank you", 3f, 9);
	}

	private void resume_spinning()
	{
		dialog_changeable = true;
		spinning = true;
		talk_narwhal("NOOO!!!", 3f, 1);
	}

	private IEnumerator do_puke()
	{
		puke.SetActive(true);
		yield return new WaitForSeconds(3.5f);
		puke.SetActive(false);
	}
}
