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
        /// PUT /api/v1/ad/events/info
        /// </summary>
        [System.Web.Http.Route("info")]
        [System.Web.Http.HttpPut]
        public async Task<IHttpActionResult> InsertOrUpdateInfoAsync([FromBody] AdEvent adEvent)
        {
            var service = new AdService();
            // update event
            if (adEvent.Id > 0)
                return Json(new {Id = await service.UpdateAdEventAsync(adEvent)});

            // new event
            var id = await service.AddAdEventAsync(adEvent);
            if (id == -1)
                return InternalServerError();
            return Json(new {Id = id});
        }

        /// <summary>
        /// DELETE /api/v1/ad/events/rm/{id}
        /// </summary>
        [System.Web.Http.Route("rm/{id}")]
        [System.Web.Http.HttpDelete]
        public async Task<IHttpActionResult> RemoveAdEvent(int id)
        {
            var service = new AdService();
            if (await service.DropAdEventByIdAsync(id))
                return Json(await service.GetAdEventsAsync());
            return InternalServerError();
        }

        /// <summary>
        /// PUT /api/v1/ad/events/
        /// </summary>
        [System.Web.Http.Route]
        [System.Web.Http.HttpPut]
        public async Task<IHttpActionResult> UpdateSoOrLocationEvent([FromBody] PutQueryData data)
        {
            var service = new AdService();
            if (await service.AddAndDropEventsAsync(data.Id, data?.Add, data?.Rm, data.Type))
                return Json(await service.GetAdEventByIdAsync(data.Id));
            return InternalServerError();
        }

        /// <summary>
        /// PUT /api/v1/ad/events/res
        /// </summary>
        [System.Web.Http.Route("res")]
        [System.Web.Http.HttpPut]
        public async Task<IHttpActionResult> UpdateResourcesAsync([FromBody] AdEvent data)
        {
            var service = new AdService();
            if (await service.UpdateAdResourceAndPlayoutParamsAsync(data))
                return Ok();
            return InternalServerError();
        }

        /// <summary>
        /// POST: /api/v1/ad/events/res/action
        /// </summary>
        /// <param name="e">eventId</param>
        /// <param name="r">resourceSeq</param>
        [System.Web.Http.Route("res/action")]
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> GetResourceActions([FromBody] ActionQueryData data)
        {
            var service = new AdService();
            var returnVal = await service.GetActionsAsync(eventId: data.E, resourceSeq: data.R);

            if (returnVal == null)
                return InternalServerError();
            return Json(returnVal);
        }

        /// <summary>
        /// POST /api/v1/ad/events/q?s=&l=
        /// </summary>
        /// <param name="data"></param>
        [System.Web.Http.Route("q")]
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> GetAdEvent([FromBody] QueryData data)
        {
            var service = new AdService();
            return Json(await service.GetAdEventsWithIdFilterAsync(data?.So, data?.Location));
        }

        /// <summary>
        /// POST /api/v1/ad/events/location
        /// </summary>
        [System.Web.Http.Route("location")]
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> AddEventLocation([FromBody] EventDataBase data)
        {
            if (data == null)
                return BadRequest("Invalid Request");

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