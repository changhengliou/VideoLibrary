using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointVideoGallery.Models
{
    public class AdEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 方式, (interval)輪播/(random)隨機
        /// </summary>
        public string PlayOutMethod { get; set; }

        /// <summary>
        /// 秒數
        /// </summary>
        public int PlayOutTimeSpan { get; set; }

        /// <summary>
        /// 順序, byasset/bytime
        /// </summary>
        public string PlayOutSequence { get; set; }

        public List<ResourceEvent> Resources { get; set; }
        public List<SoSetting> SoSettings { get; set; }
        public List<LocationTag> LocationTags { get; set; }
    }
}