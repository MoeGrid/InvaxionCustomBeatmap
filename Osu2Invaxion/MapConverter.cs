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
        // 曲速
        private int bpm;
        // 一个音符时间长度
        private float oneBeatTime;
        // 偏移
        private int offset;
        // 节拍细分
        private int beatDivisor;
        // 细分时间
        private float oneDivisorTime;
        // 列宽
        private int columnWidth;

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
            keyNum = keyMode == 0 ? 4 : keyMode == 1 ? 6 : keyMode == 2 ? 8 : 4;

            // OSU谱子文件
            beatmap = Parser.ParseBeatmap(file);
            // 模式
            if (beatmap.GeneralSection.Mode != Ruleset.Mania)
            {
                throw new Exception("不是osu!mania谱面！");
            }
            // 列宽
            columnWidth = 512 / keyNum;
            // 一个音符时间长度
            oneBeatTime = beatmap.TimingPoints[0].BeatLength;
            // 偏移
            offset = beatmap.TimingPoints[0].Offset;
            // 节拍细分
            beatDivisor = beatmap.EditorSection.BeatDivisor;
            // 细分时间
            oneDivisorTime = oneBeatTime / beatDivisor;
            // bpm
            bpm = (int)(60 * 1000 / oneBeatTime);
        }

        public void Convert(out string map, out int fill)
        {
            // 清理
            tmpNotes.Clear();
            timeLine.Clear();
            invaxionMap.Clear();

            // 计算填充
            int fillNode = (int)(offset / oneDivisorTime);
            float fillTime = oneDivisorTime - (offset % oneDivisorTime);
            if (fillTime > 0)
            {
                fillNode++;
            }

            // 转为时间对应键
            foreach (var i in beatmap.HitObjects)
            {
                int startTime = i.StartTime - offset;
                int endTime = i.EndTime - offset;

                int barIndex = 0;
                int nodeIndex = 0;

                if (i is StandardHitCircle || i is ManiaSingle)
                {
                    // 按键
                    CalcIndex(startTime, fillNode, out barIndex, out nodeIndex);
                    tmpNotes.Add(new TmpNote
                    {
                        Key = X2Key(i.Position.X),
                        Action = 11,
                        Time = startTime,
                        BarIndex = barIndex,
                        NodeIndex = nodeIndex,
                    });
                }
                else if (i is StandardSlider || i is ManiaHold)
                {
                    // 长条开始
                    CalcIndex(startTime, fillNode, out barIndex, out nodeIndex);
                    var k = X2Key(i.Position.X);
                    tmpNotes.Add(new TmpNote
                    {
                        Key = k,
                        Action = 31,
                        Time = startTime,
                        BarIndex = barIndex,
                        NodeIndex = nodeIndex
                    });
                    CalcIndex(endTime, fillNode, out barIndex, out nodeIndex);
                    // 长条结束
                    tmpNotes.Add(new TmpNote
                    {
                        Key = k,
                        Action = 41,
                        Time = endTime,
                        BarIndex = barIndex,
                        NodeIndex = nodeIndex
                    });
                }
            };

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

            if(!invaxionMap.ContainsKey(0))
            {
                invaxionMap.Add(0, null);
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
                if (i.Value != null)
                {
                    i.Value.Tracks = i.Value.Tracks.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
                    foreach (var j in i.Value.Tracks)
                    {
                        invaxionMapStr.AppendFormat("{0},", j.Key);
                        for (var k = 0; k < beatDivisor * 4; k++)
                        {
                            invaxionMapStr.Append(j.Value.Nodes.ContainsKey(k) ? j.Value.Nodes[k].Action.ToString() : "00");
                        }
                        invaxionMapStr.AppendFormat(",\n");
                    }
                }
                invaxionMapStr.Remove(invaxionMapStr.Length - 2, 2);
                invaxionMapStr.Append(";\n\n");
            }
            map = invaxionMapStr.ToString();
            fill = (int)Math.Round(fillTime);
        }

        private void CalcIndex(int time, int fill, out int barIndex, out int nodeIndex)
        {
            // 计算Index
            int oneBarNode = beatDivisor * 4;
            int divisorNum = (int)Math.Round(time / oneDivisorTime);
            barIndex = divisorNum / oneBarNode;
            nodeIndex = divisorNum % oneBarNode;
            // 填充
            nodeIndex += fill;
            barIndex += nodeIndex / oneBarNode;
            nodeIndex = nodeIndex % oneBarNode;
        }

        private int X2Key(int x)
        {
            return KeyMap[keyMode, x / columnWidth];
        }
    }
}
