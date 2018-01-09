using System;
using System.Threading.Tasks;
using DryIoc;
using DryIoc.Owin;
using Microsoft.Owin;
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
        }
    }
}
