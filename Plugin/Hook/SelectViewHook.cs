/*
using Aquatrax;
using Harmony12;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;

namespace InvaxionCustomSpectrumPlugin.Hook
{
    // Aquatrax.SelectView
    class SelectViewHook
    {
        // internal void ChangeFxSound(string id)
        public static IEnumerable<CodeInstruction> ChangeFxSoundTranspiler(ILGenerator il, IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> tmp = new List<CodeInstruction>();
            foreach (var i in instructions)
            {
                tmp.Add(i);
                if (i.opcode == OpCodes.Call && i.operand.ToString().Contains("GetMainAsset"))
                {

                    Logger.Log("找到HOOK点");

                    var log = AccessTools.Method(typeof(SelectViewHook), nameof(SelectViewHook.ChangeFxSoundHook));
                    tmp.Add(new CodeInstruction(OpCodes.Ldarg_1));
                    tmp.Add(new CodeInstruction(OpCodes.Call, log));
                }
            }
            return tmp;
        }

        public static AudioClip ChangeFxSoundHook(AudioClip audioClip, string id)
        {
            if (MusicLoader.HasMusic(id))
            {
                return MusicLoader.GetXfadeFile(id);
            }
            return audioClip;
        }

        // public void MusicListInit()
        public static IEnumerable<CodeInstruction> MusicListInitTranspiler(ILGenerator il, IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> tmp = new List<CodeInstruction>();
            foreach (var i in instructions)
            {
                tmp.Add(i);
                if (i.opcode == OpCodes.Stloc_S)
                {
                    LocalBuilder lb = (LocalBuilder)i.operand;
                    if (lb.LocalIndex == 9 && lb.LocalType.Equals(typeof(Transform)))
                    {
                        var log = AccessTools.Method(typeof(SelectViewHook), nameof(SelectViewHook.MusicListInitHook));
                        tmp.Add(new CodeInstruction(OpCodes.Ldloc_S, 6));
                        tmp.Add(new CodeInstruction(OpCodes.Ldloc_S, 9));
                        tmp.Add(new CodeInstruction(OpCodes.Call, log));
                    }
                }
            }
            return tmp;
        }

        public static void MusicListInitHook(MusicInfoData musicInfo, Transform transform)
        {
            if (MusicLoader.HasMusic(musicInfo.id.ToString()))
            {
                transform.Find("ImageMask/Image").GetComponent<Image>().sprite = MusicLoader.GetMusicImage(musicInfo.id.ToString());
            }
        }
    }
}
*/