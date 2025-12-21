using UnityEngine;

public class NewCollectible : MonoBehaviour
{
	public enum pickup_type_t
	{
		destroy = 0,
		hide_object = 1
	}

	public string corresponding_inventory_object;

	public int respawn_time;

	public pickup_type_t on_pickup;

	public GameObject hide_obj;

	public float extra_interact_dist;

	[HideInInspector]
	public string flag_special;

	public void CustomStart()
	{
		if (NewMobControl.Instance.curr_respawn_list().ContainsKey(base.transform.position))
		{
			VisuallyHarvest();
		}
		else
		{
			NewBiomeControl.Instance.active_interactibles.Add(base.gameObject);
		}
	}

	public void DropStart()
	{
		NewBiomeControl.Instance.active_interactibles.Add(base.gameObject);
	}

	public void VisuallyHarvest()
	{
		if (on_pickup == pickup_type_t.destroy)
		{
			base.gameObject.SetActive(false);
		}
		else if (on_pickup == pickup_type_t.hide_object)
		{
			hide_obj.SetActive(false);
		}
	}

	public void OnDestroy()
	{
		if (NewBiomeControl.Instance.active_interactibles.Contains(base.gameObject))
		{
			NewBiomeControl.Instance.active_interactibles.Remove(base.gameObject);
		}
	}
}
