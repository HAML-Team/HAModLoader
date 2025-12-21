using UnityEngine;

public class anm_commands_youdied : MonoBehaviour
{
	public void AnmSoundDeath()
	{
		GameController.Instance.sound_death();
	}
}
