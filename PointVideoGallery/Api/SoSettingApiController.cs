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
    [System.Web.Http.RoutePrefix("api/v1/setting/so")]
    public class SoSettingApiController : ApiController
    {
        /// <summary>
        /// GET: /api/v1/setting/so
        /// </summary>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route]
        public async Task<IHttpActionResult> GetSoSetting()
        {
            SettingService service = new SettingService();
            return Json(await service.GetSoSettingsAsync());
        }

        /// <summary>
        /// POST: /api/v1/setting/so
        /// </summary>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route]
        public async Task<IHttpActionResult> AddSoSetting([FromBody] SoSetting setting)
        {
            if (string.IsNullOrWhiteSpace(setting.Code) || string.IsNullOrWhiteSpace(setting.Name))
                return BadRequest("Invalid Request.");
            SettingService service = new SettingService();
            if (await service.AddSoSettingAsync(setting))
                return Ok();
            return StatusCode(HttpStatusCode.Gone);
        }

        /// <summary>
        /// PUT: /api/v1/setting/so
        /// </summary>
        [System.Web.Http.HttpPut]
        [System.Web.Http.Route]
        public async Task<IHttpActionResult> UpdateSoSetting([FromBody] SoSetting setting)
        {
            if (setting.Id <= 0 || string.IsNullOrWhiteSpace(setting.Code) ||
                string.IsNullOrWhiteSpace(setting.Name))
                return BadRequest("Invalid Request.");
            SettingService service = new SettingService();
            if (await service.UpdateSoSettingByIdAsync(setting))
                return Ok();
            return StatusCode(HttpStatusCode.Gone);
        }

        /// <summary>
        /// DELETE: /api/v1/setting/so/{id}
        /// </summary>
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("{id}")]
        public async Task<IHttpActionResult> DeleteSoSetting(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Request.");
            SettingService service = new SettingService();
            if (await service.RemoveSoSettingByIdAsync(id))
                return Ok();
            return StatusCode(HttpStatusCode.Gone);
        }

        /// <summary>
        /// GET: /api/v1/setting/so/{id}
        /// check if so setting existed
        /// </summary>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("{id}")]
        public async Task<IHttpActionResult> GetSoSetting(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Request.");

            SettingService service = new SettingService();
            if (await service.GetSoSettingByIdAsync(id) != null)
                return Ok();
            return StatusCode(HttpStatusCode.Gone);
        }
    }
}