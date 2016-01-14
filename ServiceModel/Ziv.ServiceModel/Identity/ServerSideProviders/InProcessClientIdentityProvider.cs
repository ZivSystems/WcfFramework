namespace Ziv.ServiceModel.Identity.ServerSideProviders
{
    public class InProcessClientIdentityProvider : IClientIdentityProvider
    {
        public IClientIdentity GetClientIdentityForCurrentThread()
        {
            return new InProcessClientIdentity();
        }
    }
}
