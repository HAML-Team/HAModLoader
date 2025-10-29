using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class morpher : MonoBehaviour
{
	public static morpher Instance;

	public GameObject floorType;

	public GameObject collidrType;

	private GameObject collidr;

	private int offset;

	private GameObject floor;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	private void Start()
	{
		if (this == Instance)
		{
			create_collidr();
			SceneManager.activeSceneChanged += ChangedActiveScene;
		}
	}

	private void create_collidr()
	{
		floor = Object.Instantiate(floorType);
		collidr = Object.Instantiate(collidrType);
		collidr.name = "Collidr";
	}

	private void ChangedActiveScene(Scene current, Scene next)
	{
		create_collidr();
	}

	public GameObject morphCreatures_(List<GameObject> creaturesToMorph, bool justHead, int screenshot)
	{
		List<string> list = new List<string>();
		List<GameObject> list2 = new List<GameObject>();
		GameObject gameObject = Object.Instantiate(Loader.Instance.type_creatureModel);
		gameObject.GetComponent<creatureModel>().Init();
		gameObject.GetComponent<creatureModel>().myName = creaturesToMorph[0].GetComponent<creatureModel>().myName + "-" + creaturesToMorph[1].GetComponent<creatureModel>().myName;
		gameObject.GetComponent<creatureModel>().nameSuffix = creaturesToMorph[creaturesToMorph.Count - 1].GetComponent<creatureModel>().nameSuffix;
		switch (creaturesToMorph.Count)
		{
		case 2:
			gameObject.GetComponent<creatureModel>().namePrefix = creaturesToMorph[0].GetComponent<creatureModel>().namePrefix;
			break;
		case 3:
			gameObject.GetComponent<creatureModel>().namePrefix = creaturesToMorph[1].GetComponent<creatureModel>().namePrefix;
			break;
		case 4:
			gameObject.GetComponent<creatureModel>().namePrefix = "Mutant";
			break;
		case 5:
			gameObject.GetComponent<creatureModel>().namePrefix = "Monstrosity";
			gameObject.GetComponent<creatureModel>().nameSuffix = "";
			break;
		default:
			gameObject.GetComponent<creatureModel>().namePrefix = creaturesToMorph[3].GetComponent<creatureModel>().namePrefix;
			gameObject.GetComponent<creatureModel>().nameSuffix = "GOD";
			break;
		}
		foreach (GameObject item in creaturesToMorph)
		{
			gameObject.GetComponent<creatureModel>().creatures_that_made_me.Add(item.GetComponent<creatureModel>().myName);
		}
		bool flag = false;
		bool flag2 = false;
		gameObject.GetComponent<creatureModel>().myCol = creaturesToMorph[0].GetComponent<creatureModel>().myCol;
		for (int i = 0; i < creaturesToMorph.Count; i++)
		{
			for (int j = 0; j < creaturesToMorph[i].GetComponent<creatureModel>().children_.Length; j++)
			{
				string compName = creaturesToMorph[i].GetComponent<creatureModel>().children_[j].GetComponent<limb_scr>().compName;
				if (list.IndexOf(compName) != -1)
				{
					continue;
				}
				list.Add(compName);
				GameObject gameObject2 = Object.Instantiate(Loader.Instance.type_limb);
				gameObject2.GetComponent<limb_scr>().Init();
				gameObject2.GetComponent<limb_scr>().compName = compName;
				gameObject2.GetComponent<limb_scr>().setupFrames(false);
				list2.Add(gameObject2);
				if (!flag2 && compName == "head")
				{
					gameObject.GetComponent<creatureModel>().head_obj = gameObject2;
					flag2 = true;
				}
				if (!flag)
				{
					switch (compName)
					{
					case "arm":
					case "wing":
						gameObject.GetComponent<creatureModel>().hand_obj = gameObject2;
						break;
					case "hand":
						gameObject.GetComponent<creatureModel>().hand_obj = gameObject2;
						flag = true;
						break;
					}
				}
			}
			if (gameObject.GetComponent<creatureModel>().hand_obj == null)
			{
				gameObject.GetComponent<creatureModel>().hand_obj = list2[0];
			}
			if (gameObject.GetComponent<creatureModel>().head_obj == null)
			{
				gameObject.GetComponent<creatureModel>().head_obj = list2[0];
			}
			gameObject.GetComponent<creatureModel>().body_obj = list2[0];
			Color myCol = gameObject.GetComponent<creatureModel>().myCol;
			Color myCol2 = creaturesToMorph[i].GetComponent<creatureModel>().myCol;
			myCol.r = (myCol.r * (float)creaturesToMorph.Count + myCol2.r) / (float)(creaturesToMorph.Count + 1);
			myCol.g = (myCol.g * (float)creaturesToMorph.Count + myCol2.g) / (float)(creaturesToMorph.Count + 1);
			myCol.b = (myCol.b * (float)creaturesToMorph.Count + myCol2.b) / (float)(creaturesToMorph.Count + 1);
			gameObject.GetComponent<creatureModel>().myCol = myCol;
		}
		for (int k = 0; k < list2.Count; k++)
		{
			list2[k].transform.parent = gameObject.transform;
			string text = list[k];
			int num = 0;
			for (int l = 0; l < creaturesToMorph.Count; l++)
			{
				for (int m = 0; m < creaturesToMorph[l].GetComponent<creatureModel>().children_.Length; m++)
				{
					if (!(creaturesToMorph[l].GetComponent<creatureModel>().children_[m].GetComponent<limb_scr>().compName == text))
					{
						continue;
					}
					limb_scr component = creaturesToMorph[l].GetComponent<creatureModel>().children_[m].GetComponent<limb_scr>();
					limb_scr component2 = list2[k].GetComponent<limb_scr>();
					GameObject visual = component.visual;
					GameObject visual2 = component2.visual;
					if (num == 0)
					{
						visual2.transform.localScale = visual.transform.localScale;
						visual2.transform.localPosition = visual.transform.localPosition;
						component2.setDecorative(component.isDecorative);
						component2.symmetrical = component.symmetrical;
						component2.inherit = component.inherit;
						list2[k].GetComponent<limb_scr>().invertSymmAnm = creaturesToMorph[l].GetComponent<creatureModel>().children_[m].GetComponent<limb_scr>().invertSymmAnm;
						component2.hsv_values = (float[])component.hsv_values.Clone();
						component2.textureName = component.textureName;
						if (component.parent_ != null)
						{
							GameObject gameObject3 = list2[list.IndexOf(component.parent_.GetComponent<limb_scr>().compName)];
							component2.create_snap(gameObject3.GetComponent<limb_scr>().visual.transform);
							component2.parent_ = gameObject3;
							list2[list.IndexOf(component.parent_.GetComponent<limb_scr>().compName)].GetComponent<limb_scr>().children_.Add(list2[k]);
						}
						component2.children_ = new List<GameObject>();
						for (int n = 0; n < component.frames_rotations.Count; n++)
						{
							for (int num2 = 0; num2 < component.frames_rotations[n].Length; num2++)
							{
								component2.GetComponent<limb_scr>().frames_rotations[n][num2] = component.GetComponent<limb_scr>().frames_rotations[n][num2];
								component2.GetComponent<limb_scr>().frames_snapPositions[n][num2] = component.GetComponent<limb_scr>().frames_snapPositions[n][num2];
							}
						}
						num = 1;
						continue;
					}
					visual2.transform.localScale = (visual2.transform.localScale * num + visual.transform.localScale) / (num + 1);
					visual2.transform.localPosition = (visual2.transform.localPosition * num + visual.transform.localPosition) / (num + 1);
					for (int num3 = 0; num3 < 4; num3++)
					{
						component2.hsv_values[num3] = (component2.hsv_values[num3] * (float)num + component.hsv_values[num3]) / (float)(num + 1);
					}
					if (component2.textureName == "None")
					{
						component2.textureName = component.textureName;
					}
					for (int num4 = 0; num4 < component.frames_rotations.Count; num4++)
					{
						for (int num5 = 0; num5 < component.frames_rotations[num4].Length; num5++)
						{
							Quaternion quaternion = Quaternion.Lerp(component2.GetComponent<limb_scr>().frames_rotations[num4][num5], component.GetComponent<limb_scr>().frames_rotations[num4][num5], 0.5f);
							component2.GetComponent<limb_scr>().frames_rotations[num4][num5] = quaternion;
							if (component.parent_ != null)
							{
								Vector3 vector = Vector3.Normalize(component2.GetComponent<limb_scr>().frames_snapPositions[num4][num5]);
								Vector3 vector2 = Vector3.Normalize(component.GetComponent<limb_scr>().frames_snapPositions[num4][num5]);
								Vector3 value = (vector * num + vector2) / (num + 1);
								component2.GetComponent<limb_scr>().frames_snapPositions[num4][num5] = Vector3.Normalize(value);
							}
						}
					}
					num++;
				}
			}
			if (num == 1 && list2[k].GetComponent<limb_scr>().compName == "mouth")
			{
				list2[k].GetComponent<limb_scr>().textureName = "zzz";
			}
		}
		GameObject[] array = new GameObject[list2.Count];
		for (int num6 = 0; num6 < list2.Count; num6++)
		{
			GetComponent<Loader>().setCubeColor(list2[num6].GetComponent<limb_scr>().hsv_values, list2[num6], gameObject.GetComponent<creatureModel>().myCol);
			GetComponent<Loader>().setTexture(list2[num6].GetComponent<limb_scr>().textureName, list2[num6]);
		}
		for (int num7 = 0; num7 < list2.Count; num7++)
		{
			if (list2[num7].GetComponent<limb_scr>().symmetrical)
			{
				list2[num7].GetComponent<limb_scr>().chain_makeDummy(true);
			}
			array[num7] = list2[num7];
		}
		gameObject.transform.position = new Vector3(100 + offset, 100 + offset, 100 + offset);
		offset += 10;
		for (int num8 = 0; num8 < list2[0].GetComponent<limb_scr>().frames_rotations.Count; num8++)
		{
			for (int num9 = 0; num9 < list2[0].GetComponent<limb_scr>().frames_rotations[num8].Length; num9++)
			{
				LoadFrame(num8, num9, array);
				for (int num10 = 0; num10 < list2.Count; num10++)
				{
					if (list2[num10].GetComponent<limb_scr>().parent_ != null)
					{
						list2[num10].GetComponent<limb_scr>().parent_.GetComponent<limb_scr>().setupCollider(collidr);
						doRaycast(list2[num10], num8, num9);
						collidr.GetComponent<BoxCollider>().enabled = false;
					}
				}
			}
		}
		collidr.transform.parent = null;
		collidr.transform.position = Vector3.up * 20f;
		LoadFrame(0, 0, array);
		gameObject.GetComponent<creatureModel>().children_ = array;
		StartCoroutine(delayed_calculate_height(gameObject, floor, screenshot, gameObject.GetComponent<creatureModel>().namePrefix, gameObject.GetComponent<creatureModel>().nameSuffix));
		if (SceneManager.GetActiveScene().name != "Menu")
		{
			gameObject.GetComponent<creatureModel>().bonus_stat = morph_Stats(creaturesToMorph[0].GetComponent<creatureModel>().seed, creaturesToMorph[1].GetComponent<creatureModel>().seed);
		}
		list2[0].transform.localPosition = Vector3.zero;
		return gameObject;
	}

	private IEnumerator delayed_calculate_height(GameObject MODEL, GameObject floor, int screenshot, string sA, string sB)
	{
		yield return new WaitForSeconds(0.01f);
		if (MODEL == null)
		{
			Debug.Log("MORPH ERROR");
			yield break;
		}
		Vector3 localPosition = MODEL.transform.localPosition;
		MODEL.transform.localPosition = new Vector3(100 + offset, 100 + offset, 100 + offset);
		offset += 10;
		floor.transform.position = MODEL.transform.position + new Vector3(0f, -10f, 0f);
		GameObject[] children_ = MODEL.GetComponent<creatureModel>().children_;
		for (int i = 0; i < children_.Length; i++)
		{
			children_[i].GetComponent<limb_scr>().visual.GetComponent<BoxCollider>().enabled = true;
		}
		RaycastHit hitInfo;
		if (floor.GetComponent<Rigidbody>().SweepTest(Vector3.up, out hitInfo, 20f))
		{
			MODEL.GetComponent<creatureModel>().height = 9.88f - hitInfo.distance;
		}
		MODEL.transform.localPosition = localPosition + MODEL.GetComponent<creatureModel>().height * Vector3.up;
		for (int j = 0; j < children_.Length; j++)
		{
			children_[j].GetComponent<limb_scr>().visual.GetComponent<BoxCollider>().enabled = false;
		}
		MODEL.GetComponent<creatureModel>().height_set = true;
	}

	public void doRaycast(GameObject obj, int anm, int frame)
	{
		GameObject visual = obj.GetComponent<limb_scr>().parent_.GetComponent<limb_scr>().visual;
		limb_scr component = obj.GetComponent<limb_scr>();
		Vector3 vector = component.snap_global - visual.transform.position;
		vector.Normalize();
		float num = 10f;
		RaycastHit hitInfo;
		if (Physics.Raycast(visual.transform.position + 10f * vector, -vector, out hitInfo, num))
		{
			num = hitInfo.distance;
		}
		component.snap_global = visual.transform.position + 10f * vector + -vector * num;
		obj.GetComponent<limb_scr>().frames_snapPositions[anm][frame] = component.get_snap_local();
	}

	public void LoadFrame(int animationNumber, int frameNumber, GameObject[] g)
	{
		for (int i = 0; i < g.Length; i++)
		{
			limb_scr component = g[i].GetComponent<limb_scr>();
			g[i].transform.rotation = component.frames_rotations[animationNumber][frameNumber];
			if (component.compName != "torso")
			{
				component.set_snap_local(component.frames_snapPositions[animationNumber][frameNumber]);
			}
			else
			{
				component.visual.transform.localPosition = component.frames_snapPositions[animationNumber][frameNumber];
			}
		}
	}

	public int morph_Stats(float seedA, float seedB)
	{
		int[] array = new int[6];
		for (int i = 0; i < 6; i++)
		{
			array[i] = Mathf.FloorToInt(seedA *= 10f) % 10;
		}
		int[] array2 = new int[6];
		for (int j = 0; j < 6; j++)
		{
			array2[j] = Mathf.FloorToInt(seedB *= 10f) % 10;
		}
		return (array[0] + array2[1]) % perk_controller.Instance.perk_defs.Keys.Count;
	}
}
