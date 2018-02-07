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
    public class LogService : IService
    {
        public static string ConnectionString = ConfigurationManager.AppSettings.Get("MySqlConnectionString");

        public async Task<List<Log>> GetLogsAsync(int days, int? userId, int limit = 100)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var logs = new List<Log>();
                try
                {
                    await connection.OpenAsync();
                    logs = (await connection.QueryAsync<Log>(
                        "SELECT a.`Action`, a.`ActionTime`, a.`UserId`, b.`UserName` " +
                        "FROM `log` AS a " +
                        "INNER JOIN `user` AS b ON b.`Id`=a.`UserId` " +
                        $"WHERE a.ActionTime < DATE_ADD(now(), INTERVAL {days} DAY) " +
                        (userId.HasValue ? "AND a.`UserId`=@userId " : string.Empty) +
                        $"ORDER BY `ActionTime` DESC LIMIT {limit};", new {userId = userId ?? 0})).ToList();
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                    throw;
                }
                await connection.CloseAsync();
                return logs;
            }
        }

        /// <summary>
        /// Write log to db
        /// </summary>
        public static async Task<bool> WriteLogAsync(Log log)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    if (await connection.ExecuteAsync("INSERT INTO `log` (`Action`, `ActionTime`, `UserId`) VALUES  " +
                                                      "(@Action, @ActionTime, @UserId);", log) != 1)
                        throw new SqlExecutionException();
                    await connection.CloseAsync();
                    return true;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                    await connection.CloseAsync();
                    return false;
                }
            }
        }

        /// <summary>
        /// Write log to db
        /// </summary>
        public static async Task<bool> WriteLogWithUserIdReference(Log log, int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    var user = await connection.QueryFirstAsync<User>("SELECT * FROM `user` WHERE `Id`=@id",
                        new {id = id});

                    if (user != null && !string.IsNullOrWhiteSpace(user.UserName))
                        log.Action = log.Action.Replace("@id", user.UserName);
                    else
                        log.Action = log.Action.Replace("@id", id.ToString());

                    if (await connection.ExecuteAsync("INSERT INTO `log` (`Action`, `ActionTime`, `UserId`) VALUES  " +
                                                      "(@Action, @ActionTime, @UserId);", log) != 1)
                        throw new SqlExecutionException();
                    await connection.CloseAsync();
                    return true;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                    await connection.CloseAsync();
                    return false;
                }
            }
        }

        /// <summary>
        /// Write log to db
        /// </summary>
        public static async Task<bool> WriteLogWithEventIdReference(Log log, int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    var adEvent = await connection.QueryFirstAsync<AdEvent>("SELECT `Id`, `Name` FROM `ad_events` WHERE `Id`=@id",
                        new { id = id });

                    if (adEvent != null && !string.IsNullOrWhiteSpace(adEvent.Name))
                        log.Action = log.Action.Replace("@id", adEvent.Name);
                    else
                        log.Action = log.Action.Replace("@id", id.ToString());

                    if (await connection.ExecuteAsync("INSERT INTO `log` (`Action`, `ActionTime`, `UserId`) VALUES  " +
                                                      "(@Action, @ActionTime, @UserId);", log) != 1)
                        throw new SqlExecutionException();
                    await connection.CloseAsync();
                    return true;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                    await connection.CloseAsync();
                    return false;
                }
            }
        }
    }
}