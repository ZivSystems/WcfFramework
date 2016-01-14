using System.ServiceModel;
using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Client
{
    /// <summary>
    /// This class creates a new Channel Factory for each call.
    /// </summary>
    public class WcfSimpleChannelFactoryProvider : IWcfChannelFactoryProvider
    {
        public ChannelFactory<TContract> GetChannelFactory<TContract>(string endpointName)
        {
            Logger.LogEvent(string.Format("Creating ChannelFactory of type {0}.", typeof(TContract).Name), this, ImportanceLevels.gUnimportant);
            var channelFactory = new ChannelFactory<TContract>(endpointName);
            Logger.LogEvent(string.Format("ChannelFactory of type {0} has been created.", typeof(TContract).Name), this, ImportanceLevels.gUnimportant);
            return channelFactory;
        }
    }
}