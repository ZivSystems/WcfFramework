using System.ServiceModel;

namespace Ziv.ServiceModel.Identity.ServerSideProviders
{
    public class WcfClientIdentityProvider : IClientIdentityProvider
    {
        public IClientIdentity GetClientIdentityForCurrentThread()
        {
            return new WcfClientIdentity(ServiceSecurityContext.Current);
        }
    }
}
