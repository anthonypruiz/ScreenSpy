using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ScreenSpy
{
    public partial class Form1 : Form
    {

        string[] sSpySettings = new string[5];

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                //radioButton2.Checked = true;
                Sqlite sData = new Sqlite();
                using (SQLiteConnection conn = new SQLiteConnection(sData.CreateConnection()))
                {
                    FileInfo finfo = new FileInfo("ScreenSpy.db");
                    if (finfo.Length < 1)
                    {
                        sData.CreateTable(conn);
                        sData.InsertData(conn);
                        sSpySettings = sData.ReadData(conn);
                    }
                    else
                    {
                        sSpySettings = sData.ReadData(conn);
                    }
                    textBox1.Text = sSpySettings[0];
                    textBox2.Text = sSpySettings[1];
                    textBox3.Text = sSpySettings[2];
                    textBox4.Text = sSpySettings[3];
                    string DuplicateImg = textBox4.Text;
                    if (DuplicateImg == "522500")
                    {
                        radioButton1.Checked = true;
                    }
                    if (DuplicateImg == "1045000")
                    {
                        radioButton2.Checked = true;
                    }
                    if (DuplicateImg == "2090000")
                    {
                        radioButton3.Checked = true;
                    }
                    textBox5.Text = sSpySettings[4];
                    int ColorPictures = int.Parse(sSpySettings[5]);
                    if (ColorPictures == 1)
                    {
                        checkBox1.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = fbd.SelectedPath;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                string ImageWidth = textBox1.Text;
                string ImageHeight = textBox2.Text;
                string ProgramDirectory = textBox3.Text;
                string DuplicateImgSensitivity = textBox4.Text;
                if (radioButton1.Checked == true)
                {
                    DuplicateImgSensitivity = "522500";
                }
                if (radioButton2.Checked == true)
                {
                    DuplicateImgSensitivity = "1045000";
                }
                if (radioButton1.Checked == true)
                {
                    DuplicateImgSensitivity = "2090000";
                }
                string CaptureFrequency = textBox5.Text;
                Process[] pname = Process.GetProcessesByName("ScreenSpy");
                if (pname.Length != 0)
                {
                    pname[0].Kill();
                }
                Sqlite sData = new Sqlite();
                SQLiteConnection conn;
                conn = sData.CreateConnection();
                int ColorPictures;
                if (checkBox1.Checked == true)
                {
                    ColorPictures = 1;
                }
                else
                {
                    ColorPictures = 0;
                }
                sData.UpdateData(conn, ImageWidth, ImageHeight, ProgramDirectory, DuplicateImgSensitivity, CaptureFrequency, ColorPictures);
                conn.Close();
                Process.Start("ScreenSpy.exe");
                MessageBox.Show("Settings Saved");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
