using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointVideoGallery.Models
{
    public class AdEvent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PlayOutMethod { get; set; }
        public List<ResourceEvent> Resources { get; set; }
        public List<SoSetting> SoSettings  { get; set; }
        public List<LocationTag> LocationTags { get; set; }
    }
}