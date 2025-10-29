namespace HAModLoaderAPI
{
    public static class Log
    {
        public static void Info(string message)
        {
            UnityEngine.Debug.Log("[HAModLoader] " + message);
        }

        public static void Warning(string message)
        {
            UnityEngine.Debug.LogWarning("[HAModLoader] " + message);
        }

        public static void Error(string message)
        {
            UnityEngine.Debug.LogError("[HAModLoader] " + message);
        }
    }
}