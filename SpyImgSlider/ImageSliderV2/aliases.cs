using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace SpyImgSlider
{
    public class SliderSqlite
    {
        public SQLiteConnection CreateConnection()
        {

            SQLiteConnection sqlite_conn;
            sqlite_conn = new SQLiteConnection("Data Source=Aliases.db; Version = 3;");
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
                string Createsql = "CREATE TABLE Aliases(ID INTEGER PRIMARY KEY, UserName VARCHAR NOT NULL, Alias VARCHAR NULL, AliasSet INTEGER NULL)";
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = Createsql;
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public void InsertData(SQLiteConnection conn, string UserName, string Alias)
        {
            try
            {
                UserName = "'" + UserName + "'";
                Alias = "'" + Alias + "'";
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = $"INSERT INTO Aliases(UserName, Alias, AliasSet) VALUES({UserName}, {Alias}, 1);";
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public void InsertWhenNotExists(SQLiteConnection conn, string UserName)
        {
            try
            {
                UserName = "'" + UserName + "'";
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = $"INSERT INTO Aliases(UserName, Alias, AliasSet) SELECT {UserName}, {UserName}, 1 WHERE NOT EXISTS(SELECT 1 FROM Aliases WHERE UserName = {UserName} AND Alias = {UserName} AND AliasSet = 1 );";
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public void UpdateAlias(SQLiteConnection conn, string UserName, string Alias)
        {
            try
            {
                SQLiteCommand command;
                command = conn.CreateCommand();
                UserName = "'" + UserName + "'";
                Alias = "'" + Alias + "'";
                command.CommandText = $"UPDATE Aliases SET Alias = {Alias}, AliasSet = 1 WHERE UserName = {UserName};";
                command.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public string[] AliasData(SQLiteConnection conn, string UserName)
        {
            try
            {
                UserName = "'" + UserName + "'";
                string[] AliasData = new string[2];
                SQLiteDataReader sdr;
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = $"SELECT Alias, AliasSet FROM Aliases WHERE UserName = {UserName};";
                sdr = sqlite_cmd.ExecuteReader();
                while (sdr.Read())
                {
                    AliasData[0] = sdr.GetString(0);
                    AliasData[1] = sdr.GetInt32(1).ToString();
                    return AliasData;
                }
                return AliasData;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        internal string PictureLocation()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=ScreenSpy.db; Version = 3;"))
            {
                try
                {
                    string result;

                    conn.Open();

                    SQLiteCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT ProgramDirectory From ScreenSpy;";
                    result = (cmd.ExecuteScalar() ?? "Not Set").ToString();
                    return $@"{result}\Users";
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show(ex.ToString());
                    return null;
                }

            }
        }
        internal void RemovePerson(string NameOfPerson)
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=Aliases.db; Version = 3;"))
            {
                try
                {

                    conn.Open();
                    SQLiteCommand cmd = conn.CreateCommand();
                    cmd.CommandText = $"DELETE FROM Aliases WHERE UserName = '{NameOfPerson}';";
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }
        }
    }
}
