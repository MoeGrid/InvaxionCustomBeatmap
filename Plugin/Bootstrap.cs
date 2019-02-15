using System;
using Aquatrax;
using Harmony12;
using UnityEngine;
using InvaxionCustomSpectrumPlugin.Hook;

namespace InvaxionCustomSpectrumPlugin
{
    public class Bootstrap
    {
        public static void Load()
        {
            try
            {
                InitHook();
            }
            catch (Exception e)
            {
                Logger.Log("ERROR " + e.Message);
                Logger.Log(e.StackTrace);
            }
            GameObject gameObject = new GameObject();
            gameObject.AddComponent<HomemadeMusic>();
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
        }

        private static void InitHook()
        {
            var instance = HarmonyInstance.Create("HomemadeMusicScore");

            // 加载Json资源Hook
            var classifyMusicInfoList = AccessTools.Method(typeof(GlobalConfig), "classifyMusicInfoList");
            var classifyMusicInfoListPrefix = AccessTools.Method(typeof(GlobalConfigHook), nameof(GlobalConfigHook.ClassifyMusicInfoListPrefix));
            instance.Patch(classifyMusicInfoList, new HarmonyMethod(classifyMusicInfoListPrefix));

            // 封面图片Hook 已过时
            //var musicListInit = AccessTools.Method(typeof(SelectView), nameof(SelectView.MusicListInit));
            //var musicListInitTranspiler = AccessTools.Method(typeof(SelectViewHook), nameof(SelectViewHook.MusicListInitTranspiler));
            //instance.Patch(musicListInit, null, null, new HarmonyMethod(musicListInitTranspiler));

            // 封面音乐Hook 已过时
            //var changeFxSound = AccessTools.Method(typeof(SelectView), "ChangeFxSound", new Type[] {
            //    typeof(string)
            //});
            //var changeFxSoundTranspiler = AccessTools.Method(typeof(SelectViewHook), nameof(SelectViewHook.ChangeFxSoundTranspiler));
            //instance.Patch(changeFxSound, null, null, new HarmonyMethod(changeFxSoundTranspiler));

            // 封面音乐Hook
            var makeSoneList = AccessTools.Method(typeof(PC_newQuickPlayView), "MakeSoneList");
            var makeSoneListTranspiler = AccessTools.Method(typeof(PCNewQuickPlayViewHook), nameof(PCNewQuickPlayViewHook.MakeSoneListTranspiler));
            instance.Patch(makeSoneList, null, null, new HarmonyMethod(makeSoneListTranspiler));
            
            // 游戏音乐Hook
            var loadPlayMusic = AccessTools.Method(typeof(PlayData), "LoadPlayMusic");
            var loadPlayMusicTranspiler = AccessTools.Method(typeof(PlayDataHook), nameof(PlayDataHook.LoadPlayMusicTranspiler));
            instance.Patch(loadPlayMusic, null, null, new HarmonyMethod(loadPlayMusicTranspiler));

            // 游戏谱面Hook
            var readOneMusicMap = AccessTools.Method(typeof(MazicData), nameof(MazicData.ReadOneMusicMap));
            var readOneMusicMapPrefix = AccessTools.Method(typeof(MazicDataHook), nameof(MazicDataHook.ReadOneMusicMapPrefix));
            instance.Patch(readOneMusicMap, new HarmonyMethod(readOneMusicMapPrefix));

            // 封面图Hook
            var getCoverSprite = AccessTools.Method(typeof(ExtensionMethods), nameof(ExtensionMethods.getCoverSprite));
            var getCoverSpritePrefix = AccessTools.Method(typeof(ExtensionMethodsHook), nameof(ExtensionMethodsHook.GetCoverSpritePrefix));
            instance.Patch(getCoverSprite, new HarmonyMethod(getCoverSpritePrefix));
            
        }
    }
}
