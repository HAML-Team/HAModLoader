using UnityEngine;

public class HouseTransitionEvents : MonoBehaviour
{
	public void Anm_faded_out_complete()
	{
		if (GameController.Instance.on_teleport)
		{
			GameController.Instance.DoTeleport();
		}
		else if (GameController.Instance.on_exit_house)
		{
			GameController.Instance.do_exit_shack();
		}
		else
		{
			GameController.Instance.load_shack();
		}
	}

	public void Anm_complete()
	{
		GetComponent<Animation>().Stop();
		base.gameObject.SetActive(false);
	}
}
