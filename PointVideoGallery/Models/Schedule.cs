using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointVideoGallery.Models
{
    /// <summary>
    /// Entire data from schedule
    /// </summary>
    public class ScheduleAdEvent
    {
        public int Id { get; set; }
        public DateTime ScheduleDate { get; set; }
        public DateTime ScheduleDateEnd { get; set; }
        public DateTime CreateDate { get; set; }
        public AdEvent AdEvent { get; set; }
    }

    public class Schedule
    {
        public int Id { get; set; }
        public DateTime ScheduleDate { get; set; }

        public DateTime ScheduleDateEnd { get; set; }
        public DateTime CreateDate { get; set; }
        public int EventId { get; set; }
    }

    public class ScheduleEvent : Schedule
    {
        public string Name { get; set; }
    }

    /// <summary>
    /// POST /api/v1/schedule/
    /// </summary>
    public class ScheduleModel
    {
        public int EventId { get; set; }

        /// <summary>
        /// Schedule start date
        /// </summary>
        public DateTime S { get; set; }

        /// <summary>
        /// Schedule end date
        /// </summary>
        public DateTime? E { get; set; }
    }
}