using System.Collections.Generic;
using UnityEngine;

public class NewGameControl : MonoBehaviour
{
	public static NewGameControl Instance;

	public Color color_dmg_hit;

	public Color color_dmg_miss;

	public Color color_dmg_mine;

	public Color color_dmg_poison;

	public List<string> list_of_permanent_purchases = new List<string>();

	public GameObject player_particle_type;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	public void ApplyArmor(creatureModel model, GameObject prefab_type)
	{
		if (model.armor_body != null)
		{
			Object.Destroy(model.armor_body);
		}
		GameObject gameObject = Object.Instantiate(prefab_type);
		gameObject.transform.parent = model.body_obj.GetComponent<limb_scr>().visual.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		model.armor_body = gameObject;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			PopupControl.Instance.ShowMessage("test");
		}
	}

	private void ReScale(Transform T)
	{
		Transform parent = T.parent;
		T.parent = null;
		float num = float.MinValue;
		if (T.transform.localScale.x > num)
		{
			num = T.transform.localScale.x;
		}
		if (T.transform.localScale.y > num)
		{
			num = T.transform.localScale.y;
		}
		if (T.transform.localScale.z > num)
		{
			num = T.transform.localScale.z;
		}
		T.transform.localScale = new Vector3(num, num, num);
		T.transform.parent = parent;
	}

	public void ApplyHat(creatureModel model, GameObject prefab_type)
	{
		if (model.hat != null)
		{
			Object.Destroy(model.hat);
		}
		GameObject gameObject = Object.Instantiate(prefab_type);
		gameObject.transform.parent = model.head_obj.GetComponent<limb_scr>().visual.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
		gameObject.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
		gameObject.transform.parent = null;
		float num = 0f;
		if (gameObject.transform.localScale.x > num)
		{
			num = gameObject.transform.localScale.x;
		}
		if (gameObject.transform.localScale.y > num)
		{
			num = gameObject.transform.localScale.y;
		}
		if (gameObject.transform.localScale.z > num)
		{
			num = gameObject.transform.localScale.z;
		}
		if (num > 0.215f)
		{
			num = 0.215f;
		}
		gameObject.transform.localScale = new Vector3(num, num, num);
		gameObject.transform.parent = model.head_obj.GetComponent<limb_scr>().visual.transform;
		model.hat = gameObject;
	}

	public void create_mutant_particle(int n_rebreeds, creatureScript scr)
	{
		if (n_rebreeds != 0)
		{
			GameObject mutantParticle = create_mutant_particle_on_creature(n_rebreeds, scr.myCreatureModel.gameObject);
			scr.mutantParticle = mutantParticle;
		}
	}

	public GameObject create_mutant_particle_on_creature(int n_rebreeds, GameObject creature)
	{
		if (n_rebreeds == 0)
		{
			return null;
		}
		GameObject obj = Object.Instantiate(player_particle_type);
		obj.transform.parent = creature.transform;
		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = Vector3.zero;
		obj.GetComponent<ParticleSystem>().startColor = get_mutant_particle_color(n_rebreeds);
		return obj;
	}

	private Color get_mutant_particle_color(int N)
	{
		switch (N)
		{
		case 1:
			return new Color(0f, 1f, 0f);
		case 2:
			return new Color(0.2f, 0.2f, 1f);
		case 3:
			return new Color(0f, 1f, 1f);
		case 4:
			return new Color(0.7f, 0.2f, 1f);
		case 5:
			return new Color(1f, 0f, 0f);
		default:
			return new Color(1f, 1f, 0f);
		}
	}
}
