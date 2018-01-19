using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using PointVideoGallery.Models;
using PointVideoGallery.Services;

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
        public async Task<IHttpActionResult> GetOrDropLocation()
        {
            AdService service = new AdService();
            return Json(await service.GetLocationTagsAsync());
        }

        /// <summary>
        /// POST: /api/v1/ad/location/{name}/add
        /// </summary>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("{name}/add")]
        public async Task<IHttpActionResult> CreateLocation(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest($"{name} Invalid request.");
            AdService service = new AdService();
            if (await service.AddLocationTagAsync(name))
                return Ok();
            return InternalServerError();
        }

        /// <summary>
        /// POST: /api/v1/ad/location/update
        /// </summary>
        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("update")]
        public async Task<IHttpActionResult> UpdateLocation([FromBody] LocationTag tag)
        {
            if (tag.Id < 0 || string.IsNullOrWhiteSpace(tag.Name))
                return BadRequest("Invalid request.");
            AdService service = new AdService();
            if (await service.UpdateLocationTagByIdAsync(tag))
                return Ok();
            return StatusCode(HttpStatusCode.Gone);
        }

        /// <summary>
        /// DELETE: /api/v1/ad/location/drop
        /// </summary>
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("{id}")]
        public async Task<IHttpActionResult> DeleteLocation(int id)
        {
            if (id < 0)
                return BadRequest("Invalid request.");
            AdService service = new AdService();
            if (await service.RemoveLocationTagByIdAsync(id))
                return Ok();
            return StatusCode(HttpStatusCode.Gone);
        }
    }
}