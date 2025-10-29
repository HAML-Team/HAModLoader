using UnityEngine;
using UnityEngine.UI;

public class crafting_slot : MonoBehaviour
{
	public int index;

	public Text TITLE;

	public Text description;

	public Text costA_txt;

	public Text costB_txt;

	public Image RESULT_img;

	public Image reqA_img;

	public Image reqB_img;

	public GameObject locked_icon;

	public Color locked_col;

	public GameObject buy_ico;

	public Text buy_cost;

	private bool allowed;

	public void onclick()
	{
		inventory_ctr.Instance.pressed_crafting_button(index, allowed);
	}

	public void set_graphic(inventory_ctr.new_inv_item item, bool sale)
	{
		bool flag = true;
		if (item.crafting_IAP_key_required != "" && PlayerData.Instance.GetGlobalInt(item.crafting_IAP_key_required) != 1)
		{
			flag = false;
		}
		allowed = flag;
		if (item.overwrite_name == "")
		{
			TITLE.text = item.name;
		}
		else
		{
			TITLE.text = item.overwrite_name;
		}
		description.text = item.crafting_desc;
		RESULT_img.sprite = item.inventory_sprite;
		RESULT_img.color = (flag ? Color.white : locked_col);
		locked_icon.SetActive(!flag);
		if (sale)
		{
			reqA_img.gameObject.SetActive(false);
			reqB_img.gameObject.SetActive(false);
			costA_txt.gameObject.SetActive(false);
			costB_txt.gameObject.SetActive(false);
			description.rectTransform.sizeDelta = new Vector2(240f, 236f);
			description.transform.localPosition = new Vector2(-1f, -100f);
			buy_ico.SetActive(true);
			buy_cost.gameObject.SetActive(true);
			buy_ico.GetComponent<Image>().sprite = inventory_ctr.Instance.get_coin_sprite(item.market_cost);
			buy_cost.text = string.Concat(item.market_cost);
			if (item.market_cost < 10)
			{
				buy_cost.fontSize = 50;
				buy_cost.transform.localPosition = new Vector2(42.4f, buy_cost.transform.localPosition.y);
				buy_ico.transform.localPosition = new Vector2(-30f, buy_ico.transform.localPosition.y);
			}
			else if (item.market_cost < 100)
			{
				buy_cost.fontSize = 48;
				buy_cost.transform.localPosition = new Vector2(35.1f, buy_cost.transform.localPosition.y);
				buy_ico.transform.localPosition = new Vector2(-33.8f, buy_ico.transform.localPosition.y);
			}
			else if (item.market_cost < 1000)
			{
				buy_cost.fontSize = 46;
				buy_cost.transform.localPosition = new Vector2(24.1f, buy_cost.transform.localPosition.y);
				buy_ico.transform.localPosition = new Vector2(-45.8f, buy_ico.transform.localPosition.y);
			}
			else if (item.market_cost < 10000)
			{
				buy_cost.fontSize = 45;
				buy_cost.transform.localPosition = new Vector2(15.3f, buy_cost.transform.localPosition.y);
				buy_ico.transform.localPosition = new Vector2(-54f, buy_ico.transform.localPosition.y);
			}
			else
			{
				buy_cost.fontSize = 43;
				buy_cost.transform.localPosition = new Vector2(5.9f, buy_cost.transform.localPosition.y);
				buy_ico.transform.localPosition = new Vector2(-64.2f, buy_ico.transform.localPosition.y);
			}
		}
		else
		{
			reqA_img.gameObject.SetActive(true);
			reqB_img.gameObject.SetActive(true);
			costA_txt.gameObject.SetActive(true);
			costB_txt.gameObject.SetActive(true);
			description.rectTransform.sizeDelta = new Vector2(240f, 140f);
			description.transform.localPosition = new Vector2(-1f, -52f);
			buy_ico.SetActive(false);
			buy_cost.gameObject.SetActive(false);
			inventory_ctr.new_inv_item new_inv_item = inventory_ctr.Instance.new_inv_items_by_name[item.crafting_ingredientA];
			reqA_img.sprite = new_inv_item.inventory_sprite;
			costA_txt.text = "x" + item.crafting_ingredientA_cnt;
			if (item.crafting_ingredientB != "")
			{
				inventory_ctr.new_inv_item new_inv_item2 = inventory_ctr.Instance.new_inv_items_by_name[item.crafting_ingredientB];
				reqB_img.sprite = new_inv_item2.inventory_sprite;
				costB_txt.text = "x" + item.crafting_ingredientB_cnt;
				reqB_img.gameObject.SetActive(true);
			}
			else
			{
				reqB_img.gameObject.SetActive(false);
				costB_txt.text = "";
			}
		}
	}
}
