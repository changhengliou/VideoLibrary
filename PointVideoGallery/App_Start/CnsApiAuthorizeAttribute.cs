using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace PointVideoGallery
{
    public class CnsApiAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        private static readonly string[] _emptyArray = new string[0];

        private readonly object _typeId = new object();

        private string _roles;
        private string[] _rolesSplit = _emptyArray;
        private string _users;
        private string[] _usersSplit = _emptyArray;

        public new string Roles
        {
            get { return _roles ?? String.Empty; }
            set
            {
                _roles = value;
                _rolesSplit = SplitString(value);
            }
        }

        public new string Users
        {
            get { return _users ?? String.Empty; }
            set
            {
                _users = value;
                _usersSplit = SplitString(value);
            }
        }

        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            return actionContext.ActionDescriptor.GetCustomAttributes<System.Web.Http.AllowAnonymousAttribute>().Any()
                   || actionContext.ControllerContext.ControllerDescriptor
                       .GetCustomAttributes<System.Web.Http.AllowAnonymousAttribute>().Any();
        }

        protected bool IsAllowed(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            var user = Thread.CurrentPrincipal;
            if (_usersSplit.Length > 0 && !_usersSplit.Contains(user.Identity.Name, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            return _rolesSplit.Length <= 0 || _rolesSplit.Any(user.IsInRole);
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            var user = HttpContext.Current.User;
            return user != null && user.Identity.IsAuthenticated;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            if (SkipAuthorization(actionContext))
            {
                return;
            }

            if (!IsAuthorized(actionContext))
            {
                actionContext.Response =
                    actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized Access.");
                return;
            }

            if (!IsAllowed(actionContext))
            {
                actionContext.Response =
                    actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "No Authority Access.");
            }
        }

        internal static string[] SplitString(string original)
        {
            if (string.IsNullOrEmpty(original))
            {
                return _emptyArray;
            }

            var split = from piece in original.Split(',')
                let trimmed = piece.Trim()
                where !string.IsNullOrEmpty(trimmed)
                select trimmed;
            return split.ToArray();
        }
    }

    public class CnsMvcAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("/Error/AccessDenied");
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}