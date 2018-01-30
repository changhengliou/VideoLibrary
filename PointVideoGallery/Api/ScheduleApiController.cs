using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using PointVideoGallery.Models;
using PointVideoGallery.Services;

namespace PointVideoGallery.Api
{
    [System.Web.Http.RoutePrefix("api/v1/schedule")]
    public class ScheduleApiController : ApiController
    {
        /// <summary>
        /// POST /api/v1/schedule/s={date}
        /// </summary>
        [System.Web.Http.Route("{s}")]
        [System.Web.Http.HttpGet]
        public async Task<IHttpActionResult> GetSchedulesAsync([FromUri] DateTime s)
        {
            var service = new ScheduleService();
            var returnVal = await service.GetSchedulesByDateAsync(s);
            Trace.WriteLine(returnVal);
            if (returnVal == null)
                return InternalServerError();
            return Json(returnVal);
        }

        /// <summary>
        /// POST /api/v1/schedule/
        /// </summary>
        [System.Web.Http.Route]
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> AddScheduleAsync([FromBody] ScheduleModel data)
        {
            if (data.EventId <= 0 || data.S < DateTime.Now)
                return BadRequest();
            
            var endDate = data.E ?? data.S;
            if (endDate < data.S)
                return BadRequest();

            var service = new ScheduleService();
            
            if (await service.AddScheduleAsync(data.S.Date, endDate, data.EventId))
                return Ok();
            return InternalServerError();
        }        
    }
}
