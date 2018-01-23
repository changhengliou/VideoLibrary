using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointVideoGallery.Models
{
    public class ResourceAction
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Action { get; set; }
        public string Parameter { get; set; }

    }
}