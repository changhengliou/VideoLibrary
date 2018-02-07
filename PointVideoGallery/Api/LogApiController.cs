using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using PointVideoGallery.Models;
using PointVideoGallery.Services;
using PointVideoGallery.Utils;

namespace PointVideoGallery.Api
{
    [RoutePrefix("api/v1/log")]
    public class LogApiController : ApiController
    {
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route]
        [CnsApiAuthorize(Roles = Role.Admin)]
        public async Task<IHttpActionResult> GetLogsAsync(int days)
        {
            if (days < 0)
                return BadRequest();
            if (days == 0)
                days = 30; // default select last 30 days logs
            var service = new LogService();
            return Json(await service.GetLogsAsync(days, null));
        }

        /// <summary>
        /// GET /api/v1/log/user
        /// </summary>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("user")]
        public async Task<IHttpActionResult> GetLogsByUserIdAsync()
        {
            var service = new LogService();
            return Json(await service.GetLogsAsync(30, Helper.GetUserId(Request)));// default select last 30 days logs
        }
    }
}
