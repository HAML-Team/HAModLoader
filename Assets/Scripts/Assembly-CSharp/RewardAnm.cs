using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardAnm : MonoBehaviour
{
	private enum reward_type
	{
		gems = 0,
		coins = 1,
		creature = 2,
		item = 3
	}

	public static RewardAnm Instance;

	public Text text_reward;

	private GameObject curr_reward_model;

	private reward_type prev_reward;

	private int reward_count;

	private int reward_creature_id;

	private string reward_item_name;

	public GameObject prefab_rewardmodel_coins;

	public GameObject prefab_rewardmodel_gems;

	public GameObject reward_model_parent;

	private void Awake()
	{
		Instance = this;
	}

	public void PlayMysteryBoxAppear()
	{
		NewAudioControl.Instance.Play(NewAudioControl.Instance.sfx_chest_whoosh);
		GetComponent<Animation>().Play("reward-appear");
	}

	public void show_okay_button()
	{
		PopupControl.Instance.okay_button.SetActive(true);
	}

	public void press_reward_okay()
	{
		if (prev_reward == reward_type.gems)
		{
			Shop_positioner.Instance.Blob_gem_count();
		}
	}

	public void ClickOpen()
	{
		NewAudioControl.Instance.Play(NewAudioControl.Instance.sfx_unlock_chest);
		GetComponent<Animation>().Stop();
		GetComponent<Animation>().Play("reward-open");
		if (curr_reward_model != null)
		{
			Object.Destroy(curr_reward_model);
		}
		Vector3 zero = Vector3.zero;
		Vector3 localScale = Vector3.one;
		Quaternion localRotation = Quaternion.identity;
		reward_type reward_type = TryRollGems();
		switch (reward_type)
		{
		case reward_type.coins:
			text_reward.text = "<color=#ffffff>You recieved: </color>" + reward_count + " Coins!";
			inventory_ctr.Instance.try_give_item("Coins", reward_count, GameController.Instance.player.transform.position, "", false);
			curr_reward_model = Object.Instantiate(prefab_rewardmodel_coins);
			break;
		case reward_type.gems:
			text_reward.text = "<color=#ffffff>You recieved: </color>" + reward_count + " Gems!";
			PlayerData.Instance.SetGlobalInt("GEMS", PlayerData.Instance.GetGlobalInt("GEMS") + reward_count);
			curr_reward_model = Object.Instantiate(prefab_rewardmodel_gems);
			break;
		case reward_type.creature:
			text_reward.text = "<color=#ffffff>You recieved creature: </color>" + NewBreedControl.first_upper(Loader.Instance.temp_static_names[reward_creature_id]) + "!";
			curr_reward_model = Loader.Instance.GetSingleAnimal(Loader.Instance.temp_static_names[reward_creature_id]);
			curr_reward_model.AddComponent<Spin>();
			curr_reward_model.GetComponent<Spin>().scale = 27f;
			localScale = Vector3.one * 56f;
			localRotation = Quaternion.Euler(0f, 130f, 0f);
			break;
		case reward_type.item:
		{
			text_reward.text = "<color=#ffffff>You recieved item: </color>" + reward_item_name + "!";
			inventory_ctr.Instance.try_give_item(reward_item_name, reward_count, GameController.Instance.player.transform.position, "", false);
			curr_reward_model = new GameObject();
			curr_reward_model.AddComponent<Spin>();
			curr_reward_model.GetComponent<Spin>().scale = 27f;
			inventory_ctr.new_inv_item new_inv_item = inventory_ctr.Instance.new_inv_items_by_name[reward_item_name];
			GameObject gameObject = Object.Instantiate(new_inv_item.world_obj);
			gameObject.transform.parent = curr_reward_model.transform;
			if (new_inv_item.type == inventory_ctr.inv_type_t.holdable)
			{
				gameObject.transform.localPosition = new Vector3(0f, -20f, -17f);
				gameObject.transform.localScale = Vector3.one * 65f;
				gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
			else if (new_inv_item.type == inventory_ctr.inv_type_t.helmet)
			{
				gameObject.transform.localPosition = new Vector3(-0.38f, -7.9f, 1.31f);
				gameObject.transform.localScale = new Vector3(14.9f, 14.9f, 14.9f);
				gameObject.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
			}
			else if (new_inv_item.type == inventory_ctr.inv_type_t.armor)
			{
				gameObject.transform.localPosition = new Vector3(0f, -0.73f, -0.32f);
				gameObject.transform.localScale = new Vector3(13.36f, 11.24f, 23.138f);
				gameObject.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
			}
			break;
		}
		}
		prev_reward = reward_type;
		curr_reward_model.transform.parent = reward_model_parent.transform;
		curr_reward_model.transform.localScale = localScale;
		curr_reward_model.transform.localPosition = zero;
		curr_reward_model.transform.localRotation = localRotation;
	}

	private reward_type TryRollGems()
	{
		bool flag = PlayerData.Instance.GetGlobalInt("n_free_gems_given") == 0;
		if (PlayerData.Instance.GetGlobalInt("GEMS") <= 12)
		{
			float iTER = (int)((float)PlayerData.Instance.GetGlobalInt("n_free_gems_given") / 12f);
			float num = chances_gem(PlayerData.Instance.GetGlobalInt("GEMS"), iTER);
			if (Random.value < num)
			{
				if (Random.value < 0.25f || flag)
				{
					reward_count = 2;
				}
				else
				{
					reward_count = 1;
				}
				PlayerData.Instance.SetGlobalInt("n_free_gems_given", PlayerData.Instance.GetGlobalInt("n_free_gems_given") + reward_count);
				return reward_type.gems;
			}
			return TryRollCreature();
		}
		return TryRollCreature();
	}

	private reward_type TryRollCreature()
	{
		if (PlayerData.Instance.GetGlobalInt("n_free_creatures") < 5)
		{
			if (Random.value < 0.3f)
			{
				List<int> list = new List<int>();
				for (int i = 0; i < PlayerData.Instance.GetGlobalInt("n_free_creatures"); i++)
				{
					list.Add(PlayerData.Instance.GetGlobalInt("free_creature_" + i));
				}
				List<int> list2 = new List<int>();
				for (int j = 57; j <= 91; j++)
				{
					if (!list.Contains(j))
					{
						list2.Add(j);
					}
				}
				reward_creature_id = list2[Random.Range(0, list2.Count)];
				PlayerData.Instance.SetGlobalInt("free_creature_" + PlayerData.Instance.GetGlobalInt("n_free_creatures"), reward_creature_id);
				PlayerData.Instance.SetGlobalInt("n_free_creatures", PlayerData.Instance.GetGlobalInt("n_free_creatures") + 1);
				return reward_type.creature;
			}
			return TryRollItems();
		}
		return TryRollItems();
	}

	private reward_type TryRollItems()
	{
		if (Random.value < 0.06f)
		{
			reward_count = 1;
			int num = Random.Range(0, inventory_ctr.Instance.MysteryBoxRewards.Length);
			reward_item_name = inventory_ctr.Instance.MysteryBoxRewards[num];
			return reward_type.item;
		}
		reward_count = Random.Range(50, 104);
		return reward_type.coins;
	}

	private float chances_gem(float n_gems_curr, float ITER)
	{
		float num = 14f;
		float num2 = 0.7f - ITER * 0.3f;
		if (num2 <= 0f)
		{
			return 0f;
		}
		return num2 + (0f - num2 / num) * n_gems_curr;
	}
}
