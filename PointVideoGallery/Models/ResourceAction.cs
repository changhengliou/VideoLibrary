using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointVideoGallery.Models
{
    public class ResourceAction
    {
        public string Type { get; set; }
        public string Action { get; set; }
        public string Parameter { get; set; }

        public string Color { get; set; }
        public int ResourceId { get; set; }
        public int ResourceSeq { get; set; }
        public int Checked { get; set; }
    }
}