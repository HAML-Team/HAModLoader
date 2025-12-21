using UnityEngine;
using UnityEngine.EventSystems;

public class inv_slot_scr : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	public int index;

	public void OnPointerDown(PointerEventData eventData)
	{
		inventory_ctr.Instance.slot_mouse_down(index, base.gameObject);
	}
}
