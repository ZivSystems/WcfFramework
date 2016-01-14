using System;
using System.Collections.Generic;

namespace Ziv.ServiceModel.CodeGeneration
{
    public class ConfigGenerationData
    {
        private IConfigurationStrategy _configurationStrategy;
        public string[] ServiceTypesPatterns { get; private set; }

        #region Constructors

        public ConfigGenerationData(string[] serviceTypesPatterns, IConfigurationStrategy configurationStrategy)
        {
            _configurationStrategy = configurationStrategy;
            ServiceTypesPatterns = serviceTypesPatterns;
        }

        public ConfigGenerationData(string[] serviceTypesPatterns, ConfigurationStrategy configurationStrategy)
            : this(serviceTypesPatterns, GetConfigurationStrategy(
                ConfigGenertionUtils.GetServiceTypesByRegularExpressions(serviceTypesPatterns),
                configurationStrategy))
        {
        }


        private static IConfigurationStrategy GetConfigurationStrategy(IEnumerable<Type> serviceTypes, ConfigurationStrategy configurationStrategy)
        {
            switch (configurationStrategy)
            {
                case CodeGeneration.ConfigurationStrategy.Http:
                    return new HttpConfigurationStrategy(serviceTypes, false);
                case CodeGeneration.ConfigurationStrategy.HttpWithMex:
                    return new HttpConfigurationStrategy(serviceTypes, true);
                case CodeGeneration.ConfigurationStrategy.WsHttp:
                    return new WsHttpConfigurationStrategy(serviceTypes, false);
                case CodeGeneration.ConfigurationStrategy.WsHttpWithMex:
                    return new WsHttpConfigurationStrategy(serviceTypes, true);
                default:
                    throw new ArgumentOutOfRangeException("configurationStrategy");
            }
        }

        #endregion

        public IEnumerable<ConfigServiceData> Services
        {
            get
            {
                return _configurationStrategy.GetServices();
            }
        }

        public IEnumerable<ConfigEndpointBehaviorData> EndpointBehaviors
        {
            get
            {
                return _configurationStrategy.GetEndpointBehaviors();
            }
        }

        public IEnumerable<ConfigServiceBehaviorData> ServiceBehaviors
        {
            get
            {
                return _configurationStrategy.GetServiceBehaviors();
            }
        }
    }
}