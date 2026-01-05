using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject authorGO;

    void Awake()
    {
        var t = transform.parent.Find("ModAuthorText");
        if (t == null)
        {
            enabled = false;
            return;
        }
        authorGO = t.gameObject;
        authorGO.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        authorGO.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        authorGO.SetActive(false);
    }
}
