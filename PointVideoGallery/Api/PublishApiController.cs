using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using OfficeOpenXml;


namespace PointVideoGallery.Api
{
    [RoutePrefix("api/v1/publish")]
    public class PublishApiController : ApiController
    {
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("excelsheet")]
        public async Task<IHttpActionResult> GetExcelSheet()
        {
            using (var package = new ExcelPackage())
            {
                using (var stream = new MemoryStream(package.GetAsByteArray()))
                {
                    
                }                
            }
            return null;
        }
    }
}
