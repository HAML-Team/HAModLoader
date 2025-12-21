using UnityEngine;

public class WindowControl : MonoBehaviour
{
	public enum window_type_t
	{
		none = 0,
		dialogue = 1,
		perkmanage = 2,
		mutant_market = 3,
		buy_gems_revive = 4,
		buy_gems_store = 5
	}

	public static WindowControl Instance;

	public GameObject close_button;

	[HideInInspector]
	public window_type_t curr_window;

	private void Awake()
	{
		Instance = this;
	}

	public void ShowClose(window_type_t window_type)
	{
		curr_window = window_type;
		close_button.SetActive(true);
	}

	public void PressClose()
	{
		if (!PopupControl.Instance.popup_open)
		{
			GameController.Instance.button_clicked = true;
			window_type_t window_type_t = window_type_t.none;
			switch (curr_window)
			{
			case window_type_t.dialogue:
				inventory_ctr.Instance.close_generic_window();
				GameController.Instance.focus_npc = null;
				GameController.Instance.player.GetComponent<creatureScript>().myCreatureModel.gameObject.SetActive(true);
				DialogueControl.Instance.ExitDialogue();
				break;
			case window_type_t.perkmanage:
				perk_controller.Instance.press_close();
				break;
			case window_type_t.mutant_market:
				Shop_positioner.Instance.close_market();
				break;
			case window_type_t.buy_gems_revive:
				Shop_positioner.Instance.close_gems_window();
				break;
			case window_type_t.buy_gems_store:
				window_type_t = window_type_t.mutant_market;
				Shop_positioner.Instance.close_gems_window();
				break;
			}
			if (window_type_t == window_type_t.none)
			{
				close_button.SetActive(false);
			}
			curr_window = window_type_t;
		}
	}

	public void temporary_attempt_ad()
	{
		GameController.Instance.ATTEMPT_AD();
	}
}
