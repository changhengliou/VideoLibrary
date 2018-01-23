using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.Owin.StaticFiles.ContentTypes;

namespace PointVideoGallery.Controllers
{
    /// <summary>
    /// manage media assets
    /// </summary>
    [System.Web.Http.Route("assets")]
    public class AssetsController : Controller
    {
        // GET: Assets
        [System.Web.Http.Route("{p:string}")]
        public async Task Index(string p)
        {
            if (string.IsNullOrWhiteSpace(p))
            {
                Response.StatusCode = 404;
                Response.Status = "404 Not Found";
                Response.Write("Not Found");
                Response.Flush();
                return;
            }
                
            try
            {
                p = p.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
                string path = Path.Combine(ConfigurationManager.AppSettings["LibraryIndexBasePath"], p);

                if (!System.IO.File.Exists(path))
                {
                    Response.StatusCode = 404;
                    Response.Status = "404 Not Found";
                    Response.Write("Not Found");
                    Response.Flush();
                    return;
                }

                using (var stream = System.IO.File.OpenRead(path))
                {
                    FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
                    string contentType;
                    if (!provider.TryGetContentType(Path.GetExtension(p), out contentType))
                        contentType = "application/octet-stream";

                    long contentLength = stream.Length;

                    Response.AddHeader("Content-Length", contentLength.ToString());
                    Response.AddHeader("Cache-Control", "max-age=43200;");
                    Response.AddHeader("Content-MediaType", contentType);

                    long dataToBeRead = contentLength;

                    while (dataToBeRead > 0)
                    {
                        if (!Response.IsClientConnected)
                            break;
                        // 8KB buffer
                        int intBufferSize = 8 * 1024;

                        byte[] bytBuffers = new Byte[intBufferSize];

                        int preparedBytesToBeSend =
                            await stream.ReadAsync(bytBuffers, 0, intBufferSize);

                        await Response.OutputStream.WriteAsync(buffer: bytBuffers, offset: 0, count: preparedBytesToBeSend);

                        Response.Flush();

                        dataToBeRead = dataToBeRead - preparedBytesToBeSend;
                    }
                }
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            finally
            {
                Response.Flush();
                HttpContext.ApplicationInstance.CompleteRequest();
            }
        }
    }
}