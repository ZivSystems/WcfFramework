using System;
using System.ServiceModel;
using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Activation
{
    public class DependencyInjectionServiceHostServiceHost : ServiceHost
    {
        public DependencyInjectionServiceHostServiceHost(
          Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            Logger.LogEvent(string.Format("Constructing DependencyInjectionServiceHostServiceHost of service type {0}.", serviceType.Name), this, ImportanceLevels.gUnimportant);
            InitializeInstanceProvider();
            Logger.LogEvent(string.Format("DependencyInjectionServiceHostServiceHost of service type {0} has been constructeed.", serviceType.Name), this, ImportanceLevels.gUnimportant);
        }

        private void InitializeInstanceProvider()
        {
            Logger.LogEvent(string.Format("Adding dependency-injection instance provider as a service behavior to DependencyInjectionServiceHostServiceHost of service type {0}.", this.Description.ServiceType.Name), this, ImportanceLevels.gUnimportant);
            Description.Behaviors.Add(new DependencyInjectionInstanceProviderServiceBehavior());
            Logger.LogEvent(string.Format("Service behavior of dependency-injection instance provider has been added to DependencyInjectionServiceHostServiceHost of service type {0}.", this.Description.ServiceType.Name), this, ImportanceLevels.gUnimportant);
        }
    }
}
