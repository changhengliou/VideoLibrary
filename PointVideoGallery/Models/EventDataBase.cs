using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointVideoGallery.Models
{
    /// <summary>
    /// Event Data Base Class
    /// </summary>
    public class EventDataBase
    {
        public int EventId { get; set; }
        public IList<int> DataId { get; set; }
    }
}