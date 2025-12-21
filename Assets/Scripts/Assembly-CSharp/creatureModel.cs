using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class creatureModel : MonoBehaviour
{
	public GameObject[] children_;

	public Color myCol;

	public float seed;

	public string namePrefix = "Untitled";

	public string nameSuffix = "Untitled";

	public string myName = "Shark";

	public limb_scr eyeObject_;

	public Texture mouthObject_open;

	public Texture mouthObject_blink;

	public List<string> creatures_that_made_me = new List<string>();

	public bool height_set;

	public float height;

	public int bonus_stat;

	public GameObject hand_obj;

	public GameObject head_obj;

	public GameObject body_obj;

	public GameObject hat;

	public GameObject armor_body;

	public GameObject obj_holding;

	private IEnumerator blnk;

	public void equip_hold_item(GameObject to_hold)
	{
		if (obj_holding != null)
		{
			Object.Destroy(obj_holding);
		}
		if (to_hold != null)
		{
			obj_holding = Object.Instantiate(to_hold);
		}
	}

	private void OnDestroy()
	{
		if (obj_holding != null)
		{
			Object.Destroy(obj_holding);
		}
	}

	private void FixedUpdate()
	{
		if (obj_holding != null)
		{
			obj_holding.transform.position = hand_obj.transform.position;
			obj_holding.transform.rotation = base.transform.rotation;
		}
	}

	public void Init()
	{
		if (blnk != null)
		{
			StopCoroutine(blnk);
		}
		blnk = creatureBlink();
		StartCoroutine(blnk);
	}

	public void CloneAdjustHeight(creatureModel cloned)
	{
		StartCoroutine(OnClone_adjustHeight(cloned));
	}

	private IEnumerator OnClone_adjustHeight(creatureModel cloned)
	{
		yield return new WaitForSeconds(0.02f);
		if (!(cloned == null))
		{
			height = cloned.height;
			base.transform.localPosition = cloned.transform.localPosition;
		}
	}

	private IEnumerator creatureBlink()
	{
		while (true)
		{
			blink(true);
			yield return new WaitForSeconds(1.5f);
			blink(false);
			yield return new WaitForSeconds(0.1f);
			blink(true);
			yield return new WaitForSeconds(0.5f);
			blink(false);
			yield return new WaitForSeconds(0.07f);
			blink(true);
			yield return new WaitForSeconds(2.2f + Random.Range(-0.2f, 0.2f));
			blink(false);
			yield return new WaitForSeconds(0.1f);
			blink(true);
			yield return new WaitForSeconds(0.1f);
			blink(false);
			yield return new WaitForSeconds(0.1f);
		}
	}

	private void blink(bool open)
	{
		if (eyeObject_ != null)
		{
			eyeObject_.visualPlane.GetComponent<Renderer>().enabled = open;
			if (eyeObject_.dummy != null)
			{
				eyeObject_.dummy.visPlane.GetComponent<Renderer>().enabled = open;
			}
		}
	}
}
