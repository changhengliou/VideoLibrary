using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using PointVideoGallery.Models;
using PointVideoGallery.Services;
using PointVideoGallery.Utils;
using WebGrease.Css.Extensions;

namespace PointVideoGallery.Api
{
    [System.Web.Http.RoutePrefix("api/v1/ad/resource")]
    public class AdResourceApiController : ApiController
    {
        /// <summary>
        /// POST: /api/v1/ad/resource/upload
        /// upload files to the server
        /// </summary>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("upload")]
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.ResourceWrite)]
        public async Task<IHttpActionResult> UploadResource()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var fileInfo = HttpContext.Current.Request.Params["fileInfo"];
            if (string.IsNullOrWhiteSpace(fileInfo))
                return BadRequest("Invalid request");

            AdService adService = new AdService();

            HttpFileCollection files = HttpContext.Current.Request.Files;

            var resourceFile = JsonConvert.DeserializeObject<Dictionary<int, UploadFileInfo>>(fileInfo);

            List<ResourceMsg> returnVal = new List<ResourceMsg>();

            for (var i = 0; i < resourceFile.Keys.Count; i++)
            {
                var fileExtension = Path.GetExtension(files[i].FileName);
                if (files[i].ContentLength > int.Parse(ConfigurationManager.AppSettings["LimitAssetsFileSize"]) * 1024)
                {
                    returnVal.Add(new ResourceMsg
                    {
                        FileName = files[i].FileName,
                        Ok = false,
                        Message = "檔案過大"
                    });
                    continue;
                }
                if (!(fileExtension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                      fileExtension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                      fileExtension.Equals(".gif", StringComparison.OrdinalIgnoreCase) ||
                      fileExtension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                      fileExtension.Equals(".bmp", StringComparison.OrdinalIgnoreCase)))
                {
                    returnVal.Add(new ResourceMsg
                    {
                        FileName = files[i].FileName,
                        Ok = false,
                        Message = "檔案格式不支援"
                    });
                    continue;
                }
                var info = await adService.SaveHttpFileToDiskAsync(files[i]);
                if (info == null)
                {
                    returnVal.Add(new ResourceMsg
                    {
                        FileName = files[i].FileName,
                        Ok = false,
                        Message = "儲存失敗"
                    });
                }
                else
                {
                    var key = resourceFile.Keys.ElementAt(i);

                    info.Name = resourceFile[key].Name;
                    info.MediaType = resourceFile[key].Type;
                    info.CreateTime = DateTime.Now;
                    if (await adService.AddResourceFileAsync(info))
                    {
                        returnVal.Add(new ResourceMsg
                        {
                            FileName = files[i].FileName,
                            Ok = true,
                            Message = "上傳成功"
                        });
                        await LogService.WriteLogAsync(new Log
                        {
                            Action = $"資源維護—上傳圖檔 {files[i].FileName}",
                            ActionTime = DateTime.Now,
                            UserId = Helper.GetUserId(Request)
                        });
                        continue;
                    }
                    returnVal.Add(new ResourceMsg
                    {
                        FileName = files[i].FileName,
                        Ok = false,
                        Message = "儲存失敗"
                    });
                }
            }
            return Json(returnVal);
        }

        /// <summary>
        /// GET: /api/v1/ad/resource/table
        /// Get resources
        /// </summary>
        /// sort=CreateTime&order=desc&offset=5&limit=5
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("table")]
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.ResourceRead)]
        public async Task<IHttpActionResult> GetResource(int offset = 0, int limit = 10, string sort = "CreateTime",
            string order = "desc", string search = null)
        {
            if (!(string.IsNullOrWhiteSpace(order) || order.Equals("desc", StringComparison.OrdinalIgnoreCase) ||
                  order.Equals("asc", StringComparison.OrdinalIgnoreCase)))
                return BadRequest("Invalid Request.");

            if (!(string.IsNullOrWhiteSpace(sort) || sort.Equals("CreateTime", StringComparison.OrdinalIgnoreCase) ||
                  sort.Equals("MediaType", StringComparison.OrdinalIgnoreCase) ||
                  sort.Equals("Name", StringComparison.OrdinalIgnoreCase)))
                return BadRequest("Invalid Request.");

            if (limit < 0 || offset < 0)
                return BadRequest("Invalid Request");

            AdService adService = new AdService();
            return Json(new
            {
                total = await adService.GetResourceFileCountAsync(search: search),
                rows = await adService.GetResourceFileAsync(offset: offset, limit: limit, sort: sort, order: order,
                    search: search)
            });
        }

        /// <summary>
        /// PUT: /api/v1/ad/resource/update
        /// Get resources
        /// </summary>
        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("update")]
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.ResourceWrite)]
        public async Task<IHttpActionResult> UpdateResource([FromBody] ResourceFile file)
        {
            AdService adService = new AdService();
            if (!await adService.FindAndUpdateResourceFileByIdAsync(file)) return InternalServerError();
            await LogService.WriteLogAsync(new Log
            {
                Action = $"資源維護—修改圖檔資訊 {file.Name}",
                ActionTime = DateTime.Now,
                UserId = Helper.GetUserId(Request)
            });
            return Ok();
        }

        /// <summary>
        /// DELETE: /api/v1/ad/resource/remove
        /// </summary>
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("remove/{id}")]
        [CnsApiAuthorize(Roles = Role.Admin + "," + Role.ResourceWrite)]
        public async Task<IHttpActionResult> RemoveResource(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Request.");
            AdService adService = new AdService();
            if (!await adService.DropResourceFileByIdAsync(id)) return InternalServerError();
            await LogService.WriteLogAsync(new Log
            {
                Action = "資源維護—刪除圖檔",
                ActionTime = DateTime.Now,
                UserId = Helper.GetUserId(Request)
            });
            return Ok();
        }
    }
}