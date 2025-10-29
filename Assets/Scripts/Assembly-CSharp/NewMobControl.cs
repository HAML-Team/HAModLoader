using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMobControl : MonoBehaviour
{
	[Serializable]
	public struct drop
	{
		public string name;

		public GameObject prefab;
	}

	public static NewMobControl Instance;

	[HideInInspector]
	public List<GameObject> active_combatants = new List<GameObject>();

	[HideInInspector]
	public List<Vector3> loaded_creatures = new List<Vector3>();

	[HideInInspector]
	public Dictionary<string, drop> drops_by_name = new Dictionary<string, drop>();

	public drop[] drops;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		drop[] array = drops;
		for (int i = 0; i < array.Length; i++)
		{
			drop value = array[i];
			drops_by_name.Add(value.name, value);
		}
		StartCoroutine(respawn_cycle());
	}

	public Dictionary<Vector3, respawn> curr_respawn_list()
	{
		switch (PlayerData.Instance.SLOT)
		{
		case 0:
			return Loader.Instance.to_respawn_slotA;
		case 1:
			return Loader.Instance.to_respawn_slotB;
		case 2:
			return Loader.Instance.to_respawn_slotC;
		default:
			return null;
		}
	}

	public void spawn_drop(string dropname, Vector3 pos, float scale = 1f)
	{
		GameObject obj = UnityEngine.Object.Instantiate(drops_by_name[dropname].prefab);
		obj.transform.position = new Vector3(pos.x, 0f, pos.z);
		obj.transform.parent = NewBiomeControl.Instance.GetChunkParent(pos);
		obj.transform.localScale = Vector3.one * scale;
		obj.GetComponent<NewCollectible>().DropStart();
	}

	private IEnumerator respawn_cycle()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			List<Vector3> list = new List<Vector3>();
			Dictionary<Vector3, respawn> dictionary = curr_respawn_list();
			foreach (KeyValuePair<Vector3, respawn> item in dictionary)
			{
				item.Value.time_remaining--;
				if (item.Value.time_remaining == 0)
				{
					list.Add(item.Key);
				}
			}
			foreach (Vector3 item2 in list)
			{
				dictionary.Remove(item2);
			}
		}
	}

	public void SnapOverhead(RectTransform R, Vector3 pos)
	{
		Vector2 anchorMax = (R.anchorMin = Camera.main.WorldToViewportPoint(pos));
		R.anchorMax = anchorMax;
	}

	public void InstantiateWildMob(string creatureA, string creatureB, int chunkX, int chunkZ, int x, int z, List<GameObject> packMates, float override_size = 1f, GameController.creatureStates auto_state = GameController.creatureStates.unknown, float wander_override = -1f, float levelmod = 1f, float speed_override = 1f, float lockon_override = 1f)
	{
		Vector3 vector = new Vector3(chunkX * 10, 0f, chunkZ * 10) + new Vector3(x, 0f, z) + new Vector3(0.5f, 1.5f, 0.5f);
		if (!loaded_creatures.Contains(vector) && !curr_respawn_list().ContainsKey(vector))
		{
			float num = GameController.Instance.depth_at(vector);
			float num2 = GameController.Instance.playerLevel;
			if (num2 < 15f)
			{
				num2 = 15f;
			}
			int num3 = (int)(num / 140f * num2 * levelmod);
			GameController.creatureStates sTATE = ((auto_state != GameController.creatureStates.unknown) ? auto_state : ((packMates.Count != 0) ? packMates[0].GetComponent<creatureScript>().STATE : DetermineCreatureState(num3, num)));
			float size;
			if (override_size == 1f)
			{
				int num4 = num3 - GameController.Instance.playerLevel;
				size = ((num3 > 3 && !(num < 15f)) ? ((num4 > 20) ? 3f : ((num4 > 10) ? 2f : ((num4 >= -10) ? 1f : 0.7f))) : 0.6f);
			}
			else
			{
				size = override_size;
			}
			string creature_type_str = ((!(creatureA == creatureB)) ? (creatureA + " + " + creatureB) : creatureA);
			GameObject gameObject = UnityEngine.Object.Instantiate(GameController.Instance.type_creature);
			gameObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
			gameObject.transform.position = vector;
			Color creature_type_col = string_to_color(creatureA + creatureB);
			List<string> list = new List<string>();
			list.Add(creatureA);
			list.Add(creatureB);
			gameObject.GetComponent<creatureScript>().Init(Loader.Instance.GetHybrid(list), false, sTATE, num3, creature_type_str, new int[9], creature_type_col, true);
			gameObject.GetComponent<creatureScript>().packMates_ = packMates;
			packMates.Add(gameObject);
			gameObject.GetComponent<NewCombatant>().SetOrigin();
			gameObject.GetComponent<NewCombatant>().ReplenishHealth();
			gameObject.GetComponent<creatureScript>().override_wander = wander_override;
			if (speed_override != 1f)
			{
				gameObject.GetComponent<creatureScript>().walk_speed = speed_override;
			}
			if (lockon_override != 1f)
			{
				gameObject.GetComponent<creatureScript>().AI_lockon_range = lockon_override;
			}
			NewBiomeControl.Instance.SetCreatureSize(size, gameObject);
			active_combatants.Add(gameObject);
			loaded_creatures.Add(vector);
		}
	}

	public Color string_to_color(string str)
	{
		int num = 97;
		int num2 = 122;
		int num3 = 0;
		for (int i = 0; i < str.Length; i++)
		{
			num3 += str[i] - num;
		}
		float h = (float)(num3 % num2) / (float)num2;
		float s = 0.59f;
		float v = 1f;
		return Color.HSVToRGB(h, s, v);
	}

	public GameController.creatureStates DetermineCreatureState(int critterLevel, float depth)
	{
		if (depth < 15f || critterLevel <= 3)
		{
			return GameController.creatureStates.fearful;
		}
		int num = critterLevel - GameController.Instance.playerLevel;
		if (num < -15)
		{
			if (UnityEngine.Random.value < 0.25f)
			{
				return GameController.creatureStates.fearful;
			}
			return GameController.creatureStates.neutral;
		}
		if (num > 10)
		{
			return GameController.creatureStates.aggressive;
		}
		if (UnityEngine.Random.value < 0.15f && critterLevel > 5)
		{
			return GameController.creatureStates.aggressive;
		}
		return GameController.creatureStates.neutral;
	}
}
