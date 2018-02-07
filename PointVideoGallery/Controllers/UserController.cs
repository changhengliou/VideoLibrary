using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace PointVideoGallery.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Info()
        {
            return View();
        }

        public ActionResult UserLog()
        {
            return View();
        }
    }
}