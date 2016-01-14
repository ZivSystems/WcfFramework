using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Client
{
    public class WcfProxyFactory<TContract> : IProxyFactory<TContract>
    {
        protected readonly IWcfChannelFactoryProvider _channelFactoryProvider;
        protected readonly string _endpointName;

        public WcfProxyFactory(IWcfChannelFactoryProvider channelFactoryProvider, string endpointName = null)
        {
            Logger.LogEvent(string.Format("Constructing WcfProxyFactory of type {0}.", typeof(TContract).Name), this, ImportanceLevels.gUnimportant);
            _channelFactoryProvider = channelFactoryProvider;
            _endpointName = endpointName;
        }

        public virtual IProxy<TContract> GetProxy()
        {
            Logger.LogEvent(string.Format("Creating WcfProxy of type {0}.", typeof(TContract).Name), this, ImportanceLevels.gUnimportant);
            var proxy = new WcfProxy<TContract>(_channelFactoryProvider, _endpointName);
            return new WcfProxy<TContract>(_channelFactoryProvider, _endpointName);
        }
    }
}