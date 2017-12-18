using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace VideoLibrary.Entity
{
    public class DbConnection : IDisposable
    {
        public MySqlConnection Connection;

        public DbConnection() 
        {
            MySqlConnection conn = new MySqlConnection("");
            conn.Open();
            string sql = "SELECT Name, HeadOfState FROM Country WHERE Continent='Oceania'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            
            MySqlDataReader rdr = cmd.ExecuteReader();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);
            var z = cmd.ExecuteReader();
            var w = z[0];
            while (rdr.Read())
            {
                Console.WriteLine(rdr[0] + " -- " + rdr[1]);
            }
            rdr.Close();
        }

        public static DbConnection Create()
        {
            return new DbConnection();
        }

        public void Dispose()
        {
            Connection.Close();
        }
    }
}