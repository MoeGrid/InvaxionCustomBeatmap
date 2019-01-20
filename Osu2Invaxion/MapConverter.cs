using OSU2INVAXION.InvaxionObj;
using OsuParsers;
using OsuParsers.Beatmaps;
using OsuParsers.Beatmaps.Objects.Mania;
using OsuParsers.Beatmaps.Objects.Standard;
using OsuParsers.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OSU2INVAXION
{
    class MapConverter
    {
        // 对应键代码
        private readonly static int[,] KeyMap = new int[,] {
            { 11, 12, 15, 16, 00, 00, 00, 00 },
            { 27, 11, 12, 15, 16, 29, 00, 00 },
            { 27, 11, 12, 13, 14, 15, 16, 29 }
        };

        // 音灵键数
        private int keyMode;
        // 音灵键数
        private int keyNum;

        // OSU 谱子
        private Beatmap beatmap;
        // OSU 模式
        private Ruleset mode;
        // 曲速
        private int bpm;
        // 一个音符时间长度
        private float oneBeatTime;
        // 列宽
        private int columnWidth;
        // 音乐长度
        private int musicLen;

        // 临时音符
        private List<TmpNote> tmpNotes = new List<TmpNote>();

        // 音灵谱
        private Dictionary<int, InvaxionBar> invaxionMap = new Dictionary<int, InvaxionBar>();
        // 时间线
        private Dictionary<float, TmpTimeline> timeLine = new Dictionary<float, TmpTimeline>();
        // 音灵谱字符串
        private StringBuilder invaxionMapStr = new StringBuilder();

        public MapConverter(string file, int key)
        {
            if (!File.Exists(file))
                throw new Exception("文件不存在！");

            // 音灵键数
            keyMode = key;
            keyNum = keyMode == 0 ? 4 : keyMode == 1 ? 6 : keyMode == 0 ? 8 : 4;

            // OSU谱子文件
            beatmap = Parser.ParseBeatmap(file);
            // 模式
            mode = beatmap.GeneralSection.Mode;

            if(mode != Ruleset.Mania)
            {
                throw new Exception("不是osu!mania谱面！");
            }

            // 列宽
            columnWidth = 512 / keyNum;
            // 一个音符时间长度
            oneBeatTime = beatmap.TimingPoints[0].BeatLength;
            // bpm
            bpm = (int)(60 * 1000 / oneBeatTime);
            // 音乐长度
            musicLen = beatmap.HitObjects[beatmap.HitObjects.Count - 1].EndTime;
        }

        public string Convert()
        {
            // 清理
            tmpNotes.Clear();
            timeLine.Clear();
            invaxionMap.Clear();

            // 转为时间对应键
            foreach (var i in beatmap.HitObjects)
            {
                if (i is StandardHitCircle || i is ManiaSingle)
                {
                    // 按键
                    tmpNotes.Add(new TmpNote
                    {
                        Key = X2Key(i.Position.X),
                        Time = i.StartTime,
                        Action = 11
                    });
                }
                else if (i is StandardSlider || i is ManiaHold)
                {
                    // 长条开始
                    var k = X2Key(i.Position.X);
                    tmpNotes.Add(new TmpNote
                    {
                        Key = k,
                        Time = i.StartTime,
                        Action = 31
                    });
                    // 长条结束
                    tmpNotes.Add(new TmpNote
                    {
                        Key = k,
                        Time = i.EndTime,
                        Action = 41
                    });
                }
            };
            tmpNotes.Sort(delegate (TmpNote x, TmpNote y)
            {
                return x.Time.CompareTo(y.Time);
            });

            // 生成时间线
            float oneTime = oneBeatTime * 4 / 32; // 4拍一节 1节32份
            float curTime = 0;
            int curBar = 0;
            while (curTime < musicLen)
            {
                for (var i = 0; i < 32; i++)
                {
                    timeLine.Add(curTime, new TmpTimeline()
                    {
                        BarIndex = curBar,
                        NodeIndex = i
                    });
                    curTime += oneTime;
                }
                curBar++;
            }

            // 对齐并加入时间线
            foreach (var i in tmpNotes)
            {
                // 找最近的音符节点
                var near = float.MaxValue;
                TmpTimeline nearObj = null;
                foreach (var j in timeLine)
                {
                    var tmpVal = Math.Abs(i.Time - j.Key);
                    if (tmpVal < near)
                    {
                        near = tmpVal;
                        nearObj = j.Value;
                    }
                }
                if (nearObj != null)
                {
                    i.BarIndex = nearObj.BarIndex;
                    i.NodeIndex = nearObj.NodeIndex;
                }
            }

            // 填充谱面
            foreach (var i in tmpNotes)
            {
                if (!invaxionMap.ContainsKey(i.BarIndex))
                {
                    invaxionMap.Add(i.BarIndex, new InvaxionBar()
                    {
                        Tracks = new Dictionary<int, InvaxionTrack>()
                    });
                }
                if (!invaxionMap[i.BarIndex].Tracks.ContainsKey(i.Key))
                {
                    invaxionMap[i.BarIndex].Tracks.Add(i.Key, new InvaxionTrack()
                    {
                        Nodes = new Dictionary<int, InvaxionNode>()
                    });
                }
                if (!invaxionMap[i.BarIndex].Tracks[i.Key].Nodes.ContainsKey(i.NodeIndex))
                {
                    invaxionMap[i.BarIndex].Tracks[i.Key].Nodes.Add(i.NodeIndex, new InvaxionNode()
                    {
                        Action = i.Action
                    });
                }
            }

            // 导出谱面
            invaxionMap = invaxionMap.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
            foreach (var i in invaxionMap)
            {
                // 准备
                if (i.Key == 0)
                {
                    invaxionMapStr.AppendFormat("0:\n1,{0};\n\n", bpm);
                }
                // Bar
                invaxionMapStr.AppendFormat("{0}:\n", i.Key + 1);
                // Start
                if (i.Key == 0)
                {
                    invaxionMapStr.Append("3,1,\n");
                }
                // Track
                i.Value.Tracks = i.Value.Tracks.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
                foreach (var j in i.Value.Tracks)
                {
                    invaxionMapStr.AppendFormat("{0},", j.Key);
                    for (var k = 0; k < 32; k++)
                    {
                        invaxionMapStr.Append(j.Value.Nodes.ContainsKey(k) ? j.Value.Nodes[k].Action.ToString() : "00");
                    }
                    invaxionMapStr.AppendFormat(",\n");
                }
                invaxionMapStr.Remove(invaxionMapStr.Length - 2, 2);
                invaxionMapStr.Append(";\n\n");
            }
            return invaxionMapStr.ToString();
        }

        private int X2Key(int x)
        {
            return KeyMap[keyMode, x / columnWidth];
        }
    }
}
