using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Mvc;
using PointVideoGallery.Models;
using PointVideoGallery.Services;
using PointVideoGallery.Utils;

namespace PointVideoGallery.Api
{
    [System.Web.Http.RoutePrefix("api/v1/ad/location")]
    public class AdLocationApiController : ApiController
    {
        /// <summary>
        /// GET: /api/v1/ad/location
        /// </summary>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route]
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.LocationRead)]
        public async Task<IHttpActionResult> GetLocation()
        {
            AdService service = new AdService();
            return Json(await service.GetLocationTagsAsync());
        }

        /// <summary>
        /// POST: /api/v1/ad/location/{name}/add
        /// </summary>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("{name}/add")]
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.LocationWrite)]
        public async Task<IHttpActionResult> CreateLocation(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest($"Invalid request.");
            AdService service = new AdService();
            if (!await service.AddLocationTagAsync(name)) return InternalServerError();
            await LogService.WriteLogAsync(new Log
            {
                Action = $"廣告位置—新增 {name}",
                ActionTime = DateTime.Now,
                UserId = Helper.GetUserId(Request)
            });
            return Ok();
        }

        /// <summary>
        /// POST: /api/v1/ad/location/update
        /// </summary>
        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("update")]
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.LocationWrite)]
        public async Task<IHttpActionResult> UpdateLocation([FromBody] LocationTag tag)
        {
            if (tag.Id < 0 || string.IsNullOrWhiteSpace(tag.Name))
                return BadRequest("Invalid request.");
            AdService service = new AdService();
            if (!await service.UpdateLocationTagByIdAsync(tag)) return StatusCode(HttpStatusCode.Gone);
            await LogService.WriteLogAsync(new Log
            {
                Action = $"廣告位置—更改 {tag.Name}",
                ActionTime = DateTime.Now,
                UserId = Helper.GetUserId(Request)
            });
            return Ok();
        }

        /// <summary>
        /// DELETE: /api/v1/ad/location/drop
        /// </summary>
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("{id}")]
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.LocationWrite)]
        public async Task<IHttpActionResult> DeleteLocation(int id)
        {
            if (id < 0)
                return BadRequest("Invalid request.");
            AdService service = new AdService();
            if (!await service.DropLocationTagByIdAsync(id)) return StatusCode(HttpStatusCode.Gone);
            await LogService.WriteLogAsync(new Log
            {
                Action = $"廣告位置—刪除",
                ActionTime = DateTime.Now,
                UserId = Helper.GetUserId(Request)
            });
            return Ok();
        }
    }
}