using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SpyImgSlider
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        internal string CurrentPictureLocation
        {
            get
            {
                string PictureLocation = sqlite.PictureLocation();
                return PictureLocation;
            }
        }
        private void PictureDirectoryExists()
        {
            if (!Directory.Exists(CurrentPictureLocation)) { Directory.CreateDirectory(CurrentPictureLocation); }
        }
        string[] subdirs
        {
            get
            {
                string[] _subdirs = Directory.GetDirectories(CurrentPictureLocation).Select(Path.GetFileName).ToArray();
                return _subdirs;
            }
        }
        private SQLiteConnection conn;
        private SliderSqlite sqlite = new SliderSqlite();
        private string workingDirectory = Directory.GetCurrentDirectory();
        private string NameOfPerson = Environment.UserName;
        private string Extension = ".jpeg";
        private string[] files;
        private int i;

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                PictureDirectoryExists();
                int margin = 10;
                string alias;
                int AliasSet;
                string[] AliasData;
                if (!File.Exists("Aliases.db"))
                {
                    conn = sqlite.CreateConnection();
                    sqlite.CreateTable(conn);
                    sqlite.InsertData(conn, NameOfPerson, NameOfPerson);
                }
                else { conn = sqlite.CreateConnection(); }
                for (int i = 0; i < subdirs.Length; i++)
                {
                    string personsName = subdirs[i];
                    sqlite.InsertWhenNotExists(conn, personsName);
                    AliasData = sqlite.AliasData(conn, personsName);
                    alias = AliasData[0];
                    AliasSet = int.Parse(AliasData[1]);
                    if (AliasSet == 1)
                    {
                        personsName = alias;
                    }
                    else
                    {
                        sqlite.InsertData(conn, personsName, "No Alias");
                    }
                    RadioButton RadioButtonAdding = new RadioButton();
                    panel2.Controls.Add(RadioButtonAdding);
                    RadioButtonAdding.AutoSize = true;
                    RadioButtonAdding.Font = new Font("Segoe UI", 12.00F, FontStyle.Bold, GraphicsUnit.Point, 0);
                    RadioButtonAdding.Location = new Point(10, (10 + margin));
                    RadioButtonAdding.Name = subdirs[i];
                    RadioButtonAdding.Size = new Size(118, 21);
                    RadioButtonAdding.TabIndex = 0;
                    RadioButtonAdding.CheckedChanged += new EventHandler(radioButtons_CheckedChanged);
                    RadioButtonAdding.Text = personsName;
                    RadioButtonAdding.UseVisualStyleBackColor = true;
                    if (i == 0) { RadioButtonAdding.Checked = true; }
                    margin = margin + 25;
                }
            }
            catch (Exception ex)
            {
                LogThis(ex);
            }
        }
        private void LoadNewPict(string SetPictureLocation, object NextPic)
        {
            try
            {
                i = trackBar1.Value;
                pictureBox1.Image = Image.FromFile($@"{SetPictureLocation}\{NextPic}");

                FileInfo TimeOfPicture = new FileInfo($@"{SetPictureLocation}\{NextPic}");
                label1.Text = TimeOfPicture.CreationTime.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem loading the screenshot for the selected timeslot.");
                LogThis(ex);
            }
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            try
            {
                string PictureLocation = $@"{CurrentPictureLocation}\{NameOfPerson}\{dateTimePicker2.Value.ToString("MM-dd-yy")}";
                DirectoryInfo di = new DirectoryInfo(PictureLocation);
                var fileNames = from f in di.EnumerateFiles()
                                where f.Extension == ".jpeg"
                                select f;
                List<FileInfo> pics = new List<FileInfo>(fileNames.OrderBy(x => x.CreationTime).ToList());
                files = Directory.GetFiles(PictureLocation, $"*{Extension}");
                var Filenames = files.Select(Path.GetFileName);
                int PictureCount = files.Length;
                i = trackBar1.Value;
                string NextPic = pics[i].Name;
                LoadNewPict(PictureLocation, NextPic);
            }
            catch (Exception ex)
            {
                LogThis(ex);
            }
        }
        void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                RadioButton rb = sender as RadioButton;
                if (rb != null)
                {
                    if (rb.Checked)
                    {
                        NameOfPerson = rb.Name;
                        FileSystemInfo LastDirectory = new DirectoryInfo($@"{CurrentPictureLocation}\{NameOfPerson}").GetFileSystemInfos().OrderBy(fi => fi.CreationTime).Last();
                        FileSystemInfo FirstDirectory = new DirectoryInfo($@"{CurrentPictureLocation}\{NameOfPerson}").GetFileSystemInfos().OrderBy(fi => fi.CreationTime).First();
                        dateTimePicker2.MaxDate = DateTime.Now;
                        dateTimePicker2.MinDate = FirstDirectory.CreationTime;
                        dateTimePicker2.MaxDate = LastDirectory.CreationTime;
                        dateTimePicker2.Value = LastDirectory.CreationTime;
                        SetPictureLocation();
                    }
                }
            }
            catch (Exception ex)
            {
                LogThis(ex);
            }
        }
        public void SetPictureLocation()
        {
            try
            {
                var checkedButton = panel2.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);

                string PersonsName = checkedButton.Name;
                trackBar1.Focus();
                trackBar1.Value = 0;
                DirectoryInfo di = new DirectoryInfo($@"{CurrentPictureLocation}\{PersonsName}\{dateTimePicker2.Value.ToString("MM-dd-yy")}");
                var files = from f in di.EnumerateFiles()
                            where f.Extension == ".jpeg"
                            select f;
                int PictureCount = files.Count();
                List<FileInfo> pics = new List<FileInfo>(files.OrderBy(x => x.CreationTime).ToList());
                i = trackBar1.Value;
                string NextPic = pics[i].Name;

                trackBar1.Maximum = PictureCount - 1;

                LoadNewPict($@"{CurrentPictureLocation}\{PersonsName}\{dateTimePicker2.Value.ToString("MM-dd-yy")}", NextPic);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No activity stored for the selected day.");
                LogThis(ex);
            }
        }
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            {
                try
                {
                    SetPictureLocation();
                }
                catch (Exception ex)
                { Console.WriteLine(ex.ToString()); }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                var checkedButton = panel2.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
                string Alias = textBox1.Text;
                checkedButton.Text = Alias;
                sqlite.UpdateAlias(conn, checkedButton.Name, Alias);
                textBox1.Text = "";
            }
            catch
            {
                MessageBox.Show("Choose A User to Assign an Alias");
            }
        }
        internal void LogThis(Exception ex)
        {
            using (StreamWriter w = File.AppendText($@"{workingDirectory}\sliderlog.txt"))
            {
                MessageBox.Show(ex.ToString());
                Log(ex.ToString(), w);
            }
        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            w.WriteLine("  :");
            w.WriteLine($"  :{logMessage}");
            w.WriteLine("-------------------------------");
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var checkedButton = panel2.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
            string personDeleting = checkedButton.Name;
            string personsDirectory = $@"{ CurrentPictureLocation}\{personDeleting}";
            MessageBox.Show(personsDirectory + " " + personDeleting);
            if (Directory.Exists(personsDirectory))
            {
                Directory.Delete(personsDirectory, true);
            }
            sqlite.RemovePerson(personDeleting);
            Form1_Load(null, EventArgs.Empty);
        }
    }
}
