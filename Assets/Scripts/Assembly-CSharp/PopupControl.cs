using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopupControl : MonoBehaviour
{
	public enum context
	{
		none = 0,
		message = 1,
		rapidclicknotif = 2,
		reward_ad = 3,
		error_log = 4,
		sell = 5,
		teleport = 6,
		restart_game = 7,
		respawn = 8
	}

	public static PopupControl Instance;

	public GameObject black_background;

	public GameObject connecting_screen;

	public GameObject message_screen;

	public GameObject okay_button;

	public GameObject yesno_buttons;

	public GameObject reward_screen;

	public GameObject reward_collect_screen;

	public Animation loading_bubbles;

	public Text message_Text;

	public Text connecting_Text;

	public Text yes_text;

	public Text no_text;

	[HideInInspector]
	public bool popup_open;

	[HideInInspector]
	public string error_log = "";

	private bool on_levelup;

	public context curr_popup;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			Application.logMessageReceived += Log;
		}
	}

	private void Start()
	{
		if (this == Instance)
		{
			UnityEngine.Object.DontDestroyOnLoad(black_background);

#if UNITY_ANDROID
            // On Android, show the normal connect flow (MultiplayerControl will set connect_fail_already)
            if (MultiplayerControl.Instance != null && MultiplayerControl.Instance.connect_fail_already)
            {
                ShowMessage("Couldn't connect to Google Play services.");
            }
            else
            {
                ShowConnecting("Connecting to Google Play Services");
            }
#else
            // Not Android (Windows / Editor / other) — don't show stuck connecting screen.
            // Mark services as failed so other code knows not to wait.
            if (MultiplayerControl.Instance != null)
            {
                MultiplayerControl.Instance.connect_fail_already = true;
            }
            HideAll();
#endif
        }
    }

	public void Log(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Exception)
		{
			string text = "version" + Application.version + " ... " + logString + " ... " + stackTrace;
			if (!text.Contains("GLSL") && error_log == "")
			{
				Instance.HideAll();
				ShowYesNo("<color=#ffa100><size=30>Oops! There was a glitch.</size></color>\n\n<color=#d1d1d1><size=16>#" + text + "</size></color>", "OKAY", "Report Glitch", context.error_log);
				error_log = text;
			}
		}
	}

	public void ShowMessage(string message, context popup_context = context.message)
	{
		HideAll();
		if (SceneManager.GetActiveScene().name == "Menu" && NewMenuController.Instance.in_sumenu)
		{
			NewMenuController.Instance.Cancel();
		}
		popup_open = true;
		black_background.SetActive(true);
		message_screen.SetActive(true);
		okay_button.SetActive(true);
		message_Text.text = message;
		curr_popup = popup_context;
	}

	public void ShowYesNo(string message, string Yes, string No, context popup_context)
	{
		HideAll();
		if (SceneManager.GetActiveScene().name == "Menu" && NewMenuController.Instance.in_sumenu)
		{
			NewMenuController.Instance.Cancel();
		}
		popup_open = true;
		black_background.SetActive(true);
		message_screen.SetActive(true);
		message_Text.text = message;
		yesno_buttons.SetActive(true);
		yes_text.text = Yes;
		no_text.text = No;
		curr_popup = popup_context;
	}

	public void ShowConnecting(string connect_string)
	{
		HideAll();
		popup_open = true;
		black_background.SetActive(true);
		connecting_Text.text = connect_string;
		connecting_screen.SetActive(true);
		loading_bubbles.Play();
	}

	public void ShowRewardAskPopup(bool on_levelup = false)
	{
		this.on_levelup = on_levelup;
		if (!on_levelup)
		{
			Shop_positioner.Instance.HideGameplayGUI();
		}
		else
		{
			GameController.Instance.LOCK_LEVEL_SCREEN = true;
		}
		GameController.Instance.PAUSE_GAME();
		ShowYesNo("", "Yes", "No", context.reward_ad);
		reward_screen.SetActive(true);
	}

	public void ShowRewardCompletePopup()
	{
		if (on_levelup)
		{
			GameController.Instance.LOCK_LEVEL_SCREEN = true;
		}
		popup_open = true;
		black_background.SetActive(true);
		reward_collect_screen.SetActive(true);
		RewardAnm.Instance.PlayMysteryBoxAppear();
	}

	private void OnPressOkay()
	{
		if (GameController.Instance != null)
		{
			GameController.Instance.button_clicked = true;
		}
		switch (curr_popup)
		{
		case context.rapidclicknotif:
			GameController.Instance.pause = false;
			break;
		case context.reward_ad:
			if (WindowControl.Instance.curr_window == WindowControl.window_type_t.none && !on_levelup)
			{
				Shop_positioner.Instance.ShowGameplayGUI();
				GameController.Instance.pause = false;
			}
			else if (on_levelup)
			{
				GameController.Instance.LOCK_LEVEL_SCREEN = false;
			}
			GameController.Instance.reset_reward_ad_timer();
			RewardAnm.Instance.press_reward_okay();
			break;
		}
		NewAudioControl.Instance.play_generic_click();
		HideAll();
	}

	private void OnPressNo()
	{
		if (GameController.Instance != null)
		{
			GameController.Instance.button_clicked = true;
		}
		bool flag = true;
		switch (curr_popup)
		{
		case context.error_log:
			flag = false;
			try
			{
				Application.OpenURL("https://www.abstractsoftwares.com/report-bug?textBox1=" + error_log);
			}
			catch (Exception)
			{
			}
			break;
		case context.reward_ad:
			if (WindowControl.Instance.curr_window == WindowControl.window_type_t.none && !on_levelup)
			{
				Shop_positioner.Instance.ShowGameplayGUI();
				GameController.Instance.pause = false;
			}
			if (on_levelup)
			{
				Loader.Instance.last_ad_skipped = 2;
				GameController.Instance.LOCK_LEVEL_SCREEN = false;
			}
			GameController.Instance.reset_reward_ad_timer();
			reward_screen.SetActive(false);
			break;
		}
		if (flag)
		{
			NewAudioControl.Instance.play_generic_click();
			HideAll();
		}
	}

	private void OnPressYes()
	{
		if (GameController.Instance != null)
		{
			GameController.Instance.button_clicked = true;
		}
		bool flag = true;
		switch (curr_popup)
		{
		case context.reward_ad:
			if (Application.systemLanguage != SystemLanguage.Chinese && Application.systemLanguage != SystemLanguage.ChineseSimplified && Application.systemLanguage != SystemLanguage.ChineseTraditional)
			{
				flag = false;
				Loader.Instance.ShowRewardAd();
			}
			break;
		case context.sell:
			inventory_ctr.Instance.Complete_Sell();
			break;
		case context.teleport:
			if (GameController.Instance.player != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(GameController.Instance.prefab_teleport);
				gameObject.transform.position = GameController.Instance.player.transform.position;
				GameController.Instance.player.GetComponent<Rigidbody>().isKinematic = true;
				GameController.Instance.player.transform.parent = gameObject.transform.Find("player goes here");
				Shop_positioner.Instance.HideGameplayGUI();
				GameController.Instance.PAUSE_GAME();
				GameController.Instance.StartCoroutine(GameController.Instance.delayed_teleport());
			}
			break;
		case context.restart_game:
			GameController.Instance.Restart_Accepted();
			break;
		case context.respawn:
			GameController.Instance.Respawn_Accepted();
			break;
		}
		if (flag)
		{
			NewAudioControl.Instance.play_generic_click();
			HideAll();
		}
	}

	public void HideAll()
	{
		popup_open = false;
		connecting_screen.SetActive(false);
		black_background.SetActive(false);
		yesno_buttons.SetActive(false);
		message_screen.SetActive(false);
		okay_button.SetActive(false);
		reward_collect_screen.SetActive(false);
		reward_screen.SetActive(false);
	}

	public void PressOkay()
	{
		OnPressOkay();
	}

	public void PressNo()
	{
		OnPressNo();
	}

	public void PressYes()
	{
		OnPressYes();
	}
}
