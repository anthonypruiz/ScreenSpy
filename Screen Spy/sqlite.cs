using System;
using System.Data.SQLite;
using System.IO;

namespace ScreenSpy
{
    public class spySqlite
    {
        public SQLiteConnection CreateConnection()
        {

            SQLiteConnection sqlite_conn;
            sqlite_conn = new SQLiteConnection(@"Data Source=ScreenSpy.db; Version = 3;");
            try
            {
                sqlite_conn.Open();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return sqlite_conn;
        }

        public void CreateTable(SQLiteConnection conn)
        {
            try
            {
                SQLiteCommand sqlite_cmd;
                string Createsql = "CREATE TABLE ScreenSpy(ID INTEGER PRIMARY KEY, ImageWidth INT, ImageHeight INT, ProgramDirectory VARCHAR, DuplicateImgSensitivity INT, CaptureFrequency INT, ColorPictures INT);";
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = Createsql;
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void InsertData(SQLiteConnection conn)
        {
            try
            {
                string workingDirectory = "'" + Directory.GetCurrentDirectory() + "'";
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = $"INSERT INTO ScreenSpy(ImageWidth, ImageHeight, ProgramDirectory, DuplicateImgSensitivity, CaptureFrequency, ColorPictures) VALUES(960, 540, {workingDirectory}, 1045000, 10, 0);";
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public string[] ReadData(SQLiteConnection conn)
        {
            try
            {
                string[] sSpySettings = new string[6];
                SQLiteDataReader sdr;
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = "SELECT * FROM ScreenSpy;";
                sdr = sqlite_cmd.ExecuteReader();
                while (sdr.Read())
                {
                    sSpySettings[0] = sdr.GetInt32(1).ToString();
                    sSpySettings[1] = sdr.GetInt32(2).ToString();
                    sSpySettings[2] = sdr.GetString(3);
                    sSpySettings[3] = sdr.GetInt32(4).ToString();
                    sSpySettings[4] = sdr.GetInt32(5).ToString();
                    sSpySettings[5] = sdr.GetInt32(6).ToString();
                    return sSpySettings;
                }
                return sSpySettings;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }
    }
}
