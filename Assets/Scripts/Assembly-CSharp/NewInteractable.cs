using UnityEngine;
using UnityEngine.UI;

public class NewInteractable : MonoBehaviour
{
	public enum interaction
	{
		none = 0,
		crafting_table = 1,
		anvil = 2,
		basket = 3,
		chest = 4,
		chair_normal = 5,
		chair_w_sound = 6,
		NPC = 7,
		gold_chest = 8,
		titanium_chest = 9,
		enter_shack = 10,
		enter_mansion = 11,
		exit_house = 12,
		cauldron = 13
	}

	[HideInInspector]
	public int unique_id;

	public float extra_interact_dist;

	[HideInInspector]
	public int temp_rot;

	public interaction interaction_type;

	[HideInInspector]
	public int icon_id;

	[HideInInspector]
	public GameObject overhead_icon;

	private void FixedUpdate()
	{
		if (overhead_icon != null)
		{
			NewMobControl.Instance.SnapOverhead((RectTransform)overhead_icon.transform, base.transform.position);
		}
	}

	public void assign_overhead_icon()
	{
		if (!(overhead_icon != null))
		{
			overhead_icon = Object.Instantiate(GameController.Instance.type_NPC_overhead);
			overhead_icon.transform.parent = NewMobControl.Instance.gameObject.transform;
			overhead_icon.transform.SetAsFirstSibling();
			overhead_icon.transform.localPosition = Vector3.zero;
			overhead_icon.transform.localScale = Vector3.one * 0.75f;
			overhead_icon.transform.localRotation = Quaternion.identity;
			RectTransform obj = (RectTransform)overhead_icon.transform;
			Vector2 anchorMin = (((RectTransform)overhead_icon.transform).anchorMax = Vector2.one * 10f);
			obj.anchorMin = anchorMin;
			GameController.Instance.possible_destroy.Add(overhead_icon);
			overhead_icon.transform.Find("StateDisplay").GetComponent<Image>().sprite = GameController.Instance.overhead_logos[icon_id];
		}
	}

	public void Start()
	{
		NewBiomeControl.Instance.active_interactibles.Add(base.gameObject);
	}

	public void OnDestroy()
	{
		NewBiomeControl.Instance.active_interactibles.Remove(base.gameObject);
		if (overhead_icon != null)
		{
			GameController.Instance.possible_destroy.Remove(overhead_icon);
			Object.Destroy(overhead_icon);
		}
	}
}
