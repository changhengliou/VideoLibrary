using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointVideoGallery.Models
{
    public class VideoFile
    {
        private string _fileId;

        public string FileId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_fileId))
                    _fileId = Guid.NewGuid().ToString();
                return _fileId;
            }
            set { _fileId = value; }
        }

        public string FileName { get; set; }

        public int FileSize { get; set; }

        public string FileLocation { get; set; }
        public DateTime? FileModifiedTime { get; set; }

        public TimeSpan? VideoLength { get; set; }
    }
}