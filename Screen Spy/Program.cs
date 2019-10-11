using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace ScreenSpy
{

    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool FreeConsole();
        private static string Compare2 = null;
        private static string CurrentUser = Environment.UserName;
        private static List<bool> GetHash(Bitmap bmpSource)
        {
            List<bool> lResult = new List<bool>();
            Bitmap bmpMin = new Bitmap(bmpSource, new Size(1024, 1024));

            for (int j = 0; j < bmpMin.Height; j++)
            {
                for (int i = 0; i < bmpMin.Width; i++)
                {
                    lResult.Add(bmpMin.GetPixel(i, j).GetBrightness() < 0.5f);
                }
            }
            bmpMin.Dispose();
            bmpSource.Dispose();
            return lResult;
        }
        private static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            w.WriteLine("  :");
            w.WriteLine($"  :{logMessage}");
            w.WriteLine("-------------------------------");
        }
        private static void LogThis(Exception ex)
        {
            using (StreamWriter w = File.AppendText("sshots.txt"))
            {
                Log(ex.ToString(), w);
            }
        }
        static void Main(string[] args)
        {
            string[] sSpySettings = new string[5];
            spySqlite sqlite = new spySqlite();
            string currentWorkingDirectory = Directory.GetCurrentDirectory();
            string UserPhotosDirectory = $@"{Directory.GetCurrentDirectory()}\Users";
            if (!Directory.Exists(UserPhotosDirectory))
            {
                Directory.CreateDirectory(UserPhotosDirectory);
            }
            SQLiteConnection sqlite_conn;
            if (!File.Exists("ScreenSpy.db"))
            {
                sqlite_conn = sqlite.CreateConnection();
                sqlite.CreateTable(sqlite_conn);
                sqlite.InsertData(sqlite_conn);
                sSpySettings = sqlite.ReadData(sqlite_conn);
            }
            else
            {
                sqlite_conn = sqlite.CreateConnection();
                sSpySettings = sqlite.ReadData(sqlite_conn);
            }
            int ImageWidth = int.Parse(sSpySettings[0]);
            int ImageHeight = int.Parse(sSpySettings[1]);
            string ProgramDirectory = sSpySettings[2];
            int DuplicateImgSensitivity = int.Parse(sSpySettings[3]);
            int CaptureFrequency = int.Parse(sSpySettings[4]);
            int ColorPictures = int.Parse(sSpySettings[5]);
            //Console.Write(ColorPictures);
            string newWorkingDirectory = $@"{ProgramDirectory}\Users";
            string filepath = $@"{newWorkingDirectory}\{CurrentUser}";
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            Encoder myEncoder = Encoder.Quality;
            EncoderParameter MyCompression = new EncoderParameter(myEncoder, Config.Default.Endcoder);
            try
            {
                void FullScreenshotWithClass(string fpath, string Extension, ImageFormat format, EncoderParameter Compression)
                {
                    using (ScreenCapture sc = new ScreenCapture())
                    {
                        try
                        {
                            FreeConsole();
                            do
                            {
                                Image img = sc.CaptureScreen();

                                DateTime TodayTime = DateTime.Now;
                                if (!Directory.Exists($@"{filepath}\{TodayTime.ToString("MM-dd-yy")}"))
                                {
                                    Directory.CreateDirectory($@"{filepath}\{TodayTime.ToString("MM-dd-yy")}");
                                }
                                string fullpath = $@"{filepath}\{TodayTime.ToString("MM-dd-yy")}\{TodayTime.ToString("hmmtt.ss")}{Extension}";
                                string Compare1 = fullpath;
                                //Console.WriteLine(" Compare2 is " + Compare2);
                                if (Compare2 != null)
                                {
                                    try
                                    {
                                        int Equality = 0;
                                        img.Save(fullpath, format);
                                        if (ColorPictures == 0)
                                        {
                                            Resizer resize = new Resizer();
                                            resize.Resizing(fullpath, ImageWidth, ImageHeight);
                                        }
                                        List<bool> iHash1 = GetHash(new Bitmap(Compare1));
                                        List<bool> iHash2 = GetHash(new Bitmap(Compare2));
                                        int equalElements = iHash1.Zip(iHash2, (i, j) => i == j).Count(eq => eq);
                                        Equality = equalElements;
                                        img.Dispose();
                                        if (Equality > DuplicateImgSensitivity)
                                        {
                                            GC.Collect();
                                            sc.Dispose();
                                            Console.WriteLine(Compare1);
                                            Console.WriteLine(Compare2);
                                            File.Delete(Compare2);

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LogThis(ex);
                                    }
                                }
                                else
                                {
                                    img.Save(fullpath, format);
                                    if (ColorPictures == 0)
                                    {
                                        Resizer resize = new Resizer();
                                        resize.Resizing(fullpath, ImageWidth, ImageHeight);
                                    }
                                }
                                Compare2 = fullpath;
                                Thread.Sleep(CaptureFrequency * 1000);
                                GC.Collect();
                                sc.Dispose();
                                img.Dispose();
                            } while (true);
                        }
                        catch (Exception ex)
                        {
                            LogThis(ex);
                        }
                    }
                }
                try
                {
                    DateTime TodayTime = DateTime.Now;
                    string Extension = ".jpeg";
                    if (!File.Exists($@"{filepath}\{TodayTime.ToString("MM-dd-yy")}"))
                    {
                        Directory.CreateDirectory($@"{filepath}\{TodayTime.ToString("MM-dd-yy")}");
                    }
                    FullScreenshotWithClass(filepath, Extension, ImageFormat.Jpeg, MyCompression);
                }
                catch (Exception ex)
                {
                    LogThis(ex);
                }
            }
            catch (Exception ex)
            {
                LogThis(ex);
            }
        }
    }
}
