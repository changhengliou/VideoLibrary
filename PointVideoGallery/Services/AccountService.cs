using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Management;
using Dapper;
using DryIoc;
using MySql.Data.MySqlClient;
using PointVideoGallery.Models;

namespace PointVideoGallery.Services
{
    public class AccountService : IService
    {
        public static string ConnectionString = ConfigurationManager.AppSettings.Get("MySqlConnectionString");

        public async Task<User> SignInAsync(string userName, string password)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                User user = null;
                try
                {
                    await connection.OpenAsync();
                    user = await connection.QueryFirstOrDefaultAsync<User>(
                        "SELECT `Id`, `UserName`, `Email`, `Enable` FROM `user` WHERE " +
                        "`UserName`=@userName AND `Password`=@password;", new
                        {
                            userName = userName,
                            password = password
                        });
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
                await connection.CloseAsync();
                return user;
            }
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    if (await connection.ExecuteAsync(
                            "INSERT INTO `User` (`UserName`, `Password`, `Email`, `Enable`) VALUE " +
                            "(@userName, @password, @email, @enable)", new
                            {
                                userName = user.UserName,
                                password = user.Password,
                                email = user.Email,
                                enable = user.Enable
                            }) != 1)
                        throw new SqlExecutionException();
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

        public async Task<List<User>> GetUsersAsync()
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                List<User> list = new List<User>();
                try
                {
                    await connection.OpenAsync();
                    list = (await connection.QueryAsync<User>("SELECT `Id`, `UserName`, `Email`, `Enable` " +
                                                              "FROM `user`;")).ToList();
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
                await connection.CloseAsync();
                return list;
            }
        }

        public async Task<User> GetUserByUserNameAsync(string name)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                User user = null;
                try
                {
                    await connection.OpenAsync();
                    user = await connection.QueryFirstOrDefaultAsync<User>(
                        "SELECT `Id`, `UserName`, `Email`, `Enable` " +
                        "FROM `user` WHERE `UserName`=@name;", new {name = name});
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
                await connection.CloseAsync();
                return user;
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    if (await connection.ExecuteAsync(
                            "Update `user` SET `UserName`=@userName, `Password`=@password, " +
                            "`Email`=@email, `Enable`=@enable) WHERE Id=@id;", new
                            {
                                id = user.Id,
                                userName = user.UserName,
                                password = user.Password,
                                email = user.Email,
                                enable = user.Enable
                            }) != 1)
                        throw new SqlExecutionException();
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

        public async Task<bool> SetUserEnableStatusByIdAsync(int id, int status)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    if (await connection.ExecuteAsync("UPDATE `user` SET `Enable` = @status " +
                                                      "WHERE `Id`= @id ;", new
                    {
                        id = id,
                        status=status
                    }) != 1)
                        throw new SqlExecutionException();
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
    }
}