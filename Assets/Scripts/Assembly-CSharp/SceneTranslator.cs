using UnityEngine;
using UnityEngine.UI;

public class SceneTranslator : MonoBehaviour
{
	public Text[] to_translate;

	private void Start()
	{
		Text[] array = to_translate;
		foreach (Text text in array)
		{
			text.text = TranslationControl.Instance.Translate(text.text);
		}
	}
}
