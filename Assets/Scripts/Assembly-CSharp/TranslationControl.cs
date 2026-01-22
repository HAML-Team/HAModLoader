using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class TranslationControl : MonoBehaviour
{
	public enum languages
	{
		English = 0,
		Russian = 1,
		Portugese = 2,
		Thai = 3,
		Polish = 4,
		Spanish = 5
	}

	public static TranslationControl Instance;

	public delegate bool TryGetTranslation(string key, out string result);
	public static TryGetTranslation ApiTranslationHook;

	public languages test_language;

	public Dictionary<string, string> translations = new Dictionary<string, string>();

	private languages use_language;

	public languages CurrentLanguage => use_language;

	private void Awake()
	{
		if (!(Instance == null))
		{
			return;
		}
		Instance = this;
		if (Application.isEditor)
		{
			use_language = test_language;
		}
		else
		{
			switch (Application.systemLanguage)
			{
			case SystemLanguage.Russian:
				use_language = languages.Russian;
				break;
			case SystemLanguage.Portuguese:
				use_language = languages.Portugese;
				break;
			case SystemLanguage.Thai:
				use_language = languages.Thai;
				break;
			case SystemLanguage.Polish:
				use_language = languages.Polish;
				break;
			case SystemLanguage.Spanish:
				use_language = languages.Spanish;
				break;
			default:
				use_language = languages.English;
				break;
			}
		}
		if (use_language == languages.English)
		{
			return;
		}
		string path = "";
		switch (use_language)
		{
		case languages.Russian:
			path = "lang-Russian";
			break;
		case languages.Portugese:
			path = "lang-Portugese";
			break;
		case languages.Thai:
			path = "lang-Thai";
			break;
		case languages.Polish:
			path = "lang-Polish";
			break;
		case languages.Spanish:
			path = "lang-Spanish";
			break;
		}
		string[] array = Regex.Split((Resources.Load(path) as TextAsset).text, "\n|\r|\r\n");
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i] == ""))
			{
				string[] array2 = array[i].Split('~');
				translations.Add(array2[0], array2[1]);
			}
		}
	}

	public string Translate(string text)
	{
		if (ApiTranslationHook != null && ApiTranslationHook(text, out string modTranslation))
		{
			return modTranslation.Replace("`", "\n");
		}
		if (use_language == languages.English)
		{
			return text.Replace("`", "\n");
		}
		if (translations.ContainsKey(text))
		{
			return translations[text].Replace("`", "\n");
		}
		return text.Replace("`", "\n");
	}
}
