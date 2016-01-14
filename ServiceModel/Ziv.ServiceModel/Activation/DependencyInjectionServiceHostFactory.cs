using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Activation
{
    public class DependencyInjectionServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            Logger.LogEvent(string.Format("Creating instance of DependencyInjectionServiceHostServiceHost for service type {0}.", serviceType.Name), this, ImportanceLevels.gUnimportant);
            var serviceHost = new DependencyInjectionServiceHostServiceHost(serviceType, baseAddresses);
            Logger.LogEvent(string.Format("Instance of DependencyInjectionServiceHostServiceHost for service type {0} created successfuly.", serviceType.Name), this, ImportanceLevels.gUnimportant);
            return serviceHost;
        }
    }
}