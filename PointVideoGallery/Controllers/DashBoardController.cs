using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using PointVideoGallery.Models;

namespace PointVideoGallery.Controllers
{
    [OutputCache(Duration = Int32.MaxValue, Location = OutputCacheLocation.ServerAndClient)]
    public class DashBoardController : Controller
    {
        // GET: DashBoard
        [CnsMvcAuthorize(Roles = Role.Admin + "," + Role.PublishRead)]
        public ActionResult Index()
        {
            return RedirectToAction(nameof(Publish));
        }

        [CnsMvcAuthorize(Roles = Role.Admin + "," + Role.ResourceRead)]
        public ActionResult Resource()
        {
            return View();
        }

        [CnsMvcAuthorize(Roles = Role.Admin + "," + Role.PublishRead)]
        public ActionResult Publish()
        {
            return View();
        }

        [CnsMvcAuthorize(Roles = Role.Admin + "," + Role.LocationRead)]
        public ActionResult Location()
        {
            return View();
        }

        [CnsMvcAuthorize(Roles = Role.Admin + "," + Role.SoRead)]
        public ActionResult SoSetting()
        {
            return View();
        }

        [CnsMvcAuthorize(Roles = Role.Admin + "," + Role.EventRead)]
        public ActionResult Events()
        {
            return View();
        }
    }
}