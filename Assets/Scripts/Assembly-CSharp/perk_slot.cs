using UnityEngine;
using UnityEngine.UI;

public class perk_slot : MonoBehaviour
{
	public int INDEX;

	public void on_mouse_down()
	{
		base.transform.SetAsLastSibling();
		GetComponent<Animation>().Play();
		GetComponent<Image>().raycastTarget = false;
		perk_controller.Instance.perk_clicked(INDEX);
	}
}
