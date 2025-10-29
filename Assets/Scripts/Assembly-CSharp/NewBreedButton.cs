using UnityEngine;

public class NewBreedButton : MonoBehaviour
{
	public int index;

	public void Click()
	{
		NewBreedControl.Instance.ClickBreedButton(base.gameObject);
	}
}
