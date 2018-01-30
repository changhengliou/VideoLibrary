using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointVideoGallery.Models
{
    /// <summary>
    /// Post Request Data model
    /// </summary>
    public class EventDataResource : EventDataBase
    {
        public int ResourceSequence { get; set; }

        public int ResourcePlayWeight { get; set; }

        public IList<ResourceAction> Actions { get; set; }

    }
}