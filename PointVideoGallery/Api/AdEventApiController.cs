using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using PointVideoGallery.Models;
using PointVideoGallery.Services;

namespace PointVideoGallery.Api
{
    [System.Web.Http.RoutePrefix("api/v1/ad/events")]
    public class AdEventApiController : ApiController
    {
        /// <summary>
        /// GET /api/v1/ad/events/
        /// </summary>
        [System.Web.Http.Route]
        [System.Web.Http.HttpGet]
        public async Task<IHttpActionResult> GetAdEvents()
        {
            var service = new AdService();
            return Json(await service.GetAdEventsAsync());
        }

        /// <summary>
        /// GET /api/v1/ad/events/
        /// </summary>
        [System.Web.Http.Route("{id}")]
        [System.Web.Http.HttpGet]
        public async Task<IHttpActionResult> GetAdEvent(int id)
        {
            var service = new AdService();
            return Json(await service.GetAdEventByIdAsync(id));
        }

        /// <summary>
        /// GET /api/v1/ad/events/q?soId=&locationId=
        /// </summary>
        /// <param name="s">soId</param>
        /// <param name="l">locationId</param>
        [System.Web.Http.Route("q")]
        [System.Web.Http.HttpGet]
        public async Task<IHttpActionResult> GetAdEvent([FromUri]int? s, int? l)
        {
            var service = new AdService();
            int soId = s ?? -1,
                locationId = l ?? -1;
            return Json(await service.GetAdEventsAsync(soId: soId, locationId: locationId));
        }

        /// <summary>
        /// POST /api/v1/ad/events/location
        /// </summary>
        [System.Web.Http.Route("location")]
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> AddEventLocation([FromBody] EventDataBase data)
        {
            AdService service = new AdService();
            if (await service.AddEventSettingAsync(data, DbEventType.Location))
                return Ok();
            return InternalServerError();
        }

        /// <summary>
        /// POST /api/v1/ad/events/resource
        /// </summary>
        [System.Web.Http.Route("resource")]
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> AddEventResource([FromBody] EventDataResource data)
        {
            AdService service = new AdService();
            if (await service.AddEventSettingAsync(data, DbEventType.Resource))
                return Ok();
            return InternalServerError();
        }

        /// <summary>
        /// POST /api/v1/ad/events/so
        /// </summary>
        [System.Web.Http.Route("so")]
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> AddEventSo([FromBody] EventDataBase data)
        {
            AdService service = new AdService();
            if (await service.AddEventSettingAsync(data, DbEventType.So))
                return Ok();
            return InternalServerError();
        }

        /// <summary>
        /// PUT /api/v1/ad/events/location
        /// </summary>
        [System.Web.Http.Route("location")]
        [System.Web.Http.HttpPut]
        public async Task<IHttpActionResult> UpdateEventLocation([FromBody] EventDataBase data)
        {
            AdService service = new AdService();
            if (await service.AddEventSettingAsync(data, DbEventType.Location))
                return Ok();
            return InternalServerError();
        }

        /// <summary>
        /// PUT /api/v1/ad/events/resource
        /// </summary>
        [System.Web.Http.Route("resource")]
        [System.Web.Http.HttpPut]
        public async Task<IHttpActionResult> UpdateEventResource([FromBody] EventDataResource data)
        {
            AdService service = new AdService();
            if (await service.AddEventSettingAsync(data, DbEventType.Resource))
                return Ok();
            return InternalServerError();
        }

        /// <summary>
        /// PUT /api/v1/ad/events/so
        /// </summary>
        [System.Web.Http.Route("so")]
        [System.Web.Http.HttpPut]
        public async Task<IHttpActionResult> UpdateEventSo([FromBody] EventDataBase data)
        {
            AdService service = new AdService();
            if (await service.AddEventSettingAsync(data, DbEventType.So))
                return Ok();
            return InternalServerError();
        }

        /// <summary>
        /// DELETE /api/v1/ad/events/location
        /// </summary>
        [System.Web.Http.Route("location")]
        [System.Web.Http.HttpDelete]
        public async Task<IHttpActionResult> DeleteEventLocation([FromBody] EventDataBase data)
        {
            AdService service = new AdService();
            if (await service.DropEventSettingAsync(data, DbEventType.Location))
                return Ok();
            return InternalServerError();
        }

        /// <summary>
        /// DELETE /api/v1/ad/events/resource
        /// </summary>
        [System.Web.Http.Route("resource")]
        [System.Web.Http.HttpDelete]
        public async Task<IHttpActionResult> DeleteEventResource([FromBody] EventDataBase data)
        {
            AdService service = new AdService();
            if (await service.DropEventSettingAsync(data, DbEventType.Resource))
                return Ok();
            return InternalServerError();
        }

        /// <summary>
        /// DELETE /api/v1/ad/events/so
        /// </summary>
        [System.Web.Http.Route("so")]
        [System.Web.Http.HttpDelete]
        public async Task<IHttpActionResult> DeleteEventSo([FromBody] EventDataBase data)
        {
            AdService service = new AdService();
            if (await service.DropEventSettingAsync(data, DbEventType.So))
                return Ok();
            return InternalServerError();
        }
    }
}