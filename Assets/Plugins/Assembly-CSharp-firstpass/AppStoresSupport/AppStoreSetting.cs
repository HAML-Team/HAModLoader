using System;

namespace AppStoresSupport
{
	[Serializable]
	public class AppStoreSetting
	{
		public string AppID = "";

		public string AppKey = "";

		public bool IsTestMode;
	}
}
