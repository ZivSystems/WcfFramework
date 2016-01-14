using System.Web;
using Ziv.ServiceModel.Identity;
using Ziv.ServiceModel.Identity.Forwarding;
using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Client
{
    public class WcfAspnetIdentityForwardingProxyFactory<TContract> : WcfIdentityForwardingProxyFactory<TContract>
    {
        public WcfAspnetIdentityForwardingProxyFactory(IForwardingContext forwardingContext, IWcfChannelFactoryProvider channelFactoryProvider, string endpointName = null)
            : base(forwardingContext, channelFactoryProvider, endpointName)
        {
        }

        public override IProxy<TContract> GetProxy()
        {
            var httpUserIdentity = ClientIdentity.Create(HttpContext.Current.User.Identity);
            Logger.LogEvent(string.Format("Creating proxy of type {0}, which forwards HttpContext identity. The forwarded identity name is '{1}'.", typeof(TContract).Name, httpUserIdentity.Identity.Name), this, ImportanceLevels.gUnimportant);
            var proxy = GetProxy(httpUserIdentity);
            Logger.LogEvent(string.Format("Proxy of type {0}, which forwards HttpContext identity has been created.", typeof(TContract).Name), this, ImportanceLevels.gUnimportant);
            return proxy;
        }
    }
}
