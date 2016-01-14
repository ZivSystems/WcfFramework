using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Client
{
    /// <summary>
    /// This class caches ChannelFactory instances so that the would not be recreated for later calls.
    /// </summary>
    public class WcfCachingChannelFactoryProvider : IWcfChannelFactoryProvider
    {
        readonly Dictionary<Type, Dictionary<string, IChannelFactory>> _cache = new Dictionary<Type, Dictionary<string, IChannelFactory>>();

        public ChannelFactory<TContract> GetChannelFactory<TContract>(string endpointName = null)
        {
            endpointName = string.IsNullOrEmpty(endpointName)
                               ? "*"
                               : endpointName;
            Type contractType = typeof(TContract);
            Logger.LogEvent(string.Format("Getting ChannelFactory of type {0} using channel factory cache.", contractType.Name), this, ImportanceLevels.gUnimportant);
            if (!_cache.ContainsKey(contractType))
            {
                Logger.LogEvent(string.Format("Type {0} doesn't exist in channel factory cache. Adding it to the cache.", contractType.Name), this, ImportanceLevels.gUnimportant);
                _cache.Add(contractType, new Dictionary<string, IChannelFactory>());
            }

            Dictionary<string, IChannelFactory> channelFactories = _cache[contractType];
            if (!channelFactories.ContainsKey(endpointName))
            {
                Logger.LogEvent(string.Format("There isn't a ChannelFactory of type {0} that associated with endpoint {1}. Creating a new one.", contractType.Name, endpointName), this, ImportanceLevels.gUnimportant);
                channelFactories.Add(endpointName, new ChannelFactory<TContract>(endpointName));
            }

            ChannelFactory<TContract> channelFactory = (ChannelFactory<TContract>)channelFactories[endpointName];
            if(channelFactory.State == CommunicationState.Faulted)
            {
                Logger.LogEvent(string.Format("The cached ChannelFactory of type {0} associated with endpoint {1} is in Faulted state. Aborting it and creating a new one.", contractType.Name, endpointName), this, ImportanceLevels.dMedium);
                channelFactory.Abort();
                channelFactory = new ChannelFactory<TContract>(endpointName);
                channelFactories[endpointName] = channelFactory;
            }
            Logger.LogEvent(string.Format("Cached ChannelFactory of type {0} associated with endpoint {1} has been retrieved.", contractType.Name, endpointName), this, ImportanceLevels.gUnimportant);
            return channelFactory;
        }
    }
}