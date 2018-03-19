using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using PointVideoGallery.Models;
using PointVideoGallery.Services;
using PointVideoGallery.Utils;

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
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.EventRead)]
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
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.EventRead)]
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
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.EventWrite)]
        public async Task<IHttpActionResult> InsertOrUpdateInfoAsync([FromBody] AdEvent adEvent)
        {
            var service = new AdService();
            // update event
            if (adEvent.Id > 0)
            {
                var returnId = await service.UpdateAdEventAsync(adEvent);
                if (returnId > 0)
                    await LogService.WriteLogAsync(new Log
                    {
                        Action = $"更新廣告活動 {adEvent.Name}",
                        ActionTime = DateTime.Now,
                        UserId = Helper.GetUserId(Request)
                    });
                return Json(new {Id = returnId});
            }

            // new event
            var id = await service.AddAdEventAsync(adEvent);

            if (id == -1)
                return InternalServerError();
            await LogService.WriteLogAsync(new Log
            {
                Action = $"建立廣告活動 {adEvent.Name}",
                ActionTime = DateTime.Now,
                UserId = Helper.GetUserId(Request)
            });
            return Json(new {Id = id});
        }

        /// <summary>
        /// DELETE /api/v1/ad/events/rm/{id}
        /// </summary>
        [System.Web.Http.Route("rm/{id}")]
        [System.Web.Http.HttpDelete]
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.EventWrite)]
        public async Task<IHttpActionResult> RemoveAdEvent(int id)
        {
            var service = new AdService();
            if (!await service.DropAdEventByIdAsync(id)) return InternalServerError();
            await LogService.WriteLogWithEventIdReference(new Log
            {
                Action = $"刪除廣告活動 — 名稱: @id",
                ActionTime = DateTime.Now,
                UserId = Helper.GetUserId(Request)
            }, id);
            return Json(await service.GetAdEventsAsync());
        }

        /// <summary>
        /// PUT /api/v1/ad/events/
        /// </summary>
        [System.Web.Http.Route]
        [System.Web.Http.HttpPut]
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.EventWrite)]
        public async Task<IHttpActionResult> UpdateSoOrLocationEvent([FromBody] PutQueryData data)
        {
            var service = new AdService();
            if (!await service.AddAndDropEventsAsync(data.Id, data?.Add, data?.Rm, data.Type))
                return InternalServerError();

            string msg = string.Empty;
            switch (data.Type)
            {
                case DbEventType.Location:
                    msg = "廣告位址";
                    break;
                case DbEventType.Resource:
                    msg = "媒體資源";
                    break;
                case DbEventType.So:
                    msg = "SO";
                    break;
            }
            await LogService.WriteLogWithEventIdReference(new Log
            {
                Action = $"更新廣告活動 — 名稱: @id 更改{msg}",
                ActionTime = DateTime.Now,
                UserId = Helper.GetUserId(Request)
            }, data.Id);
            return Json(await service.GetAdEventByIdAsync(data.Id));
        }

        /// <summary>
        /// PUT /api/v1/ad/events/res
        /// </summary>
        [System.Web.Http.Route("res")]
        [System.Web.Http.HttpPut]
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.EventWrite)]
        public async Task<IHttpActionResult> UpdateResourcesAsync([FromBody] AdEvent data)
        {
            var service = new AdService();
            if (!await service.UpdateAdResourceAndPlayoutParamsAsync(data)) return InternalServerError();

            await LogService.WriteLogWithEventIdReference(new Log
            {
                Action = $"更新廣告活動 — 名稱: @id 更新媒體資源",
                ActionTime = DateTime.Now,
                UserId = Helper.GetUserId(Request)
            }, data.Id);
            return Ok();
        }

        /// <summary>
        /// POST: /api/v1/ad/events/res/action
        /// </summary>
        /// <param name="e">eventId</param>
        /// <param name="r">resourceSeq</param>
        [System.Web.Http.Route("res/action")]
        [System.Web.Http.HttpPost]
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.EventRead)]
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
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.EventRead)]
        public async Task<IHttpActionResult> GetAdEvent([FromBody] QueryData data)
        {
            var service = new AdService();
            return Json(await service.GetAdEventsWithIdFilterAsync(data?.So, data?.Location));
        }


        [System.Web.Http.Route("action/upload")]
        [System.Web.Http.HttpPost]
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.EventWrite)]
        public async Task<IHttpActionResult> UploadActionResource()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var fileInfo = HttpContext.Current.Request.Params["fileInfo"];
            if (string.IsNullOrWhiteSpace(fileInfo))
                return BadRequest("Invalid request");

            AdService adService = new AdService();

            HttpFileCollection files = HttpContext.Current.Request.Files;
            if (files.Count == 0)
                return BadRequest();

            var fileExtension = Path.GetExtension(files[0].FileName);
            var returnMsg = new ResourceMsg {FileName = files[0].FileName, Ok = false};

            if (!(fileExtension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                  fileExtension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                  fileExtension.Equals(".gif", StringComparison.OrdinalIgnoreCase) ||
                  fileExtension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                  fileExtension.Equals(".bmp", StringComparison.OrdinalIgnoreCase)))
            {
                returnMsg.Message = "檔案格式不支援";
                return Json(returnMsg);
            }


            if (files[0].ContentLength > int.Parse(ConfigurationManager.AppSettings["LimitAssetsFileSize"]) * 1024)
            {
                returnMsg.Message = "檔案過大";
                return Json(returnMsg);
            }

            var actionFile = JsonConvert.DeserializeObject<UploadActionResFileInfo>(fileInfo);
            var info = await adService.SaveHttpFileToDiskAsync(files[0]);

            if (info == null)
            {
                returnMsg.Message = "儲存失敗";
                return Json(returnMsg);
            }

            if (!await adService.AddOrUpdateActionImageAssetAsync(actionFile, info.Path))
            {
                returnMsg.Message = "儲存失敗";
                return Json(returnMsg);
            }
            returnMsg.Message = "上傳成功";
            returnMsg.Ok = true;

            return Json(new {returnMsg, info.Path});
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Web.Http.Route("cp/{id}")]
        [System.Web.Http.HttpGet]
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.EventWrite)]
        public async Task<IHttpActionResult> CopyEventAsync(int id)
        {
            if (id <= 0)
                return BadRequest();
            AdService adService = new AdService();
            return Json(await adService.CreateCopyEvent(id));
        }
    }
}