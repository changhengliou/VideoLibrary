using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail()
        {
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