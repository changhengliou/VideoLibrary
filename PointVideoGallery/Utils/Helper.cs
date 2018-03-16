using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using Dapper;
using MySql.Data.MySqlClient;
using PointVideoGallery.Models;

namespace PointVideoGallery.Utils
{
    public class Helper
    {
        public static int GetUserId(HttpRequestMessage request)
        {
            var claim = request.GetOwinContext().Authentication.User.FindFirst(s => s.Type == ClaimTypes.NameIdentifier);
            return Convert.ToInt32(claim.Value);
        }

        public static void MysqlConnectionTest()
        {
            using (var connection = new MySqlConnection(ConfigurationManager.AppSettings.Get("MySqlConnectionString")))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception)
                {
                    Trace.WriteLine("Unable to connect MySQL Database.");
                    throw;
                }
                connection.Close();
            }
        }
    }
}