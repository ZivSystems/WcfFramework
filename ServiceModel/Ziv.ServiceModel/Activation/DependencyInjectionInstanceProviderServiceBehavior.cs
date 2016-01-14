using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Activation
{
    public class DependencyInjectionInstanceProviderServiceBehavior : IServiceBehavior
    {
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            Logger.LogEvent(string.Format("Appling dispatch behavior of DependencyInjectionInstanceProviderServiceBehavior on ServiceHost of service type {0}.", serviceDescription.ServiceType.Name), this, ImportanceLevels.gUnimportant);
            var customInstanceProvider = new DependencyInjectionInstanceProvider();
            foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher ed in cd.Endpoints)
                {
                    if (!ed.IsSystemEndpoint)
                    {
                        ed.DispatchRuntime.InstanceProvider = customInstanceProvider;
                    }
                }
            }
            Logger.LogEvent(string.Format("Dispatch behavior of DependencyInjectionInstanceProviderServiceBehavior has been applied on ServiceHost of service type {0}.", serviceDescription.ServiceType.Name), this, ImportanceLevels.gUnimportant);
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}
