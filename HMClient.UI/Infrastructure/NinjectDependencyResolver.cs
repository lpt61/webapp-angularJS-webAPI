using HMClient.Data.Abstract;
using HMClient.Data.Concrete;
using HMClient.UI.Utilities;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Web.WebApi.FilterBindingSyntax;
using System.Web.Http.Filters;
using System.Web.Http.Dependencies;
using Ninject.Web.WebApi;
using Ninject.Syntax;
using hMailServer;
using Microsoft.AspNet.Identity.EntityFramework;
using HMClient.Data.Models;


namespace HMClient.UI.Infrastructure
{
    //With Api controller, inherit from both System.Web.Mvc.IDependencyResolver and System.Web.Http.Dependencies.IDependencyResolver
    public class NinjectDependencyResolver : NinjectDependencyScope, System.Web.Mvc.IDependencyResolver, IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernelParam) : base(kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        public IDependencyScope BeginScope()
        {
            return new NinjectDependencyScope(kernel.BeginBlock());
        }

        //public object GetService(Type serviceType)
        //{
        //    return kernel.TryGet(serviceType);
        //}

        //public IEnumerable<object> GetServices(Type serviceType)
        //{
        //    return kernel.GetAll(serviceType);
        //}

        private void AddBindings()
        {
            //Mock<IRepository> mock = new Mock<IRepository>();

            IMailApi<ApplicationClass, Domain, Account, Message> api = new HMSApi();

            IdentityDbContext<ApplicationUser> identityContext = new ApplicationDbContext();

            IRepository repo = new EFRepository();

            kernel.Bind<IRepository>().To<EFRepository>();
            kernel.BindHttpFilter<SessionAuthorizeAttribute>(FilterScope.Controller).WhenControllerHas<SessionAuthorizeAttribute>().WithConstructorArgument("repo", repo);
            

            //EmailSettings emailSettings = new EmailSettings
            //{
            //    WriteAsFile = bool.Parse(ConfigurationManager.AppSettings["Email.WriteAsFile"] ?? "false")
            //};

            //kernel.Bind<IOrderProcessor>().To<EmailOrderProcessor>().WithConstructorArgument("settings", emailSettings);
        }

        //public void Dispose()
        //{
        //    IDisposable disposable = resolver as IDisposable;
        //    if (disposable != null)
        //        disposable.Dispose();

        //    resolver = null;
        //}
    }
}