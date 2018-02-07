using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace PointVideoGallery.Controllers
{
    [AllowAnonymous]
    [OutputCache(Duration = Int32.MaxValue, Location = OutputCacheLocation.ServerAndClient)]
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}