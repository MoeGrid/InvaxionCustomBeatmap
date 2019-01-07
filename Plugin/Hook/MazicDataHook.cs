using Aquatrax;
using Harmony12;
using System;

namespace InvaxionCustomSpectrumPlugin.Hook
{
    // HOOK Aquatrax.MazicData
    class MazicDataHook
    {
        // public void ReadOneMusicMap(string musicMap, int flag)
        public static bool ReadOneMusicMapPrefix(MazicData __instance, [HarmonyArgument("musicMap")]string musicMap, [HarmonyArgument("flag")]int flag)
        {
            try
            {
                var strs = musicMap.Split('_');
                if (strs.Length == 4 && MusicLoader.HasMusic(strs[0]))
                {
                    string musicScore = MusicLoader.GetMusicScore(strs[0], strs[1], strs[2]);
                    __instance.ParseText(flag, musicScore);
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Log("ERROR " + e.Message);
                Logger.Log(e.StackTrace);
            }
            return true;
            
        }
    }
}
