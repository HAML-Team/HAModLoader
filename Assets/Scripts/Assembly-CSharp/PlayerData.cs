using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
	public enum grouping_t
	{
		general = 0,
		the_inventory = 1,
		perks = 2,
		the_biome_map = 3,
		loot = 4,
		playerpos = 5
	}

	public static PlayerData Instance;

	[HideInInspector]
	public int SLOT;

	private Dictionary<string, object> GLOBAL_values;

	private Dictionary<string, object> slot0_filenames = new Dictionary<string, object>();

	private Dictionary<string, object> slot1_filenames = new Dictionary<string, object>();

	private Dictionary<string, object> slot2_filenames = new Dictionary<string, object>();

	private Dictionary<string, Dictionary<string, object>> slot0_loaded_files = new Dictionary<string, Dictionary<string, object>>();

	private Dictionary<string, Dictionary<string, object>> slot1_loaded_files = new Dictionary<string, Dictionary<string, object>>();

	private Dictionary<string, Dictionary<string, object>> slot2_loaded_files = new Dictionary<string, Dictionary<string, object>>();

	private Dictionary<string, Dictionary<string, object>> modified_this_step = new Dictionary<string, Dictionary<string, object>>();

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
			if (Application.isEditor)
			{
				Startup.persistent_data_path = Application.persistentDataPath;
			}
			GLOBAL_values = LoadOrCreateDictionary("general_");
			slot0_filenames = LoadOrCreateDictionary("slot0_filenames");
			slot1_filenames = LoadOrCreateDictionary("slot1_filenames");
			slot2_filenames = LoadOrCreateDictionary("slot2_filenames");
		}
	}

	public void DELETE_GEMS_ETC()
	{
		File.Delete(Startup.persistent_data_path + Path.DirectorySeparatorChar + "general_");
		GLOBAL_values.Clear();
		Debug.Log("DELETED GENERAL DATA");
	}

	private void FixedUpdate()
	{
		if (modified_this_step.Count != 0)
		{
			SaveModified();
		}
	}

	public void SaveModified()
	{
		foreach (KeyValuePair<string, Dictionary<string, object>> item in modified_this_step)
		{
			SaveDictionary(item.Value, item.Key);
		}
		modified_this_step.Clear();
	}

	public void DeleteSlot(int index)
	{
		string text = "";
		Dictionary<string, object> dictionary = null;
		Dictionary<string, Dictionary<string, object>> dictionary2 = null;
		switch (index)
		{
		case 0:
			dictionary = slot0_filenames;
			text = "slot0_filenames";
			dictionary2 = slot0_loaded_files;
			break;
		case 1:
			dictionary = slot1_filenames;
			text = "slot1_filenames";
			dictionary2 = slot1_loaded_files;
			break;
		case 2:
			dictionary = slot2_filenames;
			text = "slot2_filenames";
			dictionary2 = slot2_loaded_files;
			break;
		}
		foreach (KeyValuePair<string, object> item in dictionary)
		{
			File.Delete(Path.Combine(Startup.persistent_data_path, item.Key));
		}
		File.Delete(Path.Combine(Startup.persistent_data_path, text));
		dictionary2.Clear();
		switch (index)
		{
		case 0:
			slot0_filenames = LoadOrCreateDictionary(text);
			break;
		case 1:
			slot1_filenames = LoadOrCreateDictionary(text);
			break;
		case 2:
			slot2_filenames = LoadOrCreateDictionary(text);
			break;
		}
	}

	public void DeLoadGrouping(string grouping)
	{
		Dictionary<string, Dictionary<string, object>> dictionary = null;
		switch (SLOT)
		{
		case 0:
			dictionary = slot0_loaded_files;
			break;
		case 1:
			dictionary = slot1_loaded_files;
			break;
		case 2:
			dictionary = slot2_loaded_files;
			break;
		}
		dictionary.Remove(grouping);
	}

	public int GetGlobalInt(string key)
	{
		if (GLOBAL_values.ContainsKey(key))
		{
			return (int)GLOBAL_values[key];
		}
		return 0;
	}

	public void SetGlobalInt(string key, int value)
	{
		SetGlobalValue(key, value);
	}

	public void SetGlobalString(string key, string value)
	{
		SetGlobalValue(key, value);
	}

	private void SetGlobalValue(string key, object value)
	{
		if (GLOBAL_values.ContainsKey(key))
		{
			GLOBAL_values[key] = value;
		}
		else
		{
			GLOBAL_values.Add(key, value);
		}
		if (!modified_this_step.ContainsKey("general_"))
		{
			modified_this_step.Add("general_", GLOBAL_values);
		}
	}

	public string GetSlotString(string key, grouping_t grouping, int slot = -1)
	{
		return GetSlotString(key, grouping_to_string(grouping), slot);
	}

	public string GetSlotString(string key, string grouping, int slot = -1)
	{
		if (slot == -1)
		{
			slot = SLOT;
		}
		Dictionary<string, object> dictionary = get_specific_file(grouping, slot);
		if (dictionary.ContainsKey(key))
		{
			return (string)dictionary[key];
		}
		return "";
	}

	public int GetSlotInt(string key, grouping_t grouping, int slot = -1)
	{
		return GetSlotInt(key, grouping_to_string(grouping), slot);
	}

	public int GetSlotInt(string key, string grouping, int slot = -1)
	{
		if (slot == -1)
		{
			slot = SLOT;
		}
		Dictionary<string, object> dictionary = get_specific_file(grouping, slot);
		if (dictionary.ContainsKey(key))
		{
			return (int)dictionary[key];
		}
		return 0;
	}

	public float GetSlotFloat(string key, grouping_t grouping, int slot = -1)
	{
		return GetSlotFloat(key, grouping_to_string(grouping), slot);
	}

	public float GetSlotFloat(string key, string grouping, int slot = -1)
	{
		if (slot == -1)
		{
			slot = SLOT;
		}
		Dictionary<string, object> dictionary = get_specific_file(grouping, slot);
		if (dictionary.ContainsKey(key))
		{
			return (float)dictionary[key];
		}
		return 0f;
	}

	public void SetSlotInt(string key, int value, grouping_t grouping, int slot = -1)
	{
		SetSlotValue(key, value, grouping_to_string(grouping), slot);
	}

	public void SetSlotInt(string key, int value, string grouping, int slot = -1)
	{
		SetSlotValue(key, value, grouping, slot);
	}

	public void SetSlotString(string key, string value, grouping_t grouping, int slot = -1)
	{
		SetSlotValue(key, value, grouping_to_string(grouping), slot);
	}

	public void SetSlotString(string key, string value, string grouping, int slot = -1)
	{
		SetSlotValue(key, value, grouping, slot);
	}

	private void SetSlotValue(string key, object value, grouping_t grouping, int slot)
	{
		SetSlotValue(key, value, grouping_to_string(grouping), slot);
	}

	private void SetSlotValue(string key, object value, string grouping, int slot)
	{
		if (slot == -1)
		{
			slot = SLOT;
		}
		Dictionary<string, object> dictionary = get_specific_file(grouping, slot);
		if (dictionary.ContainsKey(key))
		{
			dictionary[key] = value;
		}
		else
		{
			dictionary.Add(key, value);
		}
	}

	public string grouping_to_string(grouping_t grouping)
	{
		switch (grouping)
		{
		case grouping_t.the_biome_map:
			return "the_biome_map";
		case grouping_t.the_inventory:
			return "the_inventory";
		case grouping_t.perks:
			return "perks";
		case grouping_t.loot:
			return "loot";
		case grouping_t.playerpos:
			return "playerpos";
		default:
			return "general";
		}
	}

	private Dictionary<string, object> get_specific_file(string specific_file, int slot)
	{
		string text = "";
		Dictionary<string, Dictionary<string, object>> dictionary = null;
		Dictionary<string, object> dictionary2 = null;
		string key = "";
		switch (slot)
		{
		case 0:
			text = "slot0 ";
			dictionary = slot0_loaded_files;
			dictionary2 = slot0_filenames;
			key = "slot0_filenames";
			break;
		case 1:
			text = "slot1 ";
			dictionary = slot1_loaded_files;
			dictionary2 = slot1_filenames;
			key = "slot1_filenames";
			break;
		case 2:
			text = "slot2 ";
			dictionary = slot2_loaded_files;
			dictionary2 = slot2_filenames;
			key = "slot2_filenames";
			break;
		}
		string text2 = text + specific_file;
		Dictionary<string, object> dictionary3;
		if (dictionary.ContainsKey(specific_file))
		{
			dictionary3 = dictionary[specific_file];
		}
		else
		{
			dictionary3 = LoadOrCreateDictionary(text2);
			dictionary.Add(specific_file, dictionary3);
			if (!dictionary2.ContainsKey(text2))
			{
				dictionary2.Add(text2, text2);
				if (!modified_this_step.ContainsKey(key))
				{
					modified_this_step.Add(key, dictionary2);
				}
			}
		}
		if (!modified_this_step.ContainsKey(text2))
		{
			modified_this_step.Add(text2, dictionary3);
		}
		return dictionary3;
	}

	public Dictionary<string, object> LoadOrCreateDictionary(string path, bool create = true)
	{
		string path2 = Path.Combine(Startup.persistent_data_path, path);
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		if (File.Exists(path2))
		{
			Packet packet = new Packet(File.ReadAllBytes(path2));
			short num = packet.getShort();
			for (int i = 0; i < num; i++)
			{
				string key = packet.getString();
				int num2 = packet.getShort();
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, num2);
				}
			}
			short num3 = packet.getShort();
			for (int j = 0; j < num3; j++)
			{
				string key2 = packet.getString();
				string value = packet.getString();
				if (!dictionary.ContainsKey(key2))
				{
					dictionary.Add(key2, value);
				}
			}
			short num4 = packet.getShort();
			for (int k = 0; k < num4; k++)
			{
				string key3 = packet.getString();
				float num5 = (float)packet.getShort() / 100f;
				if (!dictionary.ContainsKey(key3))
				{
					dictionary.Add(key3, num5);
				}
			}
		}
		else if (create)
		{
			try
			{
				File.Create(path2).Dispose();
			}
			catch (Exception)
			{
				try
				{
					IntPtr methodID = AndroidJNIHelper.GetMethodID(AndroidJNI.FindClass("android/content/ContextWrapper"), "getFilesDir", "()Ljava/io/File;");
					using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
					{
						using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
						{
							IntPtr obj = AndroidJNI.CallObjectMethod(androidJavaObject.GetRawObject(), methodID, new jvalue[0]);
							IntPtr methodID2 = AndroidJNIHelper.GetMethodID(AndroidJNI.FindClass("java/io/File"), "getAbsolutePath", "()Ljava/lang/String;");
							Startup.persistent_data_path = AndroidJNI.CallStringMethod(obj, methodID2, new jvalue[0]);
							if (Startup.persistent_data_path == null)
							{
								Startup.persistent_data_path = Application.persistentDataPath;
								PopupControl.Instance.HideAll();
								PopupControl.Instance.ShowYesNo("<color=#ffa100><size=30>Oops! There was a glitch.</size></color>\n\n<color=#d1d1d1><size=16>#Data redirect error 1</size></color>", "OKAY", "Report Glitch", PopupControl.context.error_log);
								PopupControl.Instance.error_log = "Data redirect error 1";
							}
						}
					}
				}
				catch (Exception)
				{
					Startup.persistent_data_path = Application.persistentDataPath;
					PopupControl.Instance.HideAll();
					PopupControl.Instance.ShowYesNo("<color=#ffa100><size=30>Oops! There was a glitch.</size></color>\n\n<color=#d1d1d1><size=16>#Data redirect error 2</size></color>", "OKAY", "Report Glitch", PopupControl.context.error_log);
					PopupControl.Instance.error_log = "Data redirect error 2";
				}
				path2 = Path.Combine(Startup.persistent_data_path, path);
			}
			Packet packet = new Packet();
			packet.putShort(0);
			packet.putShort(0);
			packet.putShort(0);
			File.WriteAllBytes(path2, packet.convert());
		}
		return dictionary;
	}

	private void SaveDictionary(Dictionary<string, object> dict, string path)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		Dictionary<string, short> dictionary3 = new Dictionary<string, short>();
		foreach (KeyValuePair<string, object> item in dict)
		{
			if (item.Value.GetType() == typeof(int))
			{
				dictionary.Add(item.Key, (int)item.Value);
			}
			else if (item.Value.GetType() == typeof(string))
			{
				dictionary2.Add(item.Key, (string)item.Value);
			}
			else if (item.Value.GetType() == typeof(float))
			{
				dictionary3.Add(item.Key, (short)((float)item.Value * 100f));
			}
		}
		Packet packet = new Packet();
		packet.putShort((short)dictionary.Count);
		foreach (KeyValuePair<string, int> item2 in dictionary)
		{
			packet.putString(item2.Key);
			packet.putShort((short)item2.Value);
		}
		packet.putShort((short)dictionary2.Count);
		foreach (KeyValuePair<string, string> item3 in dictionary2)
		{
			packet.putString(item3.Key);
			packet.putString(item3.Value);
		}
		packet.putShort((short)dictionary3.Count);
		foreach (KeyValuePair<string, short> item4 in dictionary3)
		{
			packet.putString(item4.Key);
			packet.putShort(item4.Value);
		}
		File.WriteAllBytes(Path.Combine(Startup.persistent_data_path, path), packet.convert());
	}
}
