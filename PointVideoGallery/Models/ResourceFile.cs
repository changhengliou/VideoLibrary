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

    public class UploadFileInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("filename")]
        public string FileName { get; set; }
    }
}