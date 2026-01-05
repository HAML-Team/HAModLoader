using UnityEngine;
using UnityEngine.UI;

public class ModLoaderUIController : MonoBehaviour
{
    public Transform content;
    public GameObject modRowPrefab;
    public GameObject modcontrol_elements;

    public bool GetIsOpen()
    {
        return modcontrol_elements.activeSelf;
    }

    private void OnEnable()
    {
        NewMenuController.OnRequestModMenuOpen += Open;
        NewMenuController.OnRequestModMenuClose += Close;
        NewMenuController.IsModMenuOpenCheck = GetIsOpen;
    }

    private void OnDisable()
    {
        NewMenuController.OnRequestModMenuOpen -= Open;
        NewMenuController.OnRequestModMenuClose -= Close;
        NewMenuController.IsModMenuOpenCheck -= GetIsOpen;
    }

    public void Open()
    {
        modcontrol_elements.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        modcontrol_elements.SetActive(false);
    }

    void Refresh()
    {
        foreach (Transform c in content)
            Destroy(c.gameObject);

        foreach (var mod in ModRegistry.LoadedMods)
        {
            var row = Instantiate(modRowPrefab, content);
            row.transform.Find("RowLayout").Find("TextContainer").Find("ModNameText").GetComponent<Text>().text = mod.ModName;
            row.transform.Find("RowLayout").Find("TextContainer").Find("ModNameText").GetComponent<Text>().text += " v" + mod.ModVersion;
            row.transform.Find("RowLayout").Find("TextContainer").Find("ModAuthorText").GetComponent<Text>().text = mod.ModAuthor;
            row.transform.Find("RowLayout").Find("ModLogo").GetComponent<Image>().sprite = mod.ModLogo;
            row.transform.Find("RowLayout").Find("TextContainer").Find("ModDescriptionShortText").GetComponent<Text>().text = mod.ModDescriptionShort;
            var toggle = row.transform.Find("RowLayout").Find("ExtraContainer").Find("Toggle").GetComponent<Toggle>();
            toggle.isOn = ModConfig.IsModEnabled(mod.ModName);
            toggle.onValueChanged.AddListener((value) =>
            {
                ModConfig.SetModEnabled(mod.ModName, value);
            });

            row.transform.Find("RowLayout").Find("TextContainer").Find("ModNameText").GetComponent<Text>().enabled = true;
            row.transform.GetComponent<Image>().enabled = true;
        }
    }
}
