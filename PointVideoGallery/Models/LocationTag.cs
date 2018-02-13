using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointVideoGallery.Models
{
    public class LocationTag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}