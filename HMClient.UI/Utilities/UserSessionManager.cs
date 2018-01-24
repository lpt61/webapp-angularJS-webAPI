using HMClient.Data.Abstract;
using HMClient.Data.Models;
using HMClient.UI.Properties;
using System;
using System.Net.Http;
using System.Web;
using Microsoft.AspNet.Identity;

namespace HMClient.UI.Utilities
{
    public class UserSessionManager
    {
        protected IRepository repository { get; private set; }

        public UserSessionManager(IRepository repo)
        {
            this.repository = repo;
        }

        private HttpRequestMessage CurrentRequest
        {
            get
            {
                return (HttpRequestMessage)HttpContext.Current.Items["MS_HttpRequestMessage"];
            }
        }

        /// <returns>The current bearer authorization token from the HTTP headers</returns>
        private string GetCurrentBearerAuthrorizationToken()
        {
            string authToken = null;
            if (CurrentRequest.Headers.Authorization != null)
            {
                if (CurrentRequest.Headers.Authorization.Scheme.ToLower() == "Bearer".ToLower())
                {
                    authToken = CurrentRequest.Headers.Authorization.Parameter;
                }
            }
            return authToken;
        }

        private string GetCurrentUserId()
        {
            if (HttpContext.Current.User == null)
            {
                return null;
            }
            string userId = HttpContext.Current.User.Identity.GetUserId();
            return userId;
        }

        /// <summary>
        /// Extends the validity period of the current user's session in the database.
        /// This will configure the user's bearer authorization token to expire after
        /// certain period of time (e.g. 30 minutes, see UserSessionTimeout in Web.config)
        /// </summary>
        public void CreateUserSession(string username, string authToken)
        {
            this.repository.AddUserSession(username, authToken, Settings.Default.UserSessionTimeout);
        }

        /// <summary>
        /// Makes the current user session invalid (deletes the session token from the user sessions).
        /// The goal is to revoke any further access with the same authorization bearer token.
        /// Typically this method is called at "logout".
        /// </summary>
        public void InvalidateUserSession()
        {
            string authToken = GetCurrentBearerAuthrorizationToken();
            var currentUserId = GetCurrentUserId();

            this.repository.DeleteUserSession(authToken, currentUserId);
        }

        /// <summary>
        /// Re-validates the user session. Usually called at each authorization request.
        /// If the session is not expired, extends it lifetime and returns true.
        /// If the session is expired or does not exist, return false.
        /// </summary>
        /// <returns>true if the session is valid</returns>
        public bool ReValidateSession()
        {
            string authToken = this.GetCurrentBearerAuthrorizationToken();
            var currentUserId = this.GetCurrentUserId();
            bool result = this.repository.UpdateUserSession(authToken, currentUserId, Settings.Default.UserSessionTimeout);

            return result;
        }

        public void DeleteExpiredSessions()
        {
            this.repository.DeleteExpiredSessions();
        }
    }
}