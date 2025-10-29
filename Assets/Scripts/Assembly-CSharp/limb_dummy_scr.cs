using System.Collections;
using UnityEngine;

public class limb_dummy_scr : MonoBehaviour
{
	public limb_scr original;

	public GameObject vis;

	public GameObject visPlane;

	public bool realParent;

	public GameObject parent_dummy_;

	public bool is_rigid = true;

	public float spaghettiFactor_;

	public bool isDecorative;

	public void Init()
	{
		StartCoroutine(loosenLimbs());
	}

	private IEnumerator loosenLimbs()
	{
		yield return new WaitForSeconds(0.1f);
		is_rigid = false;
	}

	public void SetDecorative()
	{
		if (!isDecorative)
		{
			Object.Destroy(visPlane);
		}
	}

	private void FixedUpdate()
	{
		float num = ((!is_rigid) ? spaghettiFactor_ : float.MaxValue);
		vis.transform.localPosition = new Vector3(0f - original.visual.transform.localPosition.x, original.visual.transform.localPosition.y, original.visual.transform.localPosition.z);
		Vector3 zero = Vector3.zero;
		Quaternion identity = Quaternion.identity;
		if (original.animationPlaying)
		{
			zero = original.evalCurve_pos(original.invertSymmAnm);
			identity = original.evalCurve_rot(original.invertSymmAnm);
		}
		else
		{
			zero = original.frames_snapPositions[0][0];
			identity = original.frames_rotations[0][0];
		}
		Vector3 localScale = original.parent_.GetComponent<limb_scr>().visual.transform.localScale;
		Vector3 position;
		Quaternion rotation;
		if (realParent)
		{
			position = original.parent_.GetComponent<limb_scr>().visual.transform.position;
			rotation = original.parent_.transform.rotation;
		}
		else
		{
			position = parent_dummy_.GetComponent<limb_dummy_scr>().vis.transform.position;
			rotation = parent_dummy_.transform.rotation;
		}
		base.transform.position = Vector3.Lerp(base.transform.position, position + Vector3.Scale(rotation * Vector3.Scale(new Vector3(0f - zero.x, zero.y, zero.z), localScale), base.transform.lossyScale), Time.deltaTime * num);
		Quaternion quaternion = new Quaternion(0f - identity.x, identity.y, identity.z, 0f - identity.w);
		if (!original.inherit)
		{
			base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, quaternion, Time.deltaTime * num);
		}
		else
		{
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, rotation * quaternion, Time.deltaTime * num);
		}
	}
}
