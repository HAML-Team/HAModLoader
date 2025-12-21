using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class creatureScript : MonoBehaviour
{
	private float accel = 0.03f;

	public float ramDMG;

	public bool isRamming;

	private int[] my_stats;

	[HideInInspector]
	public float walk_speed;

	private float attackWait;

	private float HP_regen_amount;

	public creatureModel myCreatureModel;

	[HideInInspector]
	public float override_wander;

	[HideInInspector]
	public Trans look_rotation = new Trans();

	public GameObject levelDisplay;

	public Vector3 generic_moveTo;

	public GameObject targetCombatant;

	public Vector3 velocity;

	private bool isMoving;

	public bool stopMoving;

	public int level;

	public List<GameObject> packMates_;

	public GameController.creatureStates STATE;

	[HideInInspector]
	public float AI_lockon_range = 3.8f;

	private bool fleeing;

	private Vector3 startPos;

	public int friendly_creatureA;

	public int friendly_creatureB;

	public int friendly_exp;

	public int friendly_nextLevelExp;

	public Vector3 online_position;

	private string username;

	private string creature_type_str;

	private Color creature_type_col;

	public int hatch_index_;

	public static float H = 0.3f;

	private bool retreat;

	[HideInInspector]
	public int attack_cooldown_n;

	private NewCombatant my_combatant;

	public Vector3 moving_at;

	public GameObject mutantParticle;

	private bool attack_anm_playing;

	private Rigidbody rb;

	public bool isGiant;

	public bool sprint;

	public float extra_interact_dist;

	public void DeLoad()
	{
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		if (STATE != GameController.creatureStates.friendly)
		{
			NewMobControl.Instance.loaded_creatures.Remove(GetComponent<NewCombatant>().origin);
		}
		if (mutantParticle != null)
		{
			Object.Destroy(mutantParticle);
		}
		if (levelDisplay != null)
		{
			GameController.Instance.possible_destroy.Remove(levelDisplay);
			Object.Destroy(levelDisplay);
		}
	}

	public void Init(GameObject myModel, bool playerObject, GameController.creatureStates STATE, int level, string creature_type_str, int[] stats, Color creature_type_col, bool bump_down)
	{
		this.STATE = STATE;
		myCreatureModel = myModel.GetComponent<creatureModel>();
		friendly_exp = 0;
		friendly_nextLevelExp = 5;
		startPos = base.transform.position;
		myModel.transform.parent = base.transform;
		myModel.transform.localRotation = Quaternion.identity;
		if (bump_down)
		{
			myModel.transform.localPosition = Vector3.down * 0.3f;
		}
		StartCoroutine(regen_health());
		look_rotation.rotation = base.transform.rotation;
		if (!playerObject)
		{
			StartCoroutine(ROAM());
			StartCoroutine(THINK());
			setStats(stats, level);
			this.creature_type_col = creature_type_col;
			this.creature_type_str = creature_type_str;
			if (GameController.Instance.focus_npc == null)
			{
				assign_level_display();
			}
		}
		rb = GetComponent<Rigidbody>();
	}

	private IEnumerator regen_health()
	{
		while (true)
		{
			float health_before = GetComponent<NewCombatant>().HP;
			yield return new WaitForSeconds(5.5f);
			if (GetComponent<NewCombatant>().HP == health_before && GetComponent<NewCombatant>().HP < (float)GetComponent<NewCombatant>().HP_start && GetComponent<NewCombatant>().HP > 0f)
			{
				GetComponent<NewCombatant>().HP += HP_regen_amount;
				GetComponent<NewCombatant>().set_heath_visual(0);
			}
		}
	}

	public void setStats(int[] input_stats, int level)
	{
		my_stats = input_stats;
		this.level = level;
		if (level <= 0)
		{
			level = 1;
		}
		HP_regen_amount = 5f / 42f * (float)input_stats[1] + 1f;
		int num = 0;
		int num2;
		if (base.gameObject != GameController.Instance.player)
		{
			num2 = (int)Mathf.Max(0f, level - 3);
		}
		else
		{
			num2 = level;
			float t = GameController.Instance.calc_slider(GameController.slider_type.health);
			num = (int)Mathf.Lerp(0f, Mathf.Min(10f, (float)num2 * 0.125f), t);
		}
		GetComponent<NewCombatant>().HP_start = 2 + (int)((float)num2 * 2f) + num;
		GetComponent<NewCombatant>().ReplenishHealth();
		if (base.gameObject == GameController.Instance.player)
		{
			walk_speed = 0.051f;
		}
		else
		{
			float num3 = level - GameController.Instance.playerLevel;
			if (num3 <= 0f)
			{
				float t2 = Mathf.InverseLerp(-15f, 0f, num3);
				GetComponent<NewCombatant>().exp_recieve = (int)Mathf.Lerp(9f, 45f, t2);
				walk_speed = 0.046f;
			}
			else
			{
				float t3 = Mathf.InverseLerp(0f, 15f, num3);
				GetComponent<NewCombatant>().exp_recieve = (int)Mathf.Lerp(45f, 70f, t3);
				walk_speed = Mathf.Lerp(0.046f, 0.0485f, t3);
			}
		}
		my_combatant = GetComponent<NewCombatant>();
	}

	public void set_state_icon()
	{
		if (levelDisplay != null)
		{
			levelDisplay.transform.Find("StateDisplay").GetComponent<Image>().sprite = GameController.Instance.sprites_creatureStates[(int)STATE];
		}
	}

	public IEnumerator show_level_up_text()
	{
		yield return new WaitForSeconds(0.6f);
		Object.Instantiate(GameController.Instance.type_friendly_levelup_particles).transform.position = new Vector3(base.transform.position.x, 0f, base.transform.position.z);
		GameController.Instance.showOverheadNotif("LEVEL UP", base.transform.position, true, false);
		if (levelDisplay != null)
		{
			set_level_text();
		}
	}

	public void friendly_inc_exp(int amount)
	{
		friendly_exp += amount;
		bool flag = false;
		while (friendly_exp >= friendly_nextLevelExp)
		{
			friendly_exp -= friendly_nextLevelExp;
			friendly_nextLevelExp += GameController.level_EXPONENT;
			level++;
			setStats(new int[9], level);
			if (!flag)
			{
				StartCoroutine(show_level_up_text());
				flag = true;
			}
		}
		PlayerData.Instance.SetSlotInt("friendly_exp" + hatch_index_, friendly_exp, PlayerData.grouping_t.general);
		PlayerData.Instance.SetSlotInt("friendly_nextLevelExp" + hatch_index_, friendly_nextLevelExp, PlayerData.grouping_t.general);
		PlayerData.Instance.SetSlotInt("friendly_lvl" + hatch_index_, level, PlayerData.grouping_t.general);
	}

	public void re_enable()
	{
		StartCoroutine(regen_health());
		StartCoroutine(ROAM());
		StartCoroutine(THINK());
	}

	public void assign_level_display()
	{
		levelDisplay = Object.Instantiate(GameController.Instance.type_creatureLevelDisplay);
		levelDisplay.transform.parent = NewMobControl.Instance.gameObject.transform;
		levelDisplay.transform.SetAsFirstSibling();
		levelDisplay.transform.localPosition = Vector3.zero;
		levelDisplay.transform.localScale = Vector3.one * 0.75f;
		levelDisplay.transform.localRotation = Quaternion.identity;
		RectTransform obj = (RectTransform)levelDisplay.transform;
		Vector2 anchorMin = (((RectTransform)levelDisplay.transform).anchorMax = Vector2.one * 10f);
		obj.anchorMin = anchorMin;
		GameController.Instance.possible_destroy.Add(levelDisplay);
		set_level_text();
		set_creature_type_text();
		set_state_icon();
	}

	public void set_level_text()
	{
		levelDisplay.transform.Find("level").GetComponent<Text>().text = "LVL " + level;
	}

	private void set_creature_type_text()
	{
		Text component = levelDisplay.transform.Find("creature-type").GetComponent<Text>();
		component.text = creature_type_str;
		component.color = creature_type_col;
	}

	private IEnumerator ROAM()
	{
		if (STATE == GameController.creatureStates.friendly)
		{
			GameObject last_visited_trail_node = null;
			while (true)
			{
				yield return new WaitForSeconds(0.25f);
				if (GameController.Instance.player == null)
				{
					break;
				}
				if (!(Vector3.Distance(GameController.Instance.player.transform.position, base.transform.position) > 5.5f) && !(targetCombatant == null))
				{
					continue;
				}
				int num = hatch_index_ * 2 + 1;
				if (GameController.Instance.trail_nodes__.Count > num)
				{
					GameObject gameObject = GameController.Instance.trail_nodes__[num];
					if (gameObject != last_visited_trail_node)
					{
						last_visited_trail_node = gameObject;
						generic_moveTo = last_visited_trail_node.transform.position;
					}
				}
			}
			yield break;
		}
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(5f, 10f));
			if (targetCombatant == null && !fleeing)
			{
				if (override_wander != -1f)
				{
					generic_moveTo = startPos + new Vector3(Random.value, 0f, Random.value) * override_wander;
				}
				else
				{
					generic_moveTo = startPos + new Vector3(Random.value, 0f, Random.value) * 5f;
				}
			}
		}
	}

	private IEnumerator THINK()
	{
		while (true)
		{
			yield return new WaitForSeconds(1.5f);
			if (STATE != GameController.creatureStates.friendly)
			{
				if (!retreat)
				{
					if (Vector3.Distance(base.transform.position, startPos) > 23f)
					{
						retreat = true;
					}
				}
				else if (Vector3.Distance(base.transform.position, startPos) < 3f)
				{
					retreat = false;
				}
			}
			if (targetCombatant != null || GameController.Instance.player == null || GameController.Instance.player.GetComponent<NewCombatant>().HP <= 0f)
			{
				continue;
			}
			if (STATE == GameController.creatureStates.fearful)
			{
				if (Vector3.Distance(GameController.Instance.player.transform.position, base.transform.position) < AI_lockon_range)
				{
					if (GetComponent<NewCombatant>().HP < (float)GetComponent<NewCombatant>().HP_start * (1f / 3f))
					{
						STATE = GameController.creatureStates.aggressive;
						fleeing = false;
					}
					else
					{
						fleeing = true;
						generic_moveTo = base.transform.position + (base.transform.position - GameController.Instance.player.transform.position).normalized * 5f;
					}
				}
				else
				{
					fleeing = false;
				}
			}
			else
			{
				if (STATE != GameController.creatureStates.aggressive && STATE != GameController.creatureStates.friendly)
				{
					continue;
				}
				float num = float.MaxValue;
				GameObject gameObject = null;
				foreach (GameObject active_combatant in NewMobControl.Instance.active_combatants)
				{
					float num2 = AI_lockon_range;
					bool flag = false;
					NewCombatant component = active_combatant.GetComponent<NewCombatant>();
					if (component.mob_type == NewCombatant.TYPE_T.stationary)
					{
						if (STATE == GameController.creatureStates.friendly)
						{
							num2 = 2f;
							flag = true;
						}
					}
					else if (component.mob_type == NewCombatant.TYPE_T.creature)
					{
						creatureScript component2 = active_combatant.GetComponent<creatureScript>();
						if (STATE == GameController.creatureStates.aggressive)
						{
							if (component2.STATE == GameController.creatureStates.friendly)
							{
								flag = true;
							}
						}
						else if (STATE == GameController.creatureStates.friendly && component2.STATE == GameController.creatureStates.aggressive)
						{
							flag = true;
						}
					}
					float num3 = Vector3.Distance(active_combatant.transform.position, base.transform.position);
					if (num3 < num2 && flag && num3 < num)
					{
						num = num3;
						gameObject = active_combatant;
					}
				}
				if (gameObject != null)
				{
					targetCombatant = gameObject;
					generic_moveTo = Vector3.zero;
				}
				else
				{
					targetCombatant = null;
				}
			}
		}
	}

	public void attack_myTarget()
	{
		if (isRamming)
		{
			isRamming = false;
			if (targetCombatant.GetComponent<Rigidbody>() != null)
			{
				targetCombatant.GetComponent<Rigidbody>().velocity = Vector3.up * 8f;
				targetCombatant.GetComponent<Rigidbody>().velocity += (base.transform.position - targetCombatant.transform.position).normalized * -7f;
			}
			Object.Instantiate(perk_controller.Instance.glob_fire).transform.position = Vector3.Lerp(base.transform.position, targetCombatant.transform.position, 0.5f);
			targetCombatant.GetComponent<NewCombatant>().wasHit((int)ramDMG, this, false, NewCombatant.hit_col.color_red);
			targetCombatant = null;
		}
		else if (attack_cooldown_n <= 0)
		{
			StartCoroutine(try_attack());
			attack_cooldown_n = 57;
		}
	}

	private IEnumerator try_attack()
	{
		if (!attack_anm_playing)
		{
			attack_anm_playing = true;
			GameController.Instance.start_creature_animation(myCreatureModel.children_, 3);
		}
		yield return new WaitForSeconds(0.15f);
		NewCombatant.hit_col col_ = NewCombatant.hit_col.color_red;
		if (!(targetCombatant != null) || GameController.Instance.pause)
		{
			yield break;
		}
		if (targetCombatant.GetComponent<NewCombatant>().HP > 0f && GetComponent<NewCombatant>().HP > 0f)
		{
			bool flag = false;
			int num = 0;
			int fake_damage = -1;
			NewCombatant.TYPE_T mob_type = targetCombatant.GetComponent<NewCombatant>().mob_type;
			if (mob_type == NewCombatant.TYPE_T.mineral)
			{
				bool flag2 = false;
				switch (targetCombatant.GetComponent<NewCombatant>().mob_identifying_name)
				{
				case "rock":
					if (inventory_ctr.Instance.get_best_pick() >= 1)
					{
						flag2 = true;
					}
					break;
				case "metal":
					if (inventory_ctr.Instance.get_best_pick() >= 2)
					{
						flag2 = true;
					}
					break;
				case "gold":
				case "titanium":
				case "uranium":
					if (inventory_ctr.Instance.get_best_pick() >= 3)
					{
						flag2 = true;
					}
					break;
				}
				num = (flag2 ? ((int)Random.Range(1f, 2.9f)) : 0);
				col_ = NewCombatant.hit_col.color_yellow;
			}
			else
			{
				float t = 0f;
				float t2 = 0f;
				if (base.gameObject == GameController.Instance.player)
				{
					t = GameController.Instance.calc_slider(GameController.slider_type.accuracy);
					t2 = GameController.Instance.calc_slider(GameController.slider_type.attack);
					if (Random.value < Mathf.Lerp(0.18f, 0f, t))
					{
						flag = true;
					}
				}
				else
				{
					float t3 = GameController.Instance.calc_slider(GameController.slider_type.dodge);
					if (Random.value < Mathf.Lerp(0.18f, 0.27f, t3))
					{
						flag = true;
					}
				}
				if (!flag)
				{
					int hP_start = GetComponent<NewCombatant>().HP_start;
					int num2;
					if (base.gameObject == GameController.Instance.player)
					{
						num2 = ((!(Random.value < Mathf.Lerp(0f, 0.45f, t2))) ? 1 : 2);
					}
					else
					{
						float t4 = Mathf.InverseLerp(0f, 15f, level);
						num2 = ((Random.value < Mathf.Lerp(0f, 1f, t4)) ? 1 : 0);
					}
					num = ((!(Random.value < Mathf.Lerp(0.23f, 0.28f, t))) ? (num2 + (int)Random.Range((float)hP_start * 0.018f, (float)hP_start * 0.03f)) : (num2 + (int)Random.Range((float)hP_start * 0.08f, (float)hP_start * Mathf.Lerp(0.1f, 0.13f, t2))));
					if (base.gameObject == GameController.Instance.player)
					{
						if (inventory_ctr.Instance.inventory_objType[15] == "Stone Sword")
						{
							num += (int)Random.Range(0f, 1.98f);
						}
						else if (inventory_ctr.Instance.inventory_objType[15] == "Metal Sword")
						{
							num += (int)Random.Range(0f, 2.98f);
						}
						else if (inventory_ctr.Instance.inventory_objType[15] == "Titanium Sword")
						{
							num += (int)Random.Range(0f, 3.98f);
						}
						else if (inventory_ctr.Instance.inventory_objType[15] == "Magma Sword")
						{
							num += (int)Random.Range(0f, 4.98f);
						}
						else if (inventory_ctr.Instance.inventory_objType[15] == "Ice Sword")
						{
							num += (int)Random.Range(0f, 5.98f);
						}
						else if (inventory_ctr.Instance.inventory_objType[15] == "Dark Sword")
						{
							num += (int)Random.Range(0f, 6.98f);
						}
					}
					if (base.gameObject != GameController.Instance.player && Random.value < 0.33f)
					{
						if (inventory_ctr.Instance.inventory_objType[16] == "Wood Helmet")
						{
							num -= (int)Random.Range(0f, 1.98f);
						}
						else if (inventory_ctr.Instance.inventory_objType[16] == "Metal Helmet")
						{
							num -= (int)Random.Range(1f, 2.98f);
						}
						else if (inventory_ctr.Instance.inventory_objType[16] == "Titanium Helmet")
						{
							num -= (int)Random.Range(1f, 3.98f);
						}
						else if (inventory_ctr.Instance.inventory_objType[16] == "Ice Helmet")
						{
							num -= (int)Random.Range(1f, 5.98f);
						}
						else if (inventory_ctr.Instance.inventory_objType[16] == "Dark Helmet")
						{
							num -= (int)Random.Range(1f, 6.98f);
						}
						if (inventory_ctr.Instance.inventory_objType[17] == "Wood Armor")
						{
							num -= (int)Random.Range(0f, 1.98f);
						}
						else if (inventory_ctr.Instance.inventory_objType[17] == "Metal Armor")
						{
							num -= (int)Random.Range(1f, 2.98f);
						}
						else if (inventory_ctr.Instance.inventory_objType[17] == "Titanium Armor")
						{
							num -= (int)Random.Range(1f, 3.98f);
						}
						else if (inventory_ctr.Instance.inventory_objType[17] == "Ice Armor")
						{
							num -= (int)Random.Range(1f, 5.98f);
						}
						else if (inventory_ctr.Instance.inventory_objType[17] == "Dark Armor")
						{
							num -= (int)Random.Range(1f, 6.98f);
						}
						if (num < 0)
						{
							num = 0;
						}
					}
				}
				switch (mob_type)
				{
				case NewCombatant.TYPE_T.creature:
					if (targetCombatant.GetComponent<creatureScript>().isGiant)
					{
						num = 0;
					}
					break;
				case NewCombatant.TYPE_T.stationary:
					fake_damage = num;
					if (num > 1)
					{
						num = 1;
					}
					break;
				}
			}
			targetCombatant.GetComponent<NewCombatant>().wasHit(num, this, flag, col_, fake_damage);
		}
		else
		{
			targetCombatant = null;
		}
	}

	public void MoveAt(Vector3 pos)
	{
		moving_at = pos;
		look_rotation.position = base.transform.position;
		LookSpot(new Vector3(pos.x, base.transform.position.y, pos.z));
		if (my_combatant.amount_bind != 0f)
		{
			return;
		}
		Vector3 normalized = (pos - base.transform.position).normalized;
		if (!isRamming)
		{
			velocity += new Vector3(normalized.x, 0f, normalized.z) * accel;
			float num = ((!sprint) ? walk_speed : (walk_speed * 1.9f));
			if (velocity.magnitude > num)
			{
				velocity = velocity.normalized * num;
			}
		}
		else
		{
			velocity += new Vector3(normalized.x, 0f, normalized.z) * 0.08f;
			if (velocity.magnitude > 0.15f)
			{
				velocity = velocity.normalized * 0.15f;
			}
		}
	}

	private void FixedUpdate()
	{
		moving_at = Vector3.zero;
		if (attack_cooldown_n > 0)
		{
			attack_cooldown_n--;
		}
		if (base.transform.position.y < -3f)
		{
			if (base.gameObject == GameController.Instance.player)
			{
				rb.velocity = Vector3.zero;
				base.transform.position = new Vector3(base.transform.position.x, 5f, base.transform.position.z);
			}
			else
			{
				if (STATE != GameController.creatureStates.friendly)
				{
					DeLoad();
					return;
				}
				rb.velocity = Vector3.zero;
				base.transform.position = GameController.Instance.player.transform.position + Vector3.back * 2f;
			}
		}
		if (!stopMoving && !GameController.Instance.pause)
		{
			if (targetCombatant == null)
			{
				if (isRamming)
				{
					isRamming = false;
				}
				if (generic_moveTo != Vector3.zero)
				{
					if (Vector3.Distance(base.transform.position, generic_moveTo) < 0.8f + extra_interact_dist)
					{
						generic_moveTo = Vector3.zero;
						if (base.gameObject == GameController.Instance.player)
						{
							GameController.Instance.targeted_circle_graphic.GetComponent<Animation>().PlayQueued("targetDisappear");
							GameController.Instance.targetShowing = false;
							if (GameController.Instance.player_interacting)
							{
								GameController.Instance.player_interact();
							}
						}
					}
					else
					{
						MoveAt(generic_moveTo);
					}
				}
			}
			else if (!retreat)
			{
				if (Vector3.Distance(base.transform.position, targetCombatant.transform.position) < 0.7f + (GetComponent<NewCombatant>().scale + targetCombatant.GetComponent<NewCombatant>().scale) * 0.5f + targetCombatant.GetComponent<NewCombatant>().extra_combat_dist)
				{
					look_rotation.position = base.transform.position;
					LookSpot(targetCombatant.transform.position);
					attack_myTarget();
				}
				else
				{
					MoveAt(targetCombatant.transform.position);
				}
			}
			else
			{
				MoveAt(startPos);
			}
		}
		if (levelDisplay != null)
		{
			NewMobControl.Instance.SnapOverhead((RectTransform)levelDisplay.transform, base.transform.position);
		}
		base.transform.position += velocity;
		velocity *= 0.9f;
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, look_rotation.rotation, Time.deltaTime * 8f);
		if (!attack_anm_playing)
		{
			if (velocity.magnitude > 0.01f)
			{
				if (!isMoving)
				{
					GameController.Instance.start_creature_animation(myCreatureModel.children_, 1);
					isMoving = true;
				}
			}
			else if (isMoving)
			{
				GameController.Instance.start_creature_animation(myCreatureModel.children_, 0);
				isMoving = false;
			}
		}
		else if (myCreatureModel.children_[0].GetComponent<limb_scr>().animation_complete)
		{
			attack_anm_playing = false;
			GameController.Instance.start_creature_animation(myCreatureModel.children_, 0);
			isMoving = false;
		}
	}

	private void LookSpot(Vector3 V)
	{
		look_rotation.rotation = Quaternion.LookRotation((V - look_rotation.position).normalized);
	}
}
