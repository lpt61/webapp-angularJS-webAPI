using HMClient.Data.Abstract;
using HMClient.Data.Concrete;
using Ninject;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Linq;

namespace HMClient.UI.Utilities
{
    public class SessionAuthorizeAttribute : AuthorizeAttribute
    {
        protected IRepository repository { get; private set; }

        public SessionAuthorizeAttribute(IRepository repo)
        {
            this.repository = repo;
        }

        public SessionAuthorizeAttribute() : this(new EFRepository())
        {}

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (SkipAuthorization(actionContext))
            {
                return;
            }

            var userSessionManager = new UserSessionManager(this.repository);
            if (userSessionManager.ReValidateSession())
            {
                base.OnAuthorization(actionContext);
            }
            else
            {
                //CreateErrorResponse() in System.Net.Http;
                actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized, "Session token expried or not valid.");
            }
        }

        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            //.Any() requires System.Linq;
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }
    }
}