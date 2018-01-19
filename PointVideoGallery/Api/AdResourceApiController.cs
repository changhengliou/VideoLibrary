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

            StringBuilder returnMsg = new StringBuilder();

            for (var i = 0; i < resourceFile.Keys.Count; i++)
            {
                var info = await adService.SaveHttpFileToDiskAsync(files[i]);
                if (info == null)
                {
                    returnMsg.AppendLine($"Failed to save ${files[i].FileName}");
                }
                else
                {
                    var key = resourceFile.Keys.ElementAt(i);

                    info.Name = resourceFile[key].Name;
                    info.MediaType = resourceFile[key].Type;
                    info.CreateTime = DateTime.Now;
                    if (await adService.AddResourceFileAsync(info))
                    {
                        returnMsg.AppendLine($"Successfully save {files[i].FileName}");
                        continue;
                    }
                    returnMsg.AppendLine($"Failed to save ${files[i].FileName}");
                }
            }
            return Ok(returnMsg.ToString());
        }

        /// <summary>
        /// GET: /api/v1/ad/resource/table
        /// Get resources
        /// </summary>
        /// sort=CreateTime&order=desc&offset=5&limit=5
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("table")]
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
        public async Task<IHttpActionResult> UpdateResource([FromBody] ResourceFile file)
        {
            AdService adService = new AdService();
            if (await adService.FindAndUpdateResourceFileByIdAsync(file))
                return Ok();
            return InternalServerError();
        }

        /// <summary>
        /// DELETE: /api/v1/ad/resource/remove
        /// </summary>
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("remove/{id}")]
        public async Task<IHttpActionResult> RemoveResource(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Request.");
            AdService adService = new AdService();
            if (await adService.DropResourceFileByIdAsync(id))
                return Ok();
            return InternalServerError();
        }
    }
}