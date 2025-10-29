using UnityEngine;

public class anm_commands_levelup : MonoBehaviour
{
	public void animation_set_levelup_text()
	{
		GameController.Instance.animation_set_levelup_text();
	}

	public void animation_complete()
	{
		GameController.Instance.PAUSE_GAME();
		GameController.Instance.animation_sound_levelScreenAppear();
		GameController.Instance.animation_unlock_levelup_text();
		WindowControl.Instance.GetComponent<Animation>().Play("levelup appear");
	}
}
