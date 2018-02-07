using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace PointVideoGallery.Controllers
{
    [OutputCache(Duration = Int32.MaxValue, Location = OutputCacheLocation.ServerAndClient)]
    public class AccountController : Controller
    {
        // GET: Account
        [CnsMvcAuthorize(Roles = Models.Role.Admin)]
        public ActionResult Index()
        {
            return View();
        }

        // GET: /account/role
        [CnsMvcAuthorize(Roles = Models.Role.Admin)]
        public ActionResult Role()
        {
            return View();
        }

        [CnsMvcAuthorize(Roles = Models.Role.Admin)]
        public ActionResult Log()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Signin()
        {
            return View();
        }

        public ActionResult Signout()
        {
            Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction(nameof(Signin));
        }
    }
}