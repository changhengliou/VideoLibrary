using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using PointVideoGallery.Models;
using PointVideoGallery.Services;
using PointVideoGallery.Utils;

namespace PointVideoGallery.Api
{
    [System.Web.Http.RoutePrefix("api/v1/account")]
    public class AccountApiController : ApiController
    {
        /// <summary>
        /// POST /api/v1/account/signin
        /// </summary>
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("signin")]
        public async Task<IHttpActionResult> Signin([FromBody] PostUserModel data)
        {
            var service = new AccountService();
            var user = await service.SignInAsync(userName: data.N, password: data.P);
            if (user == null)
                return StatusCode(HttpStatusCode.Forbidden);
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            }, DefaultAuthenticationTypes.ApplicationCookie);

            if (user.Enable == 1)
                identity.AddClaim(new Claim(ClaimTypes.Role, Role.AccountEnable));
            if (user.EnableAdmin == 1)
                identity.AddClaim(new Claim(ClaimTypes.Role, Role.Admin));

            var temp = new[]
                {user.EnableSo, user.EnableEvent, user.EnableLocation, user.EnablePublish, user.EnableResource};
            for (var i = 0; i < temp.Length; i++)
            {
                switch (temp[i])
                {
                    case Role.Read:
                        switch (i)
                        {
                            case 0:
                                identity.AddClaim(new Claim(ClaimTypes.Role, Role.SoRead));
                                break;
                            case 1:
                                identity.AddClaim(new Claim(ClaimTypes.Role, Role.EventRead));
                                break;
                            case 2:
                                identity.AddClaim(new Claim(ClaimTypes.Role, Role.LocationRead));
                                break;
                            case 3:
                                identity.AddClaim(new Claim(ClaimTypes.Role, Role.PublishRead));
                                break;
                            case 4:
                                identity.AddClaim(new Claim(ClaimTypes.Role, Role.ResourceRead));
                                break;
                        }
                        break;
                    case Role.ReadWrite:
                        switch (i)
                        {
                            case 0:
                                identity.AddClaim(new Claim(ClaimTypes.Role, Role.SoRead));
                                identity.AddClaim(new Claim(ClaimTypes.Role, Role.SoWrite));
                                break;
                            case 1:
                                identity.AddClaim(new Claim(ClaimTypes.Role, Role.EventRead));
                                identity.AddClaim(new Claim(ClaimTypes.Role, Role.EventWrite));
                                break;
                            case 2:
                                identity.AddClaim(new Claim(ClaimTypes.Role, Role.LocationRead));
                                identity.AddClaim(new Claim(ClaimTypes.Role, Role.LocationWrite));
                                break;
                            case 3:
                                identity.AddClaim(new Claim(ClaimTypes.Role, Role.PublishRead));
                                identity.AddClaim(new Claim(ClaimTypes.Role, Role.PublishWrite));
                                break;
                            case 4:
                                identity.AddClaim(new Claim(ClaimTypes.Role, Role.ResourceRead));
                                identity.AddClaim(new Claim(ClaimTypes.Role, Role.ResourceWrite));
                                break;
                        }
                        break;
                }
            }

            Request.GetOwinContext().Authentication.SignIn(new AuthenticationProperties(), identity);

            // write log
            await LogService.WriteLogAsync(new Log
            {
                Action = "登入",
                ActionTime = DateTime.Now,
                UserId = user.Id
            });
            return Json(new {url = string.IsNullOrWhiteSpace(data.Url) ? "/" : data.Url});
        }

        /// <summary>
        /// GET /api/v1/account/{id}/exist
        /// </summary>
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("{name}/exist")]
        public async Task<IHttpActionResult> CheckUserExistedAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest();

            AccountService service = new AccountService();
            return Json(await service.GetUserByUserNameAsync(name) == null
                ? new {existed = false}
                : new {existed = true});
        }

        /// <summary>
        /// POST /api/v1/account/user/new
        /// </summary>
        [Authorize(Roles = Role.Admin)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("user/new")]
        public async Task<IHttpActionResult> AddUserAsync([FromBody] UserInfo user)
        {
            if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Password))
                return BadRequest();

            AccountService service = new AccountService();
            if (!await service.CreateUserAsync(user)) return InternalServerError();
            // write log
            await LogService.WriteLogAsync(new Log
            {
                Action = $"創建帳號 {user.UserName}",
                ActionTime = DateTime.Now,
                UserId = Helper.GetUserId(Request)
            });
            return Ok();
        }

        /// <summary>
        /// GET /api/v1/account/users
        /// </summary>
        [Authorize(Roles = Role.Admin)]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("users")]
        public async Task<IHttpActionResult> GetUsersAsync()
        {
            AccountService service = new AccountService();
            return Json(await service.GetUsersAsync());
        }

        /// <summary>
        /// GET /api/v1/account/{id}/user
        /// </summary>
        [Authorize(Roles = Role.Admin)]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("{id}/user")]
        public async Task<IHttpActionResult> GetUserByIdAsync(int id)
        {
            AccountService service = new AccountService();
            return Json(await service.GetUserByUserIdAsync(id));
        }

        /// <summary>
        /// PUT /api/v1/account/user
        /// </summary>
        [Authorize(Roles = Role.Admin)]
        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("user")]
        public async Task<IHttpActionResult> UpdateUserAsync([FromBody] UserInfo user)
        {
            if (user.Id <= 0 || string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Password))
                return BadRequest();

            AccountService service = new AccountService();
            if (await service.UpdateUserAsync(user))
            {
                await LogService.WriteLogAsync(new Log
                {
                    Action = $"更新帳號 {user.UserName} EMAIL, 密碼",
                    ActionTime = DateTime.Now,
                    UserId = Helper.GetUserId(Request)
                });
                return Ok();
            }
            return InternalServerError();
        }

        /// <summary>
        /// POST /api/v1/account/user/status
        /// </summary>
        [Authorize(Roles = Role.Admin)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("user/status")]
        public async Task<IHttpActionResult> DisableUserAsync([FromBody] UpdateUserStatusModel user)
        {
            if (user == null || user.Id <= 0 || !(user.Enable == 0 || user.Enable == 1))
                return BadRequest();

            AccountService service = new AccountService();
            if (await service.SetUserEnableStatusByIdAsync(user.Id, user.Enable))
            {
                await LogService.WriteLogWithUserIdReference(new Log
                {
                    Action = user.Enable == 1 ? "開啟" : "關閉" + " @id 帳號",
                    ActionTime = DateTime.Now,
                    UserId = Helper.GetUserId(Request)
                }, user.Id);
                return Ok();
            }
            return InternalServerError();
        }

        /// <summary>
        /// DELETE /api/v1/account/drop/user/{id}
        /// </summary>
        [Authorize(Roles = Role.Admin)]
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("drop/user/{id}")]
        public async Task<IHttpActionResult> DropUserAsync([FromUri] DeleteUserModel user)
        {
            if (user == null || user.Id <= 0)
                return BadRequest();

            AccountService service = new AccountService();
            if (!await service.DeleteUserByIdAsync(user.Id)) return InternalServerError();

            await LogService.WriteLogWithUserIdReference(new Log
            {
                Action = "刪除 @id 帳號",
                ActionTime = DateTime.Now,
                UserId = Helper.GetUserId(Request)
            }, user.Id);
            return Ok();
        }

        /// <summary>
        /// GET /api/v1/account/users/roles
        /// </summary>
        [Authorize(Roles = Role.Admin)]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("users/roles")]
        public async Task<IHttpActionResult> GetUsersRoleAsync()
        {
            AccountService service = new AccountService();
            return Json(await service.GetRolesOfUsersAsync());
        }

        /// <summary>
        /// PUT /api/v1/account/users/roles
        /// </summary>
        [Authorize(Roles = Role.Admin)]
        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("users/roles")]
        public async Task<IHttpActionResult> UpdateUsersRoleAsync([FromBody] UserRole role)
        {
            if (role == null || !IsUserRoleValid(role))
                return BadRequest();
            AccountService service = new AccountService();
            if (await service.UpdateRolesOfUserByIdAsync(role))
            {
                var msg = new StringBuilder();
                if (role.EnableAdmin == 1)
                    msg.Append("管理員");
                else
                {
                    if (role.EnableEvent.Equals(Role.Read, StringComparison.OrdinalIgnoreCase))
                        msg.Append("廣告活動 讀取 ");
                    else if (role.EnableEvent.Equals(Role.ReadWrite, StringComparison.OrdinalIgnoreCase))
                        msg.Append("廣告活動 讀寫 ");

                    if (role.EnableLocation.Equals(Role.Read, StringComparison.OrdinalIgnoreCase))
                        msg.Append("廣告位置 讀取 ");
                    else if (role.EnableLocation.Equals(Role.ReadWrite, StringComparison.OrdinalIgnoreCase))
                        msg.Append("廣告位置 讀寫 ");

                    if (role.EnablePublish.Equals(Role.Read, StringComparison.OrdinalIgnoreCase))
                        msg.Append("廣告發佈 讀取 ");
                    else if (role.EnablePublish.Equals(Role.ReadWrite, StringComparison.OrdinalIgnoreCase))
                        msg.Append("廣告發佈 讀寫 ");

                    if (role.EnableResource.Equals(Role.Read, StringComparison.OrdinalIgnoreCase))
                        msg.Append("資源維護 讀取 ");
                    else if (role.EnableResource.Equals(Role.ReadWrite, StringComparison.OrdinalIgnoreCase))
                        msg.Append("資源維護 讀寫 ");

                    if (role.EnableSo.Equals(Role.Read, StringComparison.OrdinalIgnoreCase))
                        msg.Append("SO管理 讀取 ");
                    else if (role.EnableSo.Equals(Role.ReadWrite, StringComparison.OrdinalIgnoreCase))
                        msg.Append("SO管理 讀寫 ");
                }
                await LogService.WriteLogWithUserIdReference(new Log
                {
                    Action = $"更改 @id 帳號權限為 {msg}",
                    ActionTime = DateTime.Now,
                    UserId = Helper.GetUserId(Request)
                }, role.Id);
                return Ok();
            }
            return InternalServerError();
        }

        /// <summary>
        /// Check if user role is valid
        /// </summary>
        private bool IsUserRoleValid(UserRole role)
        {
            var type = role.GetType();
            var properties = type.GetProperties();
            foreach (var propertyInfo in properties)
            {
                var val = propertyInfo.GetValue(role, null);
                if (propertyInfo.PropertyType == typeof(string))
                {
                    if (propertyInfo.Name == "UserName")
                        continue;
                    var value = val.ToString();
                    if (!(value.Equals("x", StringComparison.OrdinalIgnoreCase) ||
                          value.Equals("r", StringComparison.OrdinalIgnoreCase) ||
                          value.Equals("rw", StringComparison.OrdinalIgnoreCase)))
                        return false;
                }
                else if (propertyInfo.PropertyType == typeof(int))
                {
                    var value = (int) val;
                    if (propertyInfo.Name == "EnableAdmin")
                    {
                        if (!(value == 0 || value == 1))
                            return false;
                        continue;
                    }
                    if (value <= 0)
                        return false;
                }
            }
            return true;
        }
    }
}