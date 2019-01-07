using Aquatrax;
using Harmony12;
using System.Collections.Generic;

namespace InvaxionCustomSpectrumPlugin.Hook
{
    // HOOK Aquatrax.GlobalConfig
    class GlobalConfigHook
    {
        // private void classifyMusicInfoList()
        public static void ClassifyMusicInfoListPrefix(GlobalConfig __instance)
        {
            Logger.Log("载入歌曲列表");
            var dic = Traverse.Create(__instance).Field<Dictionary<string, MusicInfoData>>("musicInfoDict").Value;
            foreach (var i in MusicLoader.MusicDic)
            {
                dic.Add(i.Key, i.Value);
            }
        }
    }
}
