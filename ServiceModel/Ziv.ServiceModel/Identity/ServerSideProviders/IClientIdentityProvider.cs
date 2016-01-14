namespace Ziv.ServiceModel.Identity.ServerSideProviders
{
    public interface IClientIdentityProvider
    {
        IClientIdentity GetClientIdentityForCurrentThread();
    }
}
