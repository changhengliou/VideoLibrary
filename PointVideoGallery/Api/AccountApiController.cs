using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using PointVideoGallery.Models;
using PointVideoGallery.Services;

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
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            }, DefaultAuthenticationTypes.ApplicationCookie);

            if (user.Enable)
                identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
            Request.GetOwinContext().Authentication.SignIn(new AuthenticationProperties(), identity);

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
        /// GET /api/v1/account/{id}/exist
        /// </summary>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("users")]
        public async Task<IHttpActionResult> GetUsersAsync()
        {
            AccountService service = new AccountService();
            return Json(await service.GetUsersAsync());
        }

        /// <summary>
        /// PUT /api/v1/account/{id}/exist
        /// </summary>
        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("user")]
        public async Task<IHttpActionResult> UpdateUserAsync([FromBody] User user)
        {
            if (user.Id <= 0 || string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Password))
                return BadRequest();

            AccountService service = new AccountService();
            if (await service.UpdateUserAsync(user))
                return Ok();
            return InternalServerError();
        }

        /// <summary>
        /// POST /api/v1/account/user/status
        /// </summary>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("user/status")]
        public async Task<IHttpActionResult> DisableUserAsync([FromBody] UpdateUserStatusModel user)
        {
            if (user ==null || user.Id <= 0 || !(user.Enable == 0 || user.Enable == 1))
                return BadRequest();

            AccountService service = new AccountService();
            if (await service.SetUserEnableStatusByIdAsync(user.Id, user.Enable))
                return Ok();
            return InternalServerError();
        }
    }
}