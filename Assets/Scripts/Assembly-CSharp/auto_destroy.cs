using System.Collections;
using UnityEngine;

public class auto_destroy : MonoBehaviour
{
	public int delay;

	private void Start()
	{
		StartCoroutine(destroy_self());
	}

	private IEnumerator destroy_self()
	{
		yield return new WaitForSeconds(delay);
		Object.Destroy(base.gameObject);
	}
}
