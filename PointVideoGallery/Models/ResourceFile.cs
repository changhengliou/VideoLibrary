using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PointVideoGallery.Models
{
    public class ResourceFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string ThumbnailPath { get; set; }
        public DateTime CreateTime { get; set; }
        public string MediaType { get; set; }
    }

    public class ResourceMsg
    {
        public string FileName { get; set; }
        public bool Ok { get; set; }
        public string Message { get; set; }
    }

    public class UploadFileInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("filename")]
        public string FileName { get; set; }
    }

    public class UploadActionResFileInfo
    {
        public string Color { get; set; }
        public int Sequence { get; set; }
        public int EventId { get; set; }
    }
}