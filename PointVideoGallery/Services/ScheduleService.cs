using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Management;
using Dapper;
using MySql.Data.MySqlClient;
using PointVideoGallery.Models;

namespace PointVideoGallery.Services
{
    public class ScheduleService
    {
        public static string ConnectionString = ConfigurationManager.AppSettings.Get("MySqlConnectionString");

        /// <summary>
        /// add a schedule to db
        /// </summary>
        public async Task<bool> AddScheduleAsync(DateTime scheduleStart, DateTime scheduleEnd, int eventId)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var now = DateTime.Now;
                var sqlBuilder =
                    new StringBuilder("INSERT INTO `schedule` (`ScheduleDate`, `CreateDate`, `EventId`) VALUES ");

                for (var s = scheduleStart; s <= scheduleEnd; s = s.AddDays(1))
                {
                    sqlBuilder.Append($"('{s:yyyy-MM-dd}', '{now:yyyy-MM-dd HH:mm:ss}', '{eventId}')");
                    if (s.AddDays(1) <= scheduleEnd)
                        sqlBuilder.Append(", ");
                }
                sqlBuilder.Append(";");

                try
                {
                    await connection.OpenAsync();
                    await connection.ExecuteAsync(sqlBuilder.ToString());
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                    await connection.CloseAsync();
                    return false;
                }
                await connection.CloseAsync();
                return true;
            }
        }

        /// <summary>
        /// Get schedules by date
        /// </summary>
        public async Task<List<ScheduleEvent>> GetSchedulesByDateAsync(DateTime date)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    var list = (await connection.QueryAsync<ScheduleEvent>(
                        "SELECT `s`.`*`, `a`.`Name` FROM `schedule` AS s " +
                        "LEFT JOIN `ad_events` AS a ON `s`.`EventId`=`a`.`Id` " +
                        "WHERE `s`.`ScheduleDate`=@scheduleDate;",
                        new {scheduleDate = date.ToString("yyyy-MM-dd")})
                    ).ToList();
                    await connection.CloseAsync();
                    return list;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                    await connection.CloseAsync();
                    return null;
                }
            }
        }

        public async Task<bool> DropScheduleByIdAsync(int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    if (await connection.ExecuteAsync("DELETE FROM `schedule` WHERE Id=@id AND `ScheduleDate` > now();", new {id = id}) != 1)
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