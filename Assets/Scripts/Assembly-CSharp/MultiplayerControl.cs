using System;
using System.Diagnostics;
using System.Threading;
//using GooglePlayGames;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerControl : MonoBehaviour
{
	public enum services_state
	{
		connecting = 0,
		loaded = 1,
		couldnt_load = 2
	}

	public static MultiplayerControl Instance;

	private Thread get_time;

	public Text deb;

	[HideInInspector]
	public bool last_known_real_time_set;

	[HideInInspector]
	public DateTime last_known_real_time;

	[HideInInspector]
	public System.Diagnostics.Stopwatch last_known_real_time_stopwatch = new System.Diagnostics.Stopwatch();

	public services_state services_state_t;

	[HideInInspector]
	public bool connect_fail_already;

	[HideInInspector]
	public bool was_on_press_back_to_menu;

	private System.Diagnostics.Stopwatch pause_timeout = new System.Diagnostics.Stopwatch();

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	private void Start()
	{
		if (this == Instance)
		{
			auto_time_enabled();
			//PlayGamesPlatform.DebugLogEnabled = true;
			//PlayGamesPlatform.Activate();
			Authenticate();
		}
	}

	public bool auto_time_enabled()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var androidJavaObject = activity.Call<AndroidJavaObject>("getApplicationContext", Array.Empty<object>()).Call<AndroidJavaObject>("getContentResolver", Array.Empty<object>());
            if (new AndroidJavaClass("android.provider.Settings$Global").CallStatic<string>("getString", new object[2] { androidJavaObject, "auto_time" }) == "1")
            {
                last_known_real_time_set = true;
                last_known_real_time = DateTime.Now;
                last_known_real_time_stopwatch.Stop();
                last_known_real_time_stopwatch.Reset();
                last_known_real_time_stopwatch.Start();
                return true;
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning("Android JNI access failed: " + e.Message);
            return false;
        }

        // If we reached here on Android, auto_time was not enabled
        return false;
#else
        // Not Android — return default
        return false;
#endif
	}

	public void unlock_achievement(string code)
	{
		Social.ReportProgress(code, 100.0, delegate
		{
		});
	}

	private void Authenticate()
	{
		Social.localUser.Authenticate(AuthCallback);
	}

	private void AuthCallback(bool success)
	{
		if (success)
		{
			services_state_t = services_state.loaded;
			PopupControl.Instance.HideAll();
		}
		else
		{
			services_state_t = services_state.couldnt_load;
			PopupControl.Instance.ShowMessage("Couldn't connect to Google Play services.");
			connect_fail_already = true;
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			pause_timeout.Reset();
			pause_timeout.Start();
			return;
		}
		pause_timeout.Stop();
		if ((int)((float)pause_timeout.ElapsedMilliseconds / 1000f) > 120 && SceneManager.GetActiveScene().name == "Game")
		{
			GameController.Instance.ClickBackToMenu();
		}
	}
}
