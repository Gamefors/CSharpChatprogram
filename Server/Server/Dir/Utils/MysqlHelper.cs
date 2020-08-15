using MySql.Data.MySqlClient;
using System;

namespace Server
{
    class MysqlHelper
    {
        private static LogHelper logHelper;
        private static MySqlConnection mysqlConnection;

        public MysqlHelper(LogHelper logHelper_)
        {
            logHelper = logHelper_;
        }

        private static void ExecuteCMD(String statement)
        {
            MySqlCommand CMD = new MySqlCommand();
            CMD.Connection = mysqlConnection;
            CMD.CommandText = statement;
            CMD.ExecuteNonQuery();
        }

        public bool Connect(String ip, String username, String password, String database)
        {
            string connectionString = @"server=" + ip + ";userid=" + username + ";password=" + password + ";database=" + database;
            mysqlConnection = new MySqlConnection(connectionString);
            MySqlCommand createDefaultTableCMD = new MySqlCommand();
            try
            {
                mysqlConnection.Open();
                logHelper.Log("Connection to mysql database established. Mysql Server version: " + mysqlConnection.ServerVersion, LogType.Info);
                createDefaultTableCMD.Connection = mysqlConnection;
                createDefaultTableCMD.CommandText = @"CREATE TABLE IF NOT EXISTS accounts(id INTEGER PRIMARY KEY AUTO_INCREMENT, name TEXT, password TEXT, loggedIn TEXT)";
                createDefaultTableCMD.ExecuteNonQuery();
                ExecuteCMD(@"UPDATE accounts SET loggedIn = 'false'");
                return true;
            }
            catch (MySqlException ex)
            {
                logHelper.Log("Connection to mysql database could not be established.", LogType.Error);
                return false;
            }
        }

        public bool CheckLoginCredentials(Client client, String password)
        {

            string sql = "SELECT * FROM accounts WHERE name = '" + client.name + "' and password = '" + password + "'";
            MySqlCommand cmd = new MySqlCommand(sql, mysqlConnection);
            using MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Close();
                SetClientLoginStatus(client, true);
                return true;
            }
            else
            {
                rdr.Close();
                return false;
            }
        }

        public void SetClientLoginStatus(Client client, bool status)
        {
            client.loggedIn = status;
            ExecuteCMD(@"UPDATE accounts SET loggedIn = '" + status + "' WHERE name = '" + client.name + "'");
        }

        
    }
}
