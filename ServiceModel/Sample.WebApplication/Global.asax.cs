using System;
using Microsoft.Practices.Unity;
using Sample.Operations;
using Ziv.ServiceModel.Activation;
using Ziv.ServiceModel.Operations.OperationsManager;

namespace Sample.WebApplication
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            IUnityContainer container = new UnityContainer();
            RegisterTypes(container);
            ServiceLocator.SetServiceLocator(new UnityServiceLocator(container));
        }

        private static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IOperationsManager, SingleProcessDeploymentOperationsManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISomeRequiredService, SomeRequiredService>();
        }
    }
}