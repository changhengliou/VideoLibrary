using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointVideoGallery.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Enable { get; set; }
        public string Email { get; set; }
    }

    /// <summary>
    /// POST Data model
    /// </summary>
    public class PostUserModel
    {
        /// <summary>
        /// UserName
        /// </summary>
        public string N { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string P { get; set; }
        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }
    }

    public class UpdateUserStatusModel
    {
        public int Id { get; set; }
        public int Enable { get; set; }
    }
}