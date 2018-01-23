using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Management;
using Dapper;
using MySql.Data.MySqlClient;
using PointVideoGallery.Models;

namespace PointVideoGallery.Services
{
    public class SettingService : IService
    {
        public static string ConnectionString = ConfigurationManager.AppSettings.Get("MySqlConnectionString");

        /// <summary>
        /// Get So settings from db
        /// </summary>
        public async Task<List<SoSetting>> GetSoSettingsAsync()
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                List<SoSetting> list = null;
                try
                {
                    await connection.OpenAsync();
                    list = (await connection.QueryAsync<SoSetting>("SELECT * FROM `so_settings`;")).ToList();
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }

                await connection.CloseAsync();
                return list;
            }
        }

        /// <summary>
        /// Add new so setting to db
        /// </summary>
        public async Task<bool> AddSoSettingAsync(SoSetting setting)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                bool isSuccess = true;
                try
                {
                    await connection.OpenAsync();
                    if (await connection.ExecuteAsync("INSERT INTO `so_settings` (Code, Name) VALUES (@code, @name);",
                            new {code = setting.Code, name = setting.Name}) != 1)
                        throw new SqlExecutionException("Insert failed");
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                    isSuccess = false;
                }

                await connection.CloseAsync();
                return isSuccess;
            }
        }

        /// <summary>
        /// Update so setting to db
        /// </summary>
        public async Task<bool> UpdateSoSettingByIdAsync(SoSetting setting)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                bool isSuccess = true;
                try
                {
                    await connection.OpenAsync();
                    if (await connection.ExecuteAsync("UPDATE `so_settings` SET Name=@name, Code=@code WHERE Id=@id;",
                            new {id = setting.Id, code = setting.Code, name = setting.Name}) != 1)
                        throw new SqlExecutionException("Update failed");
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                    isSuccess = false;
                }

                await connection.CloseAsync();
                return isSuccess;
            }
        }

        /// <summary>
        /// Delete so setting from db
        /// </summary>
        public async Task<bool> RemoveSoSettingByIdAsync(int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                bool isSuccess = true;
                try
                {
                    await connection.OpenAsync();
                    if (await connection.ExecuteAsync("DELETE FROM `so_settings` WHERE Id=@id;", new {id = id}) != 1)
                        throw new SqlExecutionException("Delete failed");
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                    isSuccess = false;
                }

                await connection.CloseAsync();
                return isSuccess;
            }
        }

        /// <summary>
        /// Get so setting by id
        /// </summary>
        public async Task<SoSetting> GetSoSettingByIdAsync(int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                SoSetting setting = null;
                try
                {
                    await connection.OpenAsync();
                    setting = await connection.QueryFirstAsync<SoSetting>("SELECT * FROM `so_settings` " +
                                                                          "WHERE Id=@id LIMIT 1;", new {id = id});
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }

                await connection.CloseAsync();
                return setting;
            }
        }
    }
}