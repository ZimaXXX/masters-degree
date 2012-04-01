using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Collections;

namespace WPFApp
{
    public class DbUtils
    {
        public MySqlConnection Connection { get; set; }

        public DbUtils(string serverAddress, string schema, string login, string password)
        {
            InitializeConnector(serverAddress, schema, login, password);
        }
        ~DbUtils()
        {
            if (Connection != null)
            {
                Connection.Close();
            }
        }

        public void InitializeConnector(string serverAddress, string schema, string login, string password)
        {
            string MyConString = "SERVER=" + serverAddress + ";" +
                    "DATABASE=" + schema + ";" +
                    "UID=" + login + ";" +
                    "PASSWORD=" + password + ";";
            Connection = new MySqlConnection(MyConString);
            //MySqlCommand command = connection.CreateCommand();
            //MySqlDataReader Reader;
            //command.CommandText = "select * from mycustomers";
            //connection.Open();
            //Reader = command.ExecuteReader();
            //while (Reader.Read())
            //{
            //    string thisrow = "";
            //    for (int i = 0; i < Reader.FieldCount; i++)
            //        thisrow += Reader.GetValue(i).ToString() + ",";
            //    listBox1.Items.Add(thisrow);
            //}
            //connection.Close();
        }

        public ArrayList PerformMySQLCommand(string sql)
        {
            Connection.Open();
            MySqlCommand command = Connection.CreateCommand();
            command.CommandText = sql;
            MySqlDataReader Reader;
            Reader = command.ExecuteReader();
            ArrayList rows = new ArrayList();
            
            while (Reader.Read())
            {
                ArrayList row = new ArrayList();
                for (int i = 0; i < Reader.FieldCount; i++)
                {
                    row.Add(Reader.GetValue(i).ToString());
                }
                rows.Add(row);
            }
            Connection.Close();
            return rows;
        }

        public void CloseConnection()
        {
            if (Connection != null)
                Connection.Close();
        }
    }
}
