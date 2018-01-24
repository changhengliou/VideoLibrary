using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PointVideoGallery.Services;

namespace PointVideoGallery.Models
{
    /// <summary>
    /// Query model for Post API /api/v1/ad/events/q?s={soId}&l={locationId}
    /// </summary>
    public class QueryData
    {
        private List<int> _so;
        private List<int> _location;

        public List<int> So
        {
            get { return _so ?? new List<int>(); }
            set { _so = value; }
        }

        public List<int> Location
        {
            get { return _location ?? new List<int>(); }
            set { _location = value; }
        }
    }

    /// <summary>
    /// Query model for PUT API /api/v1/ad/events/
    /// </summary>
    public class PutQueryData
    {
        public int Id { get; set; }
        public DbEventType Type { get; set; }
        public List<int> Add { get; set; }
        public List<int> Rm { get; set; }
    }
}