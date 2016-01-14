using System.Collections.Generic;

namespace Ziv.ServiceModel.CodeGeneration
{
    public interface IConfigurationStrategy
    {
        IEnumerable<ConfigServiceBehaviorData> GetServiceBehaviors();
        IEnumerable<ConfigEndpointBehaviorData> GetEndpointBehaviors();
        IEnumerable<ConfigServiceData> GetServices();
    }
}