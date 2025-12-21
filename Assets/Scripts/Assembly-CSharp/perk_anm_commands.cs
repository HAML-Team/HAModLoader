using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class perk_anm_commands : MonoBehaviour
{
	public perk_controller ctr;

	public Image perk_img;

	public Text title_text;

	public Text desc_text;

	public Text attr_text;

	public GameObject perkget_okay_button;

	private string key_last;

	private IEnumerator CYCLE;

	public void stop_cycling_image()
	{
		StopCoroutine(CYCLE);
		perk_controller.Instance.sfx_source.PlayOneShot(perk_controller.Instance.resolve);
		perk_img.sprite = (Sprite)((Dictionary<string, object>)ctr.perk_defs[key_last])["Spr"];
		int slotInt = PlayerData.Instance.GetSlotInt(key_last, PlayerData.grouping_t.perks);
		title_text.text = (string)((Dictionary<string, object>)ctr.perk_defs[key_last])["NAME"] + ((slotInt + 1 == 1) ? "" : (" - Level " + (slotInt + 1)));
		attr_text.text = ctr.get_perk_string(key_last, slotInt + 1, true);
		desc_text.text = (string)((Dictionary<string, object>)ctr.perk_defs[key_last])["desc"];
		MultiplayerControl.Instance.unlock_achievement("CgkI7aPb66UWEAIQBA");
		perkget_okay_button.SetActive(true);
	}

	public void perk_get_CLOSE()
	{
		ctr.perk_GET_screen.SetActive(false);
		perkget_okay_button.SetActive(false);
		GameController.Instance.LOCK_LEVEL_SCREEN = false;
		perk_controller.Instance.increment_perk(key_last, perk_img.sprite);
		perk_controller.Instance.try_achievement();
	}

	public void start_cycling_image()
	{
		CYCLE = cycle_img();
		StartCoroutine(CYCLE);
	}

	private IEnumerator cycle_img()
	{
		int total_perks = ctr.perk_defs.Count;
		List<string> keyList = new List<string>(ctr.perk_defs.Keys);
		int lastIndex = -1;
		while (true)
		{
			int num = Random.Range(0, total_perks);
			if (num == lastIndex)
			{
				num = (lastIndex + 1) % total_perks;
			}
			lastIndex = num;
			key_last = keyList[num];
			perk_img.sprite = (Sprite)((Dictionary<string, object>)ctr.perk_defs[key_last])["Spr"];
			yield return new WaitForSeconds(0.05f);
		}
	}
}
