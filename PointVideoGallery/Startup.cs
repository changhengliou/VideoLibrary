using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security.Cookies;
using Owin;
using PointVideoGallery.Services;

[assembly: OwinStartup(typeof(PointVideoGallery.Startup))]

namespace PointVideoGallery
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseErrorPage();
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                CookieHttpOnly = true,
                CookieSecure = CookieSecureOption.SameAsRequest,
                CookieName = "CNS",
                ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(ConfigurationManager.AppSettings["AuthTimeOut"])),
                SlidingExpiration = true,
                LoginPath = new PathString("/account/signin"),
                Provider = new CookieAuthenticationProvider
                {
                    OnApplyRedirect = ctx =>
                    {
                        if (!ctx.Request.Headers.ContainsKey("X-Requested-With"))
                            ctx.Response.Redirect(ctx.RedirectUri);
                    }
                }
            });
        }
    }
}