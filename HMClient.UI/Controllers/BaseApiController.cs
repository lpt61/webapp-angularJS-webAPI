using HMClient.Data.Abstract;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Web.Http;

namespace HMClient.UI.Controllers
{
    public class BaseApiController : ApiController
    {
        protected IRepository repository { get; private set; }

        public BaseApiController(IRepository repo)
        {
            this.repository = repo;
        }

        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}