using Aquatrax;

namespace InvaxionCustomSpectrumPlugin
{
    class MusicInfoDataExt : MusicInfoData
    {
        public string dir { get; set; }

        public string img_file {
            get {
                return dir + @"\img.png";
            }
        }

        public string music_file {
            get {
                return dir + @"\music.wav";
            }
        }

        public string xfade_file {
            get {
                return dir + @"\xfade.wav";
            }
        }

        public string info_file {
            get {
                return dir + @"\info.json";
            }
        }

    }
}
