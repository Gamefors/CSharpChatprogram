using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

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

        private static void ExecuteCMD(string statement)
        {
            MySqlCommand CMD = new MySqlCommand();
            CMD.Connection = mysqlConnection;
            CMD.CommandText = statement;
            CMD.ExecuteNonQuery();
        }

        public bool Connect(string ip, string username, string password, string database)
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
            catch (MySqlException)
            {
                logHelper.Log("Connection to mysql database could not be established.", LogType.Error);
                return false;
            }
        }

        public bool CheckLoginCredentials(Client client, string password)
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

        public string getClientRank(Client client)
        {
            string rank = "User";
            string sql = "SELECT rank FROM accounts WHERE name = '" + client.name + "'";
            MySqlCommand cmd = new MySqlCommand(sql, mysqlConnection);
            using MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                rank = rdr.GetString(0);
                rdr.Close();
            }
            else
            {
                rdr.Close();
            }
            return rank;
        }

        public List<string> GetOnlineClients()
        {
            string sql = "SELECT * FROM accounts WHERE loggedIn = 'True'";
            MySqlCommand cmd = new MySqlCommand(sql, mysqlConnection);
            using MySqlDataReader rdr = cmd.ExecuteReader();
            List<string> onlineClients = new List<string>();
            while (rdr.Read())
            {
                onlineClients.Add(rdr.GetString(1) + ";" + rdr.GetString(4) + ",");
            }
            rdr.Close();
            return onlineClients;
        }

    }
}
