using Ziv.ServiceModel.Identity;
using Ziv.ServiceModel.Identity.Forwarding;
using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Client
{
    public class WcfIdentityForwardingProxyFactory<TContract> : WcfProxyFactory<TContract>, IIdentityForwardingProxyFactory<TContract>
    {
        protected IForwardingContext _forwardingContext;

        public WcfIdentityForwardingProxyFactory(IForwardingContext forwardingContext, IWcfChannelFactoryProvider channelFactoryProvider, string endpointName = null)
            :base(channelFactoryProvider, endpointName)
        {
            _forwardingContext = forwardingContext;
        }
        
        public IProxy<TContract> GetProxy(IClientIdentity forwardedIdentity)
        {
            Logger.LogEvent(string.Format("Creating WcfIdentityForwardingProxy of type {0}. The forwarded identity is '{1}'.", typeof(TContract).Name, forwardedIdentity.Identity.Name), this, ImportanceLevels.gUnimportant);
            _forwardingContext.ClientIdentity = forwardedIdentity;
            var proxy = new WcfIdentityForwardingProxy<TContract>(_forwardingContext, _channelFactoryProvider, _endpointName);
            Logger.LogEvent(string.Format("WcfIdentityForwardingProxy of type {0} has been created.", typeof(TContract).Name, forwardedIdentity.Identity.Name), this, ImportanceLevels.gUnimportant);
            return proxy;
        }
    }
}
