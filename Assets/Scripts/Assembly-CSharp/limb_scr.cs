using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class limb_scr : MonoBehaviour
{
	public limb_dummy_scr dummy;

	public bool is_rigid = true;

	public float spaghettiFactor_;

	public string textureName;

	public List<GameObject> children_;

	public bool symmetrical;

	public string compName;

	public bool inherit;

	public GameObject parent_;

	public GameObject visual;

	public GameObject visualPlane;

	public bool isDecorative;

	public float[] hsv_values;

	public List<Quaternion[]> frames_rotations;

	public List<Vector3[]> frames_snapPositions;

	public AnimationCurve animPosx;

	public AnimationCurve animPosy;

	public AnimationCurve animPosz;

	public AnimationCurve animRotx;

	public AnimationCurve animRoty;

	public AnimationCurve animRotz;

	public AnimationCurve animRotw;

	public bool invertSymmAnm;

	public float speed;

	public bool disableInversion;

	public int animLength;

	public bool animationPlaying;

	public float started_anm_time;

	public bool animation_complete;

	public Transform snap_par;

	public Vector3 snap_global;

	public void Init()
	{
		StartCoroutine(loosenLimbs());
	}

	public void create_snap(Transform par)
	{
		snap_par = par;
	}

	public void set_snap_local(Vector3 input)
	{
		if (snap_par == null)
		{
			snap_global = input;
		}
		else
		{
			snap_global = snap_par.TransformPoint(input);
		}
	}

	public Vector3 get_snap_local()
	{
		if (snap_global == Vector3.zero)
		{
			return Vector3.zero;
		}
		if (snap_par == null)
		{
			return snap_global;
		}
		return snap_par.InverseTransformPoint(snap_global);
	}

	public void setupCollider(GameObject collidr)
	{
		collidr.GetComponent<BoxCollider>().enabled = true;
		collidr.transform.parent = GetComponent<limb_scr>().visual.transform;
		collidr.transform.localPosition = Vector3.zero;
		collidr.transform.localRotation = Quaternion.identity;
		collidr.transform.localScale = new Vector3(2f, 2f, 2f);
	}

	private IEnumerator loosenLimbs()
	{
		yield return new WaitForSeconds(0.1f);
		is_rigid = false;
	}

	public void setDecorative()
	{
		setDecorative(isDecorative);
		if (dummy != null)
		{
			dummy.SetDecorative();
		}
	}

	public void setDecorative(bool state)
	{
		isDecorative = state;
		if (state)
		{
			visual.GetComponent<MeshRenderer>().enabled = false;
			return;
		}
		visual.GetComponent<MeshRenderer>().enabled = true;
		Object.Destroy(visualPlane);
	}

	public void setupFrames(bool newCube)
	{
		frames_rotations = new List<Quaternion[]>();
		frames_snapPositions = new List<Vector3[]>();
		for (int i = 0; i < Loader.Instance.animationNames.Length; i++)
		{
			frames_rotations.Add(new Quaternion[Loader.Instance.maxFrames[i]]);
			frames_snapPositions.Add(new Vector3[Loader.Instance.maxFrames[i]]);
		}
		for (int j = 0; j < frames_rotations.Count; j++)
		{
			for (int k = 0; k < frames_rotations[j].Length; k++)
			{
				frames_rotations[j][k] = Quaternion.identity;
				frames_snapPositions[j][k] = Vector3.zero;
			}
		}
		hsv_values = new float[4];
	}

	public void makeDummy(bool first)
	{
		dummy = Object.Instantiate(Loader.Instance.type_limb_dummy).GetComponent<limb_dummy_scr>();
		dummy.Init();
		dummy.gameObject.transform.parent = base.transform.parent;
		dummy.original = this;
		dummy.vis.GetComponent<Renderer>().material.color = visual.GetComponent<Renderer>().material.color;
		if (isDecorative)
		{
			dummy.vis.GetComponent<MeshRenderer>().enabled = false;
			dummy.visPlane.GetComponent<Renderer>().material.mainTexture = visualPlane.GetComponent<Renderer>().material.mainTexture;
		}
		else
		{
			dummy.vis.GetComponent<Renderer>().material.mainTexture = visual.GetComponent<Renderer>().material.mainTexture;
			dummy.vis.GetComponent<MeshRenderer>().enabled = true;
			Object.Destroy(dummy.visPlane);
		}
		dummy.GetComponent<limb_dummy_scr>().isDecorative = isDecorative;
		if (first)
		{
			dummy.parent_dummy_ = parent_;
			dummy.realParent = true;
		}
		else
		{
			dummy.parent_dummy_ = parent_.GetComponent<limb_scr>().dummy.gameObject;
			dummy.realParent = false;
		}
		dummy.vis.transform.localScale = visual.transform.localScale;
	}

	public void chain_makeDummy(bool first)
	{
		makeDummy(first);
		for (int i = 0; i < children_.Count; i++)
		{
			children_[i].GetComponent<limb_scr>().chain_makeDummy(false);
		}
	}

	public void createandplayAnimation(Vector3[] position, Quaternion[] rotation, float speed_, bool inversion, float spaghetti_)
	{
		started_anm_time = Time.time;
		speed = speed_;
		spaghettiFactor_ = spaghetti_;
		if (dummy != null)
		{
			dummy.spaghettiFactor_ = spaghetti_;
		}
		disableInversion = inversion;
		animLength = position.Length;
		Keyframe[] array = new Keyframe[animLength + 1];
		Keyframe[] array2 = new Keyframe[animLength + 1];
		Keyframe[] array3 = new Keyframe[animLength + 1];
		Keyframe[] array4 = new Keyframe[animLength + 1];
		Keyframe[] array5 = new Keyframe[animLength + 1];
		Keyframe[] array6 = new Keyframe[animLength + 1];
		Keyframe[] array7 = new Keyframe[animLength + 1];
		for (int i = 0; i < animLength; i++)
		{
			array[i] = new Keyframe((float)i * speed, position[i].x);
			array2[i] = new Keyframe((float)i * speed, position[i].y);
			array3[i] = new Keyframe((float)i * speed, position[i].z);
			array4[i] = new Keyframe((float)i * speed, rotation[i].x);
			array5[i] = new Keyframe((float)i * speed, rotation[i].y);
			array6[i] = new Keyframe((float)i * speed, rotation[i].z);
			array7[i] = new Keyframe((float)i * speed, rotation[i].w);
		}
		array[animLength] = new Keyframe((float)animLength * speed, position[0].x);
		array2[animLength] = new Keyframe((float)animLength * speed, position[0].y);
		array3[animLength] = new Keyframe((float)animLength * speed, position[0].z);
		array4[animLength] = new Keyframe((float)animLength * speed, rotation[0].x);
		array5[animLength] = new Keyframe((float)animLength * speed, rotation[0].y);
		array6[animLength] = new Keyframe((float)animLength * speed, rotation[0].z);
		array7[animLength] = new Keyframe((float)animLength * speed, rotation[0].w);
		animPosx = new AnimationCurve(array);
		animPosy = new AnimationCurve(array2);
		animPosz = new AnimationCurve(array3);
		animRotx = new AnimationCurve(array4);
		animRoty = new AnimationCurve(array5);
		animRotz = new AnimationCurve(array6);
		animRotw = new AnimationCurve(array7);
		animationPlaying = true;
		animation_complete = false;
	}

	public Vector3 evalCurve_pos(bool offset)
	{
		float num = ((!offset) ? 0f : ((float)(animLength / 2) * speed));
		if (disableInversion)
		{
			num = 0f;
		}
		float num2 = Time.time - started_anm_time;
		return new Vector3(animPosx.Evaluate((num2 + num) % (speed * (float)animLength)), animPosy.Evaluate((num2 + num) % (speed * (float)animLength)), animPosz.Evaluate((num2 + num) % (speed * (float)animLength)));
	}

	public Quaternion evalCurve_rot(bool offset)
	{
		float num = ((!offset) ? 0f : ((float)(animLength / 2) * speed));
		if (disableInversion)
		{
			num = 0f;
		}
		float num2 = Time.time - started_anm_time;
		return new Quaternion(animRotx.Evaluate((num2 + num) % (speed * (float)animLength)), animRoty.Evaluate((num2 + num) % (speed * (float)animLength)), animRotz.Evaluate((num2 + num) % (speed * (float)animLength)), animRotw.Evaluate((num2 + num) % (speed * (float)animLength)));
	}

	private void FixedUpdate()
	{
		float num = ((!is_rigid) ? spaghettiFactor_ : float.MaxValue);
		Quaternion identity = Quaternion.identity;
		if (animationPlaying)
		{
			if (parent_ != null)
			{
				set_snap_local(evalCurve_pos(false));
			}
			else
			{
				visual.transform.localPosition = evalCurve_pos(false);
			}
			identity = evalCurve_rot(false);
			if (Time.time - started_anm_time > speed * (float)animLength)
			{
				animation_complete = true;
			}
		}
		else
		{
			set_snap_local(frames_snapPositions[0][0]);
			identity = frames_rotations[0][0];
		}
		if (snap_global != Vector3.zero)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, snap_global, Time.deltaTime * num);
		}
		if (inherit)
		{
			base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, parent_.transform.localRotation * identity, Time.deltaTime * num);
		}
		else
		{
			base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, identity, Time.deltaTime * num);
		}
	}
}
