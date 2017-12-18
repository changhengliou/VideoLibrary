using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace VideoLibrary.Entity
{
    public class UserEntity<TUserKey, TRoleKey, TLogin, TRole, TClaim> : IUser<TUserKey>
        where TLogin : UserLogin<TUserKey>
        where TRole : UserRole<TUserKey, TRoleKey>
        where TClaim : UserClaim<TUserKey>
    {
        /// <summary>Constructor</summary>
        public UserEntity()
        {
            this.Claims = (ICollection<TClaim>) new List<TClaim>();
            this.Roles = (ICollection<TRole>) new List<TRole>();
            this.Logins = (ICollection<TLogin>) new List<TLogin>();
        }

        /// <summary>User ID (Primary Key)</summary>
        public virtual TUserKey Id { get; set; }

        /// <summary>User name</summary>
        public virtual string UserName { get; set; }

        /// <summary>Email</summary>
        public virtual string Email { get; set; }

        /// <summary>True if the email is confirmed, default is false</summary>
        public virtual bool IsEmailConfirmed { get; set; }

        /// <summary>The salted/hashed form of the user password</summary>
        public virtual string PasswordHash { get; set; }

        /// <summary>
        ///     A random value that should change whenever a users credentials have changed (password changed, login removed)
        /// </summary>
        public virtual string SecurityStamp { get; set; }

        /// <summary>PhoneNumber for the user</summary>
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        ///     True if the phone number is confirmed, default is false
        /// </summary>
        public virtual bool PhoneNumberConfirmed { get; set; }

        /// <summary>Is two factor enabled for the user</summary>
        public virtual bool TwoFactorEnabled { get; set; }

        /// <summary>
        ///     DateTime in UTC when lockout ends, any time in the past is considered not locked out.
        /// </summary>
        public virtual DateTime? LockoutEndDateUtc { get; set; }

        /// <summary>Is lockout enabled for this user</summary>
        public virtual bool LockoutEnabled { get; set; }

        /// <summary>
        ///     Used to record failures for the purposes of lockout
        /// </summary>
        public virtual int AccessFailedCount { get; set; }

        /// <summary>Navigation property for user roles</summary>
        public virtual ICollection<TRole> Roles { get; private set; }

        /// <summary>Navigation property for user claims</summary>
        public virtual ICollection<TClaim> Claims { get; private set; }

        /// <summary>Navigation property for user logins</summary>
        public virtual ICollection<TLogin> Logins { get; private set; }
    }

    /// <summary>Represents a User entity</summary>
    public class UserEntity : UserEntity<string, int, UserLogin, UserRole, UserClaim>
    {
        public UserEntity()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public UserEntity(string name) : this()
        {
            this.UserName = name;
        }
    }
}