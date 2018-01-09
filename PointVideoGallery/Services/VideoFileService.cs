using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using MySql.Data.MySqlClient;

namespace PointVideoGallery.Services
{
    public class VideoFileService : IService
    {
        public int GetValue => 3;

        public void GetFilesInDirectory()
        {
//            using (var connection = new MySqlConnection())
//            {
//                connection.OpenAsync();
//                connection.Execute()
//                connection.CloseAsync();
//            }
        }
    }
}