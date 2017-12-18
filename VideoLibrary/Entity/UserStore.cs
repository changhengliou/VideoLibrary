using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace VideoLibrary.Entity
{
    public class UserStore<TUser, TRole, TUserKey, TRoleKey, TUserLogin, TUserRole, TUserClaim> :
        IUserLoginStore<TUser, TUserKey>, IUserClaimStore<TUser, TUserKey>, IUserRoleStore<TUser, TUserKey>,
        IUserPasswordStore<TUser, TUserKey>, IUserSecurityStampStore<TUser, TUserKey>,
        IQueryableUserStore<TUser, TUserKey>,
        IUserEmailStore<TUser, TUserKey>, IUserPhoneNumberStore<TUser, TUserKey>, IUserTwoFactorStore<TUser, TUserKey>,
        IUserLockoutStore<TUser, TUserKey>, IUserStore<TUser, TUserKey>, IDisposable
        where TUser : UserEntity<TUserKey, TRoleKey, TUserLogin, TUserRole, TUserClaim>
        where TRole : RoleEntity<TRoleKey>
        where TUserKey : IEquatable<TUserKey>
        where TUserLogin : UserLogin<TUserKey>, new()
        where TUserRole : UserRole<TUserKey, TRoleKey>, new()
        where TUserClaim : UserClaim<TUserKey>, IUserClaimStore<TUser, TUserKey>, new()
    {
        private readonly IDbSet<TUserLogin> _logins;
        private readonly IDbSet<TUserClaim> _userClaims;
        private readonly IDbSet<TUserRole> _userRoles;
        private bool _disposed;

        /// <summary>Constructor which takes a db context and wires up the stores with default instances using the context</summary>
        public UserStore(DbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            this.Context = context;
            this.AutoSaveChanges = true;
            this._logins = (IDbSet<TUserLogin>) this.Context.Set<TUserLogin>();
            this._userClaims = (IDbSet<TUserClaim>) this.Context.Set<TUserClaim>();
            this._userRoles = (IDbSet<TUserRole>) this.Context.Set<TUserRole>();
        }

        /// <summary>Context for the store</summary>
        public DbContext Context { get; private set; }

        /// <summary>If true will call dispose on the DbContext during Dispose</summary>
        public bool DisposeContext { get; set; }

        /// <summary>If true will call SaveChanges after Create/Update/Delete</summary>
        public bool AutoSaveChanges { get; set; }

        public Task CreateAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByIdAsync(TUserKey userId)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            throw new NotImplementedException();
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            throw new NotImplementedException();
        }

        public Task AddToRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TUser> Users { get; }

        public Task SetEmailAsync(TUser user, string email)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object) this);
        }

        /// <summary> If disposing, calls dispose on the Context.  Always nulls out the Context </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (this.DisposeContext && disposing && this.Context != null)
                this.Context.Dispose();
            this._disposed = true;
            this.Context = (DbContext) null;
        }
    }
}