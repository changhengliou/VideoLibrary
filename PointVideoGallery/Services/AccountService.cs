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
                        "SELECT `Id`, `UserName`, `Email`, `Enable`, `EnableResource`, `EnablePublish`, `EnableLocation`, `EnableSo`, `EnableEvent`, `EnableAdmin` FROM `user` WHERE " +
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

        public async Task<bool> CreateUserAsync(UserInfo user)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    if (await connection.ExecuteAsync(
                            "INSERT INTO `user` (`UserName`, `Password`, `Email`, `Enable`) VALUE " +
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

        /// <summary>
        /// Get all information of users
        /// </summary>
        public async Task<List<UserInfo>> GetUsersAsync()
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                List<UserInfo> list = new List<UserInfo>();
                try
                {
                    await connection.OpenAsync();
                    list = (await connection.QueryAsync<UserInfo>("SELECT `Id`, `UserName`, `Email`, `Enable` " +
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

        /// <summary>
        /// Get all roles of users
        /// </summary>
        public async Task<List<UserRole>> GetRolesOfUsersAsync()
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                List<UserRole> list = new List<UserRole>();
                try
                {
                    await connection.OpenAsync();
                    list = (await connection.QueryAsync<UserRole>(
                        "SELECT `Id`, `UserName`, `EnableResource`, `EnablePublish`, `EnableLocation`, `EnableSo`, `EnableEvent`, `EnableAdmin` " +
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

        /// <summary>
        /// Update role of user by id
        /// </summary>
        public async Task<bool> UpdateRolesOfUserByIdAsync(UserRole role)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    if (await connection.ExecuteAsync(
                            "UPDATE `user` SET `EnableResource`=@EnableResource, `EnablePublish`=@EnablePublish, " +
                            "`EnableLocation`=@EnableLocation, `EnableSo`=@EnableSo, `EnableEvent`=@EnableEvent, " +
                            "`EnableAdmin`=@EnableAdmin WHERE `Id`=@Id;", role) != 1)
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

        /// <summary>
        /// get user info by user id
        /// </summary>
        public async Task<UserInfo> GetUserByUserIdAsync(int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                UserInfo user = null;
                try
                {
                    await connection.OpenAsync();
                    user = await connection.QueryFirstOrDefaultAsync<UserInfo>(
                        "SELECT `Id`, `UserName`, `Email`, `Enable`, `Password` " +
                        "FROM `user` WHERE `Id`=@id;", new {id = id});
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
                await connection.CloseAsync();
                return user;
            }
        }

        /// <summary>
        /// Get user by userName
        /// </summary>
        public async Task<UserInfo> GetUserByUserNameAsync(string name)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                UserInfo user = null;
                try
                {
                    await connection.OpenAsync();
                    user = await connection.QueryFirstOrDefaultAsync<UserInfo>(
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

        /// <summary>
        /// update user data (id, password, email)
        /// </summary>
        public async Task<bool> UpdateUserAsync(UserInfo user)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    if (await connection.ExecuteAsync(
                            "Update `user` SET `UserName`=@userName, `Password`=@password, " +
                            "`Email`=@email, `Enable`=@enable WHERE Id=@id;", new
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

        /// <summary>
        /// set user enable / disable
        /// </summary>
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
                        status = status
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

        /// <summary>
        /// Delete user by id
        /// </summary>
        public async Task<bool> DeleteUserByIdAsync(int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    if (await connection.ExecuteAsync("DELETE FROM `user` WHERE `Id`= @id ;", new {id = id}) != 1)
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