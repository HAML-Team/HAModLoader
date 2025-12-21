using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewCombatant : MonoBehaviour
{
	public enum TYPE_T
	{
		unknown = 0,
		stationary = 1,
		mineral = 2,
		creature = 3
	}

	public enum hit_col
	{
		color_red = 0,
		color_blue = 1,
		color_yellow = 2,
		color_green = 3
	}

	public enum effect
	{
		accuracy = 0,
		out_dmg = 1,
		bind = 2,
		poison = 3,
		giant = 4,
		eagleEye = 5,
		sprint = 6
	}

	public string mob_identifying_name;

	public int HP_start;

	public int exp_recieve;

	public int respawn_time;

	public TYPE_T mob_type;

	public string item_recieve_on_kill;

	public float scale = 1f;

	public float extra_combat_dist;

	[HideInInspector]
	public Vector3 origin;

	private bool crushed_by_giant;

	[HideInInspector]
	public float HP;

	[HideInInspector]
	public GameObject healthbar;

	private IEnumerator animate_hit;

	public IEnumerator disappearHealthbar;

	private Image healthbarGreen;

	private Image healthbarYellow;

	private Text splatDmg;

	private float yellow_X;

	private float yellow_width;

	private float yellow_goto_X;

	private float yellow_goto_width;

	private Camera mainCamera;

	private Vector2 hit_start_pos;

	[HideInInspector]
	public float amount_dec_acc;

	[HideInInspector]
	public float amount_dec_dmg;

	[HideInInspector]
	public float amount_bind;

	[HideInInspector]
	public float amount_poison;

	[HideInInspector]
	public float amount_giant;

	[HideInInspector]
	public float amount_eagle;

	[HideInInspector]
	public float amount_sprint;

	[HideInInspector]
	public GameObject vis_webD;

	[HideInInspector]
	public GameObject vis_screechD;

	[HideInInspector]
	public GameObject vis_weakenD;

	[HideInInspector]
	public GameObject vis_poison;

	private List<Dictionary<string, object>> all_effects = new List<Dictionary<string, object>>();

	[HideInInspector]
	public creatureScript who_poisoned_me;

	public void on_crush()
	{
		crushed_by_giant = true;
	}

	public void SetOrigin()
	{
		origin = base.transform.position;
	}

	public void ReplenishHealth()
	{
		HP = HP_start;
	}

	private void Kill()
	{
		if (mob_type == TYPE_T.creature && !crushed_by_giant)
		{
			NewMobControl.Instance.spawn_drop("Bones", base.transform.position, scale);
			if (Random.value < 0.6f)
			{
				NewMobControl.Instance.spawn_drop("Coins Few", base.transform.position + new Vector3(Random.Range(-1, 1), 0f, Random.Range(-1, 1)) * 0.2f);
			}
		}
		Dictionary<Vector3, respawn> dictionary = NewMobControl.Instance.curr_respawn_list();
		if (!dictionary.ContainsKey(origin))
		{
			dictionary.Add(origin, new respawn(respawn_time));
		}
		Object.Destroy(base.gameObject);
	}

	public void OnDestroy()
	{
		NewMobControl.Instance.active_combatants.Remove(base.gameObject);
		remove_healthbar();
	}

	private void remove_healthbar()
	{
		if (healthbar != null)
		{
			GameController.Instance.possible_destroy.Remove(healthbar);
			Object.Destroy(healthbar);
		}
	}

	public void wasHit(int damage, creatureScript hit_by, bool miss, hit_col col_, int fake_damage = -1)
	{
		bool dodge = false;
		if (base.gameObject == GameController.Instance.player && miss)
		{
			miss = false;
			dodge = true;
		}
		Show_visual_splat(col_, damage, fake_damage, miss, dodge);
		creatureScript component = GetComponent<creatureScript>();
		if (component != null && base.gameObject != GameController.Instance.player)
		{
			GetComponent<creatureScript>().attack_cooldown_n = 22;
			if (hit_by != null)
			{
				if (component.STATE == GameController.creatureStates.friendly)
				{
					if (hit_by.gameObject != GameController.Instance.player)
					{
						component.targetCombatant = hit_by.gameObject;
					}
				}
				else if (component.STATE == GameController.creatureStates.aggressive || component.STATE == GameController.creatureStates.neutral)
				{
					component.targetCombatant = hit_by.gameObject;
				}
			}
		}
		if (hit_by.gameObject == GameController.Instance.player && inventory_ctr.Instance.inventory_objType[15] == "Dark Sword" && Random.value < 0.03f)
		{
			kill_darksword(hit_by);
		}
		else if (HP <= 0f)
		{
			die(hit_by, 0.5f, 0f);
		}
	}

	private void kill_darksword(creatureScript hit_by)
	{
		healthbar.SetActive(false);
		if (GetComponent<Rigidbody>() != null)
		{
			GetComponent<Rigidbody>().isKinematic = true;
		}
		GameObject gameObject = Object.Instantiate(perk_controller.Instance.prefab_darksword_kill);
		gameObject.transform.position = base.transform.position;
		base.transform.parent = gameObject.transform.Find("creature goes here");
		HP = 0f;
		die(hit_by, 2f, 1f);
	}

	private void Show_visual_splat(hit_col col_, int damage, int fake_damage, bool miss, bool dodge)
	{
		GameController.Instance.AssignHealthbar(base.gameObject);
		GameObject gameObject = healthbar.transform.Find("splat").gameObject;
		Color color = Color.black;
		switch (col_)
		{
		case hit_col.color_blue:
			color = NewGameControl.Instance.color_dmg_miss;
			break;
		case hit_col.color_green:
			color = NewGameControl.Instance.color_dmg_poison;
			break;
		case hit_col.color_red:
			color = NewGameControl.Instance.color_dmg_hit;
			break;
		case hit_col.color_yellow:
			color = NewGameControl.Instance.color_dmg_mine;
			break;
		}
		if (damage == 0)
		{
			if (color != NewGameControl.Instance.color_dmg_mine)
			{
				color = NewGameControl.Instance.color_dmg_miss;
			}
			if (miss)
			{
				splatDmg.text = "MISS";
				gameObject.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(175f, 140f);
			}
			else if (dodge)
			{
				splatDmg.text = "DODGE";
				gameObject.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(175f, 140f);
			}
			else
			{
				splatDmg.text = "0";
				splatDmg.transform.localPosition = default(Vector2);
				gameObject.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(140f, 140f);
			}
		}
		else
		{
			if (fake_damage == -1)
			{
				splatDmg.text = damage.ToString();
			}
			else
			{
				splatDmg.text = fake_damage.ToString();
			}
			gameObject.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(140f, 140f);
		}
		gameObject.GetComponent<Image>().color = color;
		gameObject.GetComponent<Animation>().Stop();
		gameObject.GetComponent<Animation>().PlayQueued("splatAppear");
		if (animate_hit != null)
		{
			StopCoroutine(animate_hit);
		}
		animate_hit = ANIMATE_HIT();
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(animate_hit);
		}
		set_heath_visual(damage);
		if (GetComponent<creatureScript>() != null && damage != 0)
		{
			if (base.gameObject == GameController.Instance.player)
			{
				GameController.Instance.sound_player_hit();
			}
			else
			{
				GameController.Instance.sound_creature_hit(0.8f);
			}
		}
	}

	public void die(creatureScript killer, float delay, float splat_delay)
	{
		StartCoroutine(Die(killer, delay, splat_delay));
	}

	private IEnumerator Die(creatureScript killer, float delay, float splat_delay)
	{
		if (killer != null)
		{
			killer.targetCombatant = null;
			if (killer.gameObject == GameController.Instance.player)
			{
				GameController.Instance.targeted_circle_graphic.GetComponent<Animation>().PlayQueued("targetDisappear");
				GameController.Instance.targetShowing = false;
			}
			if (base.gameObject == GameController.Instance.player)
			{
				string text = ((killer.GetComponent<NewCombatant>().scale > 2f) ? "Giant " : "");
				string text2 = ((killer.packMates_.Count >= 3) ? "Swarm of " : ((killer.packMates_.Count == 2) ? "Pack of " : ""));
				string text3 = ((killer.packMates_.Count > 1) ? "s" : "");
				GameController.Instance.killed_by = text2 + "<color=#bbbbff> Lvl " + killer.level + "</color>  <color=#ff3333ff>" + text + killer.myCreatureModel.myName + text3 + "</color>";
				GameController.Instance.PlayerDied();
			}
		}
		creatureScript component = GetComponent<creatureScript>();
		if (component != null)
		{
			component.stopMoving = true;
			if (component.STATE == GameController.creatureStates.friendly && component.gameObject != GameController.Instance.player)
			{
				Shop_positioner.Instance.hatched_companions.Remove(base.gameObject);
				foreach (GameObject hatched_companion in Shop_positioner.Instance.hatched_companions)
				{
					if (hatched_companion.GetComponent<creatureScript>().hatch_index_ > component.hatch_index_)
					{
						hatched_companion.GetComponent<creatureScript>().hatch_index_--;
						int hatch_index_ = hatched_companion.GetComponent<creatureScript>().hatch_index_;
						PlayerData.Instance.SetSlotInt("friendly_level" + hatch_index_, hatched_companion.GetComponent<creatureScript>().level, PlayerData.grouping_t.general);
						PlayerData.Instance.SetSlotInt("friendly_exp" + hatch_index_, hatched_companion.GetComponent<creatureScript>().friendly_exp, PlayerData.grouping_t.general);
						PlayerData.Instance.SetSlotInt("friendly_nextLevelExp" + hatch_index_, hatched_companion.GetComponent<creatureScript>().friendly_nextLevelExp, PlayerData.grouping_t.general);
						PlayerData.Instance.SetSlotInt("friendly_creatureA" + hatch_index_, hatched_companion.GetComponent<creatureScript>().friendly_creatureA, PlayerData.grouping_t.general);
						PlayerData.Instance.SetSlotInt("friendly_creatureB" + hatch_index_, hatched_companion.GetComponent<creatureScript>().friendly_creatureB, PlayerData.grouping_t.general);
					}
				}
				GameController.Instance.n_hatchlings_--;
				PlayerData.Instance.SetSlotInt("num_hatchlings", GameController.Instance.n_hatchlings_, PlayerData.grouping_t.general);
			}
		}
		yield return new WaitForSeconds(delay);
		if (delay != 0.5f)
		{
			healthbar.SetActive(true);
			Show_visual_splat(hit_col.color_red, 999, -1, false, false);
		}
		yield return new WaitForSeconds(splat_delay);
		if (GameController.Instance.player != null && killer != null)
		{
			if (killer.gameObject == GameController.Instance.player)
			{
				if (crushed_by_giant)
				{
					exp_recieve = 1;
				}
				GameController.Instance.debug_exp += exp_recieve;
				GameController.Instance.currentEXP += exp_recieve;
				PlayerData.Instance.SetSlotInt("currentEXP", GameController.Instance.currentEXP, PlayerData.grouping_t.general);
				if (item_recieve_on_kill == "")
				{
					GameController.Instance.showOverheadNotif("+" + exp_recieve + " Exp", base.transform.position, true, true);
					if (mob_type == TYPE_T.creature)
					{
						GameController.Instance.stats_kills++;
					}
					else
					{
						switch (mob_identifying_name)
						{
						case "fruit-green":
							GameController.Instance.stats_greenFruits++;
							break;
						case "fruit-red":
							GameController.Instance.stats_redFruits++;
							break;
						case "fruit-blue":
							GameController.Instance.stats_blueFruits++;
							break;
						}
					}
				}
				else
				{
					inventory_ctr.Instance.try_give_item(item_recieve_on_kill, 1, base.transform.position, "");
				}
				if (mob_type == TYPE_T.creature)
				{
					MultiplayerControl.Instance.unlock_achievement("CgkI7aPb66UWEAIQCQ");
					if (GetComponent<creatureScript>().level >= 15)
					{
						MultiplayerControl.Instance.unlock_achievement("CgkI7aPb66UWEAIQCg");
					}
					if (GetComponent<creatureScript>().level >= 40)
					{
						MultiplayerControl.Instance.unlock_achievement("CgkI7aPb66UWEAIQDA");
					}
				}
			}
			else if (killer.gameObject.GetComponent<creatureScript>().STATE == GameController.creatureStates.friendly)
			{
				GameController.Instance.showOverheadNotif("+" + exp_recieve + " Exp", killer.transform.position, false, false);
				killer.gameObject.GetComponent<creatureScript>().friendly_inc_exp(exp_recieve);
			}
		}
		Kill();
	}

	private void FixedUpdate()
	{
		if (healthbar != null)
		{
			NewMobControl.Instance.SnapOverhead((RectTransform)healthbar.transform, base.transform.position);
			yellow_width = Mathf.Lerp(yellow_width, yellow_goto_width, Time.deltaTime * 7f);
			yellow_X = Mathf.Lerp(yellow_X, yellow_goto_X, Time.deltaTime * 7f);
			healthbarYellow.rectTransform.sizeDelta = new Vector2(yellow_width, 9.4f);
			healthbarYellow.transform.localPosition = new Vector2(yellow_X, healthbarYellow.transform.localPosition.y);
		}
	}

	public void RecieveHealthbar(GameObject healthbar, Camera mainCamera)
	{
		GameController.Instance.possible_destroy.Add(healthbar);
		this.mainCamera = mainCamera;
		this.healthbar = healthbar;
		healthbarGreen = healthbar.transform.Find("GREEN").GetComponent<Image>();
		healthbarYellow = healthbar.transform.Find("yellow").GetComponent<Image>();
		splatDmg = healthbar.transform.Find("splat").Find("dmg").GetComponent<Text>();
		healthbar.transform.parent = NewMobControl.Instance.gameObject.transform;
		healthbar.transform.SetAsFirstSibling();
		healthbar.transform.localRotation = Quaternion.identity;
		healthbar.transform.localPosition = Vector2.zero;
		healthbar.transform.localScale = Vector3.one * 0.85f;
		RectTransform obj = (RectTransform)healthbar.transform;
		Vector2 anchorMin = (((RectTransform)healthbar.transform).anchorMax = Vector2.one * 10f);
		obj.anchorMin = anchorMin;
		hit_start_pos = healthbar.transform.Find("hit").transform.localPosition;
		RefreshHealthbarDisappearTimer(5f);
	}

	public void set_heath_visual(int damage)
	{
		yellow_X = 0f - (100f - (yellow_width = HP / (float)HP_start * 100f)) / 2f;
		HP -= damage;
		yellow_goto_X = 0f - (100f - (yellow_goto_width = HP / (float)HP_start * 100f)) / 2f;
		if (healthbarGreen != null)
		{
			healthbarGreen.rectTransform.sizeDelta = new Vector2(yellow_goto_width, 9.4f);
			healthbarGreen.transform.localPosition = new Vector2(yellow_goto_X, healthbarGreen.transform.localPosition.y);
		}
	}

	private IEnumerator ANIMATE_HIT()
	{
		healthbar.transform.Find("hit").GetComponent<Image>().enabled = true;
		healthbar.transform.Find("hit").transform.localPosition = hit_start_pos + Random.insideUnitCircle * 23f;
		healthbar.transform.Find("hit").transform.localRotation = Quaternion.Euler(0f, 0f, Random.value * 360f);
		for (int i = 0; i < 5; i++)
		{
			if (healthbar != null)
			{
				healthbar.transform.Find("hit").GetComponent<Image>().sprite = GameController.Instance.hit_sprite_frames[i];
			}
			yield return new WaitForSeconds(0.07f);
		}
		if (healthbar != null)
		{
			healthbar.transform.Find("hit").GetComponent<Image>().enabled = false;
		}
	}

	public void RefreshHealthbarDisappearTimer(float timer)
	{
		if (disappearHealthbar != null)
		{
			StopCoroutine(disappearHealthbar);
		}
		disappearHealthbar = DisappearHealthbar(timer);
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(disappearHealthbar);
		}
	}

	private IEnumerator DisappearHealthbar(float timer)
	{
		yield return new WaitForSeconds(timer);
		if (HP == (float)HP_start)
		{
			remove_healthbar();
			if (GetComponent<creatureScript>() != null && base.gameObject != GameController.Instance.player && GameController.Instance.focus_npc == null)
			{
				GetComponent<creatureScript>().assign_level_display();
			}
		}
		else
		{
			RefreshHealthbarDisappearTimer(timer);
		}
	}

	private void OnEnable()
	{
		StartCoroutine(clear_perk_effects());
	}

	private void interval_effect(int i)
	{
		switch ((effect)all_effects[i]["TYPE"])
		{
		case effect.poison:
			wasHit((int)amount_poison, who_poisoned_me, false, hit_col.color_green);
			break;
		case effect.giant:
		{
			foreach (GameObject active_combatant in NewMobControl.Instance.active_combatants)
			{
				if (!(active_combatant == base.gameObject) && !(active_combatant == null) && active_combatant.GetComponent<NewCombatant>().mob_type != TYPE_T.mineral && !is_companion(active_combatant) && Vector3.Distance(base.transform.position, active_combatant.transform.position) < 3f && active_combatant.GetComponent<NewCombatant>().scale != 3f)
				{
					if (active_combatant.GetComponent<NewCombatant>().scale == 2f)
					{
						int damage = (int)((float)active_combatant.GetComponent<NewCombatant>().HP_start * 0.4f);
						active_combatant.GetComponent<NewCombatant>().on_crush();
						active_combatant.GetComponent<NewCombatant>().wasHit(damage, GetComponent<creatureScript>(), false, hit_col.color_red);
					}
					else
					{
						active_combatant.GetComponent<NewCombatant>().on_crush();
						active_combatant.GetComponent<NewCombatant>().wasHit(999, GetComponent<creatureScript>(), false, hit_col.color_red);
					}
				}
			}
			break;
		}
		}
	}

	private bool is_companion(GameObject G)
	{
		if (G.GetComponent<NewCombatant>().mob_type == TYPE_T.creature && G.GetComponent<creatureScript>().STATE == GameController.creatureStates.friendly)
		{
			return true;
		}
		return false;
	}

	private void apply_effect(effect TYPE, float amount, float M)
	{
		switch (TYPE)
		{
		case effect.sprint:
			if (amount_sprint == 0f)
			{
				GetComponent<creatureScript>().sprint = true;
			}
			amount_sprint += amount * M;
			if (Mathf.Abs(amount_sprint) < 0.1f)
			{
				amount_sprint = 0f;
			}
			if (amount_sprint == 0f)
			{
				GetComponent<creatureScript>().sprint = false;
			}
			break;
		case effect.eagleEye:
			amount_eagle += amount * M;
			if (Mathf.Abs(amount_eagle) < 0.1f)
			{
				amount_eagle = 0f;
			}
			GameController.Instance.eagle_offset = ((amount_eagle == 0f) ? 0f : (0.41f + amount_eagle * 0.07f));
			break;
		case effect.giant:
			if (amount_giant == 0f)
			{
				GetComponent<creatureScript>().isGiant = true;
				GameController.Instance.giant_offset = 0.3f;
				NewBiomeControl.Instance.SetCreatureSize(4.8f, base.gameObject);
			}
			amount_giant += amount * M;
			if (Mathf.Abs(amount_giant) < 0.1f)
			{
				amount_giant = 0f;
			}
			if (amount_giant == 0f)
			{
				GetComponent<creatureScript>().isGiant = false;
				GameController.Instance.giant_offset = 0f;
				NewBiomeControl.Instance.SetCreatureSize(1f, base.gameObject);
			}
			break;
		case effect.poison:
			if (amount_poison == 0f)
			{
				vis_poison = Object.Instantiate(perk_controller.Instance.glob_screech);
				vis_poison.transform.parent = base.transform;
				vis_poison.transform.localPosition = Vector3.zero;
				vis_poison.transform.position = new Vector3(vis_poison.transform.position.x, 0.6f, vis_poison.transform.position.z);
			}
			amount_poison += amount * M;
			if (Mathf.Abs(amount_poison) < 0.1f)
			{
				amount_poison = 0f;
			}
			if (amount_poison == 0f)
			{
				Object.Destroy(vis_poison);
			}
			break;
		case effect.accuracy:
			if (amount_dec_acc == 0f)
			{
				vis_screechD = Object.Instantiate(perk_controller.Instance.glob_screech);
				vis_screechD.transform.parent = base.transform;
				vis_screechD.transform.localPosition = Vector3.zero;
				vis_screechD.transform.position = new Vector3(vis_screechD.transform.position.x, 0.6f, vis_screechD.transform.position.z);
			}
			amount_dec_acc += amount * M;
			if (Mathf.Abs(amount_dec_acc) < 0.1f)
			{
				amount_dec_acc = 0f;
			}
			if (amount_dec_acc == 0f)
			{
				Object.Destroy(vis_screechD);
			}
			break;
		case effect.out_dmg:
			if (amount_dec_dmg == 0f)
			{
				vis_weakenD = Object.Instantiate(perk_controller.Instance.glob_screech);
				vis_weakenD.transform.parent = base.transform;
				vis_weakenD.transform.localPosition = Vector3.zero;
				vis_weakenD.transform.position = new Vector3(vis_weakenD.transform.position.x, 0.6f, vis_weakenD.transform.position.z);
			}
			amount_dec_dmg += amount * M;
			if (Mathf.Abs(amount_dec_dmg) < 0.1f)
			{
				amount_dec_dmg = 0f;
			}
			if (amount_dec_dmg == 0f)
			{
				Object.Destroy(vis_weakenD);
			}
			break;
		case effect.bind:
			if (amount_bind == 0f)
			{
				vis_webD = Object.Instantiate(perk_controller.Instance.glob_web);
				vis_webD.transform.parent = base.transform;
				vis_webD.transform.localPosition = Vector3.zero;
				vis_webD.transform.transform.position = new Vector3(vis_webD.transform.transform.position.x, 0.6f, vis_webD.transform.transform.position.z);
			}
			amount_bind += amount * M;
			if (Mathf.Abs(amount_bind) < 0.1f)
			{
				amount_bind = 0f;
			}
			if (amount_bind == 0f)
			{
				Object.Destroy(vis_webD);
			}
			break;
		}
	}

	public void add_effect(int duration, effect TYPE, float amount)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("duration", duration);
		dictionary.Add("TYPE", TYPE);
		dictionary.Add("amount", amount);
		apply_effect(TYPE, amount, 1f);
		all_effects.Add(dictionary);
	}

	private IEnumerator clear_perk_effects()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			if (GameController.Instance.pause)
			{
				continue;
			}
			List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
			for (int i = 0; i < all_effects.Count; i++)
			{
				int num = Mathf.Max(0, (int)all_effects[i]["duration"] - 1);
				interval_effect(i);
				if (num == 0)
				{
					apply_effect((effect)all_effects[i]["TYPE"], (float)all_effects[i]["amount"], -1f);
					list.Add(all_effects[i]);
				}
				else
				{
					all_effects[i]["duration"] = num;
				}
			}
			foreach (Dictionary<string, object> item in list)
			{
				all_effects.Remove(item);
			}
		}
	}
}
