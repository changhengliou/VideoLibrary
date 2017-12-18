using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace VideoLibrary.Entity
{
    public class RoleEntity<TRoleKey> : IRole<TRoleKey>
    {
        public TRoleKey Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>Represents a Role entity</summary>
    public class RoleEntity : RoleEntity<int>
    {

    }
}