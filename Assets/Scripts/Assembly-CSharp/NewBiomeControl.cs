using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NewBiomeControl : MonoBehaviour
{
	[Serializable]
	public struct biome_obj
	{
		public string name;

		public GameObject prefab;

		public float commonness;

		public int width;

		public bool dont_rotate;

		[HideInInspector]
		public int DEBUG_OBJ_INDEX;
	}

	[Serializable]
	public struct biome
	{
		public string biome_name;

		public GameObject floor_prefab;

		public Texture2D[] floor_textures;

		public Texture2D grass_texture;

		public int green_min;

		public int green_max;

		public biome_obj[] biome_scenic;

		public string[] possible_mobs;

		public edge_piece[] edge_pieces;
	}

	[Serializable]
	public struct edge_piece
	{
		public string name;

		public Texture2D tex;

		public bool on_top;
	}

	public static NewBiomeControl Instance;

	[HideInInspector]
	public List<GameObject> active_interactibles = new List<GameObject>();

	public Texture2D biome_map;

	public biome[] biomes;

	public biome_obj green_fruit_define;

	public GameObject biome_edge_piece;

	public GameObject creature_type_NPC;

	public GameObject shack_interior;

	public GameObject mansion_interior;

	private int cascade_load_timer;

	[HideInInspector]
	public Dictionary<string, List<place_special>> chunks_with_specials = new Dictionary<string, List<place_special>>();

	private int generic_biome_map_w;

	private int[,] generic_biome_map_ids;

	public static float chunk_width = 10f;

	private int old_chunk_X;

	private int old_chunk_z;

	[HideInInspector]
	public int player_chunk_X;

	[HideInInspector]
	public int player_chunk_Z;

	[HideInInspector]
	public string player_zone = "overworld";

	[HideInInspector]
	public string player_zone_type = "";

	[HideInInspector]
	public int zone_origin_chunkX;

	[HideInInspector]
	public int zone_origin_chunkZ;

	[HideInInspector]
	public int zone_origin_innerX;

	[HideInInspector]
	public int zone_origin_innerZ;

	[HideInInspector]
	public int zone_rotation;

	[HideInInspector]
	public int shack_entrance_chunkX;

	[HideInInspector]
	public int shack_entrance_chunkZ;

	[HideInInspector]
	public int shack_entrance_innerX;

	[HideInInspector]
	public int shack_entrance_innerZ;

	[HideInInspector]
	public Dictionary<string, Chunk_f> chunks_loaded = new Dictionary<string, Chunk_f>();

	private List<chunk_to_load> chunks_to_load = new List<chunk_to_load>();

	[HideInInspector]
	public float zone_view_scale = 1f;

	private Dictionary<string, int[,]> biome_maps = new Dictionary<string, int[,]>();

	private int curr_chunk_version = 5;

	[HideInInspector]
	public List<string> quest_fills = new List<string>();

	public GameObject debug_cube;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		bool isEditor = Application.isEditor;
		LoadBiomeFile();
		for (int i = 0; i < biomes.Length; i++)
		{
			for (int j = 0; j < biomes[i].biome_scenic.Length; j++)
			{
				biomes[i].biome_scenic[j].DEBUG_OBJ_INDEX = j;
			}
		}
		AddGreenFruit(-5, 4, "overworld");
		AddGreenFruit(-6, 9, "overworld");
		AddGreenFruit(-10, 9, "overworld");
		AddBuildable(-11, 6, "overworld", "Cobblestone Path", 0);
		AddBuildable(-12, 6, "overworld", "Cobblestone Path", 2);
		AddBuildable(-12, 5, "overworld", "Cobblestone Path", 1);
		AddBuildable(-12, 3, "overworld", "Cobblestone Path", 1);
		AddBuildable(-12, 1, "overworld", "Cobblestone Path", 1);
		AddBuildable(-12, -1, "overworld", "Cobblestone Path", 2);
		AddBuildable(-14, -1, "overworld", "Cobblestone Path", 1);
		AddNPC(-13, 6, "overworld", 1, "carrot", "gorilla", "The Vitamin Ape", 0, "", 1);
		AddNPC(-15, -1, "overworld", 2, "pineapple", "pig", "Pineapple Pig", 0, "Metal Sword", 0);
		AddBuildable(-13, 7, "overworld", "Torch", 0);
		AddBuildable(-15, -2, "overworld", "Torch", 0);
		AddBuildable(-14, -3, "overworld", "Anvil", 1);
		AddBuildable(-11, 0, "overworld", "Cobblestone Path", 0);
		AddBuildable(-9, 0, "overworld", "Cobblestone Path", 1);
		AddBuildable(-9, -2, "overworld", "Cobblestone Path", 1);
		AddBuildable(-9, -4, "overworld", "Cobblestone Path", 1);
		AddBuildable(-10, -5, "overworld", "Cobblestone Path", 1);
		AddBuildable(-11, -7, "overworld", "Cobblestone Path", 1);
		AddBuildable(-11, -9, "overworld", "Cobblestone Path", 1);
		AddBuildable(-12, -12, "overworld", "Cobblestone Path", 1);
		AddBuildable(-12, -10, "overworld", "Cobblestone Path", 1);
		AddBuildable(-12, -14, "overworld", "Cobblestone Path", 2);
		AddBuildable(-14, -14, "overworld", "Cobblestone Path", 2);
		AddScenic(-14, -13, "overworld", 0, 0);
		AddScenic(-16, -12, "overworld", 0, 3);
		AddNPC(-15, -13, "overworld", 3, "crab", "cupcake", "Cupcake Crab", 1, "", 0);
		int num = -17;
		int num2 = 2;
		AddHouse(num, num2, "overworld", "Red Shack", 0, 9999);
		FillEmpty(num + 1, num2, "overworld");
		FillEmpty(num - 1, num2, "overworld");
		FillEmpty(num, num2 + 1, "overworld");
		FillEmpty(num, num2 - 1, "overworld");
		FillEmpty(num + 1, num2 + 1, "overworld");
		FillEmpty(num + 1, num2 - 1, "overworld");
		FillEmpty(num - 1, num2 + 1, "overworld");
		FillEmpty(num - 1, num2 - 1, "overworld");
		AddNPC(-22, 5, "shack9999", 4, "octopus", "kitten", "Octo Kitty", 0, "", 0);
		AddBuildable(-21, 3, "shack9999", "Cauldron", 0);
		FillEmpty(-15, 2, "overworld");
		FillEmpty(-14, 2, "overworld");
		FillEmpty(-13, 2, "overworld");
		for (int k = -2; k < 3; k++)
		{
			for (int l = -2; l < 3; l++)
			{
				FillEmpty(-8 + k, 6 + l, "overworld");
			}
		}
		AddScenic(-15, 6, "overworld", 0, 0);
		AddScenic(-10, 12, "overworld", 0, 12, 999);
		StartCoroutine(TRACK_PLAYER_CHUNK());
		StartCoroutine(SAVE_PLAYER_LOCATION());
	}

	public GameObject set_zone_models()
	{
		bool flag = false;
		Vector3 vector = new Vector3(-1.8f, 0f, 1.6f);
		Vector3 position = new Vector3(zone_origin_chunkX * 10 + zone_origin_innerX, 0f, zone_origin_chunkZ * 10 + zone_origin_innerZ) + vector + new Vector3(0.5f, 0f, 0.5f);
		GameObject gameObject = null;
		string text = player_zone_type;
		if (!(text == "shack"))
		{
			if (text == "mansion")
			{
				flag = true;
				gameObject = mansion_interior;
				GameController.Instance.curr_shack_type = GameController.shack_type.mansion;
			}
		}
		else
		{
			flag = true;
			gameObject = shack_interior;
			GameController.Instance.curr_shack_type = GameController.shack_type.shack;
		}
		if (flag)
		{
			gameObject.SetActive(true);
			gameObject.transform.position = position;
			GameController.Instance.circ_h = 0.25f;
			Shop_positioner.Instance.DISABLE_OBJECTS(GameController.Instance.player.transform.position, 999f, true);
			GameController.Instance.breeder_floor_plane.SetActive(true);
			GameController.Instance.breeder_floor_plane.transform.position = new Vector3(GameController.Instance.player.transform.position.x, 0f, GameController.Instance.player.transform.position.z);
			Texture2D texture2D = new Texture2D(2, 2);
			Color[] array = new Color[4];
			for (int i = 0; i < 4; i++)
			{
				array[i] = new Color(0.09f, 0.09f, 0.09f);
			}
			texture2D.SetPixels(0, 0, 2, 2, array);
			texture2D.Apply();
			GameController.Instance.breeder_floor_plane.GetComponent<Renderer>().material.mainTexture = texture2D;
			int num = zone_rotation;
			GameObject gameObject2 = null;
			switch (num)
			{
			case 0:
				gameObject2 = GameController.Instance.find_in_children("rot 0", gameObject.transform);
				break;
			case 1:
				gameObject2 = GameController.Instance.find_in_children("rot 1", gameObject.transform);
				break;
			case 2:
				gameObject2 = GameController.Instance.find_in_children("rot 2", gameObject.transform);
				break;
			case 3:
				gameObject2 = GameController.Instance.find_in_children("rot 3", gameObject.transform);
				break;
			}
			GameObject obj = GameController.Instance.find_in_children("exit door", gameObject.transform);
			obj.transform.position = gameObject2.transform.position;
			obj.transform.rotation = gameObject2.transform.rotation;
		}
		return gameObject;
	}

	private void FixedUpdate()
	{
		if (cascade_load_timer <= 0)
		{
			if (chunks_to_load.Count > 0)
			{
				chunk_to_load chunk_to_load2 = chunks_to_load[0];
				CreateAndAddChunk(player_zone, chunk_to_load2.X, chunk_to_load2.Z);
				chunks_to_load.RemoveAt(0);
			}
			cascade_load_timer = 8;
		}
		cascade_load_timer--;
	}

	private void FillEmpty(int pos_x, int pos_z, string zone)
	{
		NewSpecial(pos_x, pos_z, zone).type = 5;
	}

	private void AddNPC(int pos_x, int pos_z, string zone, int NPC_id, string creatureA, string creatureB, string creatureName, int rotation, string weapon, int icon_id)
	{
		place_special obj = NewSpecial(pos_x, pos_z, zone);
		obj.type = 6;
		obj.NPC_id = NPC_id;
		obj.icon_id = icon_id;
		obj.NPC_creatureA = creatureA;
		obj.NPC_creatureB = creatureB;
		obj.NPC_weapon = weapon;
		obj.rotation = rotation;
		DialogueControl.Instance.NPC_names.Add(NPC_id, creatureName);
	}

	private void AddBuildable(int pos_x, int pos_z, string zone, string built, int rotation)
	{
		place_special obj = NewSpecial(pos_x, pos_z, zone);
		obj.type = 7;
		obj.built = built;
		obj.rotation = rotation;
	}

	private void AddHouse(int pos_x, int pos_z, string zone, string house_item_str, int rotation, int shack_id)
	{
		place_special obj = NewSpecial(pos_x, pos_z, zone);
		obj.type = 7;
		obj.built = house_item_str;
		obj.rotation = rotation;
		obj.chest_shack_id = shack_id;
	}

	private void AddScenic(int pos_x, int pos_z, string zone, int biome_id, int obj_id, int chest_shack_id = -1)
	{
		place_special place_special2 = NewSpecial(pos_x, pos_z, zone);
		place_special2.type = 8;
		place_special2.biome_id = biome_id;
		place_special2.obj_id = obj_id;
		if (chest_shack_id != -1)
		{
			place_special2.chest_shack_id = chest_shack_id;
			inventory_ctr.Instance.NPC_chests.Add(chest_shack_id);
		}
	}

	private void AddGreenFruit(int pos_x, int pos_z, string zone)
	{
		NewSpecial(pos_x, pos_z, zone).type = 9;
	}

	private place_special NewSpecial(int pos_x, int pos_z, string zone)
	{
		Vector3 v = new Vector3((float)pos_x + 0.5f, 0f, (float)pos_z + 0.5f);
		Vector3 chunkCoords = GetChunkCoords(v);
		Vector3 inner = GetInner(v);
		place_special place_special2 = new place_special();
		place_special2.innerX = (int)inner.x;
		place_special2.innerZ = (int)inner.z;
		string chunkString_ = GetChunkString_(zone, (int)chunkCoords.x, (int)chunkCoords.z);
		if (!chunks_with_specials.ContainsKey(chunkString_))
		{
			chunks_with_specials.Add(chunkString_, new List<place_special>());
		}
		chunks_with_specials[chunkString_].Add(place_special2);
		return place_special2;
	}

	private void LoadBiomeFile()
	{
		Packet packet = new Packet((Resources.Load("biomemap") as TextAsset).bytes);
		generic_biome_map_w = packet.getShort();
		generic_biome_map_ids = new int[generic_biome_map_w, generic_biome_map_w];
		for (int i = 0; i < generic_biome_map_w; i++)
		{
			for (int j = 0; j < generic_biome_map_w; j++)
			{
				generic_biome_map_ids[i, j] = packet.getShort();
			}
		}
	}

	public void update_chunk_display()
	{
		float num = Round(GameController.Instance.player.transform.position.x / chunk_width);
		float num2 = Round(GameController.Instance.player.transform.position.z / chunk_width);
		GameController.Instance.coordinateDisplay2.text = num + ", " + num2;
	}

	private IEnumerator TRACK_PLAYER_CHUNK()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.5f);
			if (GameController.Instance.player != null)
			{
				CalcPlayerChunk();
				if (old_chunk_X != player_chunk_X || old_chunk_z != player_chunk_Z)
				{
					update_chunk_display();
					TerrainChanged(false);
				}
				if (Application.isEditor && GameController.Instance.player != null)
				{
					GameController.Instance.coordinateDisplay2.text = Round(GameController.Instance.player.transform.position.x) + ", " + Round(GameController.Instance.player.transform.position.z);
				}
			}
		}
	}

	private IEnumerator SAVE_PLAYER_LOCATION()
	{
		Vector3 prev_location = Vector3.zero;
		while (true)
		{
			yield return new WaitForSeconds(3f);
			if (GameController.Instance.player != null && Vector3.Distance(GameController.Instance.player.transform.position, prev_location) > 2.5f)
			{
				Vector3 chunkCoords = GetChunkCoords(GameController.Instance.player.transform.position);
				int value = (int)chunkCoords.x;
				int value2 = (int)chunkCoords.z;
				Vector3 inner = GetInner(GameController.Instance.player.transform.position);
				int value3 = (int)inner.x;
				int value4 = (int)inner.z;
				PlayerData.Instance.SetSlotString("player_zone", player_zone, PlayerData.grouping_t.playerpos);
				PlayerData.Instance.SetSlotString("player_zone_type", player_zone_type, PlayerData.grouping_t.playerpos);
				PlayerData.Instance.SetSlotInt("zone_origin_chunk_x", zone_origin_chunkX, PlayerData.grouping_t.playerpos);
				PlayerData.Instance.SetSlotInt("zone_origin_chunk_z", zone_origin_chunkZ, PlayerData.grouping_t.playerpos);
				PlayerData.Instance.SetSlotInt("zone_origin_inner_x", zone_origin_innerX, PlayerData.grouping_t.playerpos);
				PlayerData.Instance.SetSlotInt("zone_origin_inner_z", zone_origin_innerZ, PlayerData.grouping_t.playerpos);
				PlayerData.Instance.SetSlotInt("zone_rotation", zone_rotation, PlayerData.grouping_t.playerpos);
				PlayerData.Instance.SetSlotInt("shack_entrance_chunk_x", shack_entrance_chunkX, PlayerData.grouping_t.playerpos);
				PlayerData.Instance.SetSlotInt("shack_entrance_chunk_z", shack_entrance_chunkZ, PlayerData.grouping_t.playerpos);
				PlayerData.Instance.SetSlotInt("shack_entrance_inner_x", shack_entrance_innerX, PlayerData.grouping_t.playerpos);
				PlayerData.Instance.SetSlotInt("shack_entrance_inner_z", shack_entrance_innerZ, PlayerData.grouping_t.playerpos);
				PlayerData.Instance.SetSlotInt("player_chunk_x", value, PlayerData.grouping_t.playerpos);
				PlayerData.Instance.SetSlotInt("player_chunk_z", value2, PlayerData.grouping_t.playerpos);
				PlayerData.Instance.SetSlotInt("player_inner_x", value3, PlayerData.grouping_t.playerpos);
				PlayerData.Instance.SetSlotInt("player_inner_z", value4, PlayerData.grouping_t.playerpos);
				prev_location = GameController.Instance.player.transform.position;
			}
		}
	}

	public Vector3 GetChunkCoords(Vector3 V)
	{
		int num = Round(V.x / chunk_width);
		int num2 = Round(V.z / chunk_width);
		return new Vector3(num, 0f, num2);
	}

	public Vector3 GetInner(Vector3 V)
	{
		GetChunkCoords(V);
		int num = mod(Round(V.x), (int)chunk_width);
		int num2 = mod(Round(V.z), (int)chunk_width);
		return new Vector3(num, 0f, num2);
	}

	public string GetChunkString_(Vector3 V)
	{
		Vector3 chunkCoords = Instance.GetChunkCoords(V);
		return GetChunkString_(player_zone, (int)chunkCoords.x, (int)chunkCoords.z);
	}

	public string GetChunkString_(string zone, int X, int Z)
	{
		return "chunk(" + zone + "," + X + ", " + Z + ") ";
	}

	public Transform GetChunkParent(Vector3 V)
	{
		string chunkString_ = GetChunkString_(V);
		return chunks_loaded[chunkString_].parent_obj.transform;
	}

	public void CalcPlayerChunk()
	{
		player_chunk_X = Round((GameController.Instance.player.transform.position.x + -2.8f) / chunk_width);
		player_chunk_Z = Round((GameController.Instance.player.transform.position.z + 2.8f) / chunk_width);
	}

	public static int Round(float f)
	{
		if (f >= 0f)
		{
			return (int)f;
		}
		return (int)f - 1;
	}

	public void TerrainChanged(bool calc_chunk)
	{
		if (calc_chunk)
		{
			CalcPlayerChunk();
		}
		chunks_to_load.Clear();
		List<chunk_to_load> obj = new List<chunk_to_load>
		{
			new chunk_to_load(player_zone, player_chunk_X, player_chunk_Z),
			new chunk_to_load(player_zone, player_chunk_X + 1, player_chunk_Z),
			new chunk_to_load(player_zone, player_chunk_X - 1, player_chunk_Z),
			new chunk_to_load(player_zone, player_chunk_X, player_chunk_Z + 1),
			new chunk_to_load(player_zone, player_chunk_X, player_chunk_Z - 1),
			new chunk_to_load(player_zone, player_chunk_X + 1, player_chunk_Z + 1),
			new chunk_to_load(player_zone, player_chunk_X + 1, player_chunk_Z - 1),
			new chunk_to_load(player_zone, player_chunk_X - 1, player_chunk_Z + 1),
			new chunk_to_load(player_zone, player_chunk_X - 1, player_chunk_Z - 1)
		};
		Dictionary<string, Chunk_f> dictionary = new Dictionary<string, Chunk_f>();
		foreach (chunk_to_load item in obj)
		{
			string chunkString_ = GetChunkString_(item.zone, item.X, item.Z);
			if (chunks_loaded.ContainsKey(chunkString_))
			{
				dictionary.Add(chunkString_, chunks_loaded[chunkString_]);
				chunks_loaded.Remove(chunkString_);
			}
			else
			{
				chunks_to_load.Add(item);
			}
		}
		foreach (KeyValuePair<string, Chunk_f> item2 in chunks_loaded)
		{
			item2.Value.Delete();
			PlayerData.Instance.DeLoadGrouping(item2.Key);
		}
		chunks_loaded = dictionary;
		old_chunk_X = player_chunk_X;
		old_chunk_z = player_chunk_Z;
		if (player_zone == "overworld")
		{
			zone_view_scale = 1f;
		}
		else
		{
			zone_view_scale = 0.71f;
		}
	}

	public void RebuildChunkAt(Vector3 V)
	{
		Vector3 chunkCoords = GetChunkCoords(V);
		string chunkString_ = GetChunkString_(V);
		if (chunks_loaded.ContainsKey(chunkString_))
		{
			chunks_loaded[chunkString_].Delete();
			chunks_loaded.Remove(chunkString_);
			CreateAndAddChunk(player_zone, (int)chunkCoords.x, (int)chunkCoords.z);
		}
	}

	private void LoadBiomeMap(string biome_map_str)
	{
		int[,] array = new int[generic_biome_map_w, generic_biome_map_w];
		for (int i = 0; i < generic_biome_map_w; i++)
		{
			for (int j = 0; j < generic_biome_map_w; j++)
			{
				array[i, j] = PlayerData.Instance.GetSlotInt("biome-map(" + biome_map_str + "),(" + i + "," + j + ")", PlayerData.grouping_t.the_biome_map);
			}
		}
		biome_maps.Add(biome_map_str, array);
	}

	private void GenerateNewBiomeMap(string biome_map_str)
	{
		PlayerData.Instance.SetSlotInt("biome-map(" + biome_map_str + ") exists", 1, PlayerData.grouping_t.the_biome_map);
		int[,] array = new int[generic_biome_map_w, generic_biome_map_w];
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		for (int i = 0; i < generic_biome_map_w; i++)
		{
			for (int j = 0; j < generic_biome_map_w; j++)
			{
				int num = generic_biome_map_ids[i, j];
				if (!dictionary.ContainsKey(num))
				{
					if (num == 0)
					{
						dictionary.Add(num, 0);
					}
					else
					{
						dictionary.Add(num, UnityEngine.Random.Range(0, biomes.Length));
					}
				}
				array[i, j] = dictionary[num];
				PlayerData.Instance.SetSlotInt("biome-map(" + biome_map_str + "),(" + i + "," + j + ")", dictionary[num], PlayerData.grouping_t.the_biome_map);
			}
		}
		biome_maps.Add(biome_map_str, array);
	}

	private int get_biome_id_at(string zone, int chunkX, int chunkZ)
	{
		string chunkString_ = GetChunkString_(zone, chunkX, chunkZ);
		int num;
		if (PlayerData.Instance.GetSlotInt("exists", chunkString_) == 1)
		{
			num = PlayerData.Instance.GetSlotInt("biome", chunkString_);
		}
		else
		{
			int num2 = Round((float)chunkX / (float)generic_biome_map_w);
			int num3 = Round((float)chunkZ / (float)generic_biome_map_w);
			int num4 = mod(chunkX, generic_biome_map_w);
			int num5 = mod(chunkZ, generic_biome_map_w);
			string text = num2 + "," + num3;
			if (!biome_maps.ContainsKey(text))
			{
				if (PlayerData.Instance.GetSlotInt("biome-map(" + text + ") exists", PlayerData.grouping_t.the_biome_map) == 1)
				{
					LoadBiomeMap(text);
				}
				else
				{
					GenerateNewBiomeMap(text);
				}
			}
			num = biome_maps[text][num4, num5];
			PlayerData.Instance.SetSlotInt("biome", num, chunkString_);
		}
		return num;
	}

	private int mod(int x, int m)
	{
		int num = x % m;
		if (num >= 0)
		{
			return num;
		}
		return num + m;
	}

	private bool chunk_contains_buildable(string chunkStr)
	{
		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				if (PlayerData.Instance.GetSlotInt("fill(" + i + "," + j + ")", chunkStr) == 4)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void CreateAndAddChunk(string zone, int X, int Z)
	{
		string chunkString_ = GetChunkString_(zone, X, Z);
		if (chunks_loaded.ContainsKey(chunkString_))
		{
			return;
		}
		bool flag;
		if (PlayerData.Instance.GetSlotInt("exists", chunkString_) == 1)
		{
			flag = true;
		}
		else
		{
			flag = false;
			PlayerData.Instance.SetSlotInt("exists", 1, chunkString_);
			PlayerData.Instance.SetSlotInt("version", curr_chunk_version, chunkString_);
		}
		bool flag2 = false;
		int num = 0;
		if (zone != "overworld")
		{
			flag2 = true;
		}
		else
		{
			num = get_biome_id_at(zone, X, Z);
		}
		Chunk_f chunk_f = new Chunk_f(X, Z, zone);
		GameObject gameObject = (flag2 ? new GameObject("floor") : UnityEngine.Object.Instantiate(biomes[num].floor_prefab));
		gameObject.transform.position = new Vector3((float)X * 10f, 0f, (float)Z * 10f);
		if (!flag2)
		{
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				if (gameObject.transform.GetChild(i).name == "grass")
				{
					gameObject.transform.GetChild(i).GetComponent<MeshRenderer>().material.mainTexture = biomes[num].grass_texture;
				}
			}
		}
		chunk_f.parent_obj = gameObject;
		chunks_loaded.Add(chunkString_, chunk_f);
		if (!flag2)
		{
			int num2;
			int num3;
			if (flag)
			{
				num2 = PlayerData.Instance.GetSlotInt("floor-rot", chunkString_);
				num3 = PlayerData.Instance.GetSlotInt("floor-tex-index", chunkString_);
			}
			else
			{
				num2 = UnityEngine.Random.Range(0, 4);
				PlayerData.Instance.SetSlotInt("floor-rot", num2, chunkString_);
				num3 = UnityEngine.Random.Range(0, biomes[num].floor_textures.Length);
				PlayerData.Instance.SetSlotInt("floor-tex-index", num3, chunkString_);
			}
			Rotate(gameObject, new Vector3(5f, 0f, 5f), 90f * (float)num2);
			gameObject.transform.Find("floor-plane").GetComponent<MeshRenderer>().material.mainTexture = biomes[num].floor_textures[num3];
		}
		if (!flag2)
		{
			int num4 = get_biome_id_at(zone, X + 1, Z);
			if (num4 != num)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(biome_edge_piece);
				gameObject2.name = "edge1";
				gameObject2.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
				gameObject2.transform.position = new Vector3((float)X * 10f, 0f, (float)Z * 10f) + new Vector3(10f, 0.04f, 10f);
				gameObject2.transform.parent = gameObject.transform;
				for (int j = 0; j < biomes[num].edge_pieces.Length; j++)
				{
					if (biomes[num4].biome_name == biomes[num].edge_pieces[j].name)
					{
						if (biomes[num].edge_pieces[j].on_top)
						{
							gameObject2.transform.localScale = new Vector3(0f - gameObject2.transform.localScale.x, gameObject2.transform.localScale.y, gameObject2.transform.localScale.z);
						}
						gameObject2.GetComponent<MeshRenderer>().material.mainTexture = biomes[num].edge_pieces[j].tex;
						break;
					}
				}
			}
			int num5 = get_biome_id_at(zone, X, Z + 1);
			if (num5 != num)
			{
				GameObject gameObject3 = UnityEngine.Object.Instantiate(biome_edge_piece);
				gameObject3.name = "edge2";
				gameObject3.transform.rotation = Quaternion.Euler(-90f, 0f, -90f);
				gameObject3.transform.position = new Vector3((float)X * 10f, 0f, (float)Z * 10f) + new Vector3(0f, 0.04f, 10f);
				gameObject3.transform.parent = gameObject.transform;
				for (int k = 0; k < biomes[num].edge_pieces.Length; k++)
				{
					if (biomes[num5].biome_name == biomes[num].edge_pieces[k].name)
					{
						if (biomes[num].edge_pieces[k].on_top)
						{
							gameObject3.transform.localScale = new Vector3(0f - gameObject3.transform.localScale.x, gameObject3.transform.localScale.y, gameObject3.transform.localScale.z);
						}
						gameObject3.GetComponent<MeshRenderer>().material.mainTexture = biomes[num].edge_pieces[k].tex;
						break;
					}
				}
			}
		}
		if (!flag)
		{
			bool[,] array = new bool[10, 10];
			if (!flag2)
			{
				for (int l = 0; l < 10; l++)
				{
					for (int m = 0; m < 10; m++)
					{
						List<biome_obj> list = new List<biome_obj>();
						biome_obj[] biome_scenic = biomes[num].biome_scenic;
						foreach (biome_obj item in biome_scenic)
						{
							list.Insert(UnityEngine.Random.Range(0, list.Count), item);
						}
						bool flag3 = false;
						for (int num6 = 0; num6 < list.Count; num6++)
						{
							biome_obj biome_obj = list[num6];
							if (l + biome_obj.width > 10 || m + biome_obj.width > 10 || !all_empty(l, m, biome_obj.width, array) || !(UnityEngine.Random.value < biome_obj.commonness))
							{
								continue;
							}
							int value = UnityEngine.Random.Range(0, 4);
							if (biome_obj.dont_rotate)
							{
								value = 0;
							}
							PlayerData.Instance.SetSlotInt("fill(" + l + "," + m + ")", 1, chunkString_);
							PlayerData.Instance.SetSlotInt("obj(" + l + "," + m + ")", biome_obj.DEBUG_OBJ_INDEX, chunkString_);
							PlayerData.Instance.SetSlotInt("rot(" + l + "," + m + ")", value, chunkString_);
							fill(l, m, biome_obj.width, array);
							if (biome_obj.name == "gold-chest" || biome_obj.name == "titanium-chest")
							{
								PlayerData.Instance.SetSlotInt("special(" + l + "," + m + ")", inventory_ctr.Instance.unique_id_iterator, chunkString_);
								inventory_ctr.Instance.inc_then_set_iterator();
							}
							else if (biome_obj.name == "nest")
							{
								Vector3 position = new Vector3(X * 10, 0f, Z * 10) + new Vector3(l, 0f, m);
								if (GameController.Instance.depth_at(position) < 15f)
								{
									flag3 = false;
									break;
								}
								string value2 = biomes[num].possible_mobs[UnityEngine.Random.Range(0, biomes[num].possible_mobs.Length)];
								PlayerData.Instance.SetSlotString("mobA(" + l + "," + m + ")", value2, chunkString_);
							}
							flag3 = true;
							break;
						}
						if (!flag3)
						{
							PlayerData.Instance.SetSlotInt("fill(" + l + "," + m + ")", 0, chunkString_);
						}
					}
				}
			}
			if (!flag2)
			{
				List<Vector2> list2 = new List<Vector2>();
				for (int num7 = 0; num7 < 10; num7++)
				{
					for (int num8 = 0; num8 < 10; num8++)
					{
						if (!array[num7, num8])
						{
							list2.Insert(UnityEngine.Random.Range(0, list2.Count), new Vector2(num7, num8));
						}
					}
				}
				int num9 = UnityEngine.Random.Range(biomes[num].green_min, biomes[num].green_max);
				for (int num10 = 0; num10 < num9; num10++)
				{
					Vector2 vector = list2[0];
					list2.RemoveAt(0);
					int num11 = (int)vector.x;
					int num12 = (int)vector.y;
					PlayerData.Instance.SetSlotInt("fill(" + num11 + "," + num12 + ")", 2, chunkString_);
				}
				if (UnityEngine.Random.value < 0.146f)
				{
					string text = biomes[num].possible_mobs[UnityEngine.Random.Range(0, biomes[num].possible_mobs.Length)];
					string text2 = biomes[num].possible_mobs[UnityEngine.Random.Range(0, biomes[num].possible_mobs.Length)];
					if (Loader.Instance.temp_static_names_list.Contains(text) && Loader.Instance.temp_static_names_list.Contains(text2))
					{
						int num13 = UnityEngine.Random.Range(2, 4);
						for (int num14 = 0; num14 < num13; num14++)
						{
							Vector2 vector2 = list2[0];
							list2.RemoveAt(0);
							int num15 = (int)vector2.x;
							int num16 = (int)vector2.y;
							PlayerData.Instance.SetSlotInt("fill(" + num15 + "," + num16 + ")", 3, chunkString_);
							PlayerData.Instance.SetSlotString("mobA(" + num15 + "," + num16 + ")", text, chunkString_);
							PlayerData.Instance.SetSlotString("mobB(" + num15 + "," + num16 + ")", text2, chunkString_);
						}
					}
				}
			}
		}
		if (chunks_with_specials.ContainsKey(chunkString_))
		{
			foreach (place_special item4 in chunks_with_specials[chunkString_])
			{
				if (item4.type == 5)
				{
					fill_quest1x1(X, Z, item4.innerX, item4.innerZ, zone);
				}
				else if (item4.type == 6)
				{
					fill_quest1x1(X, Z, item4.innerX, item4.innerZ, zone);
					GameObject gameObject4 = UnityEngine.Object.Instantiate(creature_type_NPC);
					gameObject4.transform.rotation = Quaternion.Euler(0f, 90 + 90 * item4.rotation, 0f);
					gameObject4.transform.position = new Vector3((float)X * chunk_width, 0f, (float)Z * chunk_width) + new Vector3((float)item4.innerX + 0.5f, 0f, (float)item4.innerZ + 0.5f);
					gameObject4.transform.parent = gameObject.transform;
					List<string> list3 = new List<string>();
					list3.Add(item4.NPC_creatureA);
					list3.Add(item4.NPC_creatureB);
					GameObject hybrid = Loader.Instance.GetHybrid(list3);
					hybrid.transform.parent = gameObject4.transform;
					hybrid.transform.localPosition = Vector3.zero;
					hybrid.transform.localRotation = Quaternion.identity;
					gameObject4.GetComponent<NewInteractable>().unique_id = item4.NPC_id;
					gameObject4.GetComponent<NewInteractable>().icon_id = item4.icon_id;
					gameObject4.GetComponent<NewInteractable>().assign_overhead_icon();
					if (item4.NPC_weapon != "")
					{
						hybrid.GetComponent<creatureModel>().equip_hold_item(inventory_ctr.Instance.new_inv_items_by_name[item4.NPC_weapon].world_obj);
					}
				}
				else if (item4.type == 7)
				{
					fill_quest1x1(X, Z, item4.innerX, item4.innerZ, zone);
					int rotation = item4.rotation;
					string built = item4.built;
					int set_special_id = -1;
					if (built == "Red Shack")
					{
						set_special_id = item4.chest_shack_id;
					}
					inventory_ctr.Instance.BuildObject(built, new Vector3((float)X * chunk_width, 0f, (float)Z * chunk_width) + new Vector3((float)item4.innerX + 0.5f, 0f, (float)item4.innerZ + 0.5f), rotation, false, set_special_id);
				}
				else if (item4.type == 8)
				{
					biome_obj define = biomes[item4.biome_id].biome_scenic[item4.obj_id];
					for (int num17 = 0; num17 < define.width; num17++)
					{
						for (int num18 = 0; num18 < define.width; num18++)
						{
							fill_quest1x1(X, Z, item4.innerX + num17, item4.innerZ + num18, zone);
						}
					}
					int rot = 0;
					int assign_special_id = -1;
					if (item4.biome_id == 0 && item4.obj_id == 12)
					{
						assign_special_id = item4.chest_shack_id;
					}
					InstantiateBiomeObj(define, rot, gameObject.transform, zone, X, Z, item4.innerX, item4.innerZ, assign_special_id);
				}
				else if (item4.type == 9)
				{
					fill_quest1x1(X, Z, item4.innerX, item4.innerZ, zone);
					InstantiateBiomeObj(green_fruit_define, 0, gameObject.transform, zone, X, Z, item4.innerX, item4.innerZ);
				}
				else
				{
					Debug.Log("unknown type");
				}
			}
		}
		List<GameObject> packMates = new List<GameObject>();
		for (int num19 = 0; (float)num19 < chunk_width; num19++)
		{
			for (int num20 = 0; (float)num20 < chunk_width; num20++)
			{
				int slotInt = PlayerData.Instance.GetSlotInt("fill(" + num19 + "," + num20 + ")", chunkString_);
				switch (slotInt)
				{
				case 1:
				case 4:
					switch (slotInt)
					{
					case 1:
					{
						int slotInt3 = PlayerData.Instance.GetSlotInt("obj(" + num19 + "," + num20 + ")", chunkString_);
						biome_obj define2 = biomes[num].biome_scenic[slotInt3];
						bool flag4 = true;
						for (int num21 = 0; num21 < define2.width; num21++)
						{
							for (int num22 = 0; num22 < define2.width; num22++)
							{
								string item2 = (float)X * chunk_width + (float)num19 + (float)num21 + "," + ((float)Z * chunk_width + (float)num20 + (float)num22) + "," + zone;
								if (quest_fills.Contains(item2))
								{
									flag4 = false;
									break;
								}
							}
							if (!flag4)
							{
								break;
							}
						}
						if (flag4)
						{
							int slotInt4 = PlayerData.Instance.GetSlotInt("rot(" + num19 + "," + num20 + ")", chunkString_);
							InstantiateBiomeObj(define2, slotInt4, gameObject.transform, zone, X, Z, num19, num20);
						}
						break;
					}
					case 4:
					{
						int slotInt2 = PlayerData.Instance.GetSlotInt("rot(" + num19 + "," + num20 + ")", chunkString_);
						string slotString = PlayerData.Instance.GetSlotString("built(" + num19 + "," + num20 + ")", chunkString_);
						inventory_ctr.Instance.BuildObject(slotString, new Vector3((float)X * chunk_width, 0f, (float)Z * chunk_width) + new Vector3((float)num19 + 0.5f, 0f, (float)num20 + 0.5f), slotInt2, false);
						break;
					}
					}
					continue;
				case 0:
					continue;
				}
				string item3 = (float)X * chunk_width + (float)num19 + "," + ((float)Z * chunk_width + (float)num20) + "," + zone;
				if (!quest_fills.Contains(item3))
				{
					switch (slotInt)
					{
					case 2:
						InstantiateBiomeObj(green_fruit_define, 0, gameObject.transform, zone, X, Z, num19, num20);
						break;
					case 3:
					{
						string slotString2 = PlayerData.Instance.GetSlotString("mobA(" + num19 + "," + num20 + ")", chunkString_);
						string slotString3 = PlayerData.Instance.GetSlotString("mobB(" + num19 + "," + num20 + ")", chunkString_);
						NewMobControl.Instance.InstantiateWildMob(slotString2, slotString3, X, Z, num19, num20, packMates);
						break;
					}
					}
				}
			}
		}
	}

	private void fill_quest1x1(int chunkX, int chunkZ, int x, int z, string zone)
	{
		string item = (float)chunkX * chunk_width + (float)x + "," + ((float)chunkZ * chunk_width + (float)z) + "," + zone;
		quest_fills.Add(item);
	}

	private void InstantiateBiomeObj(biome_obj define, int rot, Transform parent_transform, string zone, int chunkX, int chunkZ, int x, int z, int assign_special_id = -1)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(define.prefab);
		gameObject.transform.position = new Vector3((float)chunkX * 10f, 0f, (float)chunkZ * 10f) + new Vector3(x, 0f, z) + new Vector3((float)define.width / 2f, 0f, (float)define.width / 2f);
		gameObject.transform.Rotate(Vector3.up, 90 * rot);
		gameObject.transform.parent = parent_transform;
		if (gameObject.GetComponent<NewCollectible>() != null)
		{
			gameObject.GetComponent<NewCollectible>().CustomStart();
		}
		if (gameObject.GetComponent<NewCombatant>() != null)
		{
			if (NewMobControl.Instance.curr_respawn_list().ContainsKey(gameObject.transform.position))
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			else
			{
				NewMobControl.Instance.active_combatants.Add(gameObject);
				gameObject.GetComponent<NewCombatant>().SetOrigin();
				gameObject.GetComponent<NewCombatant>().ReplenishHealth();
			}
		}
		if (define.name == "beehive")
		{
			NewMobControl.Instance.InstantiateWildMob("wasp", "wasp", chunkX, chunkZ, x, z + 1, new List<GameObject>(), 0.5f, GameController.creatureStates.aggressive, 1f, 0.7f, 0.049f, 2f);
			NewMobControl.Instance.InstantiateWildMob("wasp", "wasp", chunkX, chunkZ, x, z - 1, new List<GameObject>(), 0.5f, GameController.creatureStates.aggressive, 1f, 0.7f, 0.049f, 2f);
			NewMobControl.Instance.InstantiateWildMob("wasp", "wasp", chunkX, chunkZ, x + 1, z, new List<GameObject>(), 0.5f, GameController.creatureStates.aggressive, 1f, 0.7f, 0.049f, 2f);
		}
		else if (define.name == "nest")
		{
			string slotString = PlayerData.Instance.GetSlotString("mobA(" + x + "," + z + ")", GetChunkString_(zone, chunkX, chunkZ));
			Color color = NewMobControl.Instance.string_to_color(slotString + slotString);
			gameObject.transform.Find("egg").gameObject.GetComponent<MeshRenderer>().material.color = color;
			gameObject.GetComponent<NewCollectible>().flag_special = slotString;
			NewMobControl.Instance.InstantiateWildMob(slotString, slotString, chunkX, chunkZ, x, z, new List<GameObject>(), 2f, GameController.creatureStates.neutral, 0f, 2f, 0.04f, 0.5f);
		}
		if (!(define.name == "gold-chest") && !(define.name == "titanium-chest"))
		{
			return;
		}
		if (assign_special_id != -1)
		{
			gameObject.GetComponent<NewInteractable>().unique_id = assign_special_id;
		}
		else
		{
			gameObject.GetComponent<NewInteractable>().unique_id = PlayerData.Instance.GetSlotInt("special(" + x + "," + z + ")", GetChunkString_(zone, chunkX, chunkZ));
		}
		if (inventory_ctr.Instance.already_looted_gold_chests.ContainsKey(gameObject.GetComponent<NewInteractable>().unique_id))
		{
			UnityEngine.Object.Destroy(gameObject.transform.Find("Particle System").gameObject);
			if (define.name == "gold-chest")
			{
				gameObject.transform.Find("lid").localPosition = new Vector3(-0.335f, 0.869f, 0.038f);
				gameObject.transform.Find("lid").localRotation = Quaternion.Euler(-220f, 90f, 0f);
			}
			else if (define.name == "titanium-chest")
			{
				gameObject.transform.Find("lid").localPosition = new Vector3(0.045f, 0.588f, 0.038f);
				gameObject.transform.Find("lid").localRotation = Quaternion.Euler(-196f, 90f, 0f);
			}
		}
	}

	private void fill(int x, int z, int width, bool[,] filled)
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < width; j++)
			{
				filled[x + i, z + j] = true;
			}
		}
	}

	private bool all_empty(int x, int z, int width, bool[,] filled)
	{
		if (x + width >= 10 || z + width >= 10)
		{
			return false;
		}
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < width; j++)
			{
				if (filled[x + i, z + j])
				{
					return false;
				}
			}
		}
		return true;
	}

	private void Rotate(GameObject G, Vector3 local_origin, float degrees)
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.parent = G.transform;
		gameObject.transform.localPosition = local_origin;
		gameObject.transform.parent = G.transform.parent;
		gameObject.transform.localRotation = Quaternion.identity;
		G.transform.parent = gameObject.transform;
		gameObject.transform.Rotate(Vector3.up, degrees);
		G.transform.parent = gameObject.transform.parent;
		UnityEngine.Object.Destroy(gameObject);
	}

	public void DestroyAllTerrain()
	{
		foreach (KeyValuePair<string, Chunk_f> item in chunks_loaded)
		{
			item.Value.Delete();
			PlayerData.Instance.DeLoadGrouping(item.Key);
		}
		chunks_loaded.Clear();
	}

	private void GenerateBiomeFile()
	{
		generic_biome_map_w = biome_map.width;
		generic_biome_map_ids = new int[generic_biome_map_w, generic_biome_map_w];
		List<Color> list = new List<Color>();
		for (int i = 0; i < generic_biome_map_w; i++)
		{
			for (int j = 0; j < generic_biome_map_w; j++)
			{
				Color pixel = biome_map.GetPixel(i, j);
				if (!list.Contains(pixel))
				{
					list.Add(pixel);
				}
				int num = list.IndexOf(pixel);
				generic_biome_map_ids[i, j] = num;
			}
		}
		Packet packet = new Packet();
		packet.putShort((short)generic_biome_map_w);
		for (int k = 0; k < generic_biome_map_w; k++)
		{
			for (int l = 0; l < generic_biome_map_w; l++)
			{
				packet.putShort((short)generic_biome_map_ids[k, l]);
			}
		}
		File.WriteAllBytes("Assets/Resources/biomemap.txt", packet.outgoing_data.ToArray());
	}

	public void SetCreatureSize(float size, GameObject creature)
	{
		creatureScript component = creature.GetComponent<creatureScript>();
		component.myCreatureModel.gameObject.transform.localScale = Vector3.one * size;
		component.myCreatureModel.gameObject.transform.localPosition = Vector3.up * (size * component.myCreatureModel.height - 0.3f);
		creature.GetComponent<NewCombatant>().scale = size;
	}
}
