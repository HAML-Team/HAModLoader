using System;
using UnityEngine;
using UnityEngine.Store;

namespace AppStoresSupport
{
	[Serializable]
	public class AppStoreSettings : ScriptableObject
	{
		public string UnityClientID = "";

		public string UnityClientKey = "";

		public string UnityClientRSAPublicKey = "";

		public AppStoreSetting XiaomiAppStoreSetting = new AppStoreSetting();

		public AppInfo getAppInfo()
		{
			return new AppInfo
			{
				clientId = UnityClientID,
				clientKey = UnityClientKey,
				appId = XiaomiAppStoreSetting.AppID,
				appKey = XiaomiAppStoreSetting.AppKey,
				debug = XiaomiAppStoreSetting.IsTestMode
			};
		}
	}
}
