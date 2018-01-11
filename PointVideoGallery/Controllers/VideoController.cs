using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PointVideoGallery.Models;
using PointVideoGallery.Services;

namespace PointVideoGallery.Controllers
{
    public class VideoController : Controller
    {
        private VideoFileService Service { get; }

        public VideoController() : base()
        {
        }

        public VideoController(IService service)
        {
            Service = service as VideoFileService;
        }

        public async Task<ActionResult> Index()
        {
            var files = await Service.GetVideoFiles();
            return View(files);
        }

        public async Task<ActionResult> Detail()
        {
            await Service.AddVideoFiles(new VideoFile
            {
                FileName = "舌尖上的中國01自然的饋贈.mp4",
                FileLocation = @"\Desktop",
                FileModifiedTime = DateTime.Now,
                FileSize = 223885766,
                VideoLength = new TimeSpan(0, 55, 23)
            });
            var file = await Service.GetVideoFiles();
            return View();
        }

        public ActionResult Upload()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}