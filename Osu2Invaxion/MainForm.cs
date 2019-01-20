using System;
using System.IO;
using System.Windows.Forms;

namespace OSU2INVAXION
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            textBox1.ReadOnly = true;
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string map = new MapConverter(textBox1.Text, comboBox1.SelectedIndex).Convert();

                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Title = "保存音灵谱面文件";
                dialog.Filter = "文本文档(*.txt)|*.txt";
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string file = dialog.FileName.ToString();
                    File.WriteAllText(file, map);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择Beatmap文件";
            dialog.Filter = "Beatmap文件(*.osu)|*.osu";
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = dialog.FileName;
            }
        }
    }
}
