using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoLibrary.Entity
{
    public class UserRole<TUserKey, TRoleKey>
    {
        public UserRole()
        {
        }

        public UserRole(TUserKey userId)
        {
            this.UserId = userId;
        }

        public UserRole(TRoleKey roleId)
        {
            this.RoleId = roleId;
        }

        public UserRole(TUserKey userId, TRoleKey roldId) : this(userId)
        {
            this.RoleId = roldId;
        }

        public TUserKey UserId { get; set; }

        public TRoleKey RoleId { get; set; }
    }

    /// <summary>
    /// EntityType that represents a user belonging to a role
    /// </summary>
    public class UserRole : UserRole<string, int>
    {
        public UserRole()
        {
        }

        public UserRole(string userId) : base(userId)
        {
        }

        public UserRole(int roleId) : base(roleId)
        {
        }

        public UserRole(string userId, int roleId) : base(userId, roleId)
        {
        }
    }
}