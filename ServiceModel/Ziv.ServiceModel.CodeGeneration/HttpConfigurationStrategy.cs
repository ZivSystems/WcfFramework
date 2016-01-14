using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Ziv.ServiceModel.CodeGeneration
{
    class HttpConfigurationStrategy : IConfigurationStrategy
    {
        private readonly bool _isGeneratingMetadataEndpoint;

        private readonly List<ConfigServiceData> _services = new List<ConfigServiceData>();
        private readonly List<ConfigServiceBehaviorData> _serviceBehaviors = new List<ConfigServiceBehaviorData>();
        private readonly List<ConfigEndpointBehaviorData> _endpointBehaviors = new List<ConfigEndpointBehaviorData>();

        public HttpConfigurationStrategy(IEnumerable<Type> serviceTypes, bool isGeneratingMetadataEndpoint = false)
        {
            foreach (var serviceType in serviceTypes)
            {
                var serviceData = new ConfigServiceData(serviceType);
                serviceData.Endpoints = GetServiceEndpoints(serviceData);
                if (isGeneratingMetadataEndpoint)
                {
                    string behaviorName = serviceType.Name + "Behavior";
                    serviceData.BehaviorName = behaviorName;
                    ConfigServiceBehaviorData behavior = new ConfigServiceBehaviorData()
                                                             {
                                                                 Name = behaviorName,
                                                                 Content = new XElement("behavior", new XAttribute("name", behaviorName),
                                                                                        new XElement("serviceMetadata", new XAttribute("httpGetEnabled", true)),
                                                                                        new XElement("serviceDebug", new XAttribute("includeExceptionDetailInFaults", true)))
                                                             };
                    _serviceBehaviors.Add(behavior);
                }
                _services.Add(serviceData);
            }

            _isGeneratingMetadataEndpoint = isGeneratingMetadataEndpoint;
        }

        protected virtual string GetBindingName()
        {
            return "basicHttpBinding";
        }

        public IEnumerable<ConfigEnpointData> GetServiceEndpoints(ConfigServiceData service)
        {
            foreach (var contractInterface in service.ServiceType.GetInterfaces().Where(inf => ConfigExtensions.IsServiceContract(inf)))
            {
                yield return new ConfigEnpointData(contractInterface.FullName, GetBindingName(), contractInterface.Name);
            }
            if (_isGeneratingMetadataEndpoint)
            {
                yield return new ConfigEnpointData("IMetadataExchange", "mexHttpBinding", "mex");
            }
        }

        public IEnumerable<ConfigServiceBehaviorData> GetServiceBehaviors()
        {
            return _serviceBehaviors;
        }

        public IEnumerable<ConfigEndpointBehaviorData> GetEndpointBehaviors()
        {
            return _endpointBehaviors;
        }

        public IEnumerable<ConfigServiceData> GetServices()
        {
            return _services;
        }
    }

    class WsHttpConfigurationStrategy : HttpConfigurationStrategy
    {
        public WsHttpConfigurationStrategy(IEnumerable<Type> serviceTypes, bool isGeneratingMetadataEndpoint) : base(serviceTypes, isGeneratingMetadataEndpoint)
        {
        }

        protected override string GetBindingName()
        {
            return "wsHttpBinding";
        }
    }
}