namespace InvaxionCustomSpectrumPlugin
{
    class Logger
    {

        public static void Error(string str)
        {
            Harmony12.FileLog.Log("[ERROR] " + str);
        }

        public static void Log(string str)
        {
            Harmony12.FileLog.Log("[INFO] " + str);
        }

        public static void Warning(string str)
        {
            Harmony12.FileLog.Log("[WARN] " + str);
        }

    }
}
