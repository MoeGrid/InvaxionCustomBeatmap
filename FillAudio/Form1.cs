using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.IO;
using System.Windows.Forms;

namespace FillAudio
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var file = textBox1.Text;
            if(File.Exists(file))
            {
                int.TryParse(textBox2.Text, out int fillTime);
                var audio = new AudioFileReader(file);
                var provider = new OffsetSampleProvider(audio)
                {
                    DelayBy = TimeSpan.FromMilliseconds(fillTime)
                };
                SaveFileDialog dialog = new SaveFileDialog
                {
                    RestoreDirectory = true,
                    Filter = "音频文件(*.wav)|*.wav"
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    WaveFileWriter.CreateWaveFile16(dialog.FileName, provider);
                    MessageBox.Show("转换完成！", "提示");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                Title = "请选择音频文件",
                Filter = "音频文件(*.mp3,*.wav)|*.mp3;*.wav"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = dialog.FileName;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && e.KeyChar != (char)13 && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }
    }
}
