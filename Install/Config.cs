using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InvaxionCustomSpectrumInstall
{
    class Config
    {
        private static readonly Dictionary<string, string> cfgDic = new Dictionary<string, string>();
        private static readonly string file = "config.cfg";

        public static void Load()
        {
            cfgDic.Clear();
            string[] cfgStr = File.ReadAllLines(file);
            if (cfgStr != null && cfgStr.Length > 0)
            {
                foreach (var i in cfgStr)
                {
                    string[] opt = i.Split('=');
                    if (opt.Length == 2)
                    {
                        cfgDic.Add(opt[0], opt[1]);
                    }
                }
            }
        }

        public static void Save()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var i in cfgDic)
            {
                sb.AppendLine($"{i.Key}={i.Value}");
            }
            File.WriteAllText(file, sb.ToString());
        }

        public static void Set(string key, string value)
        {
            cfgDic.Add(key, value);
        }

        public static string Get(string key)
        {
            return cfgDic[key];
        }
    }
}
