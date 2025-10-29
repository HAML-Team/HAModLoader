using UnityEngine;

public class Spin : MonoBehaviour
{
	public float scale;

	private float initY;

	private void Start()
	{
		initY = base.transform.localPosition.y;
		if (scale == 0f)
		{
			scale = 1f;
		}
	}

	private void FixedUpdate()
	{
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, initY + Mathf.Sin(Time.time * 3f) * 0.1f * scale, base.transform.localPosition.z);
		base.transform.Rotate(Vector3.up, 0.5f);
	}
}
