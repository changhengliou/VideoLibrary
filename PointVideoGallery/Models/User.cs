using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointVideoGallery.Models
{
    public abstract class IUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
    }

    public class User : UserRole
    {
        public string Password { get; set; }
        public int Enable { get; set; }
        public string Email { get; set; }
    }

    public class UserInfo : IUser
    {
        public string Password { get; set; }
        public int Enable { get; set; }
        public string Email { get; set; }
    }

    public class UserRole : IUser
    {
        public string EnableResource { get; set; }
        public string EnablePublish { get; set; }
        public string EnableLocation { get; set; }
        public string EnableSo { get; set; }
        public string EnableEvent { get; set; }
        public int EnableAdmin { get; set; }
    }

    public static class Role
    {
        public const string Read = "r";
        public const string ReadWrite = "rw";
        public const int Enable = 1;

        public const string Admin = "Admin";
        public const string AccountEnable = "User";
        public const string SoRead = "SoRead";
        public const string SoWrite = "SoWrite";
        public const string ResourceRead = "ResourceRead";
        public const string ResourceWrite = "ResourceWrite";
        public const string PublishRead = "PublishRead";
        public const string PublishWrite = "PublishWrite";
        public const string LocationRead = "LocationRead";
        public const string LocationWrite = "LocationWrite";
        public const string EventRead = "EventRead";
        public const string EventWrite = "EventWrite";
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

    public class DeleteUserModel
    {
        public int Id { get; set; }
    }
}