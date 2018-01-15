using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Dapper;
using MySql.Data.MySqlClient;
using PointVideoGallery.Models;

namespace PointVideoGallery.Services
{
    public class VideoFileService : IService
    {
        /// <summary>
        /// Get videoFiles
        /// </summary>
        public async Task<List<VideoFile>> GetVideoFiles()
        {
            using (var connection = new MySqlConnection(ConfigurationManager.AppSettings.Get("MySqlConnectionString")))
            {
                try
                {
                    await connection.OpenAsync();
                    var data = await connection.QueryAsync<VideoFile>(sql: "SELECT * FROM VideoFile");
                    return data.ToList();
                }
                catch (Exception e)
                {
                    if (e is MySqlException || e is TimeoutException)
                        Trace.WriteLine(e);
                    else
                        throw e;
                }
                await connection.CloseAsync();
                return null;
            }
        }


        public async Task AddVideoFiles(VideoFile file)
        {
            using (var connection = new MySqlConnection(ConfigurationManager.AppSettings.Get("MySqlConnectionString")))
            {
                try
                {
                    await connection.OpenAsync();
                    await connection.ExecuteAsync(
                        "INSERT INTO VideoFile (FileId, FileName, FileSize, FileLocation, FileModifiedTime, VideoLength) VALUES " +
                        "(@id, @name, @size, @location, @modifiedTime, @length);",
                        new
                        {
                            id = file.FileId,
                            name = file.FileName,
                            size = file.FileSize,
                            location = file.FileLocation,
                            modifiedTime = file.FileModifiedTime,
                            length = file.VideoLength
                        });
                }
                catch (Exception e)
                {
                    if (e is MySqlException || e is TimeoutException)
                        Trace.WriteLine(e);
                    else
                        throw e;
                }
                await connection.CloseAsync();
            }
        }

        public async Task RemoveVideoFiles()
        {
            using (var connection = new MySqlConnection(ConfigurationManager.AppSettings.Get("MySqlConnectionString")))
            {
                try
                {
                    await connection.OpenAsync();
                    var affectedRows = await connection.ExecuteAsync(sql: "");
                    var data = await connection.QueryAsync<VideoFileService>(sql: "");
                    var d = await connection.QueryFirstAsync<VideoFileService>(sql: "");
                }
                catch (Exception e)
                {
                    if (e is MySqlException || e is TimeoutException)
                        Trace.WriteLine(e);
                    else
                        throw e;
                }
                await connection.CloseAsync();
            }
        }

        public async Task GetFilesInDirectory()
        {
            using (var connection = new MySqlConnection(ConfigurationManager.AppSettings.Get("MySqlConnectionString")))
            {
                try
                {
                    await connection.OpenAsync();
                    var affectedRows = await connection.ExecuteAsync(sql: "");
                    var data = await connection.QueryAsync<VideoFileService>(sql: "");
                    var d = await connection.QueryFirstAsync<VideoFileService>(sql: "");
                }
                catch (Exception e)
                {
                    if (e is MySqlException || e is TimeoutException)
                        Trace.WriteLine(e);
                    else
                        throw e;
                }
                await connection.CloseAsync();
            }
        }
    }
}