using Harmony12;
using UnityEngine;

namespace InvaxionCustomSpectrumPlugin.Hook
{
    // HOOK Aquatrax.ExtensionMethods
    class ExtensionMethodsHook
    {
        // public static Sprite getCoverSprite(string path)
        public static bool GetCoverSpritePrefix([HarmonyArgument("path")]string path, ref Sprite __result)
        {
            string[] paths = path.Split('_');
            if (paths.Length > 0)
            {
                if (MusicLoader.HasMusic(paths[0]))
                {
                    __result = MusicLoader.GetMusicImage(paths[0]);
                    return false;
                }
            }
            return true;
        }
    }
}
