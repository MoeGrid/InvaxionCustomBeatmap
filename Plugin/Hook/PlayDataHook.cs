using Aquatrax;
using Harmony12;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace InvaxionCustomSpectrumPlugin.Hook
{
    // HOOK Aquatrax.PlayData
    class PlayDataHook
    {
        //internal void LoadPlayMusic()
        public static IEnumerable<CodeInstruction> LoadPlayMusicTranspiler(ILGenerator il, IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> tmp = new List<CodeInstruction>();
            foreach (var i in instructions)
            {
                tmp.Add(i);
                if (i.opcode == OpCodes.Call && i.operand.ToString().Contains("GetMainAsset"))
                {
                    var call = AccessTools.Method(typeof(PlayDataHook), nameof(PlayDataHook.LoadPlayMusicHook));
                    var field = AccessTools.Field(typeof(PlayData), "MusicID");
                    tmp.Add(new CodeInstruction(OpCodes.Ldarg_0));
                    tmp.Add(new CodeInstruction(OpCodes.Ldfld, field));
                    tmp.Add(new CodeInstruction(OpCodes.Call, call));
                }
            }
            return tmp;
        }

        public static AudioClip LoadPlayMusicHook(AudioClip audioClip, string id)
        {
            if (MusicLoader.HasMusic(id))
            {
                return MusicLoader.GetMusicFile(id);
            }
            return audioClip;
        }
    }
}
