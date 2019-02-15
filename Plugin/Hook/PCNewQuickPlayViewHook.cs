using Harmony12;
using UnityEngine;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace InvaxionCustomSpectrumPlugin.Hook
{
    // Aquatrax.PC_newQuickPlayView
    class PCNewQuickPlayViewHook
    {
        // internal void ChangeFxSound(string id)
        public static IEnumerable<CodeInstruction> MakeSoneListTranspiler(ILGenerator il, IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> tmp = new List<CodeInstruction>();
            foreach (var i in instructions)
            {
                tmp.Add(i);
                if (i.opcode == OpCodes.Call && i.operand.ToString().Contains("GetMainAsset"))
                {
                    var log = AccessTools.Method(typeof(PCNewQuickPlayViewHook), nameof(PCNewQuickPlayViewHook.ChangeFxSoundHook));
                    tmp.Add(new CodeInstruction(OpCodes.Ldloc_2));
                    tmp.Add(new CodeInstruction(OpCodes.Call, log));
                }
            }
            return tmp;
        }

        public static AudioClip ChangeFxSoundHook(AudioClip audioClip, int id)
        {
            var idStr = id.ToString();
            if (MusicLoader.HasMusic(idStr))
            {
                return MusicLoader.GetXfadeFile(idStr);
            }
            return audioClip;
        }
    }
}
