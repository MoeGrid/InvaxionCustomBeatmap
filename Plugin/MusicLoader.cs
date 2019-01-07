using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

namespace InvaxionCustomSpectrumPlugin
{
    class MusicLoader
    {
        private static Dictionary<string, MusicInfoDataExt> _MusicDic;
        public static Dictionary<string, MusicInfoDataExt> MusicDic {
            get {
                if (_MusicDic == null)
                {
                    _MusicDic = new Dictionary<string, MusicInfoDataExt>();
                    if (_MusicDic.Count <= 0)
                    {
                        DirectoryInfo musicDir = new DirectoryInfo("CustomSpectrum");
                        if (musicDir.Exists)
                        {
                            var dirs = musicDir.GetDirectories();
                            foreach (var i in dirs)
                            {
                                string infoJson = i.FullName + @"\info.json";
                                if (File.Exists(infoJson))
                                {
                                    string json = File.ReadAllText(infoJson, Encoding.UTF8);
                                    var info = JsonMapper.ToObject<MusicInfoDataExt>(new JsonReader(json));
                                    info.dir = i.FullName;
                                    _MusicDic.Add(info.id.ToString(), info);
                                }
                            }
                        }
                    }

                }
                return _MusicDic;
            }
        }

        public static bool HasMusic(string id)
        {
            return _MusicDic.ContainsKey(id);
        }

        public static Sprite GetMusicImage(string id)
        {
            var music = _MusicDic[id];
            if (music != null)
            {
                if (File.Exists(music.img_file))
                {
                    var bytes = File.ReadAllBytes(music.img_file);
                    Texture2D tex = new Texture2D(512, 512);
                    tex.LoadImage(bytes);
                    return Sprite.Create(tex, new Rect(0, 0, 512, 512), Vector2.zero);
                }
            }
            return null;
        }
        
        public static AudioClip GetXfadeFile(string id)
        {
            var music = _MusicDic[id];
            return music != null ? GetAudioClip(music.xfade_file) : null;
        }

        public static AudioClip GetMusicFile(string id)
        {
            var music = _MusicDic[id];
            return music != null ? GetAudioClip(music.music_file) : null;
        }

        public static string GetMusicScore(string id, string keyNum, string diffLevel)
        {
            var music = _MusicDic[id];
            if (music != null)
            {
                string file = string.Format(@"{0}\{1}_{2}.txt", music.dir, keyNum, diffLevel);
                Logger.Log("加载谱面: " + file);
                return File.ReadAllText(file, Encoding.UTF8);
            }
            return null;
        }

        private static AudioClip GetAudioClip(string file)
        {
            WWW www = new WWW("file:///" + file);
            while (!www.isDone && www.error == null)
            {
                Thread.Sleep(10);
            }
            AudioClip audioClip = www.GetAudioClip(true, true, AudioType.WAV);
            return audioClip;
        }

    }
}
