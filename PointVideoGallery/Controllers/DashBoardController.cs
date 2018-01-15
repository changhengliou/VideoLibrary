using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PointVideoGallery.Controllers
{
    public class DashBoardController : Controller
    {
        // GET: DashBoard
        public ActionResult Index()
        {
            return RedirectPermanent("/dashboard/publish");
        }

        public ActionResult Resource()
        {
            return View();
        }

        public ActionResult Publish()
        {
            return View();
        }

        public ActionResult Location()
        {
            return View();
        }

        public ActionResult Ticker()
        {
            return View();
        }

        public ActionResult Events()
        {
            return View();
        }
    }
}