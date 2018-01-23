using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Web;
using System.Web.Http;
using DryIoc;
using DryIoc.Mvc;
using DryIoc.WebApi;
using PointVideoGallery.Controllers;
using PointVideoGallery.Services;

namespace PointVideoGallery
{
    public class DryIocConfig
    {
        public static void ConfigureDependencyInjection()
        {
            var container = new Container();            
//            container.Register<IService, VideoFileService>(Reuse.Transient);

            //.WithWebApi(GlobalConfiguration.Configuration, throwIfUnresolved: DryIocWebApi.IsController)
//            container.WithMvc(new [] { Assembly.GetAssembly(typeof(VideoController)) });
        }
    }
}