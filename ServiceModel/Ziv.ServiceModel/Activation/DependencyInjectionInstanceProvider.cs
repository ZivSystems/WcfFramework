using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Activation
{
    public class DependencyInjectionInstanceProvider : IInstanceProvider
    {
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            var requestedServiceType = instanceContext.Host.Description.ServiceType;
            Logger.LogEvent(string.Format("Resolving service of type {0}.", requestedServiceType.Name), this, ImportanceLevels.gUnimportant);
            var serviceInstance = ServiceLocator.Current.GetService(requestedServiceType);
            Logger.LogEvent(string.Format("Service of type {0} has been resolved successfuly.", requestedServiceType.Name), this, ImportanceLevels.gUnimportant);
            return serviceInstance;
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
        }
    }
}
