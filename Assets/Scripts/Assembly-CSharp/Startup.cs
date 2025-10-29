using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{
	public static string persistent_data_path;

	private void Start()
	{
		persistent_data_path = Application.persistentDataPath;
		StartCoroutine(async_load_menu());
	}

	private IEnumerator delayed_quit()
	{
		yield return new WaitForSeconds(2f);
		Application.Quit();
	}

	private IEnumerator async_load_menu()
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Menu");
		while (!asyncLoad.isDone)
		{
			yield return null;
		}
	}
}
