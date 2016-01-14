using Ziv.ServiceModel.Identity;

namespace Ziv.ServiceModel.Client
{
    public interface IIdentityForwardingProxyFactory< TContract> : IProxyFactory<TContract>
    {
        IProxy<TContract> GetProxy(IClientIdentity forwardedIdentity);
    }
}
