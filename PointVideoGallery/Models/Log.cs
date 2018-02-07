using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointVideoGallery.Models
{
    public class Log
    {
        public string Action { get; set; }
        public DateTime ActionTime { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}