using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointVideoGallery.Models
{
    /// <summary>
    /// Resource setting in event scheduler
    /// </summary>
    public class ResourceEvent : ResourceFile
    {
        public int Sequence { get; set; }
        public int PlayoutWeight { get; set; }
        public IList<ResourceAction> Actions { get; set; }
    }
}