using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using PointVideoGallery.Models;
using PointVideoGallery.Services;
using PointVideoGallery.Utils;

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
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.PublishRead)]
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
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.PublishWrite)]
        public async Task<IHttpActionResult> AddScheduleAsync([FromBody] ScheduleModel data)
        {
            if (data.EventId <= 0 || data.S < DateTime.Now)
                return BadRequest();
            
            var endDate = data.E ?? data.S;
            if (endDate < data.S)
                return BadRequest();

            var service = new ScheduleService();

            if (!await service.AddScheduleAsync(data.S.Date, endDate, data.EventId)) return InternalServerError();
            await LogService.WriteLogAsync(new Log
            {
                Action = $"廣告排程—新增一筆排程，由{data.S:yyyy-MM-dd}至{data.E:yyyy-MM-dd} 每天執行",
                ActionTime = DateTime.Now,
                UserId = Helper.GetUserId(Request)
            });
            return Ok();
        }

        /// <summary>
        /// DELETE /api/v1/schedule/{id}
        /// </summary>
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("{id}")]
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.PublishWrite)]
        public async Task<IHttpActionResult> RemoveScheduleAsync([FromUri] int id)
        {
            if (id <= 0)
                return BadRequest();
            var service = new ScheduleService();
            if (!await service.DropScheduleByIdAsync(id)) return InternalServerError();
            await LogService.WriteLogAsync(new Log
            {
                Action = $"廣告排程—刪除一筆排程",
                ActionTime = DateTime.Now,
                UserId = Helper.GetUserId(Request)
            });
            return Ok();
        }

        /// <summary>
        /// GET /api/v1/schedule/download
        /// </summary>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("download/xml/{id}")]
        public async Task<HttpResponseMessage> GetXmlAsync(int id)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new MemoryStream();
            await XmlGenService.Generate(id, stream);

            stream.Seek(0, SeekOrigin.Begin);

            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
            return result;
        }
    }
}
